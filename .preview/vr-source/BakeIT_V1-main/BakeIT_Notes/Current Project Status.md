# BakeIT Current Project Status

Updated 2026-09-15 for the Cupcake tray and baking extension. Historical snapshots below preserve prior work but do not define current behavior. Cookies and Brownies have focused coverage through smaller servings; Cupcakes has a selectable, explicitly partial workflow through rack cooling. Natural player acceptance remains open.

## Current development stage and approved scope

Desktop prototype for chocolate-chip cookies, brownies, and cupcakes only. Cookies and Brownies have implemented playable workflows awaiting full player acceptance. Cupcakes exposes **Hard | Baking Practice**, following the selected formula through staged batter, twelve individually lined/fillable wells, baking, doneness testing and rack cooling. From-scratch buttercream, piping and serving remain unfinished. Details are in [cupcakes.md](cupcakes.md). No full dish is newly marked complete.

## Agreed next work

1. **D — Reconcile documentation:** completed September 15.
2. **A — User playtest:** feedback received with eight screenshots; corrections implemented for storage, pickup, labels, guidance, bowl fill and walnut appearance. User retest is next.
3. **C — Finish Brownies before Cupcakes:** the user reached successful Brownie serving and supplied follow-up feedback. Brownies now offer a two- or three-square stack; Cookies stay at three. Follow-up fixes address pouring, walnuts, bowl appearance/containment and plate contact. Grading is deferred until all dishes and the VR base game are complete and data collection can inform the rubric. Cooling remains optional and unimplemented.
4. **B — Cupcake implementation:** preparation, nine ordered batter stages, twelve lined wells, gradual filling, baking/doneness and cooling implemented. Next: confirmed from-scratch frosting, piping and serving. See [cupcakes.md](cupcakes.md).

The [active player checklist](cookie-usability-acceptance-checklist.md) now covers both playable recipes; its existing filename is retained to preserve links.

## Recipe status

| Recipe | Implemented | Verified evidence | Still open |
|---|---|---|---|
| Cookies | Eight ingredients; 4 base + 3 finishing whisk passes; 4 rolling passes; 6 portions; parchment; preheat; 5-second bake; 3-cookie serving; outcome feedback and both reset routes | September 15 focused recipe run with actual bake timer and serving passed | Natural retest of corrected readability, carrying, gesture feel, recovery and art |
| Brownies | Exact measures and 4 eggs; 4 base + 2 finishing whisk passes; walnuts; parchment; tilted batter pour; 4 spreading passes; 8-second bake; 5 knife strokes; 12 grabbable squares; 2/3-square serving | September 15 focused recipe run with actual bake timer, slicing, pickup and serving passed | Full player run through serving; knife/spatula feel; oven-to-worktop handling; art acceptance |
| Cupcakes | Hard baking-practice selector; exact staged batter; empty-oven preheat; twelve individual liners and gradual fills; timed bake; tester; rack cooling; tutorial-only green paper; reset | See latest tray extension entry in test-log.md for focused checks and limitations | Full gathering stage, electric mixer, from-scratch frosting/piping and serving; natural input/art acceptance |

After five cuts, all twelve Brownie squares are independently grabbable. Choose two or three at plating with the **2 / 3** keys to reach **Ready to Serve / SERVING COMPLETE**; Cookies finish after plating three of the six baked cookies. Bake outcomes remain visible, without a grade. Dedicated cooling is absent and remains optional.

## Shared systems and current interaction rules

- Movement/look/whisk pickup, rotation, distance and placement practice precede recipe selection.
- Cookies, Brownies and Cupcake baking practice are selectable after movement practice. The Cupcake poster and cooling endpoint identify its partial scope. The Cupcake oven gate requires all twelve lined wells to be filled.
- Exact capacities come from the active recipe. Cookie chips use one 1/2-cup transfer; Brownie walnuts use one 1/2-cup transfer.
- Full measures retain contents on rejected/wrong-source contact. Tilt over the matching source to return; deliberately hold Q to discard. Do not expect automatic ingredient replacement.
- Stage supplies on the ingredient rest, required tools on Utensils Area, and the correct bowl in MIX. The chopping board is for dough work.
- Desktop pickup, rotation, scroll distance, storage doors, hanging-tool returns, outlines, hover names, pan/tray handling and empty-oven preheating exist; player acceptance remains open.
- Restart requires holding R for three seconds. There is no instant HUD restart button.
- Hold M for three seconds to discard the current batch and return to recipe selection; practice completion is retained. R repeats the selected recipe.
- Guided practice colors each recipe-paper ingredient line green only when its full exact requirement is satisfied, including both Flour measures. Non-guided sessions suppress this color; future mode selection is not implemented.
- Measuring transfers have a session-only added/returned/discarded ledger. This is not grading, calibrated mass, complete waste coverage or cloud persistence.

