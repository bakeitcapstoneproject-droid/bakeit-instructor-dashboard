# BakeIT Player Acceptance Checklist — Brownies and Cookies

Current checklist: 2026-09-15. Existing filename retained for compatibility. **Status: first screenshot feedback addressed; awaiting natural retest.** Focused technical checks passed; historical instructions below are reference only.

## How to use

Play normally with keyboard and mouse, without injected recipe states or the QA harness. Suggested order: Brownies, then Cookies. If blocked, note the step, action, actual result and expected result. Screenshots/video are useful when convenient. Separate visual preferences from broken behavior.

Use the current guide for gathering order and target locations. Report contradictions with the controls/recipe below. Do not follow the archived checklist.

## Setup and shared controls

- [ ] Open Kitchen Prototype in the main BakeITCapstoneProject, not CodexReview. Record Unity version and Game-view resolution.
- [ ] Record existing Console errors/warnings before clearing anything. Enter Play Mode with Game view focused.
- [ ] Complete movement/look practice, E whisk pickup/release, right-click-and-mouse rotation, scroll distance and whisk placement before recipe selection.
- [ ] Select Brownies or Cookies with E on its poster; Cupcakes remains locked.
- [ ] Prepare supplies/tools named by the guide on the ingredient rest/Utensils Area, with the correct bowl in MIX. Keep the chopping board for dough.
- [ ] Check pickup/release, rotation, reach, doors, tool-hook return and awkward-placement recovery. Note trapping, launching, shrinking, collision or unreachable objects.
- [ ] Check guide/highlight/hover readability at player distance and whether hands/tools obscure the action.

## Brownies — complete run

| Ingredient | Current prototype quantity/tool |
|---|---|
| Sugar | Two 1-cup transfers |
| Flour | One 1-cup transfer plus one 1/4-cup transfer |
| Melted butter | One provided 1-cup melted-butter pour |
| Eggs | Four whole eggs, using the existing interaction |
| Cocoa powder | One 1/2-cup transfer |
| Vanilla | One 1-teaspoon transfer |
| Baking powder | One 1/2-teaspoon transfer |
| Salt | One 1/2-teaspoon transfer |
| Walnuts | One 1/2-cup transfer after base mixing |

- [ ] Use the large green Brownie Mixing Bowl. Add base ingredients; Flour's 1-cup and 1/4-cup progress must remain separate.
- [ ] Inspect distinct unmixed ingredients low inside the bowl. Full batter should not appear before whisking starts.
- [ ] Complete four fresh base-whisk passes, add walnuts, then two finishing passes. Check gradual appearance, containment and stroke feel.
- [ ] Line the deep Brownie Pan with parchment. Tilt the held completed-batter bowl over it; inspect the transferred clump and cleared bowl.
- [ ] Make four fresh Rubber Spatula passes to spread toward the parchment edges. Check orientation, reach and visible progress; stationary contact must not repeatedly count.
- [ ] Choose an available target in the 170–180 C success range. Close the empty oven and PREHEAT to READY; physically load the prepared pan, close and BAKE.
- [ ] Complete the 8-second prototype bake through normal controls. Record result text/appearance; the actual oven-to-latest-visuals path is a priority retest.
- [ ] Remove the pan and set it on the worktop. Make five deliberate held-knife strokes; inspect twelve squares in a 4-by-3 arrangement.
- [ ] Choose two Brownies with **2**, release two squares onto the plate and confirm completion. Choose three with **3**, add a third and confirm completion again. Inspect the stacked placement from the side, pick up/set down the loaded plate, and check contact height and scale.
- [ ] Inspect crust, walnuts, cut sides, color and thickness at player distance. Carry the pan again and check stability.
- [ ] Hold R for three seconds. Confirm ingredients/tools, parchment, bowl, oven, cutting and generated food reset; begin a fresh attempt.

