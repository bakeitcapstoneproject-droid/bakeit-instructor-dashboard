import { StorageService } from './storage-service.js';
import { apiRequest } from './api-client.js';

export class SectionService {
  constructor(store = new StorageService(), request = apiRequest) {
    this.store = store;
    this.request = request;
    this.revision = 0;
    this.loadSequence = 0;
    this.sections = [{ id: 'all', name: 'All Sections' }];
  }
  async load() {
    const revision = this.revision;
    const sequence = ++this.loadSequence;
    const { sections } = await this.request('/api/sections');
    if (!Array.isArray(sections) || sections.some(section => !section || typeof section.id !== 'string' || typeof section.name !== 'string')) {
      throw new Error('The server returned invalid class data. Please try again.');
    }
    if (revision !== this.revision || sequence !== this.loadSequence) return this.sections.filter(section => section.id !== 'all');
    this.sections = [{ id: 'all', name: 'All Sections' }, ...sections];
    this.select(this.selected());
    return sections;
  }
  async create(name) {
    const { section } = await this.request('/api/sections', { method: 'POST', body: JSON.stringify({ name }) });
    this.revision += 1;
    this.sections = [...this.sections.filter(item => item.id !== section.id), section];
    this.select(section.id);
    return section;
  }
  async delete(sectionId) {
    const result = await this.request(`/api/sections/${encodeURIComponent(sectionId)}`, { method: 'DELETE' });
    this.revision += 1;
    this.sections = this.sections.filter(section => section.id !== sectionId);
    this.select(this.selected());
    return result;
  }
  selected() {
    const saved = this.selection ?? this.store.get('bakeit_section', 'all');
    return this.sections.some(section => section.id === saved) ? saved : 'all';
  }
  select(sectionId) {
    const valid = this.sections.some(section => section.id === sectionId) ? sectionId : 'all';
    this.selection = valid;
    // A preference write must not turn an already saved class mutation into a failure.
    try { this.store.set('bakeit_section', valid); } catch {}
    return valid;
  }
  name(sectionId = this.selected()) {
    return this.sections.find(section => section.id === sectionId)?.name ?? 'All Sections';
  }
}
