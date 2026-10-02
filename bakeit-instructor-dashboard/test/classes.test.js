import test from 'node:test';
import assert from 'node:assert/strict';
import { createServer } from 'node:http';
import { mkdtemp, rm, readFile } from 'node:fs/promises';
import { tmpdir } from 'node:os';
import { join } from 'node:path';
import { StaticWebsiteServer } from '../src/server.js';
import { ClassStore } from '../src/classes.js';
import { addDemoLearners, removeDemoLearners } from '../src/demo.js';
import { SectionService, StorageService, CloudDataService } from '../public/assets/js/core.js';

async function fixture(t) {
  const directory = await mkdtemp(join(tmpdir(), 'bakeit-classes-'));
  const dataFile = join(directory, 'classes.json');
  const app = new StaticWebsiteServer({ dataFile });
  const server = createServer((req, res) => app.respond(req, res));
  await new Promise(resolve => server.listen(0, '127.0.0.1', resolve));
  t.after(async () => {
    const closed = new Promise(resolve => server.close(resolve));
    server.closeAllConnections();
    await closed;
    await rm(directory, { recursive: true, force: true });
  });
  const base = `http://127.0.0.1:${server.address().port}`;
  const request = async (path, options) => {
    const response = await fetch(base + path, options);
    return { status: response.status, body: await response.json() };
  };
  const post = (path, body) => request(path, { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(body) });
  return { app, dataFile, request, post, base };
}

test('learner history API reads persisted records and checks section membership', async t => {
  const { post, request, app, dataFile } = await fixture(t);
  const section = (await post('/api/sections', { name: 'History' })).body.section;
  await post('/api/sections/join', { classCode: section.classCode, learnerId: 'learner-1', learnerName: 'Ana' });
  const path = `/api/learners/learner-1/history?sectionId=${section.id}`;
  assert.deepEqual((await request(path)).body.history.entries, []);
  await app.classes.mutate(data => { data.enrollments[0].sessionHistory = [{ id: 'one', recipe: 'Brownies',
    startedAt: '2026-09-28T01:00:00Z', endedAt: '2026-09-28T01:30:00Z', status: 'Completed',
    ratings: { decorum: 4, kitchen_organization: 4, safety_sanitation: 5, baking_skills: 4, product_appraisal: 5 } }]; });
  const before = await readFile(dataFile, 'utf8');
  const response = await request(path);
  assert.equal(response.status, 200);
  assert.equal(response.body.history.entries[0].score, 22);
  assert.equal(response.body.history.entries[0].startedAt, '2026-09-28T01:00:00Z');
  assert.deepEqual(await new ClassStore(dataFile).learnerHistory(section.id, 'learner-1'), response.body.history);
  assert.deepEqual((await request('/api/reports/sections/' + section.id)).body.report.histories[0], response.body.history);
  assert.equal((await request('/api/learners/learner-1/history?sectionId=other')).status, 404);
  assert.equal((await request('/api/learners/learner-1/history')).status, 400);
  assert.equal((await request('/api/learners/learner-1/history?sectionId=all')).status, 400);
  assert.equal((await request('/api/learners/%ZZ/history?sectionId=' + section.id)).status, 400);
  assert.equal(await readFile(dataFile, 'utf8'), before);
});

test('section report endpoint returns a scoped snapshot including unstarted learners', async t => {
  const { post, request } = await fixture(t);
  const first = (await post('/api/sections', { name: 'Report A' })).body.section;
  const second = (await post('/api/sections', { name: 'Report B' })).body.section;
  await post('/api/sections/join', { classCode: first.classCode, learnerId: 'A-1', learnerName: 'First learner' });
  await post('/api/sections/join', { classCode: second.classCode, learnerId: 'B-1', learnerName: 'Second learner' });
  const result = await request(`/api/reports/sections/${first.id}`);
  assert.equal(result.status, 200);
  assert.equal(result.body.report.section.name, 'Report A');
  assert.deepEqual(result.body.report.rows.map(row => row.learner_id), ['A-1']);
  assert.equal(result.body.report.rows[0].total_score, null);
  assert.equal(result.body.report.rows[0].completion_status, 'Not started');
  assert.equal((await request('/api/reports/sections/all')).status, 400);
  assert.equal((await request('/api/reports/sections/missing')).status, 404);
  assert.equal((await request('/api/reports/sections/%ZZ')).status, 400);
});

