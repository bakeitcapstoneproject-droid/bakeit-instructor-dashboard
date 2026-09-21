import { dataMode } from './runtime-config.js';

export class StorageService {
  constructor(storage = localStorage) { this.storage = storage; }
  get(key, fallback = null) {
    try { return JSON.parse(this.storage.getItem(key)) ?? fallback; }
    catch { return fallback; }
  }
  set(key, value) { this.storage.setItem(key, JSON.stringify(value)); }
  remove(key) { this.storage.removeItem(key); }
}

export class AuthService {
  constructor(store = new StorageService(), now = () => Date.now()) { this.store = store; this.now = now; }
  loginCooldown() {
    const attempts = this.store.get('bakeit_login_attempts', { failures: 0, until: 0 });
    if (attempts.until && attempts.until <= this.now()) {
      this.store.remove('bakeit_login_attempts');
      return 0;
    }
    return Math.max(0, Math.ceil((attempts.until - this.now()) / 1000));
  }
  validateEmail(email) {
    const normalized = email.trim().toLowerCase();
    if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(normalized)) {
      throw Object.assign(new Error('Invalid email'), { field: 'email' });
    }
    if (normalized !== 'instructor@mcl.edu.ph') {
      throw Object.assign(new Error('Invalid email'), { field: 'email' });
    }
    return normalized;
  }
  login(email, password) {
    const retryAfter = this.loginCooldown();
    if (retryAfter) throw Object.assign(new Error('Invalid Password'), { field: 'password', retryAfter });
    email = this.validateEmail(email);
    if (!password) throw Object.assign(new Error('Invalid Password'), { field: 'password' });
    const changedCredentials = this.store.get('bakeit_demo_credentials');
    const expectedPassword = changedCredentials?.email === email ? changedCredentials.password : 'demo123';
    if (password !== expectedPassword) {
      const attempts = this.store.get('bakeit_login_attempts', { failures: 0, until: 0 });
      attempts.failures += 1;
      if (attempts.failures >= 3) attempts.until = this.now() + 30000;
      this.store.set('bakeit_login_attempts', attempts);
      throw Object.assign(new Error('Invalid Password'), { field: 'password', retryAfter: this.loginCooldown() });
    }
    const user = { name: 'Authorized Instructor', email, role: 'Instructor' };
    this.store.set('bakeit_user', user);
    this.store.remove('bakeit_login_attempts');
    return user;
  }
  requestPasswordReset(email) {
    email = this.validateEmail(email);
    this.store.set('bakeit_reset_email', email);
    return email;
  }
  resetPassword(code, password, confirmation) {
    const email = this.store.get('bakeit_reset_email');
    if (!email) throw new Error('Request a verification code first.');
    if (!/^\d{6}$/.test(code)) throw new Error('Enter the 6-digit verification code.');
    if (password.length < 8) throw new Error('Password must contain at least 8 characters.');
    if (password !== confirmation) throw new Error('The passwords do not match.');
    this.store.set('bakeit_demo_credentials', { email, password });
    this.store.remove('bakeit_reset_email');
    return true;
  }
  logout() { this.store.remove('bakeit_user'); }
  current() { return this.store.get('bakeit_user'); }
  requireAuth() { if (!this.current()) location.href = '/login.html'; }
}

export class SectionService {
  constructor(store = new StorageService(), request = apiRequest) {
    this.store = store;
    this.request = request;
    this.revision = 0;
    this.sections = [{ id: 'all', name: 'All Sections' }];
  }
  async load() {
    const revision = this.revision;
    const { sections } = await this.request('/api/sections');
    if (revision !== this.revision) return this.sections.filter(section => section.id !== 'all');
    this.sections = [{ id: 'all', name: 'All Sections' }, ...sections];
    this.select(this.selected());
    return sections;
  }
  async create(name) {
    const { section } = await this.request('/api/sections', { method: 'POST', body: JSON.stringify({ name }) });
    this.revision += 1;
    this.sections = [...this.sections.filter(item => item.id !== section.id), section];
    this.select(section.id);
    return section;
  }
  async delete(sectionId) {
    const result = await this.request(`/api/sections/${encodeURIComponent(sectionId)}`, { method: 'DELETE' });
    this.revision += 1;
    this.sections = this.sections.filter(section => section.id !== sectionId);
    this.select(this.selected());
    return result;
  }
  selected() {
    const saved = this.store.get('bakeit_section', 'all');
    return this.sections.some(section => section.id === saved) ? saved : 'all';
  }
  select(sectionId) {
    const valid = this.sections.some(section => section.id === sectionId) ? sectionId : 'all';
    this.store.set('bakeit_section', valid);
    return valid;
  }
  name(sectionId = this.selected()) {
    return this.sections.find(section => section.id === sectionId)?.name ?? 'All Sections';
  }
}

