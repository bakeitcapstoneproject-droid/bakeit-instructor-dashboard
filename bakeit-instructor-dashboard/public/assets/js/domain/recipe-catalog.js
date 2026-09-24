// Shared by the browser and Node sample-session factory; no DOM or I/O dependencies.
export class RecipeCatalog {
  constructor(recipes) { this.recipes = recipes; }

  get(value) {
    return this.recipes.find(recipe => [recipe.id, recipe.name, recipe.displayName].includes(value));
  }

  progress(recipeId, stepId, completed = false) {
    const recipe = this.get(recipeId);
    const index = recipe?.steps.findIndex(item => item.id === stepId) ?? -1;
    if (index < 0) return null;
    const finished = completed && index === recipe.steps.length - 1;
    const completedSteps = finished ? recipe.steps.length : index;
    return {
      recipe, step: recipe.steps[index], current: index + 1, total: recipe.steps.length,
      completed: finished, completedSteps,
      percent: Math.round(completedSteps / recipe.steps.length * 100)
    };
  }
}