test('monitoring API reads supplied results and rejects manual grading', async t => {
  const { post, request, app, dataFile } = await fixture(t);
  const section = (await post('/api/sections', {name:'VR class'})).body.section;
  await post('/api/sections/join', {classCode:section.classCode,learnerId:'1',learnerName:'Ana'});
  const ratings = {decorum:1,kitchen_organization:2,safety_sanitation:3,baking_skills:4,product_appraisal:5};
  // Fixture represents an assessment supplied by the future VR data provider.
  await app.classes.mutate(data => { data.enrollments[0].assessment = {ratings}; });
  assert.equal((await request('/api/learners')).body.students[0].score,15);
  assert.equal((await new ClassStore(dataFile).learners(section.id))[0].score,15);
  const row = (await request('/api/reports/sections/'+section.id)).body.report.rows[0];
  assert.equal(row.total_score,15);
  assert.equal(row.decorum_rating,1);
  assert.equal(row.product_appraisal_rating,5);
  const before = await readFile(dataFile,'utf8');
  assert.equal((await post('/api/assessments',{sectionId:section.id,learnerId:'1',ratings})).status,404);
  assert.equal(await readFile(dataFile,'utf8'),before);
});

test('create a section, join from a separate client, filter learners, and reload saved enrollment', async t => {
  const { post, request, dataFile, base } = await fixture(t);
  assert.deepEqual((await request('/api/sections')).body.sections, []);
  assert.deepEqual((await request('/api/learners')).body.students, []);
  const created = await post('/api/sections', { name: '  Baking 2A  ' });
  assert.equal(created.status, 201);
  const { section } = created.body;
  assert.equal(section.name, 'Baking 2A');
  assert.match(section.classCode, /^[A-HJ-NP-Z2-9]{8}$/);
  assert.equal(section.learnerCount, 0);
  const second = (await post('/api/sections', { name: 'Baking 2B' })).body.section;
  const payload = { classCode: ` ${section.classCode.toLowerCase()} `, learnerId: 'VR-user-42', learnerName: 'Ana Santos' };
  const enrolled = await post('/api/sections/join', payload);
  assert.equal(enrolled.status, 201);
  assert.equal(enrolled.body.section.id, section.id);
  assert.equal(enrolled.body.alreadyJoined, false);
  const repeated = await post('/api/sections/join', payload);
  assert.equal(repeated.status, 200);
  assert.equal(repeated.body.alreadyJoined, true);
  assert.deepEqual(repeated.body.enrollment, enrolled.body.enrollment);
  const cloud = new CloudDataService({ request: async path => (await fetch(base + path)).json() });
  const learners = await cloud.getStudents(section.id);
  assert.equal(learners.length, 1);
  assert.equal(learners[0].name, 'Ana Santos');
  assert.equal(learners[0].status, 'Awaiting assessment');
  assert.equal(learners[0].score, null);
  assert.deepEqual(await cloud.getStudents(second.id), []);
  assert.deepEqual(await cloud.getLiveSessions(), []);
  assert.match((await cloud.getActivities())[0].text, /Ana Santos joined Baking 2A/);
  const reloaded = new ClassStore(dataFile);
  assert.equal((await reloaded.listSections())[0].classCode, section.classCode);
  assert.equal((await reloaded.listSections())[0].learnerCount, 1);
  assert.equal((await reloaded.learners(section.id))[0].id, 'VR-user-42');
  // A learner may enroll in another section without overwriting the first membership.
  await post('/api/sections/join', { ...payload, classCode: second.classCode });
  assert.equal((await cloud.getStudents()).length, 2);
});

test('invalid input, unknown codes, and duplicate names return useful errors without saving changes', async t => {
  const { post, request } = await fixture(t);
  for (const name of ['', '   ', 12, 'x'.repeat(81)]) assert.equal((await post('/api/sections', { name })).status, 400);
  const { section } = (await post('/api/sections', { name: 'Baking' })).body;
  assert.equal((await post('/api/sections', { name: ' BAKING ' })).status, 409);
  const payload = { classCode: section.classCode, learnerId: 'learner-1', learnerName: 'Ana' };
  assert.equal((await post('/api/sections/join', { ...payload, classCode: 'BAD' })).status, 400);
  const unknownCode = section.classCode === 'AAAAAAAA' ? 'BBBBBBBB' : 'AAAAAAAA';
  assert.equal((await post('/api/sections/join', { ...payload, classCode: unknownCode })).status, 404);
  for (const field of ['learnerId', 'learnerName']) {
    assert.equal((await post('/api/sections/join', { ...payload, [field]: '' })).status, 400);
  }
  assert.equal((await post('/api/sections', null)).status, 400);
  assert.equal((await post('/api/sections', [])).status, 400);
  assert.equal((await request('/api/sections', { method: 'POST', body: '{}' })).status, 415);
  assert.equal((await request('/api/sections', { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: '{' })).status, 400);
  assert.equal((await post('/api/sections', { name: 'x'.repeat(9000) })).status, 413);
  assert.equal((await request('/api/sections')).body.sections.length, 1);
  assert.deepEqual((await request('/api/learners')).body.students, []);
});

