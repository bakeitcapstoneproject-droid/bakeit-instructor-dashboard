import test from 'node:test';
import assert from 'node:assert/strict';
import { createStaticRequest, staticStorageKey } from '../public/assets/js/static-data.js';

function fixture() {
  const values = new Map();
  const storage = { getItem: key => values.get(key) ?? null, setItem: (key, value) => values.set(key, value) };
  return { values, storage, request: createStaticRequest(storage, null) };
}

test('older saved demo learners receive sample totals without clearing classes or changing real results', async () => {
  const { storage, request, values } = fixture();
  await request('/api/sections');
  const data = JSON.parse(values.get(staticStorageKey));
  const originalSections = structuredClone(data.sections);
  for (const student of data.students) {
    delete student.assessment;
    delete student.demo;
    student.score = 88;
  }
  data.students[1].assessment = { ratings: {decorum:1,kitchen_organization:1,safety_sanitation:1,baking_skills:1,product_appraisal:1} };
  data.students[2].demo = false;
  data.students.push({...data.students[0],id:'real-learner',name:'Real learner'});
  const untouched = structuredClone([data.students[1], data.students[2], data.students.at(-1)]);
  values.set(staticStorageKey,JSON.stringify(data));
  const reload = createStaticRequest(storage,null);
  const {students} = await reload('/api/learners');
  assert.equal(students.find(s=>s.id==='S-0241').score,22);
  assert.equal(students.find(s=>s.id==='S-0241').demo,true);
  assert.equal(students.find(s=>s.id==='S-0241').legacy_score_percent,88);
  assert.equal(students.find(s=>s.id==='S-0242').score,5);
  assert.equal(students.find(s=>s.id==='S-0243').score,null);
  assert.equal(students.find(s=>s.id==='real-learner').score,null);
  const migrated = JSON.parse(values.get(staticStorageKey));
  assert.deepEqual(migrated.sections,originalSections);
  assert.deepEqual([migrated.students[1],migrated.students[2],migrated.students.at(-1)],untouched);
  const saved = values.get(staticStorageKey);
  await reload('/api/learners');
  assert.equal(values.get(staticStorageKey),saved);
  const report = (await reload('/api/reports/sections/section-a')).report;
  assert.equal(report.rows.find(row=>row.learner_id==='S-0241').total_score,22);
});

test('static demo persists creation, filters samples and starts new classes empty', async () => {
  const { storage, request } = fixture();
  const initial = await request('/api/sections');
  assert.equal(initial.sections.length, 2);
  assert.ok(initial.sections.every(section => section.learnerCount > 0));
  const { section } = await request('/api/sections', { method: 'POST', body: JSON.stringify({ name: 'BSHM 3A' }) });
  assert.match(section.classCode, /^[A-HJ-NP-Z2-9]{8}$/);
  assert.equal(section.learnerCount, 0);
  const reloaded = createStaticRequest(storage, null);
  assert.equal((await reloaded('/api/sections')).sections.at(-1).id, section.id);
  assert.deepEqual(await reloaded(`/api/learners?sectionId=${section.id}`), { students: [] });
  const { students } = await request('/api/learners?sectionId=section-a');
  assert.ok(students.length > 0 && students.every(student => student.sectionId === 'section-a'));
  assert.equal(students.find(student => student.id === 'S-0242').status, 'Failed');
  const { activities } = await request('/api/activities?sectionId=section-a');
  assert.equal(activities.length, students.length);
  assert.ok(activities.length > 5);
  assert.ok(activities.every(item => Number.isFinite(Date.parse(item.time))));
  assert.deepEqual(activities.map(item => item.time), activities.map(item => item.time).sort().reverse());
});

test('deleting demo sections removes related data and never reseeds an empty workspace', async () => {
  const { request, storage } = fixture();
  const deleted = await request('/api/sections/section-a', { method: 'DELETE' });
  assert.ok(deleted.removedEnrollments > 0);
  assert.deepEqual(await request('/api/learners?sectionId=section-a'), { students: [] });
  assert.deepEqual(await request('/api/sessions?sectionId=section-a'), { sessions: [] });
  assert.ok((await request('/api/learners?sectionId=section-b')).students.length > 0);
  await request('/api/sections/section-b', { method: 'DELETE' });
  assert.deepEqual(await createStaticRequest(storage, null)('/api/sections'), { sections: [] });
});

test('invalid and duplicate names do not modify stored data', async () => {
  const { request, values } = fixture();
  await request('/api/sections');
  const initial = values.get(staticStorageKey);
  for (const name of ['', ' '.repeat(3), 'x'.repeat(81), ' section a ', null]) {
    await assert.rejects(request('/api/sections', { method: 'POST', body: JSON.stringify({ name }) }));
    assert.equal(values.get(staticStorageKey), initial);
  }
  await assert.rejects(request('/api/sections/missing', { method: 'DELETE' }), /not found/);
  await assert.rejects(request('/api/sections/join', { method: 'POST' }), /unavailable/);
});

test('storage failures and corruption report errors without silently resetting records', async () => {
  const { request, values } = fixture();
  values.set(staticStorageKey, '{broken');
  await assert.rejects(request('/api/sections'), /could not be read/);
  assert.equal(values.get(staticStorageKey), '{broken');
  const blocked = createStaticRequest({ getItem() { throw new Error(); } }, null);
  await assert.rejects(blocked('/api/sections'), /Allow browser storage/);
  const full = createStaticRequest({ getItem: () => null, setItem() { throw new Error(); } }, null);
  await assert.rejects(full('/api/sections'), /Could not save/);
});

test('separate tabs use one lock and read the latest saved workspace', async () => {
  const { storage } = fixture();
  let pending = Promise.resolve();
  const locks = { request(key, action) {
    assert.equal(key, staticStorageKey);
    const result = pending.then(action);
    pending = result.catch(() => {});
    return result;
  } };
  const first = createStaticRequest(storage, locks), second = createStaticRequest(storage, locks);
  await Promise.all([
    first('/api/sections', { method: 'POST', body: JSON.stringify({ name: 'New A' }) }),
    second('/api/sections', { method: 'POST', body: JSON.stringify({ name: 'New B' }) })
  ]);
  assert.equal((await first('/api/sections')).sections.length, 4);
});
