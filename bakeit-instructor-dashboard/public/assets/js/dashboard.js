import { AppShell, CloudDataService, DashboardMetrics, TableView, escapeHtml } from './core.js';

class DashboardPage {
  constructor() {
    this.shell = new AppShell();
    this.cloud = new CloudDataService();
    this.table = new TableView(document.querySelector('tbody'));
  }
  async init() {
    await this.shell.init();
    this.shell.watch(() => this.render());
    await this.render();
  }
  async render() {
    const sectionId = this.shell.sections.selected();
    const [students, activities] = await Promise.all([this.cloud.getStudents(sectionId), this.cloud.getActivities(sectionId)]);
    if (sectionId !== this.shell.sections.selected()) return this.render();
    const metrics = new DashboardMetrics(students).summary();
    for (const [key, value] of Object.entries(metrics)) document.querySelector(`[data-metric="${key}"]`).textContent = value;
    this.table.render(students.slice(0, 4));
    document.querySelector('[data-activity-list]').innerHTML = activities
      .map(activity => `<div class="activity"><span>${escapeHtml(activity.text)}</span><time>${escapeHtml(new Date(activity.time).toLocaleDateString())}</time></div>`).join('')
      || '<p class="empty">No recent activity for this section.</p>';
  }
}

const page = new DashboardPage();
page.init().catch(error => page.shell.showError(error));