## Known issues and blockers

- Full user-driven acceptance remains outstanding for both recipes. Untested interactions are not automatically confirmed bugs.
- Active summaries/checklist reconcile documentation drift; historical sections retain superseded names, counts, selection order and prototype states.
- Brownie cooling remains unimplemented; independent pickup and smaller serving are implemented, awaiting player acceptance.
- Cupcakes use 180°C as the game approximation of the source's listed 176°C, a 10-second bake and 10-second rack cooling per cupcake. Frosting/piping/serving, electric mixing and player acceptance remain.
- Earlier Unity account-service errors are historical diagnostics. September 15 focused runs recorded no runtime errors; see the test log for fixture failures and corrections.
- See [known-issues.md](known-issues.md) for retained issue IDs and current interpretation.

## Testing status

- Latest documented Unity version: 6000.5.0f1; scene: Kitchen Prototype.
- Focused verification differs from complete player acceptance. Neither recipe has a newly recorded full player pass.
- The editor-only BrowniePresentationQA harness exists; it is not a complete automated test suite.
- CodexReview holds an isolated September 6 scoop test and logs. Its passed automatic-replacement behavior is historical and contradicts today's intentional recovery rule; do not reuse it unchanged as a current acceptance test.
- September 15 PlayerFeedbackQA passed both actual oven timers, transfers, mixing, slicing, serving, green-paper checks and recipe reset routes. It drives public APIs/trigger callbacks and synthetic practice input; it does not replace a natural keyboard/mouse run.

## September 30 target and broader deliverables

Codex.md targets near feature-completion of the three recipe workflows by September 30, with the remaining period reserved for testing, fixes, balancing, VR integration and presentation preparation.

The broader capstone guide also lists learning modes, grading/safety/organization/waste tracking, cloud submissions and an instructor dashboard. Their schedule is not newly committed here. The old tracker listed them all as September 30 deliverables, conflicting with the timing above; this remains an unresolved planning discrepancy until explicitly scheduled.

## VR readiness

Not ready according to the current notes: Windows keyboard/mouse prototype with no configured OpenXR, XR Interaction Toolkit or Meta XR integration recorded. No headset compatibility or device-performance pass is claimed. Preserve adaptable interactions when extending existing systems.

## Documentation authority and next recommended task

Keep this tracker concise; append development/test history to existing logs and update relevant system notes. Active documentation lives in BakeIT_Notes. Codex.md names both Project_Notes and BakeIT_Notes; its later BakeIT Notes section identifies these existing primary files. Do not introduce a duplicate tracker tree during this reconciliation.

**Next: player-test the Cupcake tray/baking/cooling workflow, then implement from-scratch buttercream, piping and a smaller serving.** Cookie/Brownie feedback acceptance remains open. Preserve user-authored layout choices. Grading stays deferred under the user's latest decision.

---

<details>
<summary>Historical notes retained on 2026-09-15 — superseded wherever they conflict with the current sections above; not current test instructions</summary>

# BakeIT Current Project Status

## Current development stage

Cooked-detail follow-up (2026-09-14): crust textures now vary per square and per reset/batch while staying attached to moving brownies. Cooked tops have 28 partially embedded walnut chunks; twelve cut pieces include 60 additional cut-face fragments. Final full-grid and isolated side-face renders were reviewed. Scene saved in Edit Mode with no temporary QA objects and a clean Console.

Latest Brownie correction (2026-09-14): distinct unmixed ingredient piles/pools now sit low inside the green bowl; batter appears only after whisking starts. The half-cup uses actual crinkled walnut meshes instead of a brown powder disk. Raw food uses #663C02 with non-emissive, color-preserving shading. Five closed-volume pan meshes turn a central clump into a parchment-edge rectangular layer over four spatula passes. The spatula pickup is outward-facing. The lined footprint is 4:3, and five held-knife strokes after baking/outside the oven reveal twelve thick squares with textured crust/darker sides. The HUD and highlight target follow slicing; reset clears it. Automated public-transfer/trigger-callback checks and rendered state inspections passed. Full mouse-input playthrough, cutting feel, photo-reference aesthetic approval, and a dedicated cooling stage remain pending. This supersedes the earlier statements that slicing is unimplemented and a generic ingredient fill is visually acceptable.

Latest Brownie art implementation (2026-09-13): the user selected B1 for the fuller rustic bowl, B2 for thick raw batter in the approved pan, and A3 for the smoother baked top. Three project-owned procedural meshes, four materials, walnut-detail hierarchies, and `BrownieFoodVisual` now provide those states without replacing the pan or altering recipe logic. The final B1 local position/scale is `(0,.088,0)` / `(.105,.032,.105)`, giving an approximately `.294 m` horizontal fill in the approximately `.31 m` bowl. B2 uses ten walnut clusters; A3 swaps to a smoother mesh with eight walnut clusters and three subtle crack groups after a successful bake. Play Mode state-switch and visual checks pass. Cooling and playable slicing remain the next unfinished Brownie stages.

