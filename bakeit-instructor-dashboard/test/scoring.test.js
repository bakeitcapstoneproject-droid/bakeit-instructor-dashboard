import test from 'node:test';
import assert from 'node:assert/strict';
import { scoreRemark, CloudDataService, DashboardMetrics, TableView } from '../public/assets/js/core.js';

test('score remarks include the passing boundary and both endpoints', () => {
  for (const score of [0, 59]) assert.equal(scoreRemark(score), 'Needs Practice');
  for (const score of [60, 100]) assert.equal(scoreRemark(score), 'Passed');
});

test('learner records and dashboard totals use score-based remarks', async () => {
  const students = await new CloudDataService().getStudents();
  assert.equal(students.find(student => student.score === 60).status, 'Passed');
  assert.equal(students.find(student => student.score === 62).status, 'Passed');
  assert.equal(students.find(student => student.score === 59).status, 'Needs Practice');
  assert.equal(new DashboardMetrics(students).summary().passed, 15);
});

test('display and passed count ignore outdated stored remarks', () => {
  const students = [
    { score: 59, status: 'Passed' },
    { score: 60, status: 'Needs Practice' }
  ];
  const root = { innerHTML: '' };
  new TableView(root).render(students);
  const rows = root.innerHTML.split('</tr>');
  assert.match(rows[0], /badge warn">Needs Practice/);
  assert.match(rows[1], /badge good">Passed/);
  assert.equal(new DashboardMetrics(students).summary().passed, 1);
});
