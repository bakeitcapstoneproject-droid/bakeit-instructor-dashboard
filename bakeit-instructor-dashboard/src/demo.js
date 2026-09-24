import { resolve } from 'node:path';
import { fileURLToPath } from 'node:url';
import { ClassStore } from './classes.js';

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
}

export const addDemoLearners = store => new DemoLearnerSeeder(store).add();
export const removeDemoLearners = store => new DemoLearnerSeeder(store).remove();

if (process.argv[1] && resolve(process.argv[1]) === fileURLToPath(import.meta.url)) {
  const action = process.argv[2];
  if (!['add', 'remove'].includes(action)) throw new Error('Use demo.js add or demo.js remove.');
  const store = new ClassStore(process.env.BAKEIT_DATA_FILE || fileURLToPath(new URL('../data/classes.json', import.meta.url)));
  const count = await (action === 'add' ? addDemoLearners(store) : removeDemoLearners(store));
  console.log(`${count} demo learners ${action === 'add' ? 'added' : 'removed'}.`);
}