test('simultaneous creation and repeated join requests do not lose data or duplicate enrollment', async t => {
  const { post, request } = await fixture(t);
  const sections = await Promise.all(Array.from({ length: 8 }, (_, i) => post('/api/sections', { name: `Class ${i}` })));
  assert.ok(sections.every(result => result.status === 201));
  assert.equal(new Set(sections.map(result => result.body.section.classCode)).size, 8);
  const classCode = sections[0].body.section.classCode;
  const joins = await Promise.all(Array.from({ length: 8 }, () => post('/api/sections/join', { classCode, learnerId: 'same-id', learnerName: 'Ana' })));
  assert.equal(joins.filter(result => result.status === 201).length, 1);
  assert.equal(joins.filter(result => result.status === 200).length, 7);
  assert.equal((await request('/api/learners')).body.students.length, 1);
  assert.equal((await request('/api/sections')).body.sections.length, 8);
});

test('demo learners can be added repeatedly and removed without changing real enrollments', async t => {
  const { app, post, request } = await fixture(t);
  const first = (await post('/api/sections', { name: 'Demo A' })).body.section;
  const second = (await post('/api/sections', { name: 'Demo B' })).body.section;
  await post('/api/sections/join', { classCode: first.classCode, learnerId: 'real-1', learnerName: 'Real Learner', demo: true, demoResult: { score: 99 } });
  assert.deepEqual((await request('/api/sessions')).body.sessions, []);
  assert.equal(await addDemoLearners(app.classes), 12);
  assert.equal(await addDemoLearners(app.classes), 0);
  const learners = (await request(`/api/learners?sectionId=${first.id}`)).body.students;
  assert.equal(learners.length, 7);
  assert.equal(learners.find(item => item.id === 'real-1').score, null);
  assert.equal(learners.filter(item => item.demo).length, 6);
  assert.deepEqual(new Set(learners.map(item => item.status)), new Set(['Passed', 'Needs practice', 'Awaiting assessment']));
  assert.ok(learners.some(item => item.score === 22 && item.recipe === 'Brownies' && item.sessions === 4));
  assert.equal((await request(`/api/learners?sectionId=${second.id}`)).body.students.length, 6);
  assert.deepEqual((await request('/api/sections')).body.sections.map(item => item.learnerCount), [7, 6]);
  const samples = (await request('/api/sessions')).body.sessions;
  assert.equal(samples.length, 6);
  assert.ok(samples.every(item => item.demo && item.current > 0 && item.current < item.total && item.events.length > 0));
  const sectionSamples = (await request(`/api/sessions?sectionId=${second.id}`)).body.sessions;
  assert.equal(sectionSamples.length, 3);
  assert.ok(sectionSamples.every(item => item.sectionId === second.id));
  assert.deepEqual(new Set(sectionSamples.map(item => item.recipe)), new Set(['Brownies', 'Cookies', 'Cupcakes']));
  assert.deepEqual((await request('/api/sessions?sectionId=missing')).body.sessions, []);
  assert.equal(await removeDemoLearners(app.classes), 12);
  assert.equal(await removeDemoLearners(app.classes), 0);
  assert.deepEqual((await request('/api/learners')).body.students.map(item => item.id), ['real-1']);
  assert.equal((await request('/api/sections')).body.sections.length, 2);
  assert.deepEqual((await request('/api/sessions')).body.sessions, []);
});

