import { rubricCriteria, learnerPerformance } from './domain/performance.js';
import { getRecipeProgress, recipeVersion } from './recipes.js';

const sampleRatings = [[4,4,5,4,5],[2,3,3,3,2],[5,5,5,4,5],[3,4,4,3,4],[2,2,3,2,2],[1,2,2,2,1],[5,4,5,5,4],[3,4,4,4,3],[2,3,2,3,2],[5,5,5,5,5],[3,3,4,3,3],[1,2,2,1,2],[4,4,5,4,4],[4,4,4,4,4],[5,5,4,5,5],[2,3,3,2,3],[3,4,3,4,3],[1,1,2,2,1],[4,5,4,5,4],[3,3,4,4,3]];
export const students = [
  { id:'S-0241', name:'Angela Dela Cruz', initials:'AD', sectionId:'section-a', section:'Section A', recipe:'Brownies', sessions:4, progress:100, waste:'Low' },
  { id:'S-0242', name:'Miguel Reyes', initials:'MR', sectionId:'section-a', section:'Section A', recipe:'Cookies', sessions:2, progress:55, waste:'High', live:true },
  { id:'S-0243', name:'Bea Santos', initials:'BS', sectionId:'section-a', section:'Section A', recipe:'Cupcakes', sessions:5, progress:100, waste:'Low' },
  { id:'S-0244', name:'Noel Garcia', initials:'NG', sectionId:'section-b', section:'Section B', recipe:'Brownies', sessions:3, progress:78, waste:'Medium', live:true },
  { id:'S-0245', name:'Liane Torres', initials:'LT', sectionId:'section-b', section:'Section B', recipe:'Cookies', sessions:1, progress:45, waste:'High' },
  { id:'S-0246', name:'Carlo Mendoza', initials:'CM', sectionId:'section-b', section:'Section B', recipe:'Brownies', sessions:1, progress:44, waste:'High' },
  { id:'S-0247', name:'Sofia Ramos', initials:'SR', sectionId:'section-a', section:'Section A', recipe:'Cupcakes', sessions:4, progress:100, waste:'Low' },
  { id:'S-0248', name:'Ethan Lim', initials:'EL', sectionId:'section-a', section:'Section A', recipe:'Brownies', sessions:3, progress:85, waste:'Medium' },
  { id:'S-0249', name:'Isabel Flores', initials:'IF', sectionId:'section-a', section:'Section A', recipe:'Cookies', sessions:2, progress:60, waste:'High' },
  { id:'S-0250', name:'Gabriel Tan', initials:'GT', sectionId:'section-a', section:'Section A', recipe:'Brownies', sessions:5, progress:100, waste:'Low' },
  { id:'S-0251', name:'Chloe Navarro', initials:'CN', sectionId:'section-a', section:'Section A', recipe:'Cupcakes', sessions:2, progress:70, waste:'Medium' },
  { id:'S-0252', name:'Joshua Bautista', initials:'JB', sectionId:'section-a', section:'Section A', recipe:'Cookies', sessions:1, progress:40, waste:'High' },
  { id:'S-0253', name:'Mika Villanueva', initials:'MV', sectionId:'section-a', section:'Section A', recipe:'Cupcakes', sessions:4, progress:100, waste:'Low' },
  { id:'S-0254', name:'Daniel Aquino', initials:'DA', sectionId:'section-b', section:'Section B', recipe:'Cookies', sessions:3, progress:90, waste:'Low' },
  { id:'S-0255', name:'Alyssa Cruz', initials:'AC', sectionId:'section-b', section:'Section B', recipe:'Cupcakes', sessions:5, progress:100, waste:'Low' },
  { id:'S-0256', name:'Nathan Castillo', initials:'NC', sectionId:'section-b', section:'Section B', recipe:'Brownies', sessions:2, progress:65, waste:'High' },
  { id:'S-0257', name:'Camille Reyes', initials:'CR', sectionId:'section-b', section:'Section B', recipe:'Cupcakes', sessions:3, progress:80, waste:'Medium' },
  { id:'S-0258', name:'Lucas Santiago', initials:'LS', sectionId:'section-b', section:'Section B', recipe:'Cookies', sessions:1, progress:30, waste:'High' },
  { id:'S-0259', name:'Trisha Gonzales', initials:'TG', sectionId:'section-b', section:'Section B', recipe:'Brownies', sessions:4, progress:100, waste:'Low' },
  { id:'S-0260', name:'Adrian Mercado', initials:'AM', sectionId:'section-b', section:'Section B', recipe:'Cupcakes', sessions:2, progress:75, waste:'Medium' }
].map((student, index) => learnerPerformance({ ...student, demo: true,
  assessment: { ratings: Object.fromEntries(rubricCriteria.map(({ key }, i) => [key, sampleRatings[index][i]])) }
}));

export const sessions = [
  { student:'Miguel Reyes', sectionId:'section-a', section:'Section A', recipe:'Cookies', stepId:'combine-chips', time:'14:10', events:['Base ingredients measured','4 base whisk passes completed','1/2 cup chocolate chips added'] },
  { student:'Noel Garcia', sectionId:'section-b', section:'Section B', recipe:'Brownies', stepId:'bake', time:'22:40', events:['Batter spread','Oven preheated','Brownie pan loaded'] }
].map(session => {
  const progress = getRecipeProgress(session.recipe, session.stepId);
  return { ...session, demo: true, recipeId: progress.recipe.id, recipeVersion,
    step: progress.step.title, current: progress.current, total: progress.total };
});

export const activities = [
  { sectionId:'section-b', text:'Noel began baking brownies', time:'30s' },
  { sectionId:'section-a', text:'Miguel corrected a measurement', time:'2m' },
  { sectionId:'section-a', text:'Bea earned Master Mixer', time:'8m' },
  { sectionId:'section-a', text:'Angela completed Brownies', time:'1h' }
];
