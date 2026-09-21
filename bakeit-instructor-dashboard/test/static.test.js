import test from 'node:test';
import assert from 'node:assert/strict';
import { createStaticRequest, staticStorageKey } from '../public/assets/js/static-data.js';

function fixture() {
  const values = new Map();
  const storage = { getItem: key => values.get(key) ?? null, setItem: (key, value) => values.set(key, value) };
  return { values, storage, request: createStaticRequest(storage, null) };
}

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
  assert.equal(students.find(student => student.score === 62).status, 'Passed');
  const { activities } = await request('/api/activities?sectionId=section-a');
  assert.ok(activities.every(item => Number.isFinite(Date.parse(item.time))));
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