**Current endpoint:** A two- or three-square stack. Cooling is absent; grading is deliberately deferred until all dishes and the VR base game are complete.

## Cookies — regression run

| Ingredient | Current prototype quantity/tool |
|---|---|
| Flour | One 1-cup transfer |
| Sugar | One 1-cup transfer |
| Butter | One whole stick |
| Egg | One whole egg, using the existing interaction |
| Vanilla | One 1-teaspoon transfer |
| Baking soda | One 1/2-teaspoon transfer |
| Salt | One 1/4-teaspoon transfer |
| Chocolate chips | One 1/2-cup transfer after base mixing |

- [ ] Use the white Cookie bowl. Add seven base ingredients, make four fresh whisk passes, add one 1/2-cup chip measure and make three finishing passes.
- [ ] Put dough on the board. Roll twice, pick up/rotate/return it and verify 2/4 progress and size persist. Finish four total rolls; verify flattened dough can also return.
- [ ] Cut into six portions. Use the E-operated parchment roll, line the shallow tray and load all six.
- [ ] Set 180 C; PREHEAT the empty closed oven to READY. Carry in the tray, close and BAKE for the 5-second prototype cycle.
- [ ] All six cookies receive the successful appearance; chips remain distinct. Remove the tray and plate three cookies; completion appears only after the smaller serving is plated.
- [ ] Hold R for three seconds and begin a fresh attempt.

## Recovery and incorrect interactions

Use separate attempts where needed.

- [ ] Full measures retain contents on other-source contact. Wrong-tool, duplicate or premature bowl inputs give feedback without silently consuming the measure.
- [ ] Tilt rejected contents over the matching source to return. Wrong-source return retains contents. No immediate unintended refill; lift clear, level and dip again.
- [ ] Short Q tap retains contents; deliberate Q hold discards once. Empty-tool Q hold and ordinary rotation do not create repeated waste.
- [ ] Test premature mixing, early chips/walnuts, incomplete batter, wrong pan/tool, unlined pan/tray, incomplete spreading/loading and cutting before baking. Record rejection/recovery.
- [ ] Cutting while the Brownie pan is held or in the oven does not count.
- [ ] Fast repeated whisk/rolling movement within the 1.2-second cooldown does not count extra passes; fresh strokes afterward progress.
- [ ] Opening during preheat pauses heating; a loaded tray/pan cannot start empty-oven preheat. Changing target invalidates readiness.
- [ ] R taps/holds under three seconds do not restart; a full three-second hold does.
- [ ] M taps/holds under three seconds do not leave the recipe; a full hold clears the batch and restores recipe selection, retaining control practice. Try this before finishing a recipe.
- [ ] In guided practice, exact ingredient lines turn green on the selected recipe paper; one Sugar cup of two or one Flour capacity alone must not complete its line. Check that the chip row names Chocolate chips.
- [ ] Check the corrected butter label, pouring jug label, upright Vanilla, organized taller storage and E pickup on the visible Baking Soda carton.
- [ ] Pick a plated piece back up, replace it, and carry the loaded plate. Check counts, spacing and stability.
- [ ] Pour melted butter with the spout above the bowl: watch the stream and draining contents, level briefly to pause, then resume to complete the full cup. Check that the jug need not enter the bowl.
- [ ] Watch walnut chunks fall into the batter and remain visible for folding. Inspect the bowl from its sides for batter clipping and confirm its empty appearance after transferring to the pan.

## Separate oven outcome checks

Prototype settings only, not real-world cooking instructions. Use fresh prepared batches and normal preheat/bake controls for each case.

| Recipe | Underbaked | Successful | Overcooked | Burnt |
|---|---|---|---|---|
| Cookies, 5 seconds | 160 C | 180 C | 200 C | 220 C |
| Brownies, 8 seconds | 160 C | 180 C | 200 C | 220 C |

