import { getRecipeProgress, recipeVersion } from './recipes.js';

export const students = [
  { id:'S-0241', name:'Angela Dela Cruz', initials:'AD', sectionId:'section-a', section:'Section A', recipe:'Brownies', sessions:4, score:88, progress:100, waste:'Low', status:'Passed', rating:4 },
  { id:'S-0242', name:'Miguel Reyes', initials:'MR', sectionId:'section-a', section:'Section A', recipe:'Cookies', sessions:2, score:62, progress:55, waste:'High', status:'Needs Practice', rating:2, live:true },
  { id:'S-0243', name:'Bea Santos', initials:'BS', sectionId:'section-a', section:'Section A', recipe:'Cupcakes', sessions:5, score:95, progress:100, waste:'Low', status:'Passed', rating:5 },
  { id:'S-0244', name:'Noel Garcia', initials:'NG', sectionId:'section-b', section:'Section B', recipe:'Brownies', sessions:3, score:74, progress:78, waste:'Medium', status:'Passed', rating:3, live:true },
  { id:'S-0245', name:'Liane Torres', initials:'LT', sectionId:'section-b', section:'Section B', recipe:'Cookies', sessions:1, score:60, progress:45, waste:'High', status:'Needs Practice', rating:2 },
  { id:'S-0246', name:'Carlo Mendoza', initials:'CM', sectionId:'section-b', section:'Section B', recipe:'Brownies', sessions:1, score:59, progress:44, waste:'High', status:'Incomplete', rating:1 },
  { id:'S-0247', name:'Sofia Ramos', initials:'SR', sectionId:'section-a', section:'Section A', recipe:'Cupcakes', sessions:4, score:92, progress:100, waste:'Low', status:'Passed', rating:5 },
  { id:'S-0248', name:'Ethan Lim', initials:'EL', sectionId:'section-a', section:'Section A', recipe:'Brownies', sessions:3, score:78, progress:85, waste:'Medium', status:'Passed', rating:3 },
  { id:'S-0249', name:'Isabel Flores', initials:'IF', sectionId:'section-a', section:'Section A', recipe:'Cookies', sessions:2, score:54, progress:60, waste:'High', status:'Needs Practice', rating:2 },
  { id:'S-0250', name:'Gabriel Tan', initials:'GT', sectionId:'section-a', section:'Section A', recipe:'Brownies', sessions:5, score:100, progress:100, waste:'Low', status:'Passed', rating:5 },
  { id:'S-0251', name:'Chloe Navarro', initials:'CN', sectionId:'section-a', section:'Section A', recipe:'Cupcakes', sessions:2, score:65, progress:70, waste:'Medium', status:'Passed', rating:3 },
  { id:'S-0252', name:'Joshua Bautista', initials:'JB', sectionId:'section-a', section:'Section A', recipe:'Cookies', sessions:1, score:42, progress:40, waste:'High', status:'Needs Practice', rating:1 },
  { id:'S-0253', name:'Mika Villanueva', initials:'MV', sectionId:'section-a', section:'Section A', recipe:'Cupcakes', sessions:4, score:86, progress:100, waste:'Low', status:'Passed', rating:4 },
  { id:'S-0254', name:'Daniel Aquino', initials:'DA', sectionId:'section-b', section:'Section B', recipe:'Cookies', sessions:3, score:81, progress:90, waste:'Low', status:'Passed', rating:4 },
  { id:'S-0255', name:'Alyssa Cruz', initials:'AC', sectionId:'section-b', section:'Section B', recipe:'Cupcakes', sessions:5, score:96, progress:100, waste:'Low', status:'Passed', rating:5 },
  { id:'S-0256', name:'Nathan Castillo', initials:'NC', sectionId:'section-b', section:'Section B', recipe:'Brownies', sessions:2, score:57, progress:65, waste:'High', status:'Needs Practice', rating:2 },
  { id:'S-0257', name:'Camille Reyes', initials:'CR', sectionId:'section-b', section:'Section B', recipe:'Cupcakes', sessions:3, score:73, progress:80, waste:'Medium', status:'Passed', rating:3 },
  { id:'S-0258', name:'Lucas Santiago', initials:'LS', sectionId:'section-b', section:'Section B', recipe:'Cookies', sessions:1, score:35, progress:30, waste:'High', status:'Needs Practice', rating:1 },
  { id:'S-0259', name:'Trisha Gonzales', initials:'TG', sectionId:'section-b', section:'Section B', recipe:'Brownies', sessions:4, score:89, progress:100, waste:'Low', status:'Passed', rating:4 },
  { id:'S-0260', name:'Adrian Mercado', initials:'AM', sectionId:'section-b', section:'Section B', recipe:'Cupcakes', sessions:2, score:68, progress:75, waste:'Medium', status:'Passed', rating:3 }
];

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
