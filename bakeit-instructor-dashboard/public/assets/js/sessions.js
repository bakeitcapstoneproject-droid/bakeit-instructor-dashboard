import { AppShell, CloudDataService, escapeHtml } from './core.js';

class SessionView {
  constructor(root) { this.root = root; }
  render(items) {
    this.root.innerHTML = items.map(session => `<article class="card live-card">
      <div class="row-between"><div><h3>${escapeHtml(session.student)}</h3><p class="sub">${escapeHtml(session.recipe)} · <span class="section-tag">${escapeHtml(session.section)}</span></p></div><span class="badge ${session.demo ? 'demo-label' : 'good'}">${session.demo ? 'Demo session' : 'Live'}</span></div>
      <p class="step">Step ${escapeHtml(session.current)} of ${escapeHtml(session.total)} · ${escapeHtml(session.step)}</p>
      <div class="progress"><i style="width:${Math.max(0, Math.min(100, session.current / session.total * 100 || 0))}%"></i></div><p class="sub">${session.demo ? 'Sample session time' : 'Session time'} ${escapeHtml(session.time)}</p>
      <ul class="event-list">${session.events.map(event => `<li><span>${escapeHtml(event)}</span><span class="${session.demo ? 'demo-label' : 'good'} badge">${session.demo ? 'Sample' : 'Recorded'}</span></li>`).join('')}</ul>
    </article>`).join('') || '<div class="card empty">No active VR sessions in this section.</div>';
  }
}

class SessionsPage {
  constructor() { this.shell = new AppShell(); this.cloud = new CloudDataService(); this.view = new SessionView(document.querySelector('.live-grid')); }
  async init() {
    await this.shell.init();
    this.shell.watch(() => this.render());
    await this.render();
  }
  async render() {
    const sectionId = this.shell.sections.selected();
    const sessions = await this.cloud.getLiveSessions(sectionId);
    if (sectionId !== this.shell.sections.selected()) return;
    this.view.render(sessions);
    const demoCount = sessions.filter(session => session.demo).length;
    const activeCount = sessions.length - demoCount;
    document.querySelector('[data-session-count]').textContent = [
      activeCount || !demoCount ? `${activeCount} active session${activeCount === 1 ? '' : 's'}` : '',
      demoCount ? `${demoCount} demo session${demoCount === 1 ? '' : 's'}` : ''
    ].filter(Boolean).join(' · ');
  }
}

const page = new SessionsPage();
page.init().catch(error => page.shell.showError(error));
