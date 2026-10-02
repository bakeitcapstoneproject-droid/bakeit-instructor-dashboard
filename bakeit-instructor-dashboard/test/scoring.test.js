import test from 'node:test';
import assert from 'node:assert/strict';
import { scoreRemark, CloudDataService, DashboardMetrics, TableView } from '../public/assets/js/core.js';
const assessment = {ratings:{decorum:4,kitchen_organization:3,safety_sanitation:5,baking_skills:4,product_appraisal:5}};

test('scores pass at 15 out of 25 and missing scores stay pending', () => {
  for (const score of [null, undefined, 0, 100]) assert.equal(scoreRemark(score), 'Awaiting assessment');
  for (const score of [5, 14]) assert.equal(scoreRemark(score), 'Needs practice');
  for (const score of [15, 25]) assert.equal(scoreRemark(score), 'Passed');
});
test('demo learners and dashboard metrics use criterion totals', async () => {
  const students = await new CloudDataService({mode:'mock'}).getStudents();
  assert.ok(students.every(student=>student.score>=5 && student.score<=25 && student.status===(student.score>=15?'Passed':'Needs practice')));
  assert.equal(new DashboardMetrics(students).summary().assessed,20);
});
test('pending and legacy records do not lower the class average', () => {
  assert.deepEqual(new DashboardMetrics([{score:null},{score:90},{assessment}]).summary(), {
    enrolled:3, average:'21.0', assessed:1, pending:2
  });
  assert.equal(new DashboardMetrics([{score:null}]).summary().average,'—');
  const root={innerHTML:''};
  new TableView(root).render([{name:'<img src=x onerror=alert(1)>',section:'<script>',score:null}]);
  assert.match(root.innerHTML,/Awaiting assessment/);
  assert.doesNotMatch(root.innerHTML,/<img|<script>|class="progress"/);
});
test('display ignores obsolete percentages and remarks', () => {
  const root={innerHTML:''};
  new TableView(root).render([{score:100,status:'Passed'},{score:0,status:'Needs Practice',assessment}]);
  assert.match(root.innerHTML,/21 \/ 25/);
  assert.match(root.innerHTML,/badge good">Passed/);
  assert.doesNotMatch(root.innerHTML,/Needs Practice|100/);
});
