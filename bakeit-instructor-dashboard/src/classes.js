import { randomInt, randomUUID } from 'node:crypto';
import { mkdir, readFile, rename, writeFile } from 'node:fs/promises';
import { dirname } from 'node:path';

export class RequestError extends Error {
  constructor(status, message) { super(message); this.status = status; }
}

function requiredText(value, label, max) {
  if (typeof value !== 'string' || !value.trim() || value.trim().length > max) {
    throw new RequestError(400, `${label} must contain 1–${max} characters.`);
  }
  return value.trim();
}

// One server process owns this file. Serialize writes and replace it atomically.
export class ClassStore {
  constructor(file, codeGenerator = () => {
    const alphabet = 'ABCDEFGHJKLMNPQRSTUVWXYZ23456789';
    return Array.from({ length: 8 }, () => alphabet[randomInt(alphabet.length)]).join('');
  }) {
    this.file = file;
    this.codeGenerator = codeGenerator;
    this.pending = Promise.resolve();
  }
  async read() {
    try { return JSON.parse(await readFile(this.file, 'utf8')); }
    catch (error) {
      if (error.code === 'ENOENT') return { sections: [], enrollments: [] };
      throw error;
    }
  }
  mutate(change) {
    const operation = this.pending.then(async () => {
      const data = await this.read();
      const result = change(data);
      await mkdir(dirname(this.file), { recursive: true });
      await writeFile(`${this.file}.tmp`, JSON.stringify(data, null, 2), 'utf8');
      await rename(`${this.file}.tmp`, this.file);
      return result;
    });
    this.pending = operation.catch(() => {});
    return operation;
  }
  async listSections() {
    await this.pending;
    const data = await this.read();
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
    await this.pending;
    const data = await this.read();
    return data.enrollments.filter(item => sectionId === 'all' || item.sectionId === sectionId).map(item => {
      const result = item.demo === true ? item.demoResult : null;
      const score = result?.score ?? null;
      return {
      id: item.learnerId, name: item.learnerName,
      initials: item.learnerName.split(/\s+/).slice(0, 2).map(word => Array.from(word)[0]).join('').toUpperCase(),
      sectionId: item.sectionId, section: data.sections.find(section => section.id === item.sectionId).name,
      joinedAt: item.joinedAt, recipe: result?.recipe ?? 'Not started', sessions: result?.sessions ?? 0, score,
      progress: 0, waste: result?.waste ?? '—',
      status: score === null ? 'Not started' : score >= 60 ? 'Passed' : 'Needs Practice', rating: 0,
      demo: item.demo === true
    }; });
  }
}

export async function readJson(req) {
  if (req.headers['content-type']?.split(';')[0].trim().toLowerCase() !== 'application/json') {
    throw new RequestError(415, 'Send a JSON request with Content-Type: application/json.');
  }
  const chunks = [];
  let size = 0;
  for await (const chunk of req) {
    size += chunk.length;
    if (size > 8192) throw new RequestError(413, 'Request body is too large.');
    chunks.push(chunk);
  }
  try {
    const value = JSON.parse(Buffer.concat(chunks).toString('utf8'));
    if (!value || typeof value !== 'object' || Array.isArray(value)) throw new Error();
    return value;
  } catch { throw new RequestError(400, 'Send a valid JSON object.'); }
}
