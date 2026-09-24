import { dataMode } from '../runtime-config.js';
import { apiRequest } from './api-client.js';
import { scoreRemark } from '../domain/performance.js';

export class CloudDataService {
  constructor({ mode = 'api', request = apiRequest } = {}) { this.mode = mode; this.request = request; }
  async list(path, key) {
    const body = await this.request(path);
    const valid = item => {
      if (!item || typeof item !== 'object') return false;
      if (key === 'students') return ['id', 'name', 'sectionId'].every(field => typeof item[field] === 'string')
        && (item.score == null || (Number.isFinite(item.score) && item.score >= 0 && item.score <= 100));
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
    if (this.mode !== 'mock') return this.list(`/api/learners?sectionId=${encodeURIComponent(sectionId)}`, 'students');
    const { students } = await import('../data.js');
    return structuredClone(this.filterBySection(students, sectionId))
      .map(student => ({ ...student, status: scoreRemark(student.score) }));
  }
  async getLiveSessions(sectionId = 'all') {
    if (this.mode !== 'mock') return this.list(`/api/sessions?sectionId=${encodeURIComponent(sectionId)}`, 'sessions');
    const { sessions } = await import('../data.js');
    return structuredClone(this.filterBySection(sessions, sectionId));
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
}