Record actual text/appearance for every run. Underbaked Cookies below 170 C should retain raw material appearance. Do not assume Brownie art follows that rule. Report unavailable settings rather than changing Inspector values.

## Report and acceptance

| Run / step | What I did | Expected | Actual / preference | Pass, fail or not tested |
|---|---|---|---|---|
| | | | | |

- Record tester, date, Unity version, scene, evidence when available and Console messages after the run.
- A full player pass requires understandable, reachable and recoverable gameplay without consulting the Console for instructions.
- Untested checks stay untested. Scripted historical passes do not fill these boxes.
- Send observations to Codex; confirmed failures go in known-issues.md and the run in test-log.md. Agree on Brownie finishing work before Cupcake planning.

---

<details>
<summary>Historical notes retained on 2026-09-15 — superseded wherever they conflict with the current sections above; not current test instructions</summary>

# Chocolate Chip Cookie Player Usability Acceptance Checklist

## Current retest — whisk and rolling cooldown (2026-09-09)

1. With all base ingredients ready, make one fast, exaggerated whisk stroke and keep moving continuously for less than `1.2 s`; the guide must advance by exactly one pass, not several.
2. Pause briefly, then make a fresh stroke after the cooldown; exactly one additional pass should register. Continue naturally to `4/4`.
3. After adding all chips, repeat the immediate/fresh-stroke check and confirm combining reaches exactly `3/3`.
4. On the board, make one large rolling movement and an immediate reverse movement; only one rolling pass should register. Continue with separated strokes to `4/4` and confirm the dough flattens normally.

The deterministic component-path tests passed at `1.2 s`, including a blocked `0.65 s` repeat and post-interval progression. Focused mouse-controlled fluidity acceptance remains required.

## Current retest — measure recovery (2026-09-08 evening)

Use this section for the current build. Older sections below contain historical instructions; do not use their board-staging, both-spoons-for-powder or automatic-transfer behavior.

1. Fresh Play: follow the WHISK move/look/E/rotate/scroll tutorial and place it on Utensils Area. Follow fridge then dry-cabinet prep; ingredients go on Cold Ingredient Rest, the recipe-relevant cup/spoons/whisk go on Utensils Area, and the bowl goes in MIX.
2. Dip the 1 Cup Measuring Cup upright into sugar. It must visibly fill. Upright bowl contact must not empty it; right-click-and-mouse tilt over the bowl should pour one measure after a short hold.
3. Fill with sugar again and offer it to the bowl. Duplicate sugar stays in the cup. Tilt over the sugar source: it empties as a return, not waste. While still over the source it must not instantly refill; lift clear, level and dip again.
4. Try returning sugar over flour. It must remain sugar and show a correction. Touching another source must never silently overwrite it.
5. Away from ingredients, briefly tap Q with a full measure: contents stay. Hold Q: contents empty once and a discard correction appears. Holding Q on an empty utensil must not create more waste. Merely turning it in open space must not dump contents.
6. Verify exact roles: 1 Cup for flour/sugar, 1 tsp for vanilla, 1/2 tsp for baking soda, 1/4 tsp for salt, and 1/2 Cup for chips. Complete the seven base ingredients, four base-whisk passes, one tilted 1/2-cup chip pour, and three combining passes. Early chips remain recoverable, not silently deleted.
7. Complete the usual board/rolling/six-cookie/paper/oven path. Cookies should stay at the crosshair, release even when aiming at the roll, and remain stable on a moved tray. Bake at180C; completion stays latched after removal.
8. Restart: empty cup/spoon, restored sources and paper roll, no generated cookies, cleared bowl/oven and waste history. Verify contextual text fits your normal Game-view resolution without hiding the workstation.

Report actual mouse-tilt and Q-hold feel, failed transfers, accidental refills, contents changes, collision/reach problems or clipped instructions with screenshots/video. Technical fixture tests do not replace this full manual pass.

## Current retest — separate rests and movable bowl (2026-09-07 evening)

