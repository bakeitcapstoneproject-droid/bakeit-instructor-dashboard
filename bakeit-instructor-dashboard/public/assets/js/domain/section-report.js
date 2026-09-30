import { scoreRemark, rubricCriteria, rubricTotal, validateRatings } from './performance.js';
import { buildLearnerHistory, validateLearnerHistory } from './learner-history.js';

export const reportVersion = '2.1';
export const reportColumns = [
  'schema_version', 'generated_at', 'section_id', 'section_name', 'learner_id', 'learner_name',
  'data_source', 'recipe', 'session_id', 'assessed_at', 'session_count',
  ...rubricCriteria.map(({ key }) => key + '_rating'), 'total_score', 'max_score',
  'legacy_score_percent', 'score_remark', 'completion_status', 'completion_percent', 'completion_scope',
  'safety_score_percent', 'safety_checks_passed', 'safety_checks_total', 'safety_incident_count',
  'waste_level', 'waste_quantity', 'waste_unit',
  'procedural_correct_steps', 'procedural_assessed_steps', 'procedural_accuracy_percent'
];
const percentages = ['legacy_score_percent', 'completion_percent', 'safety_score_percent', 'procedural_accuracy_percent'];
const counts = ['session_count', 'safety_checks_passed', 'safety_checks_total', 'safety_incident_count', 'procedural_correct_steps', 'procedural_assessed_steps'];
const optional = value => value == null;

// A provider supplies measured assessment fields. Legacy display progress is not
// treated as telemetry; unrecorded measurements stay null, never fabricated zeroes.
export function buildSectionReport(section, students, { generatedAt = new Date().toISOString(), source = 'local' } = {}) {
  if (!section || !section.id || section.id === 'all') throw new Error('Choose a class section first.');
  const rows = students.filter(student => student.sectionId === section.id).map(student => {
    const assessment = student.assessment ?? {};
    if (assessment.ratings != null) {
      try { validateRatings(assessment.ratings); }
      catch { throw new Error('The server returned an invalid section report. Please try again.'); }
    }
    const row = Object.fromEntries(reportColumns.map(key => [key, null]));
    Object.assign(row, {
      schema_version: reportVersion, generated_at: generatedAt,
      section_id: section.id, section_name: section.name, learner_id: student.id, learner_name: student.name,
      data_source: student.demo === true || source === 'demo' ? 'demo' : source,
      recipe: student.recipe === 'Not started' ? null : student.recipe ?? null,
      session_count: student.sessions ?? null, legacy_score_percent: assessment.score_percent ?? student.legacy_score_percent ?? (assessment.ratings ? null : student.score ?? null),
      total_score: rubricTotal(assessment.ratings), max_score: 25,
      waste_level: ['Low', 'Medium', 'High'].includes(student.waste) ? student.waste : null
    });
    for (const key of ['recipe', 'session_id', 'assessed_at', 'completion_status', 'completion_percent',
      'completion_scope', 'safety_score_percent', 'safety_checks_passed', 'safety_checks_total', 'safety_incident_count',
      'waste_level', 'waste_quantity', 'waste_unit', 'procedural_correct_steps', 'procedural_assessed_steps', 'procedural_accuracy_percent']) {
      if (Object.hasOwn(assessment, key)) row[key] = assessment[key];
    }
    for (const { key } of rubricCriteria) row[key + '_rating'] = assessment.ratings?.[key] ?? null;
    row.score_remark = optional(row.total_score) ? null : scoreRemark(row.total_score);
    if (optional(row.completion_status) && row.session_count === 0) row.completion_status = 'Not started';
    if (optional(row.procedural_accuracy_percent) && Number.isInteger(row.procedural_correct_steps)
      && row.procedural_assessed_steps > 0) {
      row.procedural_accuracy_percent = Math.round(row.procedural_correct_steps / row.procedural_assessed_steps * 10000) / 100;
    }
    return row;
  }).sort((a, b) => a.learner_name.localeCompare(b.learner_name) || a.learner_id.localeCompare(b.learner_id));
  const histories = students.filter(student => student.sectionId === section.id).map(buildLearnerHistory);
  return validateSectionReport({ schemaVersion: reportVersion, generatedAt, section: { id: section.id, name: section.name }, rows, histories }, section.id);
}