Latest ingredient-model pass (2026-09-12): the rustic Salt Cellar, Vanilla Extract Bottle, and Chocolate-Chip Prep Bowl references are integrated as three reusable, collider-free visual prefabs. Salt is deliberately compact and shorter than Vanilla; the chip bowl reuses the established dark-chip mesh and preserves its hollow nine-collider physical shell. Root pickup, preparation, hover, placement, and measured-source behavior remain authoritative. Play Mode passed pickup/release, exact correct-tool fills, inverse wrong-tool rejection, and empty/reset for `Salt`, `Vanilla`, and `ChocolateChips`. No hosted model generation was used. The chopped-walnut bowl and wrapped butter are the remaining supplied-reference models; Egg and melted butter remain approved as-is.

Earlier pantry-asset pass (2026-09-12): three approved references were integrated as reusable, collider-free prefabs: ivory/red all-purpose Flour Tin, red/silver Cocoa Powder Tin, and smaller cream/red Baking Powder Can. Each has an open lid and visible ingredient surface. Existing source roots retain all pickup, preparation, hover, and measuring logic. Play Mode passed pickup/release, correct Brownie tool pairing, wrong-tool rejection, fill/empty, and zero-visual-collider checks. No hosted model generation was used.

Latest ingredient-asset pass (2026-09-11): `SugarContainer` now uses a project-owned, reference-driven clamp-lid glass Sugar Jar instead of the milk-bottle placeholder. The reusable prefab has a rounded clear body, visible off-white granulated sugar, an open mouth/lid, seal, hinges, and wire clamp. Its visual has zero colliders; the existing root pickup, hover, placement, and `SugarScoopZone` measuring components remain authoritative. Play Mode passed jar pickup/release and 1-cup Sugar fill/empty. At the user's request, pantry assets are proceeding one at a time; the flour tin and all later models remain untouched, and the existing Egg/melted-butter visuals are retained.

Latest onboarding/readability correction (2026-09-11): movement, look, whisk pickup/rotation/distance, and whisk placement now precede and gate recipe selection. Practice guidance uses a single clean yellow bounding outline with a subtle size pulse; it creates no Light and therefore does not scatter onto walls. A misplaced released item receives a red outline after 3.5 seconds while its correct destination receives yellow. Persistent names on carryable items were replaced by direct-ray, screen-upright hover labels that appear only while the player's hands are empty; held objects suppress all hover names. The user's final fixed-text orientations are the saved authority. The guide is bold, reserves a taller header so tray text does not overlap, names the white cookie bowl and flat/deep trays, and requires holding R for three seconds to restart.

Recipe-sheet readability update (2026-09-11): the three dish titles use the bakery-orange accent, while difficulty and recipe-detail text remains solid black against the cream paper. The former availability lines now read `DIFFICULTY: EASY` for Chocolate-Chip Cookies, `DIFFICULTY: MODERATE` for Brownies, and `DIFFICULTY: INTERMEDIATE` for Cupcakes. This presentation change does not unlock the unfinished recipes. Text outside `Recipe Selection Posters` was not changed.

Recipe-sheet wording update (2026-09-11): Cookie and Brownie ingredients now use one ingredient per line with full measurement and ingredient names; slash separators and unit abbreviations were removed. Process summaries also use one full-text step per line. The detail font is `.0043`; the user's subsequent saved layout places the Cookie/Brownie summaries at local Y `.126` and leaves Cupcakes at `-.10`. Cupcake ingredients remain explicitly unfinalized.

Brownie implementation milestone 1 (2026-09-11): `Brownies.asset` now stores the approved ten measured entries, including separate 1-cup and 1/4-cup flour requirements, 12-square yield, parchment/deep-pan direction, 4 spreading passes, an 8-second prototype bake, and a 170–180 C success band centered on the approved 175 C target. The existing large-bowl melted-butter receiver resolves this asset. The Brownie poster remains locked while measured intake, full batter visuals/mixing, pan spreading, baking, slicing, guide flow, and reset are unfinished.

Brownie implementation milestone 2 (2026-09-11): the existing large-bowl receiver now accepts the exact Brownie measures and four whole eggs, keeps 1-cup and 1/4-cup Flour progress separate, rejects wrong/duplicate/early inputs, and applies the same 1.2-second fresh-stroke mixing cooldown. Four base passes precede the 1/2-cup Walnut measure and two finishing passes. Cocoa Powder and Baking Powder jars, three additional fridge eggs, and a contained progressive chocolate-batter surface now exist. Play Mode passed the complete bowl sequence and Cookie measurement isolation. Pan pouring/spreading, baking, slicing, guide/reset, and Brownie selection remain unfinished and locked.

