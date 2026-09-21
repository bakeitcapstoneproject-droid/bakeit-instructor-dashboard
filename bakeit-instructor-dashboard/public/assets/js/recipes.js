// Matched to BakeIT_V1-main.zip: RecipeDefinitions and CookieRecipeSessionController.
// These IDs are dashboard identifiers for a future Unity adapter, not existing Unity events.
export const recipeVersion = 'bakeit-v1-2026-09-15';
const step = (id, title, instruction) => ({ id, title, instruction });

export const recipes = [
  {
    id: 'chocolate-chip-cookies', name: 'Cookies', displayName: 'Chocolate-Chip Cookies',
    completionLabel: 'Serving complete', scope: '6 cookies · Plate 3 to finish',
    steps: [
      step('collect-cold', 'Collect cold ingredients', 'Place the egg, butter and chocolate chips on Cold Ingredient Rest.'),
      step('collect-dry', 'Collect dry ingredients', 'Place flour, sugar, vanilla, baking soda and salt on the ingredient rest.'),
      step('prepare-tools', 'Prepare the utensils', 'Place the 1 cup and 1/2 cup measures, 1 tsp, 1/2 tsp and 1/4 tsp spoons, and whisk on Utensils Area.'),
      step('place-bowl', 'Place the cookie bowl', 'Move the white cookie bowl from the rack to MIX.'),
      step('measure-base', 'Add the base ingredients', 'Add 1 cup flour, 1 cup sugar, 1 stick butter, 1 egg, 1 tsp vanilla, 1/2 tsp baking soda and 1/4 tsp salt.'),
      step('mix-base', 'Whisk the base dough', 'Complete 4 whisk passes before adding chocolate chips.'),
      step('add-chips', 'Add the chocolate chips', 'Tilt one 1/2 cup measure of chocolate chips into the bowl.'),
      step('combine-chips', 'Combine the chocolate chips', 'Complete 3 more whisk passes to finish the dough.'),
      step('move-dough', 'Move the dough to the board', 'Clear the chopping board and place the finished dough on it.'),
      step('roll-dough', 'Flatten the dough', 'Complete 4 rolling-pin passes.'),
      step('portion-dough', 'Portion the dough', 'Use the portioning knife to divide the dough into 6 equal cookies.'),
      step('prepare-tray', 'Line and load the tray', 'Line the flat cookie tray with parchment and place all 6 portions on it.'),
      step('preheat', 'Preheat the oven', 'Set 170–190 °C, close the empty oven and press PREHEAT. Wait until ready.'),
      step('load-oven', 'Load the oven', 'Place the prepared cookie tray inside the preheated oven.'),
      step('bake', 'Bake the cookies', 'Close the door and press BAKE. The game uses a 5-second bake. Check the bake result.'),
      step('plate', 'Plate a serving', 'Set the tray on the worktop and place 3 cookies on the serving plate.')
    ]
  },
  {
    id: 'brownies', name: 'Brownies', displayName: 'Brownies',
    completionLabel: 'Serving complete', scope: '12 squares · Plate 2 or 3 to finish',
    steps: [
      step('collect-cold', 'Collect cold ingredients', 'Place 4 large eggs on Cold Ingredient Rest.'),
      step('collect-dry', 'Collect the remaining ingredients', 'Gather flour, sugar, melted butter, cocoa, vanilla, baking powder, salt and walnuts onto the ingredient rest.'),
      step('prepare-tools', 'Prepare the utensils', 'Prepare the 1 cup, 1/2 cup and 1/4 cup measures, 1 tsp and 1/2 tsp spoons, and whisk.'),
      step('place-bowl', 'Place the brownie bowl', 'Move the large brownie mixing bowl to MIX.'),
      step('measure-base', 'Measure the base ingredients', 'Add 2 cups sugar, 1 cup plus 1/4 cup flour, 1 cup melted butter, 4 eggs, 1/2 cup cocoa, 1 tsp vanilla, 1/2 tsp baking powder and 1/2 tsp salt.'),
      step('mix-base', 'Mix the brownie batter', 'Complete 4 base whisk passes.'),
      step('add-walnuts', 'Add the walnuts', 'Add one 1/2 cup measure of walnuts after the base is mixed.'),
      step('fold-walnuts', 'Fold in the walnuts', 'Complete 2 finishing whisk passes.'),
      step('line-pan', 'Line the deep pan', 'Place parchment inside the deep brownie pan.'),
      step('pour-batter', 'Pour the batter into the pan', 'Hold and tilt the brownie bowl over the lined pan.'),
      step('spread-batter', 'Spread the batter evenly', 'Complete 4 passes with the rubber spatula.'),
      step('preheat', 'Preheat the oven', 'Set 170–180 °C, close the empty oven and press PREHEAT. Wait until ready.'),
      step('load-oven', 'Load the brownie pan', 'Place the prepared deep pan inside the preheated oven.'),
      step('bake', 'Bake the brownies', 'Close the door and press BAKE. The game uses an 8-second bake. Check the bake result.'),
      step('slice', 'Cut the brownies', 'Remove the pan to the worktop. Complete 5 knife strokes to make 12 squares in a 4-by-3 grid.'),
      step('plate', 'Plate a serving', 'Choose 2 or 3 brownie squares and place that number on the serving plate.')
    ]
  },
  {
    id: 'cupcakes', name: 'Cupcakes', displayName: 'Vanilla Cupcakes',
    completionLabel: 'Baking practice complete', scope: '12 cupcakes · Baking practice through cooling',
    limitation: 'This build ends after cooling. Buttercream, piping and serving are not yet included.',
    steps: [
      step('prepare-tray', 'Preheat and line the cupcake tray', 'Preheat the empty oven to 180 °C. Place the tray on the worktop and line all 12 wells before mixing.'),
      step('mix-dry', 'Combine the Dry Ingredients', 'In the Dry bowl: 1 cup plus 1/4 cup flour, 1 tsp plus 1/4 tsp baking powder and 1/4 tsp salt. Whisk 2 passes.'),
      step('cream', 'Cream Butter, Sugar, Oil and Vanilla', 'In the Batter bowl: 6 tbsp softened butter (84 g), 1/2 cup plus 1/4 cup sugar, 2 tbsp oil and 1 tsp plus 1/2 tsp vanilla. Whisk 4 passes.'),
      step('first-egg', 'Add and Mix the First Egg', 'Add 1 large egg to the Batter bowl and whisk 1 pass.'),
      step('second-egg', 'Add and Mix the Second Egg', 'Add the second large egg and whisk 1 pass.'),
      step('scrape-bowl', 'Scrape Down the Bowl', 'Sweep the inside of the Batter bowl with the scraper, 1 pass.'),
      step('half-dry', 'Pour and Mix Half the Dry Mixture', 'Transfer half the Dry bowl mixture to the Batter bowl. Whisk 2 passes.'),
      step('milk', 'Slowly Add and Mix the Milk', 'Add 1/2 cup plus 2 tbsp milk to the Batter bowl. Whisk 2 passes.'),
      step('remaining-dry', 'Add and Mix the Remaining Dry Mixture', 'Transfer the remaining half of the dry mixture and whisk 2 passes.'),
      step('finish-batter', 'Scrape the Bowl and Finish the Batter', 'Complete 1 final scraper pass inside the Batter bowl.'),
      step('fill-liners', 'Fill the cupcake liners', 'Tilt the finished batter bowl over each lined well. Fill all 12 wells three-quarters full.'),
      step('bake', 'Bake the cupcakes', 'Load the tray, close the door and press BAKE at 180 °C. The game uses a 10-second bake.'),
      step('check-doneness', 'Check cupcake doneness', 'Return the tray to the worktop and touch the tester tip into a cupcake. Check the tester and bake result.'),
      step('cool', 'Cool the cupcakes on the rack', 'Move all 12 cupcakes to the cooling rack. Each must cool on the rack for 10 game seconds to finish baking practice.')
    ]
  }
];

export function getRecipe(value) {
  return recipes.find(recipe => [recipe.id, recipe.name, recipe.displayName].includes(value));
}

export function getRecipeProgress(recipeId, stepId, completed = false) {
  const recipe = getRecipe(recipeId);
  const index = recipe?.steps.findIndex(item => item.id === stepId) ?? -1;
  if (index < 0) return null;
  const finished = completed && index === recipe.steps.length - 1;
  return {
    recipe, step: recipe.steps[index], current: index + 1, total: recipe.steps.length,
    completed: finished, completedSteps: finished ? recipe.steps.length : index,
    percent: Math.round((finished ? recipe.steps.length : index) / recipe.steps.length * 100)
  };
}
