import { AppShell } from './app-shell.js';

// Template lifecycle shared by protected pages; each page supplies setup and refresh.
export class PageController {
  constructor({ shell = new AppShell() } = {}) { this.shell = shell; }
  async init() {
    await this.shell.init();
    await this.setup();
    await this.shell.watch(() => this.refresh());
    return this;
  }
  setup() {}
  async refresh() {}
  dispose() { this.shell.dispose(); }
}
