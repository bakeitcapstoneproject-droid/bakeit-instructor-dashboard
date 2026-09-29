import test from 'node:test';
import assert from 'node:assert/strict';
import { readFile } from 'node:fs/promises';
import { buildSectionReport, reportColumns } from '../public/assets/js/domain/section-report.js';
import { sectionReportWorkbook, sectionWorkbookFilename } from '../public/assets/js/domain/section-workbook.js';
import { workbookParts } from './helpers/xlsx.js';

const generatedAt = '2026-09-29T00:00:00.000Z';
const section = { id: 'section-a', name: 'BSHM 2A & 2B' };
const build = students => buildSectionReport(section, students, { generatedAt });
const learner = { id: '00123', name: 'Ana Santos', sectionId: section.id, recipe: 'Cookies', sessions: 2, score: 88 };
const contents = (part, reference) => part.match(new RegExp(`<c r="${reference}"[^>/]*(?:/>|>[\\s\\S]*?</c>)`))?.[0];

test('Excel reports have readable category tabs, highlighted headers, filters and frozen identities', () => {
  const parts = workbookParts(sectionReportWorkbook(build([learner])));
  assert.equal(parts.size, 9);
  for (const title of ['Scores and completion', 'Safety and waste', 'Procedural accuracy', 'Report data']) {
    assert.ok(parts.get('xl/workbook.xml').includes(`name="${title}"`));
  }
  for (let sheet = 1; sheet <= 4; sheet++) {
    const data = parts.get(`xl/worksheets/sheet${sheet}.xml`);
    assert.match(data, /state="frozen"/);
    assert.match(data, /ySplit="6"/);
    assert.match(data, /<autoFilter ref="A6:[A-Z]+7"/);
    assert.match(data, /customWidth="1"/);
    assert.match(data, /BSHM 2A &amp; 2B/);
    assert.match(data, /orientation="landscape"/);
  }
  assert.match(parts.get('xl/styles.xml'), /FFFFAE38/);
  assert.match(parts.get('xl/styles.xml'), /FF704329/);
  assert.match(parts.get('xl/worksheets/sheet1.xml'), /Learner name/);
  assert.match(sectionWorkbookFilename(build([])), /\.xlsx$/);
});

test('Excel reports keep numeric percentages, recorded zeroes, unknown blanks and original raw data', () => {
  const report = build([{ ...learner, assessment: { completion_percent: 0, safety_incident_count: 0,
    procedural_correct_steps: 14, procedural_assessed_steps: 16 } }]);
  const parts = workbookParts(sectionReportWorkbook(report));
  const scores = parts.get('xl/worksheets/sheet1.xml'), safety = parts.get('xl/worksheets/sheet2.xml');
  assert.match(contents(scores, 'F7'), /<v>0.88<\/v>/);
  assert.match(contents(scores, 'I7'), /<v>0<\/v>/);
  assert.match(contents(safety, 'E7'), /\/>$/);
  assert.match(contents(safety, 'H7'), /<v>0<\/v>/);
  assert.match(contents(parts.get('xl/worksheets/sheet3.xml'), 'G7'), /<v>0.875<\/v>/);
  const raw = parts.get('xl/worksheets/sheet4.xml');
  for (const key of reportColumns) assert.ok(raw.includes(`>${key}</t>`), key);
  assert.match(contents(raw, 'L7'), /<v>88<\/v>/);
  assert.equal(report.rows[0].score_percent, 88, 'Export cannot mutate the provider data');
});

test('Excel preserves leading zeroes and treats formula-like and Unicode content as literal text', () => {
  const report = build([{ ...learner, name: '=HYPERLINK("https://example.invalid") & Ñ 🍞 _x0041_' },
    { ...learner, id: '00002', name: '+Ana\nSantos' }]);
  const parts = workbookParts(sectionReportWorkbook(report));
  for (let sheet = 1; sheet <= 4; sheet++) {
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
  assert.match(template.get('xl/worksheets/sheet1.xml'), /<row r="18"/);
  assert.match(template.get('xl/worksheets/sheet1.xml'), /Blank report template/);
  assert.match(contents(template.get('xl/worksheets/sheet1.xml'), 'A7'), /\/>$/);
  assert.throws(() => sectionReportWorkbook({ ...blank, schemaVersion: 'invalid' }), /invalid section report/);
  assert.throws(() => sectionReportWorkbook(build([{ ...learner, name: 'A'.repeat(32768) }])), /cell limit/);
});
