import test from 'node:test';
import assert from 'node:assert/strict';
import { getRecipe, getRecipeProgress, recipes } from '../public/assets/js/recipes.js';
import { demoSessions } from '../src/demo-sessions.js';
import { sessions } from '../public/assets/js/data.js';

test('recipe paths preserve the game-specific ordering and completion scope', () => {
  assert.deepEqual(recipes.map(recipe => recipe.steps.length), [16, 16, 14]);
  for (const recipe of recipes) assert.equal(new Set(recipe.steps.map(step => step.id)).size, recipe.steps.length);
  const cookie = getRecipe('chocolate-chip-cookies');
  assert.equal(cookie, getRecipe('Cookies'));
  assert.deepEqual(cookie.steps.slice(5, 8).map(step => step.id), ['mix-base', 'add-chips', 'combine-chips']);
  assert.deepEqual(getRecipe('Brownies').steps.slice(-3).map(step => step.id), ['bake', 'slice', 'plate']);
  assert.deepEqual(getRecipe('Cupcakes').steps.slice(1, 10).map(step => step.id),
    ['mix-dry', 'cream', 'first-egg', 'second-egg', 'scrape-bowl', 'half-dry', 'milk', 'remaining-dry', 'finish-batter']);
  assert.equal(getRecipe('Cupcakes').steps.at(-1).id, 'cool');
  assert.equal(getRecipe('Cupcakes').completionLabel, 'Baking practice complete');
});

test('active steps are not counted as complete and invalid progress is not invented', () => {
  assert.equal(getRecipeProgress('Cookies', 'collect-cold').percent, 0);
  assert.equal(getRecipeProgress('Brownies', 'plate').completedSteps, 15);
  assert.equal(getRecipeProgress('Brownies', 'plate', true).percent, 100);
  assert.equal(getRecipeProgress('Cupcakes', 'cool', true).completedSteps, 14);
  assert.equal(getRecipeProgress('Cupcakes', 'milk', true).completed, false);
  assert.equal(getRecipeProgress('missing', 'bake'), null);
  assert.equal(getRecipeProgress('Brownies', 'check-doneness'), null);
});

test('API samples and mock sessions use the same recipe catalog', () => {
  const learners = recipes.map((recipe, index) => ({ id: `demo-${index}`, name: 'Learner', sectionId: 'A', section: 'A', recipe: recipe.name, demo: true }));
  const samples = demoSessions(learners);
  assert.equal(samples.length, 3);
  for (const sample of [...samples, ...sessions]) {
    const progress = getRecipeProgress(sample.recipeId, sample.stepId);
    assert.equal(sample.step, progress.step.title);
    assert.equal(sample.current, progress.current);
    assert.equal(sample.total, progress.total);
    assert.equal(sample.demo, true);
  }
  assert.deepEqual(demoSessions([{ ...learners[0], demo: false }]), []);
  assert.deepEqual(demoSessions([{ ...learners[0], recipe: 'unknown' }]), []);
});