export class SectionSelector {
  constructor(root, sections) { this.root = root; this.sections = sections; }
  mount() {
    if (!this.root) return;
    this.refresh();
    this.root.addEventListener('change', () => {
      const sectionId = this.sections.select(this.root.value);
      this.updateLabels();
      document.dispatchEvent(new CustomEvent('bakeit:section-change', {
        detail: { sectionId, sectionName: this.sections.name(sectionId) }
      }));
    });
  }
  refresh() {
    if (!this.root) return;
    this.root.replaceChildren(...this.sections.sections.map(section => new Option(section.name, section.id)));
    this.root.value = this.sections.selected();
    this.updateLabels();
  }
  updateLabels() {
    const name = this.sections.name();
    document.querySelectorAll('[data-section-label]').forEach(element => { element.textContent = name; });
  }
}

export function scoreRemark(score) {
  if (score === null || score === undefined) return 'Not started';
  return score >= 60 ? 'Passed' : 'Needs Practice';
}

export function escapeHtml(value) {
  return String(value ?? '').replace(/[&<>"']/g, character => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' })[character]);
}

let staticRequest;
export async function apiRequest(path, options = {}) {
  if (dataMode === 'static') {
    staticRequest ??= import('./static-data.js').then(({ createStaticRequest }) => createStaticRequest(localStorage));
    return (await staticRequest)(path, options);
  }
  let response;
  try { response = await fetch(path, { ...options, headers: { 'Content-Type': 'application/json', ...options.headers } }); }
  catch { throw new Error('Cannot reach the server. Check your connection and try again.'); }
  let body;
  try { body = await response.json(); }
  catch { throw new Error('Could not load class data. Restart the BakeIT server and try again.'); }
  if (!response.ok) throw new Error(body.error || 'Unable to load class data. Please try again.');
  return body;
}

export class CloudDataService {
  constructor({ mode = 'api', request = apiRequest } = {}) { this.mode = mode; this.request = request; }
  filterBySection(items, sectionId) {
    return sectionId === 'all' ? items : items.filter(item => item.sectionId === sectionId);
  }
  async getStudents(sectionId = 'all') {
    if (this.mode !== 'mock') return (await this.request(`/api/learners?sectionId=${encodeURIComponent(sectionId)}`)).students;
    const { students } = await import('./data.js');
    return structuredClone(this.filterBySection(students, sectionId))
      .map(student => ({ ...student, status: scoreRemark(student.score) }));
  }
  async getLiveSessions(sectionId = 'all') {
    if (this.mode !== 'mock') return (await this.request(`/api/sessions?sectionId=${encodeURIComponent(sectionId)}`)).sessions;
    const { sessions } = await import('./data.js');
    return structuredClone(this.filterBySection(sessions, sectionId));
  }
  async getActivities(sectionId = 'all') {
    if (this.mode !== 'mock') return (await this.request(`/api/activities?sectionId=${encodeURIComponent(sectionId)}`)).activities;
    const { activities } = await import('./data.js');
    return structuredClone(this.filterBySection(activities, sectionId));
  }
  async getHealth() {
    if (this.mode !== 'mock') await this.request('/api/sections');
    return { connected: true, source: this.mode === 'mock' ? 'Prototype data' : dataMode === 'static' ? 'Browser storage' : 'Class server', updated: new Date() };
  }
}

export class DashboardMetrics {
  constructor(students) { this.students = students; }
  summary() {
    const count = this.students.length;
    const scored = this.students.filter(student => Number.isFinite(student.score));
    return {
      enrolled: count,
      average: scored.length ? (scored.reduce((total, student) => total + student.score, 0) / scored.length).toFixed(1) : '—',
      passed: this.students.filter(student => scoreRemark(student.score) === 'Passed').length,
      waste: this.students.filter(student => student.waste === 'High').length
    };
  }
}

export class TableView {
  constructor(root) { this.root = root; }
  statusClass(value) {
    return value === 'Passed' || value === 'Low' ? 'good' : value === 'High' || value === 'Incomplete' ? 'danger' : 'warn';
  }
  render(students) {
    this.root.innerHTML = students.map(student => `<tr>
      <td><div class="person"><span class="avatar">${escapeHtml(student.initials)}</span><div><strong>${escapeHtml(student.name)}</strong><small style="display:block;color:var(--muted)">${escapeHtml(student.demo ? student.id.replace(/^DEMO-/, '') : student.id)}</small></div></div></td>
      <td><span class="section-tag">${escapeHtml(student.section)}</span></td><td>${escapeHtml(student.recipe)}</td><td>${escapeHtml(student.sessions)}</td>
      <td><strong>${student.score == null ? '—' : escapeHtml(student.score)}</strong>${student.score == null ? '' : `<div class="progress"><i style="width:${Math.max(0, Math.min(100, Number(student.score) || 0))}%"></i></div>`}</td>
      <td><span class="badge ${this.statusClass(student.waste)}">${escapeHtml(student.waste)}</span></td>
      <td><span class="badge ${this.statusClass(scoreRemark(student.score))}">${scoreRemark(student.score)}</span></td>
    </tr>`).join('') || '<tr><td colspan="7" class="empty">No learners to show.</td></tr>';
  }
}

export class AppShell {
  constructor() { this.auth = new AuthService(); this.sections = new SectionService(); }
  async init({ protect = true } = {}) {
    if (protect) this.auth.requireAuth();
    this.initNavigation();
    document.querySelector('[data-logout]')?.addEventListener('click', () => {
      this.auth.logout();
      location.href = '/login.html';
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
    await this.sections.load();
    this.selector = new SectionSelector(document.querySelector('[data-section-select]'), this.sections);
    this.selector.mount();
    return this;
  }
  initNavigation() {
    const sidebar = document.querySelector('#main-sidebar');
    const toggle = document.querySelector('[data-nav-toggle]');
    if (!sidebar || !toggle) return;
    const mobile = window.matchMedia('(max-width: 800px)');
    const main = document.querySelector('.main');
    const header = document.querySelector('.mobile-header');
    const overlay = document.querySelector('[data-nav-overlay]');
    const setOpen = open => {
      open = mobile.matches && open;
      sidebar.classList.toggle('is-open', open);
      sidebar.inert = mobile.matches && !open;
      main.inert = header.inert = open;
      overlay.hidden = !open;
      toggle.setAttribute('aria-expanded', String(open));
      document.body.classList.toggle('nav-open', open);
      if (open) {
        sidebar.setAttribute('role', 'dialog');
        sidebar.setAttribute('aria-modal', 'true');
        (sidebar.querySelector('.nav a[aria-current="page"]') || sidebar.querySelector('.nav a')).focus();
      } else {
        sidebar.removeAttribute('role');
        sidebar.removeAttribute('aria-modal');
      }
    };
    const dismiss = () => { setOpen(false); toggle.focus(); };
    toggle.addEventListener('click', () => setOpen(true));
    overlay.addEventListener('click', dismiss);
    sidebar.addEventListener('keydown', event => {
      if (!sidebar.classList.contains('is-open')) return;
      if (event.key === 'Escape') { event.preventDefault(); dismiss(); }
      if (event.key === 'Tab') {
        const items = [...sidebar.querySelectorAll('a[href], button:not(:disabled)')];
        const first = items[0], last = items.at(-1);
        if (event.shiftKey && document.activeElement === first) { event.preventDefault(); last.focus(); }
        else if (!event.shiftKey && document.activeElement === last) { event.preventDefault(); first.focus(); }
      }
    });
    mobile.addEventListener('change', () => {
      const focusWasInside = sidebar.contains(document.activeElement);
      setOpen(false);
      if (mobile.matches && focusWasInside) toggle.focus();
    });
    setOpen(false);
  }
  showError(error) {
    let notice = document.querySelector('[data-page-error]');
    if (!notice) {
      notice = document.createElement('p');
      notice.dataset.pageError = '';
      notice.className = 'error page-error';
      notice.setAttribute('role', 'alert');
      document.querySelector('.main').prepend(notice);
    }
    notice.textContent = error.message;
  }
  watch(refresh) {
    let busy = false;
    let queued = false;
    const update = async () => {
      if (busy) { queued = true; return; }
      busy = true;
      try {
        await this.sections.load();
        this.selector.refresh();
        await refresh();
        document.querySelector('[data-page-error]')?.remove();
      } catch (error) { this.showError(error); }
      finally {
        busy = false;
        if (queued) { queued = false; update(); }
      }
    };
    document.addEventListener('bakeit:section-change', update);
    window.addEventListener('focus', update);
    setInterval(() => { if (!document.hidden) update(); }, 15000);
  }
}
