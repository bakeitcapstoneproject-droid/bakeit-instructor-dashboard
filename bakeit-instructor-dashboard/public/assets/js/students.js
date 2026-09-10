import { AppShell, CloudDataService, TableView } from './core.js';

class StudentsPage {
  constructor() {
    this.shell = new AppShell();
    this.cloud = new CloudDataService();
    this.table = new TableView(document.querySelector('tbody'));
  }
  async init() {
    this.shell.init();
    this.students = await this.cloud.getStudents();
    document.querySelectorAll('[data-filter]').forEach(element => element.addEventListener('input', () => this.render()));
    document.addEventListener('bakeit:section-change', () => this.render());
    this.render();
  }
  render() {
    const query = document.querySelector('#search').value.trim().toLowerCase();
    const status = document.querySelector('#status').value;
    const recipe = document.querySelector('#recipe').value;
    const sectionId = this.shell.sections.selected();
    this.table.render(this.students.filter(student =>
      (sectionId === 'all' || student.sectionId === sectionId) &&
      (student.name.toLowerCase().includes(query) || student.id.toLowerCase().includes(query)) &&
      (!status || student.status === status) && (!recipe || student.recipe === recipe)
    ));
  }
}

new StudentsPage().init();
