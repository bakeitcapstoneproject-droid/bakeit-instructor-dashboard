import { BrowserWorkspaceRepository, staticStorageKey } from './services/browser-workspace-repository.js';
import { scoreRemark } from './domain/performance.js';

export { staticStorageKey, BrowserWorkspaceRepository };

export class StaticDataService {
  constructor(storage, locks = globalThis.navigator?.locks, repository = new BrowserWorkspaceRepository(storage, locks)) {
    this.repository = repository;
  }
  async request(path, options = {}) {
    return this.repository.transact(data => this.handle(path, options, data));
  }
  handle(path, options, data) {
    const url = new URL(path, 'https://bakeit.invalid');
    const method = options.method || 'GET';
    const sectionId = url.searchParams.get('sectionId') || 'all';
    const filter = items => items.filter(item => sectionId === 'all' || item.sectionId === sectionId);
    if (method === 'GET' && url.pathname === '/api/sections') {
      return { sections: data.sections.map(section => ({ ...section,
        learnerCount: data.students.filter(student => student.sectionId === section.id).length
      })) };
    }
    if (method === 'POST' && url.pathname === '/api/sections') return this.createSection(data, options.body);
    if (method === 'DELETE' && url.pathname.startsWith('/api/sections/')) return this.deleteSection(data, decodeURIComponent(url.pathname.slice('/api/sections/'.length)));
    if (method === 'GET' && url.pathname === '/api/learners') return { students: filter(data.students).map(student => ({
      ...student, status: scoreRemark(student.score)
    })) };
    if (method === 'GET' && url.pathname === '/api/sessions') return { sessions: filter(data.sessions) };
    if (method === 'GET' && url.pathname === '/api/activities') {
      return { activities: filter(data.students).slice(0, 5).map(student => ({
        text: `${student.name} joined ${student.section}`, time: student.joinedAt
      })) };
    }
    throw new Error('This action is unavailable in the static demo.');
  }
  createSection(data, body) {
    let input;
    try { input = JSON.parse(body); } catch { throw new Error('Enter a section name.'); }
    const name = typeof input?.name === 'string' ? input.name.trim() : '';
    if (!name || name.length > 80) throw new Error('Section name must contain 1–80 characters.');
    if (data.sections.some(section => section.name.toLowerCase() === name.toLowerCase())) {
      throw new Error('A section with this name already exists.');
    }
    const alphabet = 'ABCDEFGHJKLMNPQRSTUVWXYZ23456789';
    let classCode;
    for (let attempt = 0; attempt < 100; attempt++) {
      const bytes = crypto.getRandomValues(new Uint8Array(8));
      const candidate = Array.from(bytes, byte => alphabet[byte % alphabet.length]).join('');
      if (!data.sections.some(section => section.classCode === candidate)) { classCode = candidate; break; }
    }
    if (!classCode) throw new Error('Could not generate a class code. Please try again.');
    const section = { id: crypto.randomUUID(), name, classCode, createdAt: new Date().toISOString() };
    data.sections.push(section);
    this.repository.save(data);
    return { section: { ...section, learnerCount: 0 } };
  }

  deleteSection(data, id) {
    const section = data.sections.find(item => item.id === id);
    if (!section) throw new Error('Section not found. It may already have been deleted.');
    const removedEnrollments = data.students.filter(student => student.sectionId === id).length;
    data.sections = data.sections.filter(item => item.id !== id);
    data.students = data.students.filter(item => item.sectionId !== id);
    data.sessions = data.sessions.filter(item => item.sectionId !== id);
    this.repository.save(data);
    return { section, removedEnrollments };
  }
}

// Backward-compatible function adapter for callers that accept request callbacks.
export function createStaticRequest(storage, locks = globalThis.navigator?.locks) {
  const service = new StaticDataService(storage, locks);
  return service.request.bind(service);
}
