# Chocolate-Chip Cookie Vertical Slice

## Approved measured formula — 2026-09-10

- 1 stick of softened butter (one whole butter item in the prototype)
- 1 cup granulated white sugar
- 1 large egg
- 1 teaspoon vanilla extract
- 1 cup all-purpose flour
- 1/2 teaspoon baking soda
- 1/4 teaspoon salt
- 1/2 cup chocolate chips represented by one `1/2 Cup Measuring Cup` transfer

This formula supersedes the earlier simplified four-base-ingredient prototype notes below. Mixing counts, six-cookie yield, parchment behavior, shaping, cooldowns, and bake profile were not changed in this pass.

Deferred user note: a later mode may let the learner choose the chocolate-chip amount and reflect excess visually, including an intentionally comedic nearly-all-chip result. This is recorded only; it is not implemented in the current guided cookie flow.

## Data-driven definition — 2026-09-09

The current recipe facts are now centralized in `Assets/BakeIT/RecipeDefinitions/ChocolateChipCookies.asset` and assigned on the scene bowl. It contains the same approved ingredient quantities/tool roles, 4+3 mixing passes, four rolling passes, six target portions, parchment preparation, no post-bake finish, runtime ingredient names, and 5-second 170–190 C bake profile. This was an architecture migration only: it did not change the cookie flow, 1.2-second cooldowns, parchment behavior, oven outcomes, or visuals. See `recipe-data-system.md` for consumers, tests, and limits.

## Current scope

Chocolate-chip cookies are the first complete dish to develop and validate. Brownies, cupcakes, shared recipe architecture, guided modes, scoring, VR, cloud submission, and the instructor dashboard remain later work.

## Recipe definition

- Required ingredients: one 1 Cup measure each of flour and sugar, one stick of butter, one egg, the three teaspoon-sized ingredients, and one 1/2 Cup measure of chocolate chips after base mixing.
- Optional ingredients: None in the current prototype recipe.
- Valid measuring tools: 1 Cup for flour/sugar, 1/2 Cup for chocolate chips, and 1 tsp / 1/2 tsp / 1/4 tsp for vanilla, baking soda, and salt. Other equipment: white cookie bowl, whisk, preparation board, rolling pin, portioning knife, parchment, flat cookie tray, and oven controls.
- Preparation stages: Whisk-based move/look/pickup/rotation/scroll/placement tutorial; fridge ingredients then dry-cabinet ingredients on Cold Ingredient Rest, both spoons/whisk on Utensils Area, bowl from holder rack to MIX; measure base, whisk base, add chips, combine chips, move dough to chopping board, roll, portion, line the tray, preheat the empty oven, load the tray, and bake.
- Interaction counts: four base whisk passes, one 1/2-cup chip transfer, three final combining passes, and four rolling passes; one valid knife pass portions the current batch.
- Pass pacing: each whisk or rolling count requires fresh tool travel after a `1.2 s` cooldown; movement during the cooldown cannot be banked into another count.
- Correct prototype bake: Five seconds at `170-190 C`.

| Temperature | Outcome | Ingredient result |
| ---: | --- | --- |
| `100-160 C` | Undercooked | `UnderbakedCookie` |
| `170-190 C` | Successful | `BakedCookie` |
| `200-210 C` | Overcooked | `OvercookedCookie` |
| `220-250 C` | Burnt | `BurntCookie` |

The current controls move in `10 C` increments, so the table enumerates every selectable prototype temperature. Result thresholds remain serialized for later balancing. An undercooked result changes the logical result/name only; the cookie keeps its exact pre-bake appearance. Other outcomes retain their established material changes.

## Current prototype sequence

1. Move with WASD, look with the mouse, aim at the WHISK and press E. Hold right click and move the mouse to rotate it, scroll to change holding distance, then E-release it onto Utensils Area. This tutorial occurs before and gates recipe selection.
2. Complete MISE EN PLACE in separate fridge and dry-cabinet checklist screens: egg/butter/chips, then flour/sugar, onto Cold Ingredient Rest. Place the remaining utensils on Utensils Area and bowl from the holder rack onto MIX. All must be correctly resting before mixing unlocks. Measure one scoop each of flour and sugar into the placed bowl; carrying a whole bag does not add a measure.
3. Tilt the held cup over the bowl to pour each powder measure; upright contact alone does not transfer. Add one butter portion and one egg. Extra measures and unsupported ingredients are rejected without being consumed. Tilt unwanted contents over their matching source to return, or hold Q to discard as waste.
4. Complete four whisk passes to form the visible base dough.
5. Fill the `1/2 Cup Measuring Cup` with chips while upright and tilt it over the bowl once. Premature chips are rejected but retained for later pouring, source return, or deliberate discard.
6. Whisk three more passes to combine the chips. Dough remains inactive after scooping and after only two combining passes. Then take the finished dough to the clear chopping board; the preparation supplies stay on their separate rests.
7. Complete four rolling passes to produce `FlattenedDough`.
8. Pick up `Prop_DoughPortioningKnife` and move it through the flattened dough work area.
9. The unchanged ingredient batch becomes six equal CookieDoughPortion objects, with individually varied chips and pale raw dough.
10. E-pull parchment from its roll, release it above the tray, and place all six portions. Near-drop placement settles smoothly at the chosen position. The tray is not oven-ready until the complete batch has settled.
11. Set the target temperature, close the empty oven, and press PREHEAT. Wait for `READY`; the oven light remains on once heated.
12. Put the prepared tray in the oven, close the door, and press BAKE. Every portion receives the same undercooked, successful, overcooked, or burnt result while its chocolate chips retain their chocolate color. The guide keeps that result visible until recipe restart.

