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
  constructor(store = new StorageService()) { this.store = store; }
  login(email, password) {
    if (!email || !password) throw new Error('Enter your email and password.');
    const changedCredentials = this.store.get('bakeit_demo_credentials');
    if (changedCredentials?.email === email && changedCredentials.password !== password) {
      throw new Error('The password is incorrect.');
    }
    const user = { name: 'Authorized Instructor', email, role: 'Instructor' };
    this.store.set('bakeit_user', user);
    return user;
  }
  requestPasswordReset(email) {
    if (!/^\S+@\S+\.\S+$/.test(email)) throw new Error('Enter a valid institutional email.');
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
  constructor(store = new StorageService()) {
    this.store = store;
    this.sections = [
      { id: 'all', name: 'All Sections' },
      { id: 'section-a', name: 'Section A' },
      { id: 'section-b', name: 'Section B' }
    ];
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
    this.root.innerHTML = this.sections.sections
      .map(section => `<option value="${section.id}">${section.name}</option>`).join('');
    this.root.value = this.sections.selected();
    this.updateLabels();
    this.root.addEventListener('change', () => {
      const sectionId = this.sections.select(this.root.value);
      this.updateLabels();
      document.dispatchEvent(new CustomEvent('bakeit:section-change', {
        detail: { sectionId, sectionName: this.sections.name(sectionId) }
      }));
    });
  }
  updateLabels() {
    const name = this.sections.name();
    document.querySelectorAll('[data-section-label]').forEach(element => { element.textContent = name; });
  }
}

export function scoreRemark(score) {
  return score >= 60 ? 'Passed' : 'Needs Practice';
}

export class CloudDataService {
  constructor({ mode = 'mock' } = {}) { this.mode = mode; }
  filterBySection(items, sectionId) {
    return sectionId === 'all' ? items : items.filter(item => item.sectionId === sectionId);
  }
  async getStudents(sectionId = 'all') {
    const { students } = await import('./data.js');
    return structuredClone(this.filterBySection(students, sectionId))
      .map(student => ({ ...student, status: scoreRemark(student.score) }));
  }
  async getLiveSessions(sectionId = 'all') {
    const { sessions } = await import('./data.js');
    return structuredClone(this.filterBySection(sessions, sectionId));
  }
  async getActivities(sectionId = 'all') {
    const { activities } = await import('./data.js');
    return structuredClone(this.filterBySection(activities, sectionId));
  }
  async getHealth() {
    return { connected: true, source: this.mode === 'mock' ? 'Prototype data' : 'AWS API', updated: new Date() };
  }
}

export class DashboardMetrics {
  constructor(students) { this.students = students; }
  summary() {
    const count = this.students.length;
    return {
      enrolled: count,
      average: count ? (this.students.reduce((total, student) => total + student.score, 0) / count).toFixed(1) : '0.0',
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
      <td><div class="person"><span class="avatar">${student.initials}</span><div><strong>${student.name}</strong><small style="display:block;color:var(--muted)">${student.id}</small></div></div></td>
      <td><span class="section-tag">${student.section}</span></td><td>${student.recipe}</td><td>${student.sessions}</td>
      <td><strong>${student.score}</strong><div class="progress"><i style="width:${student.score}%"></i></div></td>
      <td><span class="badge ${this.statusClass(student.waste)}">${student.waste}</span></td>
      <td><span class="badge ${this.statusClass(scoreRemark(student.score))}">${scoreRemark(student.score)}</span></td>
    </tr>`).join('') || '<tr><td colspan="7" class="empty">No learners match the selected section and filters.</td></tr>';
  }
}

export class AppShell {
  constructor() { this.auth = new AuthService(); this.sections = new SectionService(); }
  init({ protect = true } = {}) {
    if (protect) this.auth.requireAuth();
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
    new SectionSelector(document.querySelector('[data-section-select]'), this.sections).mount();
    return this;
  }
}
