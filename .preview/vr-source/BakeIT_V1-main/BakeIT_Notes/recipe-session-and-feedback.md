# BakeIT Cookie Recipe Session and Feedback

## Measured-cookie guide update — 2026-09-10

Phase 2 stages egg, one stick of butter, and chips from cold storage; flour, sugar, vanilla, baking soda, and salt from dry storage; the 1 Cup and 1/2 Cup measures, three teaspoon measures, and whisk on `Utensils Area`; then the white cookie bowl in MIX. The 1/4-cup vessel remains available for later dishes but is not required for cookies. Phase 3 is generated from the eight recipe requirements and displays their exact measurements.

## 2026-09-11 guided-onboarding correction

- Movement/look and whisk pickup, rotation, distance, and placement now occur before recipe selection. Cookie selection is rejected until the tutorial completes.
- Guidance is one target at a time: a subtly pulsing yellow bounding outline identifies the next object or introductory area. It uses unlit line geometry and creates no Light, so nothing spills across walls. Once an area introduction is completed, normal guidance does not repeatedly outline it.
- Carrying an out-of-order preparation item points to its correct destination. If it remains misplaced for 3.5 seconds after release, the item receives a red outline and its destination a yellow outline until corrected or picked up again.
- Carryable tool/ingredient names are direct-ray screen-space hover labels, always upright and depth-occluded. The guide uses heavier text and a taller header reserve, preventing long tray labels from overlapping the first task row.
- Restart is deliberate: hold `R` for three seconds. The former instant HUD button was removed.

## Recipe selection and heading/readability update — 2026-09-10

When `RecipeSelectionClipboard` is present, the persistent session now starts at `Choose a Recipe / RECIPE SELECTION` and ignores tutorial movement/look/tool progress until the player chooses the available cookie row with `E`. The physical paper lists all three approved dishes, but brownie and cupcake rows are explicitly locked as `IN DEVELOPMENT`; no unfinished recipe branch is implied or started.

The yellow title line now describes the current learning part instead of repeating the recipe name. The cream line combines the active recipe with the existing numbered/action label. Examples include `Movement Tutorial`, `Let's Practice Mise En Place`, `Let's Practice Mixing`, `Prepare the Baking Tray`, and `Preheat the Oven`. Uniform guide scaling increased from `1.35` to `1.5` for a modest readability improvement.

## Recipe-data migration — 2026-09-09

The session controller now resolves the active cookie definition and uses its display title, base-ingredient checklist, finishing-ingredient label, product label, and oven range. Existing component state still drives checklist completion, and the current thirteen-stage order remains cookie-specific. Persistent step records, a generic step graph, brownie/cupcake branches, scoring, and mode differences were not added.

## Purpose

`CookieRecipeSessionController` owns the current prototype guide presentation and complete-sequence restart. It does not duplicate ingredient, mixing, tray, or oven state. Those systems remain the authority for their own gameplay decisions.

## Runtime creation

- The controller is created automatically after the playable scene loads and persists across the recipe restart.
- The project still has one enabled build scene: `Kitchen Prototype`.
- The controller observes desktop input, the held pickup object, `MiseEnPlaceStation`, bowl, preparation board, tray liner, tray, oven zone, oven, and oven door to derive the current guide stage from authoritative gameplay state.

## Compact staged guide

- Evening measuring extension (2026-09-08): base rows specify Measuring Cup for flour/sugar; chip row specifies Small Measuring Spoon. While holding a full utensil, one extra row displays its name/contents and pour-return hint; footer switches to RMB tilt / hold Q discard. Discard hold progress appears in that same row. No permanent action log or grading panel was added. Session records MeasureTransfer events and discarded-unit count for future grading; scene restart clears this data.

- The guide shows only the current phase rather than the full recipe at once.
- Phase 1 has two compact three-row screens. First: WASD movement, mouse look and E pickup of the WHISK (not either spoon). Then: hold right click and move the mouse to rotate the held whisk, scroll to change distance, and E-release onto Utensils Area. Actual whisk holding, rotation/scroll input and settled placement are required; hovering or the wrong tool does not finish it. The current guide uses 150% uniform scaling.
- Phase 2 separates `COLD INGREDIENTS` (open fridge; egg, butter and chips), `DRY INGREDIENTS` (open dry cabinet; flour, sugar, vanilla, baking soda and salt), `UTENSILS AREA` (the six cookie-relevant tools), then `PLACE BOWL IN MIX`. Door-open hints latch when observed, including opening before the relevant screen. Staging an ingredient early still counts on its later screen. All active-cookie carriers must occupy the correct rests before workspace readiness latches. No automatic consumption or board staging is introduced.
- Phase 3 displays the four base-ingredient rows. Each changes from `[ ]` to a muted green `[x]` when its required quantity is in the bowl.
- Phase 4 shows four base whisk passes; phase 5 requires one 1/2-cup chip transfer; phase 6 counts three combining passes before dough appears. There are thirteen phases total.
- The lifted bowl produces `RETURN BOWL TO MIX` before recipe completion. Later phases cover board transfer, rolling, portioning, tray and oven. Board-clearing guidance remains only as recovery if supplies are accidentally put there, not a normal preparation task.
- Phase 11 is `PREHEAT OVEN`: set the recipe-approved target, keep the door closed, remove a prematurely loaded tray, and press `PREHEAT`. It shows current/target temperature while heating. Phase 12 loads the prepared tray only after the oven reports ready, and phase 13 starts/observes baking. The completion latch is unchanged.
- Normal progress messages remain in the Console but no longer replace the guide with action narration.
- Incorrect or incomplete actions appear as one compact orange correction for 4.5 seconds while the current checklist remains visible.
- Genuine configuration problems, such as a missing oven bake zone or inconsistent cookie-batch metadata, remain Unity warnings and use the same temporary correction area.
- The guide uses a uniform `1.35` presentation scale following the first readability retest. The widest panel is capped at approximately 554 pixels. The four-item base phase is approximately 265 pixels tall normally and approximately 321 pixels tall while a correction is visible.
- The first observed bake result is latched before tray and oven-state checks. Once `RECIPE COMPLETE` appears, moving the tray or changing live oven state cannot return the guide to `LOAD OVEN`; only a recipe restart clears the latch.

