import { AppShell, CloudDataService, DashboardMetrics, TableView } from './core.js';

class DashboardPage {
  constructor() {
    this.shell = new AppShell();
    this.cloud = new CloudDataService();
    this.table = new TableView(document.querySelector('tbody'));
  }
  async init() {
    this.shell.init();
    document.addEventListener('bakeit:section-change', () => this.render());
    await this.render();
  }
  async render() {
    const sectionId = this.shell.sections.selected();
    const [students, activities] = await Promise.all([this.cloud.getStudents(sectionId), this.cloud.getActivities(sectionId)]);
    const metrics = new DashboardMetrics(students).summary();
    for (const [key, value] of Object.entries(metrics)) document.querySelector(`[data-metric="${key}"]`).textContent = value;
    this.table.render(students.slice(0, 4));
    document.querySelector('[data-activity-list]').innerHTML = activities
      .map(activity => `<div class="activity"><span>${activity.text}</span><time>${activity.time}</time></div>`).join('')
      || '<p class="empty">No recent activity for this section.</p>';
  }
}

new DashboardPage().init();