export function validateSectionReport(report, sectionId = report?.section?.id) {
  const invalid = () => { throw new Error('The server returned an invalid section report. Please try again.'); };
  if (report?.schemaVersion !== reportVersion || !report.section || typeof report.section.id !== 'string'
    || report.section.id === 'all' || !report.section.id || report.section.id !== sectionId
    || typeof report.section.name !== 'string' || !report.section.name.trim()
    || typeof report.generatedAt !== 'string' || !Number.isFinite(Date.parse(report.generatedAt)) || !Array.isArray(report.rows)) invalid();
  const ids = new Set();
  for (const row of report.rows) {
    if (!row || reportColumns.some(key => !Object.hasOwn(row, key))) invalid();
    if (row.schema_version !== reportVersion || row.generated_at !== report.generatedAt
      || row.section_id !== sectionId || row.section_name !== report.section.name
      || typeof row.learner_id !== 'string' || !row.learner_id.trim() || ids.has(row.learner_id)
      || typeof row.learner_name !== 'string' || !row.learner_name.trim()
      || !['local', 'demo', 'aws'].includes(row.data_source)) invalid();
    ids.add(row.learner_id);
    for (const key of percentages) if (!optional(row[key]) && (!Number.isFinite(row[key]) || row[key] < 0 || row[key] > 100)) invalid();
    for (const key of counts) if (!optional(row[key]) && (!Number.isSafeInteger(row[key]) || row[key] < 0)) invalid();
    for (const key of reportColumns.filter(key => ![...percentages, ...counts, 'waste_quantity', 'total_score', 'max_score', ...rubricCriteria.map(({ key }) => key + '_rating')].includes(key))) {
      if (!optional(row[key]) && typeof row[key] !== 'string') invalid();
    }
    if (!optional(row.assessed_at) && !Number.isFinite(Date.parse(row.assessed_at))) invalid();
    const ratings = Object.fromEntries(rubricCriteria.map(({ key }) => [key, row[key + '_rating']]));
    const anyRating = Object.values(ratings).some(value => value != null);
    const total = rubricTotal(ratings);
    if (row.max_score !== 25 || (anyRating && total === null) || row.total_score !== total) invalid();
    if (row.score_remark !== (total === null ? null : scoreRemark(total))) invalid();
    if (!optional(row.completion_status) && !['Not started', 'In progress', 'Completed', 'Abandoned'].includes(row.completion_status)) invalid();
    if (!optional(row.waste_level) && !['Low', 'Medium', 'High'].includes(row.waste_level)) invalid();
    if (!optional(row.waste_quantity) && (!Number.isFinite(row.waste_quantity) || row.waste_quantity < 0 || !row.waste_unit?.trim())) invalid();
    if (optional(row.waste_quantity) && !optional(row.waste_unit)) invalid();
    for (const [part, total] of [['safety_checks_passed', 'safety_checks_total'], ['procedural_correct_steps', 'procedural_assessed_steps']]) {
      if (!optional(row[part]) && !optional(row[total]) && row[part] > row[total]) invalid();
    }
  }
  if (!Array.isArray(report.histories) || report.histories.length !== report.rows.length) invalid();
  const historyIds = new Set();
  for (const history of report.histories) {
    if (!history || !ids.has(history.learnerId) || historyIds.has(history.learnerId)) invalid();
    try { validateLearnerHistory(history, sectionId, history.learnerId); }
    catch { invalid(); }
    historyIds.add(history.learnerId);
  }
  return report;
}

export function sectionReportFilename(report) {
  const safe = value => value.normalize('NFKD').replace(/[^a-zA-Z0-9_-]+/g, '-').replace(/^-+|-+$/g, '').slice(0, 60) || 'section';
  return `bakeit-${safe(report.section.name)}-${safe(report.section.id)}-${new Date(report.generatedAt).toISOString().replace(/[:.]/g, '-')}.xlsx`;
}
