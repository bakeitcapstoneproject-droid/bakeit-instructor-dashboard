import { escapeHtml } from '../utils/html.js';
import { scoreRemark } from '../domain/performance.js';

export class TableView {
  constructor(root) { this.root = root; }
  statusClass(value) {
    return value === 'Passed' || value === 'Low' ? 'good' : value === 'High' || value === 'Incomplete' ? 'danger' : 'warn';
  }
  render(students) {
    const snapshot = JSON.stringify(students);
    if (snapshot === this.snapshot) return;
    this.root.innerHTML = students.map(student => `<tr>
      <td><div class="person"><span class="avatar">${escapeHtml(student.initials)}</span><div><strong>${escapeHtml(student.name)}</strong><small style="display:block;color:var(--muted)">${escapeHtml(student.demo ? student.id.replace(/^DEMO-/, '') : student.id)}</small></div></div></td>
      <td><span class="section-tag">${escapeHtml(student.section)}</span></td><td>${escapeHtml(student.recipe)}</td><td>${escapeHtml(student.sessions)}</td>
      <td><strong>${student.score == null ? '—' : escapeHtml(student.score)}</strong>${student.score == null ? '' : `<div class="progress"><i style="width:${Math.max(0, Math.min(100, Number(student.score) || 0))}%"></i></div>`}</td>
      <td><span class="badge ${this.statusClass(student.waste)}">${escapeHtml(student.waste)}</span></td>
      <td><span class="badge ${this.statusClass(scoreRemark(student.score))}">${scoreRemark(student.score)}</span></td>
    </tr>`).join('') || '<tr><td colspan="7" class="empty">No learners to show.</td></tr>';
    this.snapshot = snapshot;
  }
}
