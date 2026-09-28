import { PageController } from '../app/page-controller.js';
import { CloudDataService } from '../services/cloud-data-service.js';
import { DashboardMetrics } from '../domain/performance.js';
import { TableView } from '../views/table-view.js';
import { escapeHtml } from '../utils/html.js';

export class DashboardPage extends PageController {
  constructor({ shell, cloud = new CloudDataService(), table = new TableView(document.querySelector('tbody')) } = {}) {
    super({ shell });
    this.cloud = cloud;
    this.table = table;
  }
  refresh() { return this.render(); }
  async render() {
    const sectionId = this.shell.sections.selected();
    const [students, activities] = await Promise.all([this.cloud.getStudents(sectionId), this.cloud.getActivities(sectionId)]);
    if (sectionId !== this.shell.sections.selected()) return;
    const metrics = new DashboardMetrics(students).summary();
    for (const [key, value] of Object.entries(metrics)) document.querySelector(`[data-metric="${key}"]`).textContent = value;
    this.table.render(students);
    const activityMarkup = activities
      .map(activity => `<div class="activity"><span>${escapeHtml(activity.text)}</span><time>${escapeHtml(new Date(activity.time).toLocaleDateString())}</time></div>`).join('')
      || '<p class="empty">No recent activity for this section.</p>';
    if (activityMarkup !== this.activityMarkup) {
      document.querySelector('[data-activity-list]').innerHTML = activityMarkup;
      this.activityMarkup = activityMarkup;
    }
    if (sectionId !== this.renderedSectionId) {
      document.querySelectorAll('[data-overview-scroll]').forEach(region => region.scrollTo(0, 0));
      this.renderedSectionId = sectionId;
    }
  }
}
