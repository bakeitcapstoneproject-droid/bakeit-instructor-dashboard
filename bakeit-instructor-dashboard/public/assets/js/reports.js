import { ReportsPage } from './pages/reports-page.js';

const page = new ReportsPage();
page.init().catch(error => page.shell.showError(error));
