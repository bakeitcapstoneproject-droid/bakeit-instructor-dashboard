import { AppShell, CloudDataService } from './core.js';
import { SessionView } from './session-view.js';

class SessionsPage {
  constructor() { this.shell = new AppShell(); this.cloud = new CloudDataService(); this.view = new SessionView(document.querySelector('.live-grid')); }
  async init() {
    await this.shell.init();
    this.shell.watch(() => this.render());
    await this.render();
  }
  async render() {
    const sectionId = this.shell.sections.selected();
    const sessions = await this.cloud.getLiveSessions(sectionId);
    if (sectionId !== this.shell.sections.selected()) return;
    this.view.render(sessions);
    const activeCount = sessions.length;
    document.querySelector('[data-session-count]').textContent = `${activeCount} active session${activeCount === 1 ? '' : 's'}`;
  }
}

const page = new SessionsPage();
page.init().catch(error => page.shell.showError(error));
