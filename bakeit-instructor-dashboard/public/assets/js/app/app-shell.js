import { AuthService } from '../services/auth-service.js';
import { SectionService } from '../services/section-service.js';
import { SectionSelector } from '../views/section-selector.js';
import { NavigationController } from '../views/navigation-controller.js';
import { PageStatusView } from '../views/page-status-view.js';
import { RefreshController } from './refresh-controller.js';

export class AppShell {
  constructor({ auth = new AuthService(), sections = new SectionService(), navigation = new NavigationController() } = {}) {
    this.auth = auth;
    this.sections = sections;
    this.navigation = navigation;
  }
  async init({ protect = true } = {}) {
    if (protect) this.auth.requireAuth();
    this.navigation.init();
    this.status = new PageStatusView(document.querySelector('.main'));
    document.querySelector('[data-logout]')?.addEventListener('click', () => {
      try { this.auth.logout(); location.href = '/login.html'; }
      catch (error) { this.showError(error); }
    });
    const path = location.pathname;
    document.querySelectorAll('.nav a').forEach(link => {
      const active = path.endsWith(link.getAttribute('href'));
      link.classList.toggle('active', active);
      if (active) link.setAttribute('aria-current', 'page');
      else link.removeAttribute('aria-current');
    });
    document.querySelector('[data-date]')?.replaceChildren(document.createTextNode(
      new Intl.DateTimeFormat('en-PH', { dateStyle: 'long' }).format(new Date())
    ));
    this.selector = new SectionSelector(document.querySelector('[data-section-select]'), this.sections);
    this.selector.mount();
    return this;
  }

  showError(error) { this.status.showError(error); }
  watch(refresh) {
    this.watcher?.stop();
    this.watcher = new RefreshController({ sections: this.sections, selector: this.selector,
      view: this.status, auth: this.auth, refresh });
    return this.watcher.start();
  }
  dispose() {
    this.watcher?.stop();
    this.navigation.dispose();
  }
}
