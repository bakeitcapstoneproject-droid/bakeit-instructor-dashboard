import { getRecipeProgress, recipeVersion } from '../public/assets/js/recipes.js';

const samples = {
  Brownies: {
    stepId: 'bake', time: '22:40',
    events: ['Batter spread with 4 spatula passes', 'Oven preheated', 'Brownie pan loaded']
  },
  Cookies: {
    stepId: 'combine-chips', time: '14:10',
    events: ['Base ingredients measured', '4 base whisk passes completed', '1/2 cup chocolate chips added']
  },
  Cupcakes: {
    stepId: 'fill-liners', time: '10:25',
    events: ['Oven preheated and 12 wells lined', '9 batter phases completed', 'Final bowl scrape completed']
  }
};

// Fixed presentation samples derived only from explicitly tagged demo learners.
// Removing demo learners also removes their sample sessions.
export function demoSessions(learners) {
  const counts = new Map();
  return learners.filter(learner => {
    const count = counts.get(learner.sectionId) || 0;
    if (!learner.demo || !Object.hasOwn(samples, learner.recipe) || count >= 3) return false;
    counts.set(learner.sectionId, count + 1);
    return true;
  }).map(learner => {
    const sample = samples[learner.recipe];
    const progress = getRecipeProgress(learner.recipe, sample.stepId);
    return {
      id: `session-${learner.id}`, learnerId: learner.id, student: learner.name,
      sectionId: learner.sectionId, section: learner.section, recipe: learner.recipe,
      ...sample, recipeId: progress.recipe.id, recipeVersion,
      step: progress.step.title, current: progress.current, total: progress.total, demo: true
    };
  });
}
