import { AppShell, CloudDataService, TableView } from './core.js';
import { SectionManager } from './sections.js';

class StudentsPage {
  constructor() {
    this.shell = new AppShell();
    this.cloud = new CloudDataService();
    this.table = new TableView(document.querySelector('tbody'));
  }
  async init() {
    await this.shell.init();
    this.students = await this.cloud.getStudents();
    this.sectionManager = new SectionManager(this.shell, () => this.render(), section => {
      this.students = this.students.filter(student => student.sectionId !== section.id);
      if (new URLSearchParams(location.hash.slice(1)).get('section') === section.id) {
        history.replaceState(null, '', location.pathname + location.search);
      }
      this.showSection();
    });
    this.sectionManager.init();
    this.optionsToggle = document.querySelector('[data-section-options]');
    this.optionsPanel = document.querySelector('#section-options');
    this.optionsToggle.addEventListener('click', () => this.setOptionsOpen(this.optionsPanel.hidden));
    const actions = document.querySelector('.section-detail-actions');
    document.addEventListener('click', event => {
      if (!actions.contains(event.target)) this.setOptionsOpen(false);
    });
    actions.addEventListener('focusout', event => {
      if (!actions.contains(event.relatedTarget)) this.setOptionsOpen(false);
    });
    actions.addEventListener('keydown', event => {
      if (event.key === 'Escape' && !this.optionsPanel.hidden) {
        event.preventDefault();
        this.setOptionsOpen(false, true);
      }
    });
    window.addEventListener('hashchange', () => this.showSection(true));
    document.querySelector('[data-copy-detail]').addEventListener('click', () => {
      const section = this.currentSection();
      if (section) this.sectionManager.copy(section.classCode);
      this.setOptionsOpen(false, true);
    });
    document.querySelector('[data-delete-detail]').addEventListener('click', () => {
      const section = this.currentSection();
      this.setOptionsOpen(false);
      if (section) this.sectionManager.openDelete(section.id, this.optionsToggle);
    });
    document.querySelectorAll('[data-filter]').forEach(element => element.addEventListener('input', () => this.render()));
    this.shell.watch(async () => {
      this.students = await this.cloud.getStudents();
      this.sectionManager.render();
      this.showSection();
    });
    this.showSection();
  }
  setOptionsOpen(open, restoreFocus = false) {
    this.optionsPanel.hidden = !open;
    this.optionsToggle.setAttribute('aria-expanded', String(open));
    if (restoreFocus) this.optionsToggle.focus();
  }
  currentSection() {
    const id = new URLSearchParams(location.hash.slice(1)).get('section');
    return this.shell.sections.sections.find(section => section.id !== 'all' && section.id === id);
  }
  showSection(moveFocus = false) {
    const section = this.currentSection();
    const previous = this.openSectionId;
    this.openSectionId = section?.id;
    document.querySelector('.learner-sections').hidden = !!section;
    document.querySelector('#learner-records').hidden = !section;
    if (previous !== this.openSectionId) {
      this.setOptionsOpen(false);
      document.querySelectorAll('[data-filter]').forEach(element => { element.value = ''; });
      document.querySelector('[data-section-message]').textContent = '';
    }
    if (section) {
      this.shell.sections.select(section.id);
      document.querySelector('#section-title').textContent = section.name;
      document.querySelector('[data-detail-code]').textContent = section.classCode;
      this.render();
    }
    if (moveFocus) {
      const target = section ? document.querySelector('#section-title')
        : [...document.querySelectorAll('[data-view]')].find(link => link.dataset.view === previous)
          || document.querySelector('[data-open-create]');
      target?.focus({ preventScroll: true });
      window.scrollTo({ top: 0 });
    }
  }
  render() {
    const section = this.currentSection();
    if (!section) return;
    const query = document.querySelector('#search').value.trim().toLowerCase();
    const status = document.querySelector('#status').value;
    const recipe = document.querySelector('#recipe').value;
    const learners = this.students.filter(student => student.sectionId === section.id);
    document.querySelector('[data-learner-count]').textContent = `${learners.length} learner${learners.length === 1 ? '' : 's'}`;
    this.table.render(learners.filter(student =>
      (student.name.toLowerCase().includes(query) || student.id.toLowerCase().includes(query)) &&
      (!status || student.status === status) && (!recipe || student.recipe === recipe)
    ));
  }
}

const page = new StudentsPage();
page.init().catch(error => page.shell.showError(error));
