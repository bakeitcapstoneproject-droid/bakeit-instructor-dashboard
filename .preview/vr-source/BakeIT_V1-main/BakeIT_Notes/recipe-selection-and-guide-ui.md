# BakeIT Recipe Selection and Guide UI

## Current clarification — 2026-09-15

Movement/look/whisk practice gates recipe selection. Cookies and Brownies are selectable; Cupcakes remains locked. Brownie guidance now covers parchment, pouring, four spreading passes, preheat/bake, pan removal and five knife cuts, ending at Brownies Ready / 12 BROWNIE SQUARES. That title does not itself establish a successful bake; inspect the result row too. Earlier stop-after-mixing/baking guidance below is historical. Restart requires holding R for three seconds, with no instant HUD restart button. Player readability and gesture acceptance remain pending; no new runtime verification occurred today.


## Full-word framed recipe sheets — 2026-09-11

- Each framed sheet uses an orange dish title and a black difficulty line: Cookies `EASY`, Brownies `MODERATE`, and Cupcakes `INTERMEDIATE`.
- Cookie and Brownie summaries use one ingredient or process step per line. Measurements and ingredient names are written in full; slash-separated compact copy is no longer used.
- The summary TextMesh size is `.0043`. After the user adjusted the live poster layout, Cookie and Brownie summary local Y is `.126`; Cupcakes remains `-.10`. These saved user values are authoritative. Cupcake ingredient details remain unfinalized rather than inferred.
- Difficulty presentation is independent of completion status. Cookies are complete enough for their full current loop; Brownies is selectable as an explicitly bounded development branch; Cupcakes remains locked.

## Brownie development selection — 2026-09-11

- The movement-and-whisk tutorial still gates all recipe selection.
- After the tutorial, both the Cookie and Brownie framed papers accept `E`. The selected `RecipeDefinition` is propagated to the measuring set.
- The Brownie guide continues from its implemented large-bowl intake and mixing through lining the deep pan, pouring the finished batter, four Rubber Spatula passes, oven preheating/loading, and the 8-second bake. It now stops after the bake result at `NEXT PART STILL IN DEVELOPMENT` and says cooling and slicing into 12 squares will be added next.
- Cupcakes still reports that it is in development and does not change the active recipe.

### Brownie tutorial correction

The Brownie branch no longer jumps straight to bowl placement while showing Cookie guidance. Stage 2 is a Brownie-specific mise-en-place checklist, beginning with the fridge and four Eggs, then its dry ingredients, exact tools, and large bowl. The yellow outline and visible checklist are driven by the same active-recipe state. During measurement, an empty required cup points to its ingredient source and a filled cup points to the Brownie bowl.

After the batter is complete, the same state-driven pairing continues: parchment or deep pan, held bowl or pan, Rubber Spatula or batter surface, oven preheat, held pan or oven zone, then the oven controls. The outline stops when the bake result is complete so the development-boundary message is not accompanied by a misleading target.

## Onboarding/readability revision — 2026-09-11

Movement and whisk-handling practice is now the first stage. Recipe posters remain visible, but selection is rejected until movement, look, whisk pickup, held rotation, hold-distance adjustment, and placement on `Utensils Area` have all been observed. After that, the cookie poster becomes the guided target; brownies and cupcakes remain locked.

Guide labels use bold text and reserve a taller two-line header block so long stage names do not merge with the first checklist row. Bowl/tray language is explicit: `WHITE COOKIE BOWL`, `FLAT COOKIE TRAY`, and the locked Brownie poster names its `DEEP TRAY`. Restart guidance says `Hold R for 3 seconds`; the instant HUD button is removed.

The BakeIT identity panel is smaller, higher, warm brown, and flanked by simple cookie emblems. The three recipe papers sit higher in squarer brown frames. The user's final saved world-text orientations supersede automated facing adjustments and must be preserved in later layout passes.

## Separate framed sheets — 2026-09-10

The first shared clipboard was replaced after the user's visual clarification. The current wall display is `Kitchen Environment/Recipe Selection Posters`: three separate dark-framed cream papers arranged in one horizontal row beneath BakeIT, matching the reference's individual recipe-poster structure. Each whole poster is its own `E` target.

- Cookie poster: title, `READY - PRESS E`, approved ingredient summary, and `MIX / ROLL / CUT / BAKE` summary.
- Brownie poster: title, `IN DEVELOPMENT`, the approved measured base ingredients including 1/2 cup walnuts, and `MIX / SPREAD / BAKE / SLICE` summary.
- Cupcake poster: title, `IN DEVELOPMENT`, and `RECIPE DETAILS TO BE FINALIZED`; no unapproved formula was invented.

The selector state, one-ready/two-locked availability, guide gate, restart persistence, and warnings are unchanged. Play Mode verification passed the three independent poster structures, horizontal spacing, initial gate, locked Brownie interaction, Cookie start interaction, and removal of the old shared board.

## Implemented start flow — 2026-09-10

`Kitchen Prototype` begins with physical recipe papers centered beneath the existing `BakeIT` wall sign. The current framed posters use dark borders, cream paper, and world-space text made from existing project materials. They list the complete approved dish scope:

- `[E] CHOCOLATE-CHIP COOKIES - READY`
- `BROWNIES - IN DEVELOPMENT`
- `CUPCAKES - IN DEVELOPMENT`

The player looks at a row and presses `E`. `PickupController` routes its existing forward interaction ray to `RecipeSelectionButton`. Only the assigned and implemented cookie recipe can begin. Interacting with either unfinished dish consumes the interaction and gives a clear warning without changing recipe state.

