import { AppShell, CloudDataService } from './core.js';

class SessionView {
  constructor(root) { this.root = root; }
  render(items) {
    this.root.innerHTML = items.map(session => `<article class="card live-card">
      <div class="row-between"><div><h3>${session.student}</h3><p class="sub">${session.recipe} · <span class="section-tag">${session.section}</span></p></div><span class="badge good">Live</span></div>
      <p class="step">Step ${session.current} of ${session.total} · ${session.step}</p>
      <div class="progress"><i style="width:${session.current / session.total * 100}%"></i></div><p class="sub">Session time ${session.time}</p>
      <ul class="event-list">${session.events.map(event => `<li><span>${event}</span><span class="good badge">Recorded</span></li>`).join('')}</ul>
    </article>`).join('') || '<div class="card empty">No active VR sessions in this section.</div>';
  }
}

class SessionsPage {
  constructor() { this.shell = new AppShell(); this.cloud = new CloudDataService(); this.view = new SessionView(document.querySelector('.live-grid')); }
  async init() {
    this.shell.init();
    document.addEventListener('bakeit:section-change', () => this.render());
    await this.render();
  }
  async render() {
    const sessions = await this.cloud.getLiveSessions(this.shell.sections.selected());
    this.view.render(sessions);
    document.querySelector('[data-session-count]').textContent = `${sessions.length} active session${sessions.length === 1 ? '' : 's'}`;
  }
}

new SessionsPage().init();
