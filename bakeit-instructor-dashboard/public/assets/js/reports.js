import { AppShell } from './core.js';

class ReportsPage {
  constructor() { this.shell = new AppShell(); }
  async init() {
    await this.shell.init();
    this.shell.watch(async () => {});
    document.querySelectorAll('[data-export]').forEach(button => button.addEventListener('click', () => {
      button.textContent = `Prepared for ${this.shell.sections.name()}`;
      setTimeout(() => { button.textContent = 'Prepare report'; }, 1600);
    }));
  }
}

const page = new ReportsPage();
page.init().catch(error => page.shell.showError(error));
