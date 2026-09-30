import test from 'node:test';
import assert from 'node:assert/strict';
import { readFile } from 'node:fs/promises';
import { buildSectionReport } from '../public/assets/js/domain/section-report.js';
import { sectionReportWorkbook, sectionWorkbookFilename } from '../public/assets/js/domain/section-workbook.js';
import { workbookParts } from './helpers/xlsx.js';

const generatedAt = '2026-09-29T00:00:00.000Z';
const section = { id: 'section-a', name: 'BSHM 2A & 2B' };
const build = students => buildSectionReport(section, students, { generatedAt });
const learner = { id: '00123', name: 'Ana Santos', sectionId: section.id, recipe: 'Cookies', sessions: 2, score: 88 };
const contents = (part, reference) => part.match(new RegExp(`<c r="${reference}"[^>/]*(?:/>|>[\\s\\S]*?</c>)`))?.[0];

test('Excel reports have readable category tabs, highlighted headers, filters and frozen identities', () => {
  const parts = workbookParts(sectionReportWorkbook(build([learner])));
  assert.equal(parts.size, 7);
  assert.deepEqual([...parts.get('xl/workbook.xml').matchAll(/<sheet name="([^"]+)"/g)].map(match=>match[1]), ['Class performance','Session history']);
  for (let sheet = 1; sheet <= 2; sheet++) {
    const data = parts.get(`xl/worksheets/sheet${sheet}.xml`);
    assert.match(data, /state="frozen"/);
    assert.match(data, /ySplit="6"/);
    assert.match(data, /<autoFilter ref="A6:[A-Z]+[67]"/);
    assert.match(data, /customWidth="1"/);
    assert.match(data, /BSHM 2A &amp; 2B/);
    assert.match(data, /orientation="landscape"/);
  }
  assert.match(parts.get('xl/styles.xml'), /FFFFAE38/);
  const fonts = [...parts.get('xl/styles.xml').matchAll(/<font>(.*?)<\/font>/g)];
  assert.equal(fonts.length, 5);
  assert.ok(fonts.every(match => /<color rgb="FF000000"\/>/.test(match[1])));
  assert.doesNotMatch(parts.get('xl/styles.xml'), /<dxfs/);
  assert.match(parts.get('xl/worksheets/sheet1.xml'), /Learner name/);
  assert.match(sectionWorkbookFilename(build([])), /\.xlsx$/);
});

test('Excel matches ten-digit display IDs, history dates in PHT, and monitoring results without changing source identities', () => {
  const ratings = {decorum:4,kitchen_organization:4,safety_sanitation:5,baking_skills:4,product_appraisal:5};
  const report = build([{...learner,id:'S-0241',demo:true,assessment:{ratings}},
    {...learner,id:'S-0242',demo:true,sessions:0,assessment:null},
    {...learner,id:'S-0243',demo:true,sessions:1,assessment:{ratings:{...ratings,decorum:1,kitchen_organization:1,safety_sanitation:2,baking_skills:2,product_appraisal:2}}}]);
  const parts = workbookParts(sectionReportWorkbook(report));
  for (const sheet of [1,2]) {
    assert.match(parts.get(`xl/worksheets/sheet${sheet}.xml`), /0000000241<\/t>/);
    assert.doesNotMatch(parts.get(`xl/worksheets/sheet${sheet}.xml`), /S-0241<\/t>/);
  }
  const performance = parts.get('xl/worksheets/sheet1.xml');
  assert.match(contents(performance, 'L7'), />Passed<\/t>/);
  assert.match(contents(performance, 'L8'), />Awaiting assessment<\/t>/);
  assert.match(contents(performance, 'L9'), />Failed<\/t>/);
  assert.doesNotMatch(performance, /conditionalFormatting/);
  for (const row of [7,8,9]) assert.match(contents(performance, `L${row}`), /s="15"/);
  const styles = parts.get('xl/styles.xml');
  const cellStyles = [...styles.match(/<cellXfs[^>]*>([\s\S]*?)<\/cellXfs>/)[1].matchAll(/<xf\b[^>]*>/g)];
  assert.match(cellStyles[15][0], /fontId="0" fillId="0"/);
  const history = parts.get('xl/worksheets/sheet2.xml');
  for (const row of [7,8,9]) assert.match(contents(history, `H${row}`), /s="15"/);
  assert.equal((history.match(/<row r="(?:7|8|9)"/g)||[]).length, 3);
  assert.match(contents(history, 'G7'), /<v>22<\/v>/);
  assert.match(history, /Started \(PHT\)/);
  const actualDate = Number(contents(history, 'E7').match(/<v>([^<]+)<\/v>/)[1]);
  assert.ok(Math.abs(actualDate - (Date.parse('2026-09-29T10:00:00Z')/86400000 + 25569)) < 0.0000001);
  assert.match(parts.get('xl/styles.xml'), /formatCode="mmm d, yyyy h:mm AM\/PM"/);
  assert.equal(report.rows[0].learner_id, 'S-0241');
  const emptyHistory = workbookParts(sectionReportWorkbook(build([learner]))).get('xl/worksheets/sheet2.xml');
  assert.doesNotMatch(emptyHistory, /<row r="7"/);
});

test('Excel reports omit completion columns without changing provider data', () => {
  const report = build([{ ...learner, assessment: { ratings: {decorum:4,kitchen_organization:3,safety_sanitation:5,baking_skills:4,product_appraisal:5}, score_percent:88, completion_percent: 0, safety_incident_count: 0,
    procedural_correct_steps: 14, procedural_assessed_steps: 16 } }]);
  const before = structuredClone(report);
  const parts = workbookParts(sectionReportWorkbook(report));
  const scores = parts.get('xl/worksheets/sheet1.xml');
  assert.match(contents(scores, 'J7'), /<v>21<\/v>/);
  assert.match(scores, /<dimension ref="A1:L7"/);
  assert.doesNotMatch(scores, /Completion status|Completion \(%\)|Completion scope/);
  assert.match(contents(scores, 'D7'), /\/>$/);
  assert.deepEqual(report, before);
  assert.equal(report.rows[0].legacy_score_percent, 88, 'Export cannot mutate the provider data');
});

test('Excel preserves leading zeroes and treats formula-like and Unicode content as literal text', () => {
  const report = build([{ ...learner, name: '=HYPERLINK("https://example.invalid") & Ñ 🍞 _x0041_' },
    { ...learner, id: '00002', name: '+Ana\nSantos' }].map(record => ({...record, sessionHistory:[{id:'one',recipe:'Cookies',status:'Completed',startedAt:generatedAt,endedAt:generatedAt}]})));
  const parts = workbookParts(sectionReportWorkbook(report));
  for (let sheet = 1; sheet <= 2; sheet++) {
    const data = parts.get(`xl/worksheets/sheet${sheet}.xml`);
    assert.doesNotMatch(data, /<f[ >]/);
    assert.match(data, /00123<\/t>/);
    assert.match(data, /=HYPERLINK\(&quot;/);
    assert.match(data, /&amp; Ñ 🍞 _x005F_x0041_/);
  }
});

test('blank Excel templates have writable styled rows; empty sections never invent learner rows', async () => {
  const blank = build([]);
  const empty = workbookParts(sectionReportWorkbook(blank));
  assert.doesNotMatch(empty.get('xl/worksheets/sheet1.xml'), /<row r="7"/);
  const template = workbookParts(await readFile('public/assets/reports/section-report-template.xlsx'));
  const example = workbookParts(await readFile('public/assets/reports/section-report-example.xlsx'));
  for (const artifact of [template, example]) {
    assert.doesNotMatch(artifact.get('xl/worksheets/sheet1.xml'), /Completion status|Completion \(%\)|Completion scope/);
    assert.deepEqual([...artifact.get('xl/workbook.xml').matchAll(/<sheet name="([^"]+)"/g)].map(match=>match[1]), ['Class performance','Session history']);
    assert.equal(artifact.size, 7);
    const fonts = [...artifact.get('xl/styles.xml').matchAll(/<font>(.*?)<\/font>/g)];
    assert.ok(fonts.every(match => /<color rgb="FF000000"\/>/.test(match[1])));
    for (const sheet of [1, 2]) assert.doesNotMatch(artifact.get(`xl/worksheets/sheet${sheet}.xml`), /conditionalFormatting/);
  }
  assert.match(template.get('xl/worksheets/sheet1.xml'), /<row r="18"/);
  assert.match(template.get('xl/worksheets/sheet1.xml'), /Blank report template/);
  assert.match(contents(template.get('xl/worksheets/sheet1.xml'), 'A7'), /\/>$/);
  assert.throws(() => sectionReportWorkbook({ ...blank, schemaVersion: 'invalid' }), /invalid section report/);
  assert.throws(() => sectionReportWorkbook(build([{ ...learner, name: 'A'.repeat(32768) }])), /cell limit/);
});
