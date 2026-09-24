import { students as sampleStudents, sessions as sampleSessions } from '../data.js';

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

export class BrowserWorkspaceRepository {
  constructor(storage, locks = globalThis.navigator?.locks) { this.storage = storage; this.locks = locks; }
  read() {
    let saved;
    try { saved = this.storage.getItem(staticStorageKey); }
    catch { throw new Error('Allow browser storage to use this demo.'); }
    if (saved === null) {
      const data = seed();
      this.save(data);
      return data;
    }
    try {
      const data = JSON.parse(saved);
      if (!data || !['sections', 'students', 'sessions'].every(key => Array.isArray(data[key]))) throw new Error();
      const text = value => typeof value === 'string';
      if (data.sections.some(section => !section || !text(section.id) || !text(section.name) || !text(section.classCode))
        || new Set(data.sections.map(section => section.id)).size !== data.sections.length
        || data.students.some(student => !student || !text(student.id) || !text(student.name) || !text(student.sectionId))
        || data.sessions.some(session => !session || !text(session.sectionId) || !text(session.student)
          || (session.events != null && !Array.isArray(session.events)))
        || [...data.students, ...data.sessions].some(item => !data.sections.some(section => section.id === item.sectionId))) throw new Error();
      return data;
    } catch { throw new Error('Saved demo data could not be read. Clear this site’s browser data to reset the demo.'); }
  }
  save(data) {
    try { this.storage.setItem(staticStorageKey, JSON.stringify(data)); }
    catch { throw new Error('Could not save demo changes. Allow browser storage and check available space.'); }
  }
  transact(action) {
    // Reads and mutations share a cross-tab lock; action must stay synchronous.
    if (this.locks) return this.locks.request(staticStorageKey, () => action(this.read()));
    return action(this.read());
  }
}
