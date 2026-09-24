import { SessionsPage } from './pages/sessions-page.js';

const page = new SessionsPage();
page.init().catch(error => page.shell.showError(error));
