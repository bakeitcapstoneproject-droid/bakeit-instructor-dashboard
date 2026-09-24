import { StudentsPage } from './pages/students-page.js';

const page = new StudentsPage();
page.init().catch(error => page.shell.showError(error));