Brownie implementation milestone 3 (2026-09-12): the selectable Brownie branch now continues past completed mixing. The existing deep pan accepts parchment, receives the finished batter from a held tilted Brownie Mixing Bowl, and requires four fresh Rubber Spatula passes before oven loading. The shared oven reads the active recipe and supports the approved 8-second Brownie bake at 170-180 C, including `UnderbakedBrownie`, `BakedBrownie`, `OvercookedBrownie`, and `BurntBrownie`. The guide and yellow contour follow the pan, bowl, spatula, preheat, oven-load, and bake stages, then stop truthfully at `NEXT PART STILL IN DEVELOPMENT` before cooling and slicing. Temporary slab geometry is intentionally replaceable when the user's raw and baked Brownie references arrive. Focused runtime verification passed the pan/batter/bake state flow and a Cookie recipe-data regression; normal player gestures and appearance still need acceptance.

The same correction restores `Utensils Area` to its original `.412` X scale and enlarges `Tool Home Mat` to `.76 x .68`; raises dry storage to `1.18` Y scale with its shelf contents; moves the smaller brown bakery sign and squarer brown recipe frames higher; replaces the dedicated chip scoop with the `1/2 Cup Measuring Cup`; and changes cookie data to one stick of butter plus one 1/2-cup chocolate-chip transfer. Six measuring vessels and the whisk remain on seven matching hooks. Final Play Mode interaction/layout, recipe gate, hover, exact measuring, outline/correction, and 3-second threshold checks passed; the stopped scene is clean and the Console has zero errors/warnings.

Historical measured-recipe/tool pass (2026-09-10, superseded where noted above): this introduced the eight-ingredient formula, full capacity set, hanging rail, new measured sources, and walnuts. The 2026-09-11 pass subsequently changed butter/chips, removed the dedicated chip scoop and persistent labels, restored `Utensils Area`, and enlarged `Tool Home Mat`.

Latest recipe-start/UI pass (2026-09-10, superseded in onboarding order 2026-09-11): three separate framed cream recipe sheets are arranged beneath the `BakeIT` wall sign. Cookies are `READY`; brownies/cupcakes remain `IN DEVELOPMENT`. Movement/tool practice now occurs before selection and blocks early poster selection. The guide remains at 150%, with a bold yellow learning-part heading and separate recipe/stage detail.

Latest manual organization update (2026-09-10): the user renamed the two rack tray-support objects to `Cookie Bowl Holder` and `Brownie Bowl Holder` and finalized the larger bowl's resting position. A focused Play Mode check confirmed both holders remain children of `Bowl Holder Rack`, retain support colliders, and do not interrupt either bowl/butter receiver system. No runtime code depends on the old support name.

Latest brownie equipment/interaction pass (2026-09-09): a separate larger `Brownie Mixing Bowl` and labeled `Melted Butter Cup` are implemented. The cup provides the recipe's butter already melted and transfers once through a held 75-degree tilt over the larger bowl for `.35 s`; the bowl must be on MIX. Upright contact, storage-position pouring, the cookie bowl, and duplicate pours are rejected. Focused Play Mode verification passed, the user's corrected rack/tray/spatula transforms were preserved and saved, and the cookie receiver remains the sole active `BowlReceiver`. A 2026-09-10 post-crash recovery audit reopened all additions/transforms intact with a clean scene and Console. The selector now lists brownies as `IN DEVELOPMENT`; remaining brownie ingredients, batter/mixing, pan parchment, spreading, baking, cooling, slicing, guide branch, and actual recipe activation remain unimplemented.

Latest user-authored layout correction (2026-09-09): the Unity scene contains the user's re-leveled/replaced holder arrangement, both pans lying on the rack, and a manually positioned/oriented rubber spatula. These transforms supersede the earlier authored storage poses and were preserved when the subsequent approved brownie additions saved the scene. Brownie planning targets 12 true-square portions in a 4-by-3 grid.

Latest equipment pass (2026-09-09): the approved physical bakeware foundation is complete. The rack beside cold storage now visibly stores the separate shallow cookie tray and deep rectangular brownie pan on different rails, retains the bowl on its existing shelf, and carries a green rubber spatula for future batter leveling. All three tools passed pickup/release checks; the open deep pan fits the oven when rotated from storage orientation. Recipe selection, brownie batter/spreading/slicing, and optional toppings remain unimplemented.

