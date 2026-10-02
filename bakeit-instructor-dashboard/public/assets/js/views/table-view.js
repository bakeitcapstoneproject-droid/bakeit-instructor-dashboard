import { escapeHtml } from '../utils/html.js';
import { learnerPerformance } from '../domain/performance.js';
import { learnerDisplayId } from '../domain/learner-identity.js';

export class TableView {
  constructor(root, { history = false } = {}) { this.root = root; this.history = history; }
  statusClass(value) {
    return value === 'Passed' ? 'good' : value === 'Needs practice' ? 'danger' : 'warn';
  }
  render(students) {
    const snapshot = JSON.stringify(students);
    if (snapshot === this.snapshot) return;
    this.root.innerHTML = students.map(learnerPerformance).map(student => `<tr>
      <td><div class="person"><span class="avatar">${escapeHtml(student.initials)}</span><div><strong>${escapeHtml(student.name)}</strong><small class="learner-id">${escapeHtml(learnerDisplayId(student))}</small></div></div></td>
      <td><span class="section-tag">${escapeHtml(student.section)}</span></td><td>${escapeHtml(student.recipe)}</td><td>${escapeHtml(student.sessions)}</td>
      <td><strong>${student.score == null ? '—' : `${student.score} / 25`}</strong></td>
      <td><span class="badge ${this.statusClass(student.status)}">${student.status}</span></td>
      ${this.history ? `<td><button class="btn history-button" type="button" data-history="${escapeHtml(student.id)}" aria-label="View session history for ${escapeHtml(student.name)}" aria-haspopup="dialog">View history</button></td>` : ''}
    </tr>`).join('') || `<tr><td colspan="${this.history ? 7 : 6}" class="empty">No learners to show.</td></tr>`;
    this.snapshot = snapshot;
  }
}