## Complete-sequence restart

- Press `R` at any time to restart the cookie sequence.
- Hold `R` for three seconds to restart. The HUD reports hold progress; there is no instant restart button.
- The controller reloads the active saved scene in `LoadSceneMode.Single` while Play Mode remains active.
- Reloading restores storage and removes runtime results: both door types close, ingredients return to cabinet/fridge, tools to home, bowl to the rack. All nine carriers and workspace readiness reset; both whisk counters clear. In-session tutorial completion persists, so repeat batches start at COLD INGREDIENT REST; fresh Play Mode starts at BASIC CONTROLS.

## Verification

- Focused Play Mode verification disabled the saved butter ingredient and created a runtime-only marker before restarting.
- After restart, the butter ingredient was active, the marker was gone, all five bowl quantities were zero, the bowl was incomplete, the oven was idle with no bake result, and the opening preparation stage was visible again.
- A follow-up isolated Unity 6000.5.0f1 compilation completed with no project-script errors or warnings.
- The original staged-guide verifier passed all eight earlier phase transitions. A later focused verifier passed the revised ten-stage flow: three tutorial tasks, four base ingredients, four-pass base mixing, three post-mix chip scoops, board progression, and completion latching.
- The completion verifier cleared the oven's live `BakeCompleted` value after the guide recorded a successful result; the guide correctly remained on `RECIPE COMPLETE`.
- A separate focused Play Mode verifier confirmed that three rolling passes do not flatten the dough and the fourth pass does.
- A focused Play Mode test confirmed that the renamed `Small Measuring Spoon` has the measuring component, can replace stale flour with chocolate chips during trigger stay, refills after being emptied while still overlapping the source, and activates its dense three-layer contents visual.
- The later visual correction uses eight contained cluster meshes and rejects chips in the large spoon with a short correction. Returning uncut dough now restores the board's authoritative occupancy and saved preparation progress, so the guide can resume rolling or cutting instead of remaining blocked at board transfer. The recovery path passed through a complete successful bake.

## Current limitations

- The session controller does not yet store a data-driven step graph, score, timer, waste events, or cloud payload.
- Restart returns the entire scene to its saved state; selective checkpoint restart is not implemented.
- Human retesting of guide readability and screen obstruction remains required after the action-log design failed its first player test.
- Physical reachability, pickup recovery, and gesture feel also remain human acceptance items.

## 2026-09-15 feedback — current completion and reset

The serving plate now determines completion: three Cookies or four Brownie squares. Brownies become grabbable only after all five cuts. Hold M clears the batch and restores recipe selection; R repeats the current recipe. Both preserve completed input practice. The recipe paper reflects exact ingredient completion in guided practice only. The user explicitly deferred grading until all dishes and the VR base game are complete and a data-informed rubric can be defined. See serving-and-plating-system.md.

## September 15 — partial Cupcake session

CookieRecipeSessionController resolves MixingSequence for Cupcake batter practice, shows its current bowl/ingredients/action, and reads aggregate exact amounts for the paper. The guide returns early from this branch before Cookie/Brownie oven/serving completion logic. BATTER PRACTICE COMPLETE leaves IsRecipeCompletionLatched false. R/M continue to use the established scene reload; M restores recipe selection and the normal Cookie/Brownie bowls. RecipeDefinition.batterPracticeOnly also prevents an unfinished dish from starting an oven bake. No grade or persistence beyond existing session behavior was added.

## September 15 — Cupcake baking session (current)

CupcakeBatch extends the existing MixingSequence branch through preparation, filling, timed baking, tester and rack cooling. Cupcakes.asset now clears batterPracticeOnly so the dedicated tray can bake. The Cupcake guide still returns before Cookie/Brownie serving logic: AllCooled is a partial practice endpoint and IsRecipeCompletionLatched remains false. R reloads/replenishes this recipe; M reloads selection and hides Cupcake equipment. No grading or new persistence was introduced.
