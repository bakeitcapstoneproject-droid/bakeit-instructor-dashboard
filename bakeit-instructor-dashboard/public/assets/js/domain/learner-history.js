import { rubricCriteria, rubricTotal, scoreRemark, validateRatings } from './performance.js';

// Fictional, stable history only for explicitly marked sample learners.
function sampleHistory(student) {
  const count = Number.isSafeInteger(student.sessions) ? Math.max(0, Math.min(student.sessions, 100)) : 0;
  const latest = Date.parse('2026-09-29T02:00:00.000Z');
  return Array.from({ length: count }, (_, index) => {
    const started = latest - (count - index - 1) * 86400000;
    const ratings = rubricTotal(student.assessment?.ratings) === null ? null
      : Object.fromEntries(rubricCriteria.map(({ key }) => [key, Math.max(1, student.assessment.ratings[key] - (count - index - 1) % 3)]));
    return { id: `sample-session-${index + 1}`, recipe: student.recipe,
      startedAt: new Date(started).toISOString(), endedAt: new Date(started + (24 + index * 3) * 60000).toISOString(),
      status: 'Completed', ratings };
  });
}

export function buildLearnerHistory(student) {
  const source = student.sessionHistory ?? (student.demo === true ? sampleHistory(student) : []);
  if (!Array.isArray(source)) throw new Error('Session history could not be read. Please try again.');
  const entries = source.map(entry => {
    if (!entry || typeof entry !== 'object') throw new Error('Session history could not be read. Please try again.');
    if (entry.ratings != null) validateRatings(entry.ratings);
    return {
    id: entry.id, recipe: entry.recipe, startedAt: entry.startedAt ?? null, endedAt: entry.endedAt ?? null,
    status: entry.status, score: rubricTotal(entry.ratings),
    result: scoreRemark(rubricTotal(entry.ratings))
    };
  });
  const history = { learnerId: student.id, sectionId: student.sectionId, sample: student.demo === true,
    sessionCount: Math.max(Number.isSafeInteger(student.sessions) ? student.sessions : 0, entries.length), entries };
  validateLearnerHistory(history, student.sectionId, student.id);
  history.entries.sort((a, b) => (Date.parse(b.startedAt) || 0) - (Date.parse(a.startedAt) || 0) || a.id.localeCompare(b.id));
  return history;
}

export function validateLearnerHistory(history, sectionId, learnerId) {
  const invalid = () => { throw new Error('The server returned invalid session history. Please try again.'); };
  if (!history || history.learnerId !== learnerId || history.sectionId !== sectionId
    || typeof history.sample !== 'boolean' || !Number.isSafeInteger(history.sessionCount) || history.sessionCount < 0
    || !Array.isArray(history.entries) || history.entries.length > history.sessionCount) invalid();
  const ids = new Set();
  const timestamp = value => value === null || (typeof value === 'string'
    && /T.*(?:Z|[+-]\d{2}:\d{2})$/.test(value) && Number.isFinite(Date.parse(value)));
  for (const entry of history.entries) {
    if (!entry || typeof entry.id !== 'string' || !entry.id || ids.has(entry.id)
      || typeof entry.recipe !== 'string' || !entry.recipe.trim()
      || !['Completed', 'In progress', 'Abandoned'].includes(entry.status)
      || !timestamp(entry.startedAt) || !timestamp(entry.endedAt)
      || (entry.startedAt && entry.endedAt && Date.parse(entry.endedAt) < Date.parse(entry.startedAt))
      || (entry.score !== null && (!Number.isInteger(entry.score) || entry.score < 5 || entry.score > 25))
      || entry.result !== scoreRemark(entry.score)) invalid();
    ids.add(entry.id);
  }
  return history;
}