test('deleting a section removes only its enrollments and sessions and persists after reload', async t => {
  const { app, post, request, dataFile } = await fixture(t);
  const first = (await post('/api/sections', { name: 'Delete me' })).body.section;
  const second = (await post('/api/sections', { name: 'Keep me' })).body.section;
  for (const section of [first, second]) {
    await post('/api/sections/join', { classCode: section.classCode, learnerId: 'shared-learner', learnerName: 'Ana' });
  }
  await addDemoLearners(app.classes);
  const before = await app.classes.read();
  const deleted = await request(`/api/sections/${first.id}`, { method: 'DELETE' });
  assert.equal(deleted.status, 200);
  assert.equal(deleted.body.section.id, first.id);
  assert.equal(deleted.body.removedEnrollments, 7);
  const reloaded = new ClassStore(dataFile);
  assert.deepEqual((await reloaded.listSections()).map(section => section.id), [second.id]);
  assert.deepEqual((await reloaded.read()).enrollments, before.enrollments.filter(item => item.sectionId === second.id));
  assert.equal((await reloaded.learners(second.id)).length, 7);
  assert.deepEqual((await request(`/api/sessions?sectionId=${first.id}`)).body.sessions, []);
  assert.equal((await request(`/api/sessions?sectionId=${second.id}`)).body.sessions.length, 3);
  assert.equal((await post('/api/sections/join', { classCode: first.classCode, learnerId: 'new', learnerName: 'New' })).status, 404);
  const saved = await readFile(dataFile, 'utf8');
  assert.equal((await request(`/api/sections/${first.id}`, { method: 'DELETE' })).status, 404);
  assert.equal((await request('/api/sections/%ZZ', { method: 'DELETE' })).status, 400);
  assert.equal(await readFile(dataFile, 'utf8'), saved);
  assert.equal((await request(`/api/sections/${second.id}`, { method: 'DELETE' })).status, 200);
  assert.deepEqual((await reloaded.read()), { sections: [], enrollments: [] });
});

test('concurrent joins and section deletion cannot leave orphan enrollments', async t => {
  const { post, request, app } = await fixture(t);
  const section = (await post('/api/sections', { name: 'Concurrent' })).body.section;
  const results = await Promise.all([
    request(`/api/sections/${section.id}`, { method: 'DELETE' }),
    post('/api/sections/join', { classCode: section.classCode, learnerId: 'racing-join', learnerName: 'Ana' })
  ]);
  assert.equal(results[0].status, 200);
  assert.ok([201, 404].includes(results[1].status));
  assert.deepEqual(await app.classes.read(), { sections: [], enrollments: [] });
});

test('section deletion resets selection and ignores an older in-flight section list', async () => {
  const values = new Map();
  const storage = new StorageService({ getItem: key => values.get(key) ?? null, setItem: (key, value) => values.set(key, value) });
  let finishLoad;
  const service = new SectionService(storage, (path, options) => {
    if (options?.method === 'DELETE') return Promise.resolve({ section: { id: 'first' }, removedEnrollments: 0 });
    return new Promise(resolve => { finishLoad = resolve; });
  });
  service.sections = [{ id: 'all', name: 'All Sections' }, { id: 'first', name: 'First' }, { id: 'second', name: 'Second' }];
  service.select('first');
  const loading = service.load();
  await service.delete('first');
  finishLoad({ sections: [{ id: 'first', name: 'First' }, { id: 'second', name: 'Second' }] });
  await loading;
  assert.equal(service.selected(), 'all');
  assert.equal(storage.get('bakeit_section'), 'all');
  assert.deepEqual(service.sections.map(section => section.id), ['all', 'second']);
  service.select('second');
  service.request = async () => { throw new Error('Connection failed'); };
  await assert.rejects(service.delete('second'), /Connection failed/);
  assert.equal(service.selected(), 'second');
});

test('class code collisions are retried', async t => {
  const { dataFile } = await fixture(t);
  const codes = ['ABCDEFGH', 'ABCDEFGH', 'BCDEFGHJ'];
  const store = new ClassStore(dataFile, () => codes.shift());
  assert.equal((await store.createSection({ name: 'First' })).classCode, 'ABCDEFGH');
  assert.equal((await store.createSection({ name: 'Second' })).classCode, 'BCDEFGHJ');
});

test('section selection uses saved sections across pages and resets stale fixed selections', async t => {
  const { post, request } = await fixture(t);
  const values = new Map();
  const storage = new StorageService({ getItem: key => values.get(key) ?? null, setItem: (key, value) => values.set(key, value) });
  const api = async (path, options) => {
    const result = options ? await post(path, JSON.parse(options.body)) : await request(path);
    if (result.status >= 400) throw new Error(result.body.error);
    return result.body;
  };
  storage.set('bakeit_section', 'section-b');
  const service = new SectionService(storage, api);
  await service.load();
  assert.equal(service.selected(), 'all');
  const section = await service.create('My new class');
  assert.equal(service.selected(), section.id);
  const reloaded = new SectionService(storage, api);
  await reloaded.load();
  assert.equal(reloaded.selected(), section.id);
  assert.equal(reloaded.name(), 'My new class');
  for (const page of ['dashboard', 'students', 'sessions', 'reports']) {
    const html = await readFile(`public/${page}.html`, 'utf8');
    assert.doesNotMatch(html, /href="\/sections.html"/);
    if (page === 'students') {
      assert.match(html, /id="learner-records"[^>]*hidden/);
      assert.match(html, /data-back-sections/);
    } else assert.match(html, /data-section-select/);
  }
});
