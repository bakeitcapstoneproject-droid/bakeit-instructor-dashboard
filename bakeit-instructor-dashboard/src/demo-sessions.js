const recipes = {
  Brownies: {
    step: 'Check brownie doneness', current: 7, total: 9, time: '22:40',
    events: ['Ingredients measured', 'Batter mixed and pan prepared', 'Baking timer started']
  },
  Cookies: {
    step: 'Mix wet ingredients', current: 6, total: 11, time: '14:10',
    events: ['Ingredients measured', 'Measurement corrected', 'Wet ingredients combined']
  },
  Cupcakes: {
    step: 'Fill cupcake liners', current: 5, total: 10, time: '10:25',
    events: ['Oven preheated', 'Batter mixed', 'Cupcake liners prepared']
  }
};

// Fixed presentation samples derived only from explicitly tagged demo learners.
// Removing demo learners also removes their sample sessions.
export function demoSessions(learners) {
  const counts = new Map();
  return learners.filter(learner => {
    const count = counts.get(learner.sectionId) || 0;
    if (!learner.demo || !recipes[learner.recipe] || count >= 3) return false;
    counts.set(learner.sectionId, count + 1);
    return true;
  }).map(learner => ({
    id: `session-${learner.id}`, learnerId: learner.id, student: learner.name,
    sectionId: learner.sectionId, section: learner.section, recipe: learner.recipe,
    ...recipes[learner.recipe], demo: true
  }));
}
