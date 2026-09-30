import { dataMode } from '../runtime-config.js';
import { apiRequest } from './api-client.js';
import { learnerPerformance, rubricTotal } from '../domain/performance.js';
import { validateSectionReport } from '../domain/section-report.js';
import { buildLearnerHistory, validateLearnerHistory } from '../domain/learner-history.js';

export class CloudDataService {
  constructor({ mode = 'api', request = apiRequest } = {}) { this.mode = mode; this.request = request; }
  async list(path, key) {
    const body = await this.request(path);
    const valid = item => {
      if (!item || typeof item !== 'object') return false;
      if (key === 'students') return ['id', 'name', 'sectionId'].every(field => typeof item[field] === 'string')
        && (item.score == null || (Number.isFinite(item.score) && item.score >= 0 && item.score <= 100))
        && (item.assessment?.ratings == null || rubricTotal(item.assessment.ratings) !== null);
      if (key === 'sessions') return typeof item.student === 'string' && typeof item.sectionId === 'string'
        && (item.events == null || Array.isArray(item.events));
      return typeof item.text === 'string' && Number.isFinite(Date.parse(item.time));
    };
    if (!Array.isArray(body?.[key]) || !body[key].every(valid)) {
      throw new Error('The server returned invalid data. Please try again.');
    }
    return body[key];
  }
  filterBySection(items, sectionId) {
    return sectionId === 'all' ? items : items.filter(item => item.sectionId === sectionId);
  }
  async getStudents(sectionId = 'all') {
    if (this.mode !== 'mock') return (await this.list(`/api/learners?sectionId=${encodeURIComponent(sectionId)}`, 'students')).map(learnerPerformance);
    const { students } = await import('../data.js');
    return structuredClone(this.filterBySection(students, sectionId))
      .map(learnerPerformance);
  }
  async getLiveSessions(sectionId = 'all') {
    if (this.mode !== 'mock') return this.list(`/api/sessions?sectionId=${encodeURIComponent(sectionId)}`, 'sessions');
    const { sessions } = await import('../data.js');
    return structuredClone(this.filterBySection(sessions, sectionId));
  }
  async getLearnerHistory(sectionId, learnerId, options = {}) {
    if (!sectionId || sectionId === 'all') throw new Error('Choose a class section first.');
    if (this.mode === 'mock') {
      const student = (await this.getStudents(sectionId)).find(item => item.id === learnerId);
      if (!student) throw new Error('Learner not found in this section.');
      return buildLearnerHistory(student);
    }
    const body = await this.request(`/api/learners/${encodeURIComponent(learnerId)}/history?sectionId=${encodeURIComponent(sectionId)}`, options);
    return validateLearnerHistory(body?.history, sectionId, learnerId);
  }
  async getActivities(sectionId = 'all') {
    if (this.mode !== 'mock') return this.list(`/api/activities?sectionId=${encodeURIComponent(sectionId)}`, 'activities');
    const { activities } = await import('../data.js');
    return structuredClone(this.filterBySection(activities, sectionId));
  }
  async getHealth() {
    if (this.mode !== 'mock') await this.request('/api/sections');
    return { connected: true, source: this.mode === 'mock' ? 'Prototype data' : dataMode === 'static' ? 'Browser storage' : 'Class server', updated: new Date() };
  }
  async getSectionReport(sectionId) {
    if (!sectionId || sectionId === 'all') throw new Error('Choose a class section first.');
    const body = await this.request(`/api/reports/sections/${encodeURIComponent(sectionId)}`);
    return validateSectionReport(body?.report, sectionId);
  }
}
