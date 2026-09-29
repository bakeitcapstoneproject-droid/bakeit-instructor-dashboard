import test from 'node:test';
import assert from 'node:assert/strict';
import { buildSectionReport, validateSectionReport, sectionReportFilename, reportColumns } from '../public/assets/js/domain/section-report.js';
import { CloudDataService } from '../public/assets/js/services/cloud-data-service.js';
import { createStaticRequest } from '../public/assets/js/static-data.js';

const section = { id: 'section-a', name: 'BSHM 2A' };
const generatedAt = '2026-09-29T00:00:00.000Z';
const learner = overrides => ({ id: 'learner-1', name: 'Ana Santos', sectionId: section.id,
  recipe: 'Cookies', sessions: 2, score: 88, progress: 0, waste: 'Low', ...overrides });
const build = students => buildSectionReport(section, students, { generatedAt });

test('reports include only the selected section and never invent missing measurements', () => {
  const report = build([learner(), learner({ id: 'foreign', sectionId: 'section-b' })]);
  assert.equal(report.rows.length, 1);
  assert.equal(report.rows[0].score_percent, 88);
  assert.equal(report.rows[0].score_remark, 'Passed');
  assert.equal(report.rows[0].waste_level, 'Low');
  for (const key of ['safety_score_percent', 'safety_incident_count', 'completion_percent', 'completion_status', 'procedural_accuracy_percent', 'waste_quantity', 'assessed_at']) {
    assert.equal(report.rows[0][key], null, key);
  }
  assert.equal(report.rows[0].data_source, 'local');
});

test('unstarted learners keep unknown results blank and measured zeroes are retained', () => {
  const unstarted = build([learner({ recipe: 'Not started', sessions: 0, score: null, waste: '—' })]).rows[0];
  assert.equal(unstarted.completion_status, 'Not started');
  assert.equal(unstarted.score_remark, null);
  assert.equal(unstarted.recipe, null);
  assert.equal(unstarted.waste_level, null);
  const measured = build([learner({ score: 0, demo: true, assessment: {
    safety_incident_count: 0, waste_quantity: 0, waste_unit: 'g', completion_percent: 0,
    procedural_correct_steps: 0, procedural_assessed_steps: 16
  } })]).rows[0];
  assert.equal(measured.score_percent, 0);
  assert.equal(measured.score_remark, 'Needs Practice');
  assert.equal(measured.procedural_accuracy_percent, 0);
  assert.equal(measured.waste_quantity, 0);
  assert.equal(measured.data_source, 'demo');
});

test('measured assessment fields populate all categories with explicit completion scope', () => {
  const row = build([learner({ assessment: {
    session_id: 'attempt-1', assessed_at: generatedAt,
    completion_status: 'Completed', completion_percent: 100, completion_scope: 'Cupcake baking practice through cooling',
    safety_score_percent: 80, safety_checks_passed: 4, safety_checks_total: 5, safety_incident_count: 1,
    waste_quantity: 15, waste_unit: 'g', procedural_correct_steps: 14, procedural_assessed_steps: 16
  } })]).rows[0];
  assert.equal(row.procedural_accuracy_percent, 87.5);
  assert.equal(row.safety_score_percent, 80);
  assert.equal(row.completion_percent, 100);
  assert.match(row.completion_scope, /baking practice/);
});

test('invalid measurements and ambiguous reports are rejected', () => {
  for (const assessment of [
    { score_percent: 101 }, { score_percent: '' }, { safety_incident_count: -1 }, { safety_checks_total: 1.5 },
    { safety_checks_passed: 6, safety_checks_total: 5 },
    { procedural_correct_steps: 17, procedural_assessed_steps: 16 },
    { waste_quantity: 1 }, { waste_unit: 'g' }, { completion_status: 'Passed' },
    { procedural_accuracy_percent: '90' }, { assessed_at: 'invalid' }
  ]) assert.throws(() => build([learner({ assessment })]), /invalid section report/);
  assert.throws(() => build([learner(), learner()]), /invalid section report/);
  assert.throws(() => buildSectionReport({id:'all',name:'All Sections'},[]), /Choose a class section/);
});

test('empty section reports preserve the contract and use a safe Excel filename', () => {
  const report = build([]);
  assert.deepEqual(report.rows, []);
  assert.equal(reportColumns.length, 26);
  assert.match(sectionReportFilename(report), /^bakeit-BSHM-2A-section-a-2026-09-29T00-00-00-000Z\.xlsx$/);
  assert.doesNotMatch(sectionReportFilename({ ...report, section: {id:'../bad',name:'<Bad>: / name'} }), /[<>:"/\\|?*]/);
});

test('client validates the report section, schema and rows before download', async () => {
  const report = build([learner()]);
  const cloud = new CloudDataService({ request: async path => {
    assert.equal(path, '/api/reports/sections/section-a');
    return { report };
  } });
  assert.equal(await cloud.getSectionReport('section-a'), report);
  await assert.rejects(cloud.getSectionReport('all'), /Choose a class section/);
  assert.throws(() => validateSectionReport(report, 'section-b'), /invalid section report/);
  assert.throws(() => validateSectionReport({...report, schemaVersion:'2.0'}), /invalid section report/);
  assert.throws(() => validateSectionReport({...report, rows:[{}]}), /invalid section report/);
});

test('static reports are isolated by section and label demonstration rows', async () => {
  const values = new Map();
  const request = createStaticRequest({getItem:key=>values.get(key)??null,setItem:(key,value)=>values.set(key,value)}, null);
  const {report} = await request('/api/reports/sections/section-a');
  assert.equal(report.rows.length, 10);
  assert.ok(report.rows.every(row=>row.section_id==='section-a' && row.data_source==='demo'));
  const {section:empty} = await request('/api/sections',{method:'POST',body:JSON.stringify({name:'Empty section'})});
  assert.deepEqual((await request('/api/reports/sections/'+empty.id)).report.rows,[]);
  await assert.rejects(request('/api/reports/sections/all'), /Choose a class section/);
  await request('/api/sections/section-a',{method:'DELETE'});
  await assert.rejects(request('/api/reports/sections/section-a'), /Section not found/);
});