Current brownie consultation (updated 2026-09-10): Allrecipes `Quick and Easy Brownies` remains the chosen base. The user has now approved the recipe's 1/2 cup walnuts for added visible texture; other additions such as marshmallows remain deferred. Approved direction still includes parchment-only pan preparation, a deep rectangular metal pan, a rubber/plastic spatula for batter leveling, playable slicing, and 12 true-square pieces in a four-by-three grid. The pan, spatula, larger bowl, pre-melted butter pour, walnut source, and locked selector listing now exist; combined batter, spreading, baking, slicing, and the playable brownie selection branch remain unimplemented. Cooling remains tentatively optional.

Latest oven pass (updated 2026-09-12): the oven begins at 0 C and requires the player to choose a target before preheating. The player closes the empty oven, presses PREHEAT, waits for READY with the interior light remaining on, then loads the prepared tray and presses BAKE. Heating to the selected target takes 5 seconds; opening the door pauses preheating, a loaded tray blocks it, and changing the target invalidates readiness.

Latest architecture pass (updated 2026-09-12): `RecipeDefinition` stores exact cup/teaspoon and whole-item roles; a legacy chip-scoop enum remains only for backward compatibility, while the active cookie uses `HalfCup`. Measuring tools, receivers, guide, highlighting, and oven now resolve the selected Cookie or Brownie asset. Cookies retain their complete prototype loop; Brownies are playable through baking and then stop before unfinished cooling/slicing; Cupcakes remain locked. The guide/manuscript is provisional, and any mechanic, ingredient, workflow, scoring rule, or improvement outside an approved request requires user consultation first.

Latest parchment accessibility retune (2026-09-09): the sheet is reduced by about four additional tray-percentage points, from approximately 90% to 86% coverage. `TrayLinerReceiver` now has a parchment-only placement assist: a precise free-position fit is still preferred, while a nearby misaligned release can magnetically center and align within configurable horizontal/vertical detection ranges. Focused Play Mode verification passed precise preservation, high/off-center/rotated assisted placement, far/below rejection, six-cookie loading, and loaded-tray movement. Human placement feel remains for player acceptance.

Latest usability retune (2026-09-09): parchment now occupies approximately 90% of the tray support on X/Z instead of 95%, providing practical centering tolerance while keeping the pan covered. The knife's default held pose changed from vertical/crosshair-pointing to blade-forward. Whisk and rolling cooldowns increased from `0.35 s` to `1.2 s`; their fresh-travel and one-count-per-sample protections remain. Focused Play Mode verification passed an off-center paper release, six-cookie tray load, forward blade alignment, knife-created six portions, `0.65 s` repeat blocking, and full base/chip/rolling completion. Human mouse feel remains for player acceptance.

Latest interaction correction (2026-09-09): whisking and rolling now require fresh tool travel after a tunable `0.35 s` cooldown for every counted pass. Movement during the cooldown is discarded rather than banked, and a single movement sample can count at most once. Focused Play Mode tests passed immediate-repeat blocking, normal 4/4 base mixing, 3/3 post-chip mixing, and 4/4 dough flattening. Existing required counts, dough scaling, ingredient logic, and the prior oven/parchment correction remain unchanged; natural player gesture feel still needs manual acceptance.

Latest focused correction (2026-09-09): temperatures below `170 C` still report `UnderbakedCookie` but now make zero cookie material changes—no whitening, recoloring, smoothing, or bake-detail write. Parchment grew from `(.55,.002,.32)` to `(.6851,.002,.4115)`, slightly above 95% of the actual tray support on both horizontal axes. A six-cookie Play Mode fixture passed underbaked logical state with unchanged `_BaseColor`, `_BakeAmount`, and `_Smoothness`, followed by a normal `180 C` visual-change regression. No storage, workstation, pickup, recipe, threshold, timer, material/shader asset, or unrelated scene behavior changed.

Latest recovery change (2026-09-08 evening): the large utensil is explicitly Measuring Cup for flour/sugar; Small Measuring Spoon is chips-only. A held tilted measure pours into the bowl or returns to the matching source. Hold Q deliberately discards; ordinary rotation in empty space does not waste contents. Full tools cannot silently replace ingredients. Per-session transfer records distinguish bowl additions, returns and discarded measures; grading, grams and source depletion are not implemented. The guide names each tool and shows recovery controls while it is full. Focused Game-view input/tilt acceptance remains next.

Latest pass (2026-09-08): six cookies total; pale dough, varied chip shapes/placement and golden successful bake. Parchment comes from an E-operated roll. Released supplies, dough, paper, cookies and the tray use smooth near-drop settling. Utensils Area accepts tools up to .50m above it, permits .035m handle overhang and registers physical landings. Cookie centers align with the crosshair; the roll no longer intercepts release while holding food.