## Session behavior

- `RecipeSelectionClipboard` marks scenes that require a start choice. Existing isolated fixtures without the clipboard retain their earlier immediate cookie behavior.
- `CookieRecipeSessionController` waits at `Choose a Recipe / RECIPE SELECTION` and does not count movement, mouse, or tool tutorial progress before selection.
- Selecting cookies starts the existing cookie flow at `Movement Tutorial / 1 / 13 BASIC CONTROLS`.
- The selected recipe remains chosen during an in-session `R`/button restart because the persistent session controller owns the selection. A fresh Play Mode session starts at the clipboard again.
- The clipboard is a truthful selection shell, not a dynamic multi-recipe runtime yet. Brownie and cupcake rows must remain unavailable until their complete recipe assets, interactions, validation, guide branches, and reset behavior exist.

## Guide readability and headings

The guide scale increased from `1.35` to `1.5`. The yellow line now names the current learning section rather than repeating the recipe title. Examples include `Movement Tutorial`, `Let's Practice Mise En Place`, `Let's Build the Dough`, `Let's Practice Mixing`, `Prepare the Baking Tray`, and `Preheat the Oven`. The cream line retains the selected recipe name plus the exact numbered/action stage.

## Verification

- Visual render confirmed the clipboard is centered under the sign and all three rows remain readable above the worktop.
- Real Play Mode interaction-ray test passed initial selection gating, locked brownie feedback, cookie selection, transition to the movement tutorial, one available/three total row configuration, and intended wall placement.
- Marker: `RECIPE_CLIPBOARD_QA PASS=True; structure=True; initialGate=True; lockedRay=True; cookieRay=True; position=True; heading='Movement Tutorial'; stage='1 / 13  BASIC CONTROLS'`.
- An additional scene-reload check passed selection persistence through the existing restart path: `RECIPE_SELECTION_RESTART_QA PASS=True; selected=chocolate-chip-cookies; reset=False; heading=Movement Tutorial`.
- Final saved-scene audit: clipboard present, three rows, one available, zero preview/QA objects, `dirty=false`, Play Mode stopped, and zero Console errors/warnings.

## Player acceptance still needed

The selector is technically reachable within the existing three-metre interaction distance, but natural first-time discovery, row-targeting comfort, paper text size, and the enlarged HUD at the player's preferred resolution still need normal human playtesting.

## Contour guidance revision — 2026-09-11

Tutorial targets use a tight yellow mesh silhouette rather than a rectangular wire box or a light source. The outline follows the object's visible shape without illuminating the room. It disappears from an object as soon as that object is held, after which the required destination may be outlined. The center crosshair is `10 x 10` pixels, and Eggs are held by their visible center directly on that crosshair.

The visibility-tuned contour pulses between `6.65` and `7.85` pixels in a brighter warm yellow. It remains an outline only and still emits no scene light.

## 2026-09-15 feedback — current guide and completion

Hold M for three seconds to clear the current batch and return to recipe selection; completed control practice is retained. Hold R for three seconds to repeat the selected recipe. Both routes use the existing scene reset. A mid-batch poster selection explains the M route. The HUD shows its hold progress and the leave-batch meaning.

The guide ends with plating three Cookies or four Brownie squares, then Ready to Serve / SERVING COMPLETE. It retains bake outcomes without assigning grades. RecipeSelectionButton colors satisfied ingredient lines green only for the active recipe in guided practice. SetGuidedPractice(false) removes that assistance for future mode integration; no new mode-selection UI was added. See serving-and-plating-system.md and test-log.md.

## September 15 follow-up — serving choices and completed-bowl target

At the Brownie plating stage, the guide exposes [2] two / [3] three brownies and the selected target. These keys also work after completing a smaller serving, allowing the third piece. Choosing two while three are plated asks the player to remove one first. Both targets are acceptable. The default is two. The empty transferred Brownie bowl no longer becomes a tutorial target through earlier mise-en-place checks; pan preparation/spreading/baking guidance takes over. R/M behavior and deferred grading are unchanged.

## September 15 — Hard Cupcake batter practice

The Cupcake paper is selectable after movement practice and says HARD | BATTER PRACTICE. It lists the nine batter ingredients and explicitly defers baking/fresh frosting. Guide steps name the Dry or Batter bowl, ingredient/unit/count, and whisk/scraper work. Nine paper lines turn green only at exact aggregate quantities in guided practice; the two eggs and compound measures are not complete after partial additions. BATTER PRACTICE COMPLETE is distinct from full recipe/serving completion. The completed batter bowl stops receiving yellow guidance. Existing R/M scene-reset routes remain available.

Cupcake guide rows use measured wrapped-text heights, and the stage prefix is shortened to CUPCAKES so the practice/difficulty/step context stays readable. Cookie/Brownie row heights retain their existing layout. Bowl rows name the actual CUPCAKE DRY or MIX rest rather than relying on color alone.

## September 15 — Cupcake baking-practice guide (current)

The Cupcake selector now says HARD | BAKING PRACTICE. Guide order is empty-oven preheat and twelve liners, the established nine batter phases, twelve three-quarter fills, baking, centre tester and rack cooling. The tutorial target moves to the relevant equipment/well/rack position. Green ingredient-paper behavior is unchanged. The final boundary is BAKING PRACTICE COMPLETE; from-scratch frosting, piping and serving are explicitly next.
