import { AppShell } from './app/app-shell.js';

const shell = new AppShell();
shell.init().catch(error => shell.showError(error));
