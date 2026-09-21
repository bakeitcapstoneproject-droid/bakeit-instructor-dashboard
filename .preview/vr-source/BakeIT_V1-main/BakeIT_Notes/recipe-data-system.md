# BakeIT Recipe Data System

## Current clarification — 2026-09-15

Both Cookie and Brownie RecipeDefinition assets exist and drive their selectable branches. Cookies uses eight ingredients, 4+3 whisk passes, four rolls, six portions and a five-second 170–190 C success profile (burnt at 220 C). Brownies uses exact measures including compound Flour and four eggs, 4+2 whisk passes, four spreading passes, twelve squares and an eight-second 170–180 C success profile (burnt at 210 C). Five knife strokes are implemented in BrowniePanReceiver; do not assume every workflow count lives in RecipeDefinition. Cupcakes.asset now drives the selected exact formula and staged baking-practice branch through rack cooling; its game bake is ten seconds at 180 C (burnt at 220 C). From-scratch frosting/piping/serving remain next. Older Cookie-only, locked-Brownie and unfinalized-Cupcake claims below are historical. Current focused runtime results are in test-log.md.


## Purpose

`RecipeDefinition` is the shared configuration layer for the three approved recipe families. It keeps recipe facts outside the interaction scripts so an ingredient amount, valid measuring tool, interaction count, product state name, or bake profile can be changed in one asset rather than repeated across the bowl, guide, board, tray, and oven.

This remains a configuration foundation rather than a fully generic recipe engine. The working Cookie flow is migrated, and a locked Brownie definition now supplies approved recipe facts to its existing melted-butter receiver. Full Brownie runtime orchestration and all Cupcake workflow, icing interaction, scoring, mode, and cloud behavior remain unimplemented.

## Files and scene ownership

- Model: `Assets/BakeIT/Scripts/Recipes/RecipeDefinition.cs`.
- Assets: `Assets/BakeIT/RecipeDefinitions/ChocolateChipCookies.asset` and the locked `Assets/BakeIT/RecipeDefinitions/Brownies.asset`.
- Scene authority: `IngredientTrigger/BowlReceiver.recipeDefinition` in `Kitchen Prototype` references the cookie asset.
- Existing systems discover the active definition through `BowlReceiver.ActiveRecipe`; no second recipe manager or duplicate state machine was added.
- If no definition is assigned, the migrated components retain their established cookie fallback values so isolated fixtures and older scenes do not fail.

## Configured cookie data

| Category | Current value |
|---|---|
| Recipe ID / display | `chocolate-chip-cookies` / Chocolate-Chip Cookies |
| Base ingredients | Flour: 1 cup; Sugar: 1 cup; Butter: 1 stick whole item; Egg: 1 large whole egg; Vanilla: 1 teaspoon; BakingSoda: 1/2 teaspoon; Salt: 1/4 teaspoon |
| Finishing ingredient | ChocolateChips: 1 x 1/2 Cup measure (`HalfCup`) |
| Mixing | 4 base passes + 3 finishing passes |
| Shaping | Roll and portion; 4 rolling passes |
| Yield | Target 6, minimum 6, maximum 6 |
| Pan preparation | Parchment |
| Post-bake finish | None |
| Runtime names | Dough, FlattenedDough, CookieDoughPortion, UnderbakedCookie, BakedCookie, OvercookedCookie, BurntCookie |
| Bake profile | 5 seconds; perfect 170–190 C; burnt at 220 C and above |

These reflect the user's 2026-09-11 correction to one butter stick and one 1/2-cup chocolate-chip transfer. Six-cookie yield and 1.2-second whisk/rolling cooldowns remain unchanged.

## Consumers

- `BowlReceiver`: reads ingredient requirements/stages and base/finishing mix counts; whole-item consumption follows the configured tool role. Existing cookie visuals remain presentation logic.
- `MeasuringScoop`: accepts an ingredient only when the active definition lists it for that exact tool; an unlisted ingredient is rejected instead of falling back to cookie assumptions.
- `DoughBoardController`: reads mixed/shaped/portion names, rolling count, and target/minimum/maximum yield.
- `TrayReceiver`: reads accepted portion and baked-result names plus product labels.
- `OvenController`: reads bake duration, perfect range, burnt threshold, and result ingredient names. Underbaked cookies still receive zero material changes. Ambient temperature and preheat duration remain oven interaction tuning rather than recipe data.
- `CookieRecipeSessionController`: reads the recipe title, base-ingredient checklist, finishing-ingredient label, product label, and oven target range. Its thirteen-stage sequencing remains cookie-specific.

