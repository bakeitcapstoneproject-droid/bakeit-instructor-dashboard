export const rubricCriteria = [
  { key: 'decorum', label: 'Decorum', description: 'Waste management and responsible use of ingredients.' },
  { key: 'kitchen_organization', label: 'Kitchen Organization', description: 'Order and readiness of the work area and equipment.' },
  { key: 'safety_sanitation', label: 'Safety and Sanitation', description: 'Safe handling, hygiene, and cleanliness.' },
  { key: 'baking_skills', label: 'Baking Skills', description: 'Execution of baking techniques and procedures.' },
  { key: 'product_appraisal', label: 'Product Appraisal', description: 'Quality of the finished baked product.' }
];
export const ratingScale = [
  { value: 5, label: 'Excellent', description: 'Exceeds standard' },
  { value: 4, label: 'Good', description: 'Meets standard' },
  { value: 3, label: 'Satisfactory', description: 'Acceptable' },
  { value: 2, label: 'Needs improvement', description: 'Needs improvement' },
  { value: 1, label: 'Poor', description: 'Below standard' }
];
export function validateRatings(ratings) {
  if (!ratings || typeof ratings !== 'object' || Array.isArray(ratings)
    || Object.keys(ratings).length !== rubricCriteria.length
    || rubricCriteria.some(({ key }) => !Number.isInteger(ratings[key]) || ratings[key] < 1 || ratings[key] > 5)) {
    throw new Error('VR assessment ratings must include all five criteria as whole numbers from 1 to 5.');
  }
  return { ...ratings };
}
export function rubricTotal(ratings) {
  try { return Object.values(validateRatings(ratings)).reduce((sum, value) => sum + value, 0); }
  catch { return null; }
}
export function scoreRemark(score) {
  if (!Number.isInteger(score) || score < 5 || score > 25) return 'Awaiting assessment';
  return score >= 15 ? 'Passed' : 'Needs practice';
}
export function learnerPerformance(student) {
  const score = rubricTotal(student.assessment?.ratings);
  return { ...student, legacy_score_percent: student.legacy_score_percent ?? (student.assessment?.ratings ? null : student.score ?? null), score, status: scoreRemark(score) };
}

export class DashboardMetrics {
  constructor(students) { this.students = students.map(learnerPerformance); }
  summary() {
    const count = this.students.length;
    const scored = this.students.filter(student => Number.isFinite(student.score));
    return {
      enrolled: count,
      average: scored.length ? (scored.reduce((total, student) => total + student.score, 0) / scored.length).toFixed(1) : '—',
      assessed: scored.length,
      pending: count - scored.length
    };
  }
}