Earlier board-staging steps below are historical and superseded. Player acceptance remains open:

- Retrieve flour/sugar/vanilla/baking soda/salt from sliding cabinet; egg/butter/chips from fridge. Stage on enlarged Cold Ingredient Rest, not the chopping board.
- Stage small spoon, large spoon and whisk on Mix Tool Rest. Confirm each rests near release and can be re-picked; no tool is required on the chopping board.
- Collect bowl from left-side tray-holder rack. Carry/rotate/scroll, then release in MIX. Try returning it to the rack and another clear point in MIX.
- Measure base, whisk four passes, add one 1/2-cup chip measure, then whisk three more. No finished dough before the third combining pass.
- Lift a partly filled bowl: quantities/visuals should persist, recipe actions should pause until it rests in MIX again.
- Carry finished dough rightward to board, roll/cut, line/load tray and bake. Test full natural walking/door/edge/collision paths and the thirteen-phase HUD.
- Cleaning has no control yet; it is planned-only documentation.

## Test setup

- Open `Assets/BakeIT/Scenes/Kitchen Prototype.unity` in Unity 6000.5.0f1.
- Allow Unity to finish importing and compiling, then clear the Console.
- Enter Play Mode with the Game view focused.
- Confirm the 150%-scaled guide opens at `Choose a Recipe / RECIPE SELECTION`; use `E` on the separate `READY` cookie poster, then confirm `Movement Tutorial / 1 / 13 BASIC CONTROLS` appears with three compact tasks and the restart footer remains visible without obscuring the workstation.

## Interaction acceptance

1. Walk and crouch around the complete workstation without becoming trapped or pushed by held objects.
2. Pick up, rotate, move closer or farther, and release the marked measures, whisk, rolling pin, knife, parchment, tray, butter, and egg. Return at least one measure and the whisk to their labeled wall hooks.
3. Confirm each required object can be reached from a normal standing or crouched position.
4. Confirm released objects remain recoverable and do not become embedded, launch the player, or fall through the work surfaces.
5. Complete the controls tutorial and confirm `2 / 11 PLACE ON PREP BOARD`. Retrieve flour/sugar from the sliding cabinet and egg/butter/chips from the fridge. Deliberately release all five on the board; then place small spoon/whisk there too. Hovering does not count, lifting an item before all seven are ready unchecks it, and early bowl entry must not consume food. All seven staged together advances to PREPARE BASE.
6. Add the four base ingredients one at a time and confirm only the corresponding checklist row changes from `[ ]` to `[x]`.
7. Try adding chocolate chips before whisking. Confirm a short orange correction appears, the chip count remains zero, and the scoop becomes empty so the recipe can continue.
8. Try mixing before all base ingredients are present and confirm a short orange correction appears without replacing the preparation checklist.
9. Try a duplicate measure, an unsupported ingredient, dough on an unlined tray, and an incomplete tray in the oven. Confirm each action is rejected without consuming or corrupting the object and gives a clear temporary correction.

## Complete recipe acceptance

