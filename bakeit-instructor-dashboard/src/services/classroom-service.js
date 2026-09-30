import { randomInt, randomUUID } from 'node:crypto';
import { RequestError } from '../http/request-error.js';
import { learnerPerformance } from '../../public/assets/js/domain/performance.js';
import { buildSectionReport } from '../../public/assets/js/domain/section-report.js';
import { buildLearnerHistory } from '../../public/assets/js/domain/learner-history.js';

function requiredText(value, label, max) {
  if (typeof value !== 'string' || !value.trim() || value.trim().length > max) {
    throw new RequestError(400, `${label} must contain 1–${max} characters.`);
  }
  return value.trim();
}

// Section/enrollment rules; persistence is supplied by the repository.
export class ClassroomService {
  constructor(repository, codeGenerator = () => {
    const alphabet = 'ABCDEFGHJKLMNPQRSTUVWXYZ23456789';
    return Array.from({ length: 8 }, () => alphabet[randomInt(alphabet.length)]).join('');
  }) {
    this.repository = repository;
    this.codeGenerator = codeGenerator;
  }
  read() { return this.repository.read(); }
  mutate(change) { return this.repository.mutate(change); }
  async listSections() {
    const data = await this.repository.snapshot();
    return data.sections.map(section => ({ ...section,
      learnerCount: data.enrollments.filter(item => item.sectionId === section.id).length
    }));
  }
  createSection(input) {
    const name = requiredText(input.name, 'Section name', 80);
    return this.mutate(data => {
      if (data.sections.some(section => section.name.toLowerCase() === name.toLowerCase())) {
        throw new RequestError(409, 'A section with this name already exists.');
      }
      let classCode;
      for (let attempt = 0; attempt < 100; attempt++) {
        const candidate = this.codeGenerator();
        if (!data.sections.some(section => section.classCode === candidate)) { classCode = candidate; break; }
      }
      if (!classCode) throw new RequestError(503, 'Could not generate a class code. Please try again.');
      const section = { id: randomUUID(), name, classCode, createdAt: new Date().toISOString() };
      data.sections.push(section);
      return { ...section, learnerCount: 0 };
    });
  }
  deleteSection(sectionId) {
    const id = requiredText(sectionId, 'Section ID', 128);
    return this.mutate(data => {
      const section = data.sections.find(item => item.id === id);
      if (!section) throw new RequestError(404, 'Section not found. It may already have been deleted.');
      const removedEnrollments = data.enrollments.filter(item => item.sectionId === id).length;
      data.sections = data.sections.filter(item => item.id !== id);
      data.enrollments = data.enrollments.filter(item => item.sectionId !== id);
      return { section, removedEnrollments };
    });
  }
  joinSection(input) {
    const classCode = requiredText(input.classCode, 'Class code', 8).toUpperCase();
    if (!/^[A-HJ-NP-Z2-9]{8}$/.test(classCode)) throw new RequestError(400, 'Enter a valid 8-character class code.');
    const learnerId = requiredText(input.learnerId, 'Learner ID', 128);
    const learnerName = requiredText(input.learnerName, 'Learner name', 100);
    return this.mutate(data => {
      const section = data.sections.find(item => item.classCode === classCode);
      if (!section) throw new RequestError(404, 'Class code not found. Check the code with your instructor.');
      const existing = data.enrollments.find(item => item.sectionId === section.id && item.learnerId === learnerId);
      if (existing) return { section: { id: section.id, name: section.name }, enrollment: existing, alreadyJoined: true };
      const enrollment = { sectionId: section.id, learnerId, learnerName, joinedAt: new Date().toISOString() };
      data.enrollments.push(enrollment);
      return { section: { id: section.id, name: section.name }, enrollment, alreadyJoined: false };
    });
  }
  async learners(sectionId = 'all') {
    const data = await this.repository.snapshot();
    return this.learnerRows(data, sectionId);
  }
  async sectionReport(sectionId) {
    if (sectionId === 'all') throw new RequestError(400, 'Choose a class section first.');
    const data = await this.repository.snapshot();
    const section = data.sections.find(item => item.id === sectionId);
    if (!section) throw new RequestError(404, 'Section not found. It may already have been deleted.');
    return buildSectionReport(section, this.learnerRows(data, sectionId));
  }
  async learnerHistory(sectionId, learnerId) {
    const data = await this.repository.snapshot();
    const enrollment = data.enrollments.find(item => item.sectionId === sectionId && item.learnerId === learnerId);
    if (!enrollment) throw new RequestError(404, 'Learner not found in this section. Refresh the learner list.');
    const student = this.learnerRows(data, sectionId).find(item => item.id === learnerId);
    return buildLearnerHistory({ ...student, sessionHistory: enrollment.sessionHistory ?? enrollment.demoResult?.sessionHistory });
  }
  learnerRows(data, sectionId) {
    return data.enrollments.filter(item => sectionId === 'all' || item.sectionId === sectionId).map(item => {
      const result = item.demo === true ? item.demoResult : null;
      const score = result?.score ?? null;
      return learnerPerformance({
      assessment: item.assessment ?? result?.assessment,
      sessionHistory: item.sessionHistory ?? result?.sessionHistory,
      legacy_score_percent: result?.score ?? null,
      id: item.learnerId, name: item.learnerName,
      initials: item.learnerName.split(/\s+/).slice(0, 2).map(word => Array.from(word)[0]).join('').toUpperCase(),
      sectionId: item.sectionId, section: data.sections.find(section => section.id === item.sectionId).name,
      joinedAt: item.joinedAt, recipe: result?.recipe ?? 'Not started', sessions: result?.sessions ?? 0, score,
      progress: 0, waste: result?.waste ?? '—',

      demo: item.demo === true
    }); });
  }
}
