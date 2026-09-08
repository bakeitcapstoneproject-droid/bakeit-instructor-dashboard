import { AppShell } from './core.js';

class ReportsPage {
  init() {
    this.shell = new AppShell().init();
    document.querySelectorAll('[data-export]').forEach(button => button.addEventListener('click', () => {
      button.textContent = `Prepared for ${this.shell.sections.name()} (demo)`;
      setTimeout(() => { button.textContent = 'Prepare report'; }, 1600);
    }));
  }
}

new ReportsPage().init();
