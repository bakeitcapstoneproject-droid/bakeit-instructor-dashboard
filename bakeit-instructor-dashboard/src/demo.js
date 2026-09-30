import { resolve } from 'node:path';
import { fileURLToPath } from 'node:url';
import { ClassStore } from './classes.js';
import { students as rubricSamples } from '../public/assets/js/data.js';

const examples = [
  { name: 'Angela Dela Cruz', recipe: 'Brownies', sessions: 4, score: 88, waste: 'Low' },
  { name: 'Miguel Reyes', recipe: 'Cookies', sessions: 2, score: 54, waste: 'High' },
  { name: 'Bea Santos', recipe: 'Cupcakes', sessions: 5, score: 95, waste: 'Low' },
  { name: 'Noel Garcia', recipe: 'Brownies', sessions: 3, score: 74, waste: 'Medium' },
  { name: 'Liane Torres', recipe: 'Cookies', sessions: 1, score: 42, waste: 'High' },
  { name: 'Carlo Mendoza', recipe: 'Not started', sessions: 0, score: null, waste: '—' },
  { name: 'Sofia Ramos', recipe: 'Cupcakes', sessions: 4, score: 92, waste: 'Low' },
  { name: 'Ethan Lim', recipe: 'Brownies', sessions: 3, score: 78, waste: 'Medium' },
  { name: 'Isabel Flores', recipe: 'Cookies', sessions: 2, score: 54, waste: 'High' },
  { name: 'Daniel Aquino', recipe: 'Cookies', sessions: 3, score: 81, waste: 'Low' },
  { name: 'Alyssa Cruz', recipe: 'Cupcakes', sessions: 1, score: 57, waste: 'High' },
  { name: 'Nathan Castillo', recipe: 'Not started', sessions: 0, score: null, waste: '—' }
];

export class DemoLearnerSeeder {
  constructor(store) { this.store = store; }
  add() {
    return this.store.mutate(data => {
      if (!data.sections.length) throw new Error('Create a class section in Learners first.');
      let added = 0;
      data.sections.forEach((section, sectionIndex) => {
        for (let index = 0; index < 6; index++) {
          const learnerId = `DEMO-${section.id.slice(0, 8).toUpperCase()}-${index + 1}`;
          if (data.enrollments.some(item => item.sectionId === section.id && item.learnerId === learnerId)) continue;
          const { name, ...result } = examples[(sectionIndex * 6 + index) % examples.length];
          const sample = rubricSamples.find(student => student.name === name);
          result.assessment = result.sessions > 0 ? structuredClone(sample.assessment) : undefined;
          delete result.score;
          data.enrollments.push({ sectionId: section.id, learnerId, learnerName: name,
            joinedAt: new Date().toISOString(), demo: true, demoResult: result });
          added++;
        }
      });
      return added;
    });
  }

  remove() {
    return this.store.mutate(data => {
      const count = data.enrollments.length;
      data.enrollments = data.enrollments.filter(item => item.demo !== true);
      return count - data.enrollments.length;
    });
  }

  scores() {
    return this.store.mutate(data => {
      let updated = 0;
      for (const learner of data.enrollments) {
        if (learner.demo !== true || learner.assessment != null
          || learner.demoResult?.assessment != null || !(learner.demoResult?.sessions > 0)) continue;
        const sample = rubricSamples.find(student => student.name === learner.learnerName);
        if (!sample) continue;
        learner.demoResult.assessment = structuredClone(sample.assessment);
        updated++;
      }
      return updated;
    });
  }
}

export const addDemoLearners = store => new DemoLearnerSeeder(store).add();
export const removeDemoLearners = store => new DemoLearnerSeeder(store).remove();
export const addDemoScores = store => new DemoLearnerSeeder(store).scores();

if (process.argv[1] && resolve(process.argv[1]) === fileURLToPath(import.meta.url)) {
  const action = process.argv[2];
  if (!['add', 'remove', 'scores'].includes(action)) throw new Error('Use demo.js add, demo.js remove, or demo.js scores.');
  const store = new ClassStore(process.env.BAKEIT_DATA_FILE || fileURLToPath(new URL('../data/classes.json', import.meta.url)));
  const count = await ({ add: addDemoLearners, remove: removeDemoLearners, scores: addDemoScores }[action])(store);
  console.log(action === 'scores' ? `${count} existing demo learners received sample scores.` : `${count} demo learners ${action === 'add' ? 'added' : 'removed'}.`);
}
