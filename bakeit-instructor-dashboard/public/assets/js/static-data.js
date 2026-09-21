import { students as sampleStudents, sessions as sampleSessions } from './data.js';

export const staticStorageKey = 'bakeit_static_workspace_v1';

function seed() {
  const createdAt = new Date().toISOString();
  return {
    sections: [
      { id: 'section-a', name: 'Section A', classCode: 'DEMA2345', createdAt },
      { id: 'section-b', name: 'Section B', classCode: 'DEMB2345', createdAt }
    ],
    students: sampleStudents.map(student => ({ ...student, joinedAt: createdAt })),
    sessions: structuredClone(sampleSessions)
  };
}

// All mutations read and write synchronously inside a cross-tab Web Lock.
// No network request or server-side data is involved in the static demo.
export function createStaticRequest(storage, locks = globalThis.navigator?.locks) {
  function read() {
    let saved;
    try { saved = storage.getItem(staticStorageKey); }
    catch { throw new Error('Allow browser storage to use this demo.'); }
    if (saved === null) {
      const data = seed();
      save(data);
      return data;
    }
    try {
      const data = JSON.parse(saved);
      if (!data || !['sections', 'students', 'sessions'].every(key => Array.isArray(data[key]))) throw new Error();
      return data;
    } catch { throw new Error('Saved demo data could not be read. Clear this site’s browser data to reset the demo.'); }
  }
  function save(data) {
    try { storage.setItem(staticStorageKey, JSON.stringify(data)); }
    catch { throw new Error('Could not save demo changes. Allow browser storage and check available space.'); }
  }
  function handle(path, options) {
    const url = new URL(path, 'https://bakeit.invalid');
    const method = options.method || 'GET';
    const data = read();
    const sectionId = url.searchParams.get('sectionId') || 'all';
    const filter = items => items.filter(item => sectionId === 'all' || item.sectionId === sectionId);
    if (method === 'GET' && url.pathname === '/api/sections') {
      return { sections: data.sections.map(section => ({ ...section,
        learnerCount: data.students.filter(student => student.sectionId === section.id).length
      })) };
    }
    if (method === 'POST' && url.pathname === '/api/sections') {
      let input;
      try { input = JSON.parse(options.body); } catch { throw new Error('Enter a section name.'); }
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
      save(data);
      return { section: { ...section, learnerCount: 0 } };
    }
    if (method === 'DELETE' && url.pathname.startsWith('/api/sections/')) {
      const id = decodeURIComponent(url.pathname.slice('/api/sections/'.length));
      const section = data.sections.find(item => item.id === id);
      if (!section) throw new Error('Section not found. It may already have been deleted.');
      const removedEnrollments = data.students.filter(student => student.sectionId === id).length;
      data.sections = data.sections.filter(item => item.id !== id);
      data.students = data.students.filter(item => item.sectionId !== id);
      data.sessions = data.sessions.filter(item => item.sectionId !== id);
      save(data);
      return { section, removedEnrollments };
    }
    if (method === 'GET' && url.pathname === '/api/learners') return { students: filter(data.students).map(student => ({
      ...student, status: student.score == null ? 'Not started' : student.score >= 60 ? 'Passed' : 'Needs Practice'
    })) };
    if (method === 'GET' && url.pathname === '/api/sessions') return { sessions: filter(data.sessions) };
    if (method === 'GET' && url.pathname === '/api/activities') {
      return { activities: filter(data.students).slice(0, 5).map(student => ({
        text: `${student.name} joined ${student.section}`, time: student.joinedAt
      })) };
    }
    throw new Error('This action is unavailable in the static demo.');
  }
  return async (path, options = {}) => {
    if (locks) return locks.request(staticStorageKey, () => handle(path, options));
    return handle(path, options);
  };
}