## Desktop controls

- Press `E` to pick up or release the knife and other supported objects.
- Use the existing right-mouse rotation and scroll-wheel hold-distance controls while carrying an object.
- Dip the held, upright `1/2 Cup Measuring Cup` into the chip source to fill; tilt over the bowl to pour. The 1 Cup measure handles flour/sugar. Different sources never replace a full utensil's contents silently.
- Tilt over the matching source to return a measure, then lift clear before refilling. Hold Q for .60s to discard; release Q early to cancel. Returns are not waste. Transfer history and discarded-unit counts reset with the recipe; no grade is computed yet.
- Uncut dough can be returned to the preparation board after pickup, including when already flattened. Releasing it with `E` while still overlapping the board also places it; rolling progress and size are retained.
- A knife pass only portions dough after all rolling passes are complete.
- Hold `R` for three seconds to restart the complete recipe without leaving Play Mode. A tap or short hold does nothing, and there is no instant HUD restart button.

## Prototype tuning

| Setting | Value |
| --- | ---: |
| Reference flattened-dough volume | `0.00255` world-volume units |
| Reference yield | 6 cookies total |
| Minimum yield | 6 cookies |
| Maximum yield | 6 cookies |
| Flour, sugar, butter, egg | 1 of each |
| Chocolate-chip measures after mixing | 1 x 1/2 cup |
| Required base whisk passes | 4 |
| Required chip-combining passes | 3 |
| Required rolling passes | 4 |
| Prototype bake duration | 5 seconds |
| Successful temperature range | `170-190 C` |
| Burnt threshold | `220 C` and above |
| Preparation-board spacing | `0.14` |
| Tray spacing | `0.17` X, `0.13` Z |

Each portion keeps the flattened dough thickness and scales its horizontal area by `sqrt(1 / portion count)`. This keeps all portions equal and preserves the represented total dough volume.

## Verification

- A knife cannot portion an empty board or unflattened dough.
- The reference batch produced four equal portions with combined rendered volume `0.002505`, matching the `0.002505` source volume.
- A 1.25-times scaled test batch produced eight equal portions with combined rendered volume `0.004893`, matching the `0.004893` source volume.
- Each portion retained an enabled collider and Rigidbody for pickup.
- A complete four-cookie batch snapped to parchment, entered the oven, and all four portions became `BakedCookie` after the prototype bake.
- A focused Play Mode sequence passed from the controls tutorial through exact four-ingredient base validation, four-pass base mixing, three post-mix chip scoops, and finished-dough creation.
- A focused Play Mode result test passed at `160 C`, `180 C`, `200 C`, and `220 C`, producing undercooked, successful, overcooked, and burnt batches respectively.
- A focused Play Mode test confirmed that dough remains unflattened after three rolling passes and becomes `FlattenedDough` on the fourth pass.
- The compact guide's ten stages, full scoop mound, and completion latch passed an isolated Play Mode progression verifier. Clearing the oven's live completion flag after the bake did not return the guide to tray loading.
- Duplicate, unsupported, and post-completion inputs do not change ingredient totals. A chip scoop attempted before base mixing is deliberately emptied with corrective feedback so the only shared scoop does not deadlock the recipe.
- Every raw and baked portion retained one visible chip decoration; chip renderers did not change yield and kept their chocolate color during baking.
- Historical/superseded source test: the former `Small Measuring Spoon` chip workflow passed before the 2026-09-11 change to a single 1/2-cup measure. Current verification is recorded in `test-log.md`.
- The later screenshot-driven correction replaced the overflowing three-layer visual with eight contained interleaved cluster meshes, rendered and checked against the cup geometry. Connected Play Mode tests then passed partial-dough return at `2/4`, exactly two further rolls, flattened return after rotation, three repeated pickup/return cycles without scale drift, actual knife contact, four portions, parchment-first tray loading, and a perfect `180 C` bake of all four recovered portions.

## Known limitations

- One valid knife pass divides and lays out the entire batch; individual repeated cut gestures are not simulated yet.
- Yield uses visible world-space volume rather than recipe grams.
- Nine is the current capacity cap for the prototype tray layout.
- The sequence-level restart is implemented and focused verification confirmed restoration of consumed, generated, bowl, and oven state.
- The state-machine and reset paths are verified, but a final human usability pass must still evaluate reach, feedback readability, and gesture feel using `cookie-usability-acceptance-checklist.md`.
