import test from 'node:test';
import assert from 'node:assert/strict';
import { rubricTotal, validateRatings, rubricCriteria, learnerPerformance } from '../public/assets/js/domain/performance.js';
import { buildSectionReport, validateSectionReport } from '../public/assets/js/domain/section-report.js';
import { createStaticRequest, staticStorageKey } from '../public/assets/js/static-data.js';
const ratings = { decorum: 1, kitchen_organization: 2, safety_sanitation: 3, baking_skills: 4, product_appraisal: 5 };

test('five unweighted integer ratings sum to 5–25; missing and invalid provider ratings are rejected', () => {
  assert.equal(rubricTotal(ratings), 15);
  for (const value of [1, 5]) assert.equal(rubricTotal(Object.fromEntries(rubricCriteria.map(({key}) => [key, value]))), value * 5);
  for (const value of [0, 6, 2.5, '3', null, undefined, NaN, Infinity]) {
    assert.throws(() => validateRatings({...ratings, baking_skills: value}));
    assert.equal(rubricTotal({...ratings, baking_skills: value}), null);
  }
  assert.throws(() => validateRatings({...ratings, unknown: 3}));
  assert.equal(learnerPerformance({score: 90}).score, null);
  assert.equal(learnerPerformance({score: 90}).legacy_score_percent, 90);
});

test('static monitoring preserves results and refuses manual grade changes', async () => {
  const values = new Map();
  const storage = {getItem: key => values.get(key) ?? null, setItem: (key, value) => values.set(key, value)};
  const request = createStaticRequest(storage, null);
  const first = (await request('/api/learners?sectionId=section-a')).students[0];
  const saved = values.get(staticStorageKey);
  await assert.rejects(request('/api/assessments',{method:'POST',body:JSON.stringify({sectionId:'section-a',learnerId:first.id,ratings})}), /unavailable/);
  assert.equal(values.get(staticStorageKey),saved);
  const reload = createStaticRequest(storage,null);
  assert.equal((await reload('/api/learners?sectionId=section-a')).students[0].score,first.score);
  const row = (await reload('/api/reports/sections/section-a')).report.rows.find(row=>row.learner_id===first.id);
  assert.equal(row.total_score,first.score);
  for (const {key} of rubricCriteria) assert.equal(row[key+'_rating'], first.assessment.ratings[key]);
});

test('report totals are derived from criterion ratings and tampered totals are rejected', () => {
  const report = buildSectionReport({id:'a',name:'A'}, [{id:'1', name:'Ana',sectionId:'a',assessment:{ratings}}]);
  assert.equal(report.rows[0].total_score,15);
  assert.equal(report.rows[0].baking_skills_rating,4);
  assert.equal(report.rows[0].legacy_score_percent,null);
  assert.equal(report.rows[0].score_remark,'Passed');
  for (const patch of [{total_score:100},{max_score:100},{decorum_rating:0},{score_remark:'Failed'}]) {
    assert.throws(()=>validateSectionReport({...report,rows:[{...report.rows[0],...patch}]}),/invalid section report/);
  }
});