Tutorial now uses the whisk specifically: WASD/look, E pickup, right-click-and-mouse rotation, scroll distance, then release onto Utensils Area. Compact mise-en-place substeps show fridge food, dry cabinet supplies, remaining utensils, then the bowl. Final Play Mode input, physical landing, placement and six-cookie timed bake checks passed; full human handling/visual acceptance and VR remain pending.

Desktop prototype with a technically verified cookie slice and separate prep → mix → chopping-board flow. Flour, sugar, vanilla, baking soda, and salt come from dry storage; egg, butter, and chips come from the fridge. Prepare eight ingredient carriers on Cold Ingredient Rest, the cookie-relevant exact measures plus whisk on `Utensils Area`, and the white cookie bowl in MIX. Four base whisk passes precede one 1/2-cup chip transfer and three further combining passes. Cleaning is documented as planned only. Human handling/readability acceptance remains open.

Latest visual fix (2026-09-08): flour no longer protrudes below the bowl. Contents now use a bowl-local interior anchor rather than the scoop trigger; flour/sugar mounds fit the cavity. Play Mode containment, ingredient/mixing, bowl pickup/return and reset checks passed. Player visual acceptance remains pending; recipe quantities and physics are unchanged.

## Approved recipe scope

- Chocolate-chip cookies
- Brownies
- Cupcakes

## Completed foundations

- A shared recipe-data model and assigned chocolate-chip-cookie asset. Active roles cover 1 cup, 1/2 cup, 1/4 cup, 1 teaspoon, 1/2 teaspoon, 1/4 teaspoon, and whole items. The active cookie asset drives its eight ingredients, 4+3 mixing, four rolling passes, six-cookie yield, state names, and the 5-second 170–190 C bake profile.
- Three individual framed recipe sheets beneath the BakeIT sign. Cookies are selectable; unfinished brownies/cupcakes are clearly marked in development and remain locked.
- Desktop first-person navigation.
- Pickup, release, rotation, and hold-distance adjustment.
- Exact measuring interactions for flour, sugar, vanilla, baking soda, salt, and chocolate chips. Full measures cannot silently replace ingredients; matching-source return and intentional discard provide recovery.
- The `1/2 Cup Measuring Cup` collects the cookie recipe's chocolate chips and displays the dense interleaved chip fill; the dedicated chip scoop was removed. The 1 Cup measure handles flour/sugar and the three teaspoon sizes handle their matching small ingredients.
- Ordered cookie validation: 1 cup flour, 1 cup sugar, 1 stick of butter, 1 egg, 1 teaspoon vanilla, 1/2 teaspoon baking soda, and 1/4 teaspoon salt; four base whisk passes; one 1/2-cup chip transfer; three combining passes.
- A seven-hook wall rack stores six measuring vessels plus the whisk. Each tool returns only to its matching hook. `Utensils Area` is restored to its original footprint; the separate `Tool Home Mat` is the enlarged tool surface.
- Bowl ingredient visuals and multi-pass mixing.
- Dough-board, parchment-first tray preparation, and basic oven components.
- A dedicated reusable silver `Bakeware Metal` material on the cookie tray, ready to match the future deep brownie pan without altering other shared kitchen materials.
- Separate stored shallow cookie tray and deep metal brownie pan, plus a stored grabbable rubber spatula. The deep pan has an open compound-collider interior and physically fits the existing oven in use orientation.
- Knife-based portioning into six equal cookies from the unchanged ingredient batch.
- Raw and flattened uncut dough can return to the board after pickup; the same slab retains rolling progress and exact board-local size, including release while still overlapping the board.
- Complete-batch tray loading and multi-cookie oven result propagation.
- Working oven-door interaction connected to the baking controller.
- Empty-oven preheating with current-temperature feedback, READY state, context-sensitive PREHEAT/BAKE control, bake gating, and an interior light that remains on while heated.
- Enlarged 150% thirteen-phase guide: a yellow plain-language learning-part heading; recipe/numbered-stage detail; basic controls; ingredient rest, tool rest, and bowl setup substeps; base ingredients; base whisking; chips; final combining; board/tray preparation; oven preheat; tray loading/baking; latched completion.
- E-operated fridge and sliding cabinet doors expose supported shelves. Eight cookie ingredient carriers, six cookie-relevant tools, and the bowl can stabilize near release and be picked up again; two additional future cup sizes remain on the rack. Old fixed egg/butter return slots remain disabled.
- Bowl starts on a tray shelf in the reused Prop_TrayHolder rack, is carryable with hollow compound collision, and must rest in MIX to accept ingredients or whisking.
- Clear pantry/mixing, wooden-board, and parchment/tray zones; separate tool home; warm wall finish and rounded, labeled oven controls.
- An in-session full-recipe restart requires holding `R` for three seconds; the instant HUD restart button was removed to prevent accidental resets.
- `Kitchen Prototype` as the sole enabled Build Settings scene.
- Unity MCP connection and repository ignore configuration.

