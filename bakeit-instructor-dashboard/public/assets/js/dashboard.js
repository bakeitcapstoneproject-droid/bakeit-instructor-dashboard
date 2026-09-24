import { DashboardPage } from './pages/dashboard-page.js';

const page = new DashboardPage();
page.init().catch(error => page.shell.showError(error));
