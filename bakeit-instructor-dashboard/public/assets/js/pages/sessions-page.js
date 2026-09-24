import { PageController } from '../app/page-controller.js';
import { CloudDataService } from '../services/cloud-data-service.js';
import { SessionView } from '../session-view.js';

export class SessionsPage extends PageController {
  constructor({ shell, cloud = new CloudDataService(), view = new SessionView(document.querySelector('.live-grid')) } = {}) {
    super({ shell });
    this.cloud = cloud;
    this.view = view;
  }
  refresh() { return this.render(); }
  async render() {
    const sectionId = this.shell.sections.selected();
    const sessions = await this.cloud.getLiveSessions(sectionId);
    if (sectionId !== this.shell.sections.selected()) return;
    this.view.render(sessions);
    const activeCount = sessions.length;
    document.querySelector('[data-session-count]').textContent = `${activeCount} active session${activeCount === 1 ? '' : 's'}`;
  }
}