## Implemented systems pending player acceptance

- The chocolate-chip-cookie sequence is implemented and callback-verified from ingredient collection through baking, including distinct undercooked, successful, overcooked, and burnt results.
- The reset path is callback-verified to remove runtime-created objects and restore consumed ingredients, bowl quantities, oven state, and the initial instruction without leaving Play Mode.
- Player-facing pickup, reachability, collision-recovery, feedback-readability, and gesture-feel acceptance remains open, so the recipe is not yet marked complete under `Codex.md`.
- Temperature-based baking result visuals and player-facing textual feedback work without final recipe scoring.

## Current priority

Keep the Cookie path stable while extending the now-selectable Brownie development branch. Brownie-specific mise en place, exact measuring, four-Egg intake, progressive large-bowl batter, four base whisk passes, half-cup Walnuts, and two finishing passes are playable and use matching current-step outlines. The guide then stops at `NEXT PART STILL IN DEVELOPMENT`. Next connect parchment/deep-pan pouring, spatula spreading, the approved 8-second bake, and 4-by-3 slicing. Cooling remains optional presentation rather than a gate. Cupcakes, icing, cleaning, and actual grading remain later decisions.

## Remaining September 30 deliverables

- Complete chocolate-chip-cookie, brownie, and cupcake workflows.
- Guided Practice and Challenge modes.
- VR interaction and tested Meta Quest build.
- Scoring, safety, organization, and waste-event tracking.
- Cloud session submission and instructor-dashboard integration.
- Full Play Mode, device, performance, and regression testing.

## Known blockers

- Recipe facts are now data-driven for cookies, but the thirteen-stage session graph, mise-en-place inventory/layout, cookie-specific visuals, and runtime step progress are not yet generic or persistent. They should be generalized only against an approved second-recipe design.
- `UI-001` now waits for the physical recipe choice, uses plain-language yellow headings without phase counters, splits Cookie tool preparation across progressive screens, retains the separate preheat step and completion latch, and routes outlines through the full Cookie workflow; player readability/clutter retest remains.
- `ENV-001` / `PREP-001` now use separate rests and a movable bowl; technical release, wrong-surface, mixing and bake/reset checks passed. Full manual carrying paths, door feel and visual preference remain pending.
- `SERVICE-001`: Unity Connect/AI account-service errors were present at session start; no account credentials/settings changed. See known issues and test log for separate diagnostic failures.
- `MEASURE-001` is technically resolved through small-spoon configuration, refill/replacement logic, and a larger chip-source trigger, but still requires the player's physical dip-and-transfer retest.
- `MEASURE-002` (sparse/overflowing chip visuals and large-spoon chip acceptance) and `FLOW-003` (uncut dough pickup blocks progress) now have implemented, Play Mode-verified fixes; player acceptance of the final appearance and handling remains pending.
- No OpenXR, XR Interaction Toolkit, or Meta XR package is configured.
- Guided modes, scoring, telemetry, cloud submission, and instructor dashboard remain planned rather than implemented.

## Testing status

- Focused Play Mode verification passed for the parchment-first tray sequence and oven-controller wiring.
- Focused Play Mode verification passed the full preheat lifecycle: cold bake rejection, loaded-tray preheat rejection, visible heating progress, READY/light retention, loaded-tray bake, six Perfect cookies, and retained READY/light state after completion.
- Focused Play Mode verification passed for invalid early cutting, four- and eight-cookie proportional yields, equal and volume-preserving portions, complete-batch tray loading, oven readiness, and result propagation to all cookies.
- An earlier pre-reorder Play Mode baseline passed from the chocolate-chip source through the shared scoop, then-current five-ingredient validation, four equal chip-bearing portions, parchment-first tray loading, and a perfect 180 C bake without recoloring the chips.
- A focused four-outcome Play Mode pass produced `UnderbakedCookie` at `160 C`, `BakedCookie` at `180 C`, `OvercookedCookie` at `200 C`, and `BurntCookie` at `220 C`; all four portions updated and chip colors remained unchanged.
- Unity Editor and Play Mode startup smoke testing completed without project-script errors.
- No persistent automated project tests exist yet.
- A final player-driven usability and reachability pass remains; current end-to-end verification drove the real runtime callbacks deterministically.
- Isolated Unity 6000.5.0f1 compilation passed with no project-script errors or warnings after the reset, feedback, and input-device guards were added.
- Focused reset verification passed after disabling a saved ingredient and creating a runtime-only object: the scene restart restored the ingredient, removed the runtime object, cleared bowl quantities, reset the oven, and restored the opening instruction.
- The first player UI test failed because the original panel narrated recent actions instead of guiding the next task. The first replacement guide passed its then-current eight-phase progression checks and confirmed that normal action messages no longer replace its checklist; later ten-phase and current eleven-phase flows have their own verification entries below.
- A focused Play Mode verifier confirmed that dough remains unflattened through three rolling passes and becomes `FlattenedDough` on the new fourth pass.
- A focused Play Mode verifier passed the controls-first guide, premature-chip recovery, four-pass base-dough threshold, three post-mix chip transfers, three-layer filled-scoop visual, ten-stage progression, and completion latch. A final verifier-free compile reported no project C# errors or warnings.
- A focused isolated Play Mode test passed for `Small Measuring Spoon`: the saved `_01` instance had its measuring component, replaced stale flour with chocolate chips, refilled during retained trigger overlap, and showed three dense active chip layers. The subsequent verifier-free Unity compile exited with code `0` and no C# errors or warnings.
- The subsequent screenshot-driven retest exposed visual overflow despite that earlier state-only pass. The final evening pass used the connected Unity Editor: real ray pickup, physics source refill, vertex containment and rendered visual QA, wrong-tool rejection, raw/flattened dough return, partial progress and size preservation, actual knife contact, parchment placement, complete tray loading, and a successful `180 C` bake for all four recovered portions. Final Console query reported zero errors/warnings. No persistent test script or review-project copy was added.