## Scope and approval boundary

- The guide/manuscript is provisional, while the approved recipe scope remains cookies, brownies, and cupcakes.
- Configuration vocabulary includes rolling/pouring/filling, parchment/greased-pan/cupcake-liner, and optional icing values so the one model can describe the approved recipe families. These values do not implement those interactions.
- A future recipe asset must not be treated as authorization to invent its formula or flow. Ingredient lists, leavening, pan preparation, icing stages, quantities, temperature bands, and new mechanics require user agreement before implementation when the guide does not settle them.

## Verification — 2026-09-09

- Unity compilation succeeded after the model, consumers, asset, and scene reference were added.
- The original 2026-09-09 Edit Mode migration audit confirmed the then-current five-ingredient asset. On 2026-09-10 the approved formula expanded it to eight requirements and exact 1-cup/teaspoon/chip-scoop roles.
- Play Mode confirmed the same asset instance was resolved by bowl, board, tray, oven, measuring tools, and session guide.
- Both actual measuring tools retained their current correct rules and rejected `Cocoa`, which is absent from the active recipe.
- Runtime-only receiver callbacks rejected early chocolate chips, rejected an unlisted ingredient, rejected duplicate/extra quantities, accepted the four base ingredients, formed base dough at 4 passes, accepted exactly 3 chip measures, and completed at 3 finishing passes.
- The first attempted deterministic gate audit used reflection and was rejected by the Unity bridge before execution. It made no gameplay or scene change and was replaced with receiver callbacks.
- Final scene reload found the recipe assigned, `dirty=false`, zero `RecipeDataQA_*` objects, and zero Console errors/warnings.

## Known limitations

- Cookie is the only playable asset. The Brownie asset and large-bowl binding exist, but data alone does not provide its measured intake, mixing visuals, pan spreading, baking, slicing, guide, or reset workflow; Cupcakes have no finalized asset.
- The 2026-09-10 physical recipe posters are a truthful start selector shell. They select only the already assigned `chocolate-chip-cookies` definition; brownie and cupcake posters remain locked and labeled `IN DEVELOPMENT`. Dynamic asset switching must wait for those approved recipe definitions and complete runtime branches.
- `CookieRecipeSessionController`, `MiseEnPlaceStation`, cookie ingredient visuals, dough decoration, and some feedback wording still encode the current cookie sequence. Generalizing those should happen alongside an approved second recipe rather than speculating now.
- Runtime progress is still component state, not a serialized recipe-step graph or persistent cross-session record.
- No custom Inspector validation currently checks duplicate ingredient IDs or empty runtime-name fields.

## Next approved decision point

Brownie ingredients, parchment-only pan preparation, pour/spread behavior, walnuts, and 12-piece slicing are approved. Before activating the brownie poster, confirm its prototype bake duration and whether cooling is required or optional. Cupcake leavening and icing remain a separate later discussion.

## 2026-09-15 — Cupcake source update

Cupcake formula/source is selected and recorded in cupcakes.md. Existing RecipeMeasureTool has no tablespoon role; the selected formula and staged creaming/dry-mixture/frosting workflow will need explicit additions. No Cupcake RecipeDefinition asset or playable branch has been created in this source-selection pass. Older statements that the formula is unfinalized are superseded; gameplay tuning is still pending.

## September 15 — ordered Cupcake batter data

Cupcakes.asset now contains the exact selected batter formula, nine RecipeMixingPhase entries and an explicit batterPracticeOnly boundary. RecipeMixingPhase identifies bowl role, required additions, Mix/Scrape action and pass count. Existing Cookie/Brownie assets keep their base/finishing structure. OneTablespoon is appended at enum value 10; prior serialized values remain stable. Internal CupcakeDryBlend belongs to transfer phases, not raw-ingredient paper totals. MixingSequence owns per-phase quotas and aggregate ingredient counts. Cupcakes remain an incomplete dish even when batter practice ends. See cupcakes.md and test-log.md.

## September 15 — Cupcake baking values (current)

Cupcakes.asset now enables baking: 10-second duration, 180 C perfect target, below 180 underbaked, above 180 and below 220 overcooked, 220 or more burnt. Product/result IDs are CupcakeBatter, CupcakeBatterPortion, UnderbakedCupcake, BakedCupcake, OvercookedCupcake and BurntCupcake. Twelve-well and per-cupcake cooling state lives in CupcakeBatch. This remains an unfinished dish even though batterPracticeOnly is false; full completion is controlled by the session branch, not that flag alone.
