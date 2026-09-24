export function scoreRemark(score) {
  if (score === null || score === undefined) return 'Not started';
  return score >= 60 ? 'Passed' : 'Needs Practice';
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