- 2026-09-07 connected Play Mode checks passed for initial fridge contents, E-ray door use with/without a held ingredient, closed-door pickup blocking, player obstruction/resume, egg return through physical trigger contact, wrong-slot rejection, shelf support and loose-tool retrieval, setup checklist progression, settled mixing mats, relocated chip/board/knife/parchment triggers, rounded oven-button rays, a four-cookie Perfect bake, completion latch, and restart. Deterministic calls drove recipe counts/transfers and oven loading; this is not a complete manual traversal/gesture acceptance run. See the dated test-log entry.

Earlier board-preparation checks are retained in the 14:15 test entry. Latest separate-rest checks passed wrong-board rejection, eight supply placements, bowl-required readiness, rack pickup/return/re-pick, delayed bowl carry/rotation/scroll, high/occupied placement refusal, filled-bowl relocation without quantity/scale loss, three actual post-chip whisk movements, final dough pickup, relocated board/flattened recovery, four-cookie Perfect bake and reset. See the evening test entry for callback versus physical-input limits and corrected diagnostic failures.

## VR readiness

Not ready. The project currently targets Windows and uses direct keyboard and mouse input.

## Manuscript alignment

The manuscript retains the capstone premise while naming the approved recipe scope as chocolate-chip cookies, brownies, and cupcakes and identifying Unity 6 as the implementation version. It is a working guide rather than a finalized feature specification; proposed additions or deviations require user consultation before implementation.

## Next recommended task

The supplied pantry-reference sequence is complete, and Baking Soda now also has a distinct open-carton model. Egg and melted Butter remain accepted as-is. Teacher-test feedback is addressed with front-facing held poses, one-step stuck recovery, non-emissive copper measuring tools, permanent work-area names, a returnable `BAKING TOOLS` rack, action-based guide screens, six-cookie consistency, and full-path current-step outlines. These usability changes await the user's in-game acceptance. The next gameplay milestone remains the Brownie deep-pan/spatula stage when the user redirects back to gameplay.

## Brownie correction status — 2026-09-13

The selected B1/B2/A3 presentation now progresses with measured ingredients instead of showing the full batter after the first Sugar transfer. The bowl expands from approximately `.153 m` to `.294 m`, stays bottom-anchored and closed, changes to the approved dark-brown color, and hides Walnuts until the sixth required whisk pass. Transferred B2 batter is closed and seated on the pan liner.

The deep pan now substitutes a fitted five-piece parchment presentation after the loose sheet is accepted: one bottom plus four raised sides. The source sheet is locked to the pan without collisions and is fully restored on restart. Package-front identifiers were added to the ingredient sources, and the malformed Baking Soda label was rebuilt. Final Play Mode acceptance and Console checks passed. Next priority is user acceptance in a normal playthrough; cooling and slicing into 12 squares remain unfinished.

## Ingredient visual follow-up — 2026-09-13

Cocoa and Baking Powder now have clearly exposed, larger front product names rather than labels buried in their badge geometry. Flour shows an uneven powder surface below its open rim, while Sugar uses a bright contained mound with visible granules. The Walnut source was rebuilt from the supplied reference as 34 golden, faceted, crinkled chopped pieces in a dense three-layer pile; Brownie Walnut details were also made less symmetrical. All source identities, measuring triggers, pickup physics, quantities, and the six-pass Walnut reveal gate passed Play Mode regression.

</details>
