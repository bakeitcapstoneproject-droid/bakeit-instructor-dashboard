import { escapeHtml } from '../utils/html.js';

const dateFormat = new Intl.DateTimeFormat('en-PH', { timeZone: 'Asia/Manila', dateStyle: 'medium', timeStyle: 'short' });
function dateCell(value) {
  return value ? `<time datetime="${escapeHtml(value)}">${escapeHtml(dateFormat.format(new Date(value)))}</time>` : 'Not recorded';
}

export class LearnerHistoryDialog {
  constructor({ cloud, table, findStudent }) {
    this.cloud = cloud;
    this.findStudent = findStudent;
    this.sequence = 0;
    this.dialog = document.createElement('dialog');
    this.dialog.className = 'class-dialog history-dialog';
    this.dialog.dataset.historyDialog = '';
    this.dialog.setAttribute('aria-labelledby', 'history-title');
    this.dialog.innerHTML = `<div class="history-head"><div><h2 id="history-title">Session history</h2><p data-history-learner></p></div><button class="btn" type="button" data-close-history autofocus>Close</button></div>
      <p class="history-summary" data-history-summary role="status"></p>
      <div data-history-content></div><div class="history-error" hidden><p class="error" data-history-error role="alert"></p><button class="btn" type="button" data-retry-history>Try again</button></div>`;
    document.body.append(this.dialog);
    this.dialog.querySelector('[data-close-history]').addEventListener('click', () => this.close());
    this.dialog.querySelector('[data-retry-history]').addEventListener('click', () => this.load());
    this.dialog.addEventListener('keydown', event => {
      if (event.key === 'Escape') { event.preventDefault(); this.close(); }
    });
    this.dialog.addEventListener('close', () => {
      this.sequence++;
      this.request?.abort();
      const trigger = [...table.querySelectorAll('[data-history]')].find(button => button.dataset.history === this.student?.id);
      (trigger ?? document.querySelector('#section-title'))?.focus({ preventScroll: true });
    });
    table.addEventListener('click', event => {
      const button = event.target.closest('[data-history]');
      if (!button) return;
      const student = this.findStudent(button.dataset.history);
      if (student) this.open(student);
    });
  }
  open(student) {
    this.student = student;
    this.dialog.querySelector('[data-history-learner]').textContent = `${student.name} · ${student.section}`;
    if (!this.dialog.open) this.dialog.showModal();
    this.dialog.scrollTop = 0;
    this.load();
  }
  close() { if (this.dialog.open) this.dialog.close(); }
  async load() {
    const sequence = ++this.sequence;
    this.request?.abort();
    this.request = new AbortController();
    const student = this.student;
    const content = this.dialog.querySelector('[data-history-content]');
    const summary = this.dialog.querySelector('[data-history-summary]');
    const failure = this.dialog.querySelector('.history-error');
    content.innerHTML = '';
    content.setAttribute('aria-busy', 'true');
    failure.hidden = true;
    summary.textContent = 'Loading session history…';
    try {
      const history = await this.cloud.getLearnerHistory(student.sectionId, student.id, { signal: this.request.signal });
      if (sequence !== this.sequence || !this.dialog.open) return;
      const count = history.entries.length;
      summary.textContent = `${count} session${count === 1 ? '' : 's'}${count < history.sessionCount ? ` of ${history.sessionCount} recorded` : ''}`;
      content.innerHTML = count ? `<div class="history-table-wrap" tabindex="0" role="region" aria-label="Session records"><table class="history-table"><thead><tr><th scope="col">Recipe</th><th scope="col">Started (PHT)</th><th scope="col">Ended (PHT)</th><th scope="col">Total / 25</th><th scope="col">Result</th></tr></thead><tbody>${history.entries.map(entry => `<tr>
        <td><strong>${escapeHtml(entry.recipe)}</strong><small>${escapeHtml(entry.status)}</small></td>
        <td>${dateCell(entry.startedAt)}</td><td>${entry.endedAt ? dateCell(entry.endedAt) : entry.status === 'In progress' ? 'In progress' : 'Not recorded'}</td>
        <td>${entry.score === null ? '—' : `${entry.score} / 25`}</td>
        <td><span class="badge ${entry.result === 'Passed' ? 'good' : entry.result === 'Failed' ? 'danger' : 'warn'}">${escapeHtml(entry.result)}</span></td>
      </tr>`).join('')}</tbody></table></div>` : `<p class="empty">${history.sessionCount ? 'No detailed session logs received yet.' : 'No sessions recorded yet.'}</p>`;
    } catch (error) {
      if (sequence !== this.sequence || !this.dialog.open) return;
      summary.textContent = '';
      this.dialog.querySelector('[data-history-error]').textContent = error.message;
      failure.hidden = false;
    } finally {
      if (sequence === this.sequence) content.removeAttribute('aria-busy');
    }
  }
}
