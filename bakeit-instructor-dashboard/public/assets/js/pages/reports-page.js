import { PageController } from '../app/page-controller.js';

export class ReportsPage extends PageController {
  setup() {
    document.querySelectorAll('[data-export]').forEach(button => button.addEventListener('click', () => {
      document.querySelector('[data-report-status]').textContent = 'Report downloads are not available yet.';
    }));
  }
}