1. Add 1 cup flour, 1 cup sugar, the softened 1/2-cup butter block, one egg, 1 teaspoon vanilla, 1/2 teaspoon baking soda, and 1/4 teaspoon salt.
2. Complete four recognizable whisk passes and confirm the mixture changes gradually into base dough before any chocolate chips are required.
3. Leave a rejected flour or sugar measure in `Small Measuring Spoon`, then dip it into the chocolate-chip container. Confirm it visibly changes to chocolate chips without requiring a restart. Empty it into the bowl, keep it inside the source briefly, and confirm it can refill without a precise exit/re-entry.
4. Confirm the small spoon has a dense fill seated in its cup, with no wide floating overflow. Dip `Large Measuring Spoon` into chips: it must remain empty or retain its flour/sugar and explain that chips require the small spoon. Add three small spoonfuls and confirm the guide counts `1 / 3`, `2 / 3`, then advances after `3 / 3`.
5. Move dough to the board and roll twice. Pick it up, rotate it, return it, and confirm progress stays at `2 / 4`. Finish the remaining two rolls, pick up the flattened uncut dough again, and return it both by re-entry and by pressing `E` while still over the board. It must remain full-sized and ready to cut. Use one deliberate knife pass to produce equal portions.
6. Place parchment on the tray and load every portion from the batch.
7. Leave the prepared tray outside the oven, set `180 C`, close the empty oven, and press PREHEAT. Confirm the temperature visibly rises, the light comes on, and opening the door pauses heating without losing progress.
8. At `READY`, confirm the light remains on, load the complete tray, close the door, and press BAKE to complete the five-second prototype bake.
9. Confirm all cookies receive the successful appearance, chocolate chips remain brown, the completion feedback is readable, and the completion sound is audible.
10. Remove or move the tray after baking and interact with the oven controls. Confirm the light and READY state remain visible and the guide stays on `RECIPE COMPLETE` until `R` or the restart button is used.
11. Confirm the guide advances through all thirteen phases and never grows large enough to cover the top half of the Game view.

## Kitchen and storage acceptance (2026-09-07)

- Confirm chips, bowl, wood board, knife, rolling pin, tray, and parchment have clear separate homes and are reachable without climbing onto the counter.
- Aim at either fridge door and press E. Take egg/butter/chips out, then release near a clear spot on a shelf. Food must stay near the release X/Z with unchanged size, not teleport to an old slot or get stolen while held.
- Try E on a door while carrying an item: it should operate the targeted door without dropping the item. Stand in the moving door's path: it should stop, allow reversal, and resume once clear without launching the player.
- Close the door; food must not be pickable through it. Shelf labels must not appear through the closed door.
- Drop a spoon on a spare shelf, close/reopen the fridge, and retrieve it. Check rotation, scroll distance, awkward releases, shelf edges, and recovery around the door/player.
- E slides the storage-cabinet panels sideways; access one half at a time. Retrieve flour/sugar (Ctrl crouch if useful), carry/rotate/scroll them, return them to a clear shelf position and retrieve again. Their names must travel with them and scooping must still work after transport.
- Release each of the seven supplies near a free board position. It should settle vertically by only a small amount, keep its X/Z and yaw, stay stable, and be re-grabbable. Try an occupied point, edge and high release: no teleport or overlap assistance should occur. A normal failed-assist drop remains subject to physics.
- Once all seven are prepared together, readiness should stay complete while ingredients/tools are used. Clear containers/tools beside the board before dough; the guide must explain this and then resume dough placement. Restart puts sources back in cabinet/fridge and clears setup.
- Confirm the rounded minus/plus/context-sensitive PREHEAT–BAKE controls read clearly and can be selected from normal standing height. Complete an actual hand-carried tray load through the open oven door after READY.
- Record subjective environment preference and any reach, text-size, gesture, or collision failure. The technical callback/physics checks do not replace this acceptance pass.

## Restart acceptance

1. Press `R` halfway through ingredient collection and confirm the opening scene returns without leaving Play Mode.
2. Verify the scoop is empty, consumed ingredients have returned, bowl visuals and quantities are clear, generated dough or portions are gone, parchment and tray are restored, and the oven is idle at its saved temperature.
3. Complete a bake, press `R`, and confirm a second fresh batch can begin.
4. Tap `R` and hold it for less than three seconds; confirm neither restarts. Then hold `R` continuously for three seconds and confirm the batch restarts. There is no instant HUD restart button.

## Final result

- Pass only if the complete workflow is understandable without consulting the Unity Console and every required object is reachable, readable, controllable, and recoverable.
- After testing, confirm the Console has no project-script errors or warnings.
- Record the tester, date, result, observations, screenshots or video, and every failed checklist item in `test-log.md` and `known-issues.md`.

</details>
