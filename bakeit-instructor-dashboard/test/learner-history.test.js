import test from 'node:test';
import assert from 'node:assert/strict';
import { buildLearnerHistory, validateLearnerHistory } from '../public/assets/js/domain/learner-history.js';
import { createStaticRequest } from '../public/assets/js/static-data.js';
import { CloudDataService } from '../public/assets/js/services/cloud-data-service.js';

const ratings = { decorum: 4, kitchen_organization: 4, safety_sanitation: 5, baking_skills: 4, product_appraisal: 5 };
const learner = { id: 'learner', sectionId: 'class', recipe: 'Brownies', sessions: 4, assessment: { ratings } };
const record = { id: 'session', recipe: 'Cookies', startedAt: '2026-09-28T01:00:00Z', endedAt: '2026-09-28T01:30:00Z', status: 'Completed', ratings };

test('sample history matches count and latest score, stays stable, and never invents real learner logs', () => {
  const history = buildLearnerHistory({ ...learner, demo: true });
  assert.equal(history.entries.length, 4);
  assert.equal(history.entries[0].score, 22);
  assert.equal(history.entries[0].result, 'Passed');
  assert.ok(history.entries.every((entry, index, all) => !index || entry.startedAt < all[index - 1].startedAt));
  assert.deepEqual(buildLearnerHistory({ ...learner, demo: true }), history);
  assert.deepEqual(buildLearnerHistory(learner).entries, []);
  assert.deepEqual(buildLearnerHistory({ ...learner, demo: true, sessionHistory: [] }).entries, []);
  assert.deepEqual(buildLearnerHistory({ ...learner, demo: true, sessions: 0 }).entries, []);
});

test('received logs retain timestamps, sort newest first, and distinguish unscored sessions', () => {
  const history = buildLearnerHistory({ ...learner, sessionHistory: [record,
    { ...record, id: 'newer', startedAt: '2026-09-29T02:00:00Z', endedAt: null, status: 'In progress', ratings: null }] });
  assert.equal(history.sample, false);
  assert.equal(history.sessionCount, 4);
  assert.equal(history.entries[0].id, 'newer');
  assert.equal(history.entries[0].score, null);
  assert.equal(history.entries[0].result, 'Awaiting assessment');
  assert.equal(history.entries[1].startedAt, record.startedAt);
  for (const patch of [{ startedAt: 'yesterday' }, { endedAt: '2026-09-27T00:00:00Z' }, { ratings: { ...ratings, decorum: 6 } }]) {
    assert.throws(() => buildLearnerHistory({ ...learner, sessionHistory: [{ ...record, ...patch }] }));
  }
  assert.throws(() => buildLearnerHistory({ ...learner, sessionHistory: [record, record] }));
  assert.throws(() => validateLearnerHistory(history, 'wrong-class', learner.id));
});

test('static history supports reload and scopes learners to their section', async () => {
  const values = new Map();
  const storage = { getItem: key => values.get(key) ?? null, setItem: (key, value) => values.set(key, value) };
  const request = createStaticRequest(storage, null);
  const cloud = new CloudDataService({ request });
  const first = (await cloud.getStudents('section-a'))[0];
  const history = await cloud.getLearnerHistory('section-a', first.id);
  assert.equal(history.entries.length, first.sessions);
  assert.equal(history.entries[0].score, first.score);
  assert.deepEqual(await new CloudDataService({ request: createStaticRequest(storage, null) }).getLearnerHistory('section-a', first.id), history);
  await assert.rejects(cloud.getLearnerHistory('section-b', first.id), /not found/);
  await assert.rejects(cloud.getLearnerHistory('all', first.id), /section/);
  await assert.rejects(new CloudDataService({ request: async () => ({ history }) }).getLearnerHistory('section-a', 'someone-else'), /invalid session history/);
});
