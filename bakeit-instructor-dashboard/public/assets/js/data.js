export const students = [
  { id:'S-0241', name:'Angela Dela Cruz', initials:'AD', sectionId:'section-a', section:'Section A', recipe:'Brownies', sessions:4, score:88, progress:100, waste:'Low', status:'Passed', rating:4 },
  { id:'S-0242', name:'Miguel Reyes', initials:'MR', sectionId:'section-a', section:'Section A', recipe:'Cookies', sessions:2, score:62, progress:55, waste:'High', status:'Needs Practice', rating:2, live:true },
  { id:'S-0243', name:'Bea Santos', initials:'BS', sectionId:'section-a', section:'Section A', recipe:'Cupcakes', sessions:5, score:95, progress:100, waste:'Low', status:'Passed', rating:5 },
  { id:'S-0244', name:'Noel Garcia', initials:'NG', sectionId:'section-b', section:'Section B', recipe:'Brownies', sessions:3, score:74, progress:78, waste:'Medium', status:'Passed', rating:3, live:true },
  { id:'S-0245', name:'Liane Torres', initials:'LT', sectionId:'section-b', section:'Section B', recipe:'Cookies', sessions:1, score:60, progress:45, waste:'High', status:'Needs Practice', rating:2 },
  { id:'S-0246', name:'Carlo Mendoza', initials:'CM', sectionId:'section-b', section:'Section B', recipe:'Brownies', sessions:1, score:59, progress:44, waste:'High', status:'Incomplete', rating:1 }
];

export const sessions = [
  { student:'Miguel Reyes', sectionId:'section-a', section:'Section A', recipe:'Cookies', step:'Mix wet ingredients', current:6, total:11, time:'14:10', events:['Measurement corrected','Safety reminder acknowledged','First mixture completed'] },
  { student:'Noel Garcia', sectionId:'section-b', section:'Section B', recipe:'Brownies', step:'Check brownie doneness', current:7, total:9, time:'22:40', events:['Batter mixed','Pan prepared','Baking timer started'] }
];

export const activities = [
  { sectionId:'section-b', text:'Noel began Second proof', time:'30s' },
  { sectionId:'section-a', text:'Miguel corrected a measurement', time:'2m' },
  { sectionId:'section-a', text:'Bea earned Master Mixer', time:'8m' },
  { sectionId:'section-a', text:'Angela completed Brownies', time:'1h' }
];
