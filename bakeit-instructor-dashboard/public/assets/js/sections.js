import { escapeHtml } from './core.js';

export class SectionManager {
  constructor(shell, onSelect) {
    this.shell = shell;
    this.onSelect = onSelect;
    this.form = document.querySelector('[data-create-section]');
    this.list = document.querySelector('[data-section-list]');
    this.message = document.querySelector('[data-section-message]');
    this.dialog = document.querySelector('[data-create-dialog]');
  }
  init() {
    this.form.addEventListener('submit', event => this.create(event));
    this.list.addEventListener('click', event => this.handleAction(event));
    document.querySelector('[data-cancel-create]').addEventListener('click', () => this.dialog.close());
    this.dialog.addEventListener('cancel', event => { if (this.saving) event.preventDefault(); });
    this.dialog.addEventListener('close', () => document.querySelector('[data-open-create]')?.focus());
    this.render();
  }
  render() {
    const sections = this.shell.sections.sections.filter(section => section.id !== 'all');
    document.querySelector('[data-section-count]').textContent = `${sections.length} section${sections.length === 1 ? '' : 's'}`;
    // Avoid replacing a focused card during the automatic refresh.
    const active = document.activeElement;
    const focusKey = active?.dataset.copy || active?.dataset.view;
    const createFocused = active?.hasAttribute('data-open-create');
    const markup = sections.map(section => `<article class="card class-card" id="class-${escapeHtml(section.id)}">
      <div class="class-card-heading"><h3>${escapeHtml(section.name)}</h3><span class="count-badge">${section.learnerCount} learner${section.learnerCount === 1 ? '' : 's'}</span></div>
      <div class="class-code-row"><div><span class="code-label">Class code</span><code class="class-code">${escapeHtml(section.classCode)}</code></div>
      <button class="btn" data-copy="${escapeHtml(section.classCode)}" aria-label="Copy class code for ${escapeHtml(section.name)}">Copy code</button></div>
      <a class="btn section-open" href="#section=${encodeURIComponent(section.id)}" data-view="${escapeHtml(section.id)}" aria-label="View learners in ${escapeHtml(section.name)}">View learners <span aria-hidden="true">&rarr;</span></a>
    </article>`).join('') + '<button class="create-section-card" type="button" data-open-create><span aria-hidden="true">+</span> Create class section</button>';
    if (this.lastMarkup === markup) return;
    this.list.innerHTML = markup;
    this.lastMarkup = markup;
    if (createFocused) this.list.querySelector('[data-open-create]').focus();
    if (focusKey) [...this.list.querySelectorAll('[data-copy], [data-view]')]
      .find(button => button.dataset.copy === focusKey || button.dataset.view === focusKey)?.focus();
  }
  async create(event) {
    event.preventDefault();
    if (this.saving) return;
    const button = this.form.querySelector('[type="submit"]');
    const input = this.form.querySelector('input');
    const error = document.querySelector('[data-create-error]');
    error.textContent = '';
    input.removeAttribute('aria-invalid');
    if (!input.value.trim()) {
      error.textContent = 'Enter a section name.';
      input.setAttribute('aria-invalid', 'true');
      input.focus();
      return;
    }
    button.disabled = true;
    this.saving = true;
    this.form.querySelector('[data-cancel-create]').disabled = true;
    button.textContent = 'Creating…';
    try {
      const section = await this.shell.sections.create(input.value.trim());
      this.shell.selector.refresh();
      this.render();
      this.form.reset();
      this.dialog.close();
      this.onSelect(section);
      this.message.textContent = `${section.name} created. Class code: ${section.classCode}`;
    } catch (failure) {
      error.textContent = failure.message;
    } finally {
      button.disabled = false;
      this.saving = false;
      this.form.querySelector('[data-cancel-create]').disabled = false;
      button.textContent = 'Create class section';
    }
  }
  openCreate() {
    this.form.reset();
    document.querySelector('[data-create-error]').textContent = '';
    this.form.querySelector('input').removeAttribute('aria-invalid');
    this.dialog.showModal();
  }
  handleAction(event) {
    if (event.target.closest('[data-open-create]')) this.openCreate();
    const copy = event.target.closest('[data-copy]');
    if (copy) this.copy(copy.dataset.copy);
  }
  async copy(code) {
    try {
      await navigator.clipboard.writeText(code);
      this.message.textContent = `Class code ${code} copied.`;
    } catch {
      this.message.textContent = `Select and copy this class code: ${code}`;
    }
  }
}
