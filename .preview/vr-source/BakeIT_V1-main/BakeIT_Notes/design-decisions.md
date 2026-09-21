# BakeIT Design Decisions

## 2026-09-11 onboarding visibility and restart safeguards

- Decision: teach and gate the desktop movement/whisk interaction vocabulary before permitting recipe selection.
- Reason: the recipe wall is a goal, but the player first needs to understand how to reach and manipulate it safely.
- Decision: use unlit bounding-line outlines rather than scene Lights or material replacement for guided targets.
- Reason: yellow/red edges remain bright and readable without scattering onto the wall, changing authored materials, or making every nearby object glow. A small bounds pulse provides motion without glare.
- Decision: show carryable names in an occlusion-aware screen-space hover label and preserve the user's final fixed-sign rotations.
- Reason: labels stay upright regardless of item orientation and cannot be seen through a closed cabinet.
- Decision: require a continuous three-second `R` hold and remove the instant HUD restart button.
- Reason: restarting destroys current batch progress; a deliberate hold prevents accidental single-key resets.
- Decision: keep one-time tutorial area introductions separate from delayed mistake correction.
- Reason: repeated normal highlighting would become noisy, while a red misplaced item paired with its yellow destination remains useful after the first introduction.

## 2026-09-04 Git repository contents

- Decision: Version the complete reconstructable Unity source rather than only `.cs` files.
- Included source: `Assets`, `.meta` files, `Packages`, `ProjectSettings`, project documentation, and repository configuration.
- Excluded content: Generated caches, local editor state, builds, IDE output, local workspace metadata, and the manuscript DOCX.
- Reason: Unity asset references depend on `.meta` GUIDs, scenes, prefabs, settings, and package declarations. A code-only repository would not reproduce the project.
- Large-file decision: Defer Git LFS until required binary sizes and the remote hosting limit are reviewed.

## 2026-09-04 Prototype stabilization boundaries

- Decision: Preserve deliberately short bake times, broad interaction distances, and other test-friendly Inspector values during prototype stabilization.
- Reason: These values support rapid iteration and are not confirmed defects. Production balancing belongs to a later usability and device-testing pass.
- Decision: Preserve the existing oven-door implementation and repair only its connection to the baking controller.
- Reason: The door interaction is already working; replacing it would add risk without addressing the confirmed integration defect.
- Decision: Treat recipe architecture, guided modes, scoring, VR, cloud submission, and the instructor dashboard as planned backlog rather than defects in this correction pass.
- Reason: The prototype is intentionally incomplete, and those systems require separate design and validation work.
- Decision: Keep the manuscript's overall capstone premise while aligning concrete implementation statements to the approved recipes and the current Unity version.
- Approved recipes: Chocolate-chip cookies, brownies, and cupcakes.

## 2026-09-05 First complete dish and cookie portioning

- Decision: Finish the chocolate-chip-cookie vertical slice before introducing shared recipe architecture or starting brownies and cupcakes.
- Reason: A complete playable sequence will expose the real workflow and feedback requirements more reliably than generalizing from incomplete interactions.
- Decision: Reuse the licensed `Prop_Knife_06` as a physical, grabbable portioning tool. One valid pass through fully flattened dough divides and lays out the whole batch during the prototype stage.
- Reason: This preserves embodied interaction while keeping the first complete-dish iteration fast to test. Repeated individual cuts can be evaluated later if they materially improve training value.
- Decision: Derive yield from the flattened dough's rendered world-space volume, using four cookies as the reference yield and nine as the current tray-capacity ceiling.
- Reason: The existing prototype has no gram-based recipe quantity model, but visual volume lets larger dough quantities produce proportionally more cookies now.
- Decision: Preserve thickness and scale each portion's horizontal area by `sqrt(1 / yield)`.
- Reason: This creates equal-looking portions while preserving the represented combined dough volume.
- Decision: Treat a tray as oven-ready only when every portion from the same batch is present.
- Reason: This prevents baking partial or mixed batches and gives the later recipe session a clear completion boundary.

## 2026-09-05 Chocolate-chip ingredient and recipe implementation

- Decision: Reuse the existing open bowl model for the chocolate-chip source and give it a labeled, visible chip surface rather than representing chips with another closed powder carton.
- Reason: The open container communicates scooping directly and distinguishes a discrete ingredient from flour and sugar while staying within the current licensed asset set.
- Decision: Use one shared `MeasuringScoop` for flour, sugar, and chocolate chips, with ingredient-specific contents visuals.
- Reason: This extends the established measured-ingredient interaction without adding a parallel tool or special-case input path.
- Decision: Require exactly one flour scoop, one sugar scoop, one butter portion, one egg, and one chocolate-chip scoop for the prototype cookie batch. Reject extras and unsupported inputs before consuming them.
- Reason: Exact quantities make recipe state predictable and prevent accidental inputs from silently changing a batch. Values remain serialized so later balancing can change them without rewriting the interaction.
- Decision: Mark chocolate renderers with `ChocolateChipVisual` and exclude them from dough recoloring and volume-based yield calculations.
- Reason: Chips must remain visibly chocolate through mixing, portioning, and baking, and decorative geometry must not inflate the amount of dough or its cookie yield.
- Decision: Keep the five-second prototype bake duration and `10 C` controls, but divide results into undercooked below `170 C`, successful from `170-190 C`, overcooked above `190 C` and below `220 C`, and burnt at or above `220 C`.
- Reason: The distinct overcooked band satisfies the required learning feedback without slowing rapid prototype testing or changing the user's deliberately test-friendly controls.

## 2026-09-06 Cookie session reset and player feedback

- Decision: Treat `Codex.md` as the authority when the condensed implementation guide conflicts with the approved recipe scope or current Unity version.
- Reason: The hard rules approve chocolate-chip cookies, brownies, and cupcakes in Unity 6. The guide's Unity 2022 and pandesal references were stale and have been corrected rather than copied back into the manuscript.
- Decision: Reset the cookie workflow by reloading the sole enabled prototype scene while remaining in Play Mode.
- Reason: Scene reload reliably restores every serialized object and private component state, removes runtime cookie portions and generated visuals, and avoids duplicating reset logic across the bowl, board, tray, oven, tools, and player.
- Decision: Route existing recipe progress and correction messages through a presentation-independent `RecipeFeedback` event and display the latest message in one session-owned HUD.
- Reason: Interaction systems remain responsible for recipe decisions, while the HUD only presents their messages. Expected player mistakes remain ordinary Console logs; genuine configuration defects remain warnings.
- Decision: Keep subjective reachability, readability, and gesture-feel acceptance as a human checklist item.
- Reason: Automated callback and headless verification can confirm state and recovery, but it cannot determine whether physical interaction feels clear and comfortable to a player.

## 2026-09-06 State-derived staged recipe guide

- Decision: Present the cookie workflow as eight progressively disclosed phases instead of displaying the latest successful action.
- Reason: The first player test found that action narration was confusing and did not answer what should be done next. Showing only the current phase preserves guidance without covering the upper half of the screen.
- Decision: Derive checkmarks and progress directly from existing bowl, board, tray, and oven state.
- Reason: Gameplay components remain authoritative, so the guide cannot drift merely because a log message was missed or reworded and no duplicate recipe-state manager is introduced.
- Decision: Keep normal progress in the Console and reserve the panel's orange message area for short-lived corrections.
- Reason: A learner can see both the required task and why an invalid attempt failed without losing the checklist.
- Decision: Require four rolling passes for the saved cookie scene.
- Reason: The player's requested sequence calls for at least four visible flattening actions. Four matches the existing four-pass whisk rhythm while remaining short enough for prototype iteration.

## 2026-09-06 Tutorial, post-mix chips, and completion latch

- Decision: Begin each new Play Mode session with three observed desktop tasks—movement, mouse look, and measuring-scoop pickup—before showing recipe ingredients.
- Reason: These tasks teach the minimum interaction vocabulary needed for the prototype without over-investing in bindings that will later be replaced by VR input.
- Decision: Form base dough from flour, sugar, butter, and egg over four whisk passes, then require three small chocolate-chip scoops.
- Reason: The requested preparation order keeps chips out of the initial whisking stage, while three transfers make the ingredient quantity more legible than one sparse measure.
- Decision: Build the filled scoop as three overlapping collider-free instances of the existing chip cluster rather than creating another utensil or changing its physics.
- Reason: A fuller visual directly addresses the sparse-measure feedback while preserving the established shared-scoop interaction and keeping the path to VR simpler.
- Decision: Latch the first completed oven result in the session guide until recipe restart.
- Reason: Baking changes downstream object state and the player may move the tray afterward; neither should make a completed recipe appear incomplete or send the learner back to oven loading.

## 2026-09-06 Intended small measuring spoon and source recovery

- Decision: Configure the licensed `Prop_MeasuringSpoon_01` scene instance as the intended `Small Measuring Spoon`, while labeling the already configured `_04` utensil as `Large Measuring Spoon`.
- Reason: The player correctly identified `_01` as the small utensil, but only `_04` previously had measuring behavior. Explicit scene names and compatible components remove the visual/functional mismatch without modifying the vendor prefab assets.
- Decision: Let a new ingredient source replace a different ingredient already in a measuring spoon and retry fills during trigger stay.
- Reason: A rejected base measure otherwise deadlocks the one-utensil workflow, and a retained-overlap retry makes small-tool dipping accessible without changing recipe quantities.
- Decision: Give the small spoon a dense, collider-free procedural fill and enlarge only the chocolate-chip trigger.
- Reason: The `_01` vendor prefab has no serialized contents child. Runtime visual-only geometry provides clear fullness while preserving its collider, Rigidbody, pickup behavior, and future VR interaction surface.

## 2026-09-06 Contained chip measure and recoverable dough preparation

- Decision: Only `Small Measuring Spoon` measures chocolate chips; both spoons retain flour/sugar measuring. A serialized capability controls chip eligibility, with clear wrong-tool feedback.
- Reason: The user requested a small-only chip measure after the two differently sized cups gave inconsistent, confusing visual quantities. Three post-whisk transfers remain the recipe requirement.
- Decision: Reuse the existing low-poly chip-cluster mesh and material in eight interleaved instances, normalize its actual mesh pivot/footprint, and fit it within the small cup's inner radius. Permit only a shallow mound above the rim.
- Reason: The previous sphere fallback and fixed offsets were too large for `_01`. Close-up rendered inspection also showed that merely adding layers at the old height left visible gaps; interleaved packing gives a visibly full cup without a floating cloud or new licensed-asset changes.
- Decision: Returning the same uncut dough slab resumes preparation, including after flattening and rotation. Trigger stay can place released dough but cannot pull it out of the player's hand.
- Reason: Pickup is reversible and must not invalidate the batch. Saved board-local scale prevents repeated return from changing the amount of dough, while rejecting actual portions keeps the board and tray responsibilities distinct.

## 2026-09-07 Modest kitchen and mise-en-place pass

- User-directed priority: Make kitchen organization/storage viable before final cookie polish, rather than immediately expanding to another recipe. Approved recipe scope remains cookies, brownies and cupcakes.
- Reuse furniture and pivoted fridge doors. Separate pantry/MIX, wood-board prep, and tray zones across the existing counter; move the existing sideboard into the spare oven space. Preserve the unused oven inactive for recovery rather than deleting it.
- Give egg/butter dedicated fridge return slots and empty shelves normal physical storage. Avoid adding an inventory menu, stock system, temperature simulation, or new recipe manager to this pass.
- Reuse E for targeted fridge opening/closing; expose the action independently for future VR input. Stop moving doors before player/item penetration and use static shell/solid shelf support instead of a full solid box across the cavity.
- Add a small, latched four-item preparation checklist before base ingredients. Already-consumed egg/butter count as gathered, preventing an early-action deadlock. Keep original recipe validation authoritative and UI compact.
- Use a brown code-native fine-grain board finish and warm plaster/sage/slate accents. Keep the game-like station badges and rounded minus/plus/BAKE controls, preserving the actual oven model and working bake/door systems.
- Technical callback/physics tests establish regression safety, not subjective acceptance. Require the user's normal carrying/gesture/readability pass before calling the environment or cookie fully polished.

## 2026-09-07 User correction: prepare first on the board

- Replace the first MIX-volume interpretation with physical board preparation: all five ingredients and small spoon/whisk must be present together before the bowl accepts additions. Count deliberate release, not hover or early consumption. Preserve readiness once preparation is complete so using ingredients is not penalized.
- Reuse the storage cabinet's real sliding panels/platform for flour/sugar and put chips in the fridge. Make the original sources carryable while keeping source versus measured-ingredient responsibilities separate.
- Stabilization retains drop X/Z/yaw and adjusts only resting orientation/height near a clear support plane. No fixed horizontal grid, automatic held-item capture or preset return teleport. Retain old slot objects disabled for reversibility.
- Slightly widen the existing board to fit seven supplies, keeping depth/thickness and dough world quantities unchanged. Ask the player to clear tools/containers beside it before dough preparation.
- Put depth-tested labels on both sides of the original food/containers. This keeps names attached through transport and avoids table labels pointing at empty locations. Exclude label/empty-renderer bounds from held centering.
- No new recipe, stock/inventory manager, spilling simulation, package or input binding. E/rotation/scroll/Ctrl use the existing desktop vocabulary; full VR mapping remains later work.

## 2026-09-07 Separate ingredient preparation, mixing and chopping

- Supersede board-based supply preparation with user-requested Cold Ingredient Rest and Mix Tool Rest. Preserve the user's enlarged mat dimensions/positions. Require all supplies to be at the correct rest concurrently before latching readiness; keep them re-grabbable near their release location.
- Include the large measuring spoon in tool preparation along with small spoon/whisk; keep pin/knife beside the chopping board for the later stage. Chips remain fridge-stored and are staged with the other ingredients, but added only after base mixing.
- Put mixing between the rests and wooden board. Make the original bowl carryable; reuse the existing tray-holder rack with a tray shelf instead of inventing a dish-rest asset. No vendor prefab edits.
- Require the bowl to rest in MIX for recipe actions, preserving contents while carried. Do not let moving the bowl count as stirring.
- Use three post-chip whisk passes, separate from the four base passes. Three chip measures alone no longer spawn dough. Preserve original baking quantities, roll counts and oven tuning.
- Cleaning is notes-only future pre-work design; do not pretend a cleanliness gate exists. No new recipe, inventory, waste simulation, package or VR claim.

## 2026-09-08 Bowl contents use vessel coordinates

- Do not derive ingredient presentation height from the scoop-detection trigger. Keep detection reach and visible vessel contents independent, with a serialized bowl-local center.
- Fit the flour/sugar visuals to the existing mesh cavity rather than adding colliders to decorative ingredients or changing measured quantities. This corrects clipping without implementing spills or altering recipe physics.
- Use actual bowl-mesh containment and Play Mode rendered/interaction checks, not compilation alone, to validate placement. Keep prior geometry iterations and before/after evidence in the notes.

## 2026-09-08 Six-cookie visual and release-assistance pass

- Six cookies total from unchanged ingredients; split into six smaller equal portions, not six extra or a larger recipe. Keep four base/four rolling/three chip/three combining interactions.
- Reuse existing low-poly dough and chocolate material with generated geometry/grain; reference photos guide color and scatter, not an exact asset replacement. Successful cookies should read golden, with darker states reserved for incorrect bake outcomes.
- Player release selects X/Z/yaw. Shared short settling and obstruction/support validation replace fixed-slot teleports. Small tool overhang is permitted only on Utensils Area; physical landings count. Never credit a held hover as preparation.
- One parchment sheet per recipe restart, drawn from a simple roll. Protect loaded liner from removal and allow cooked-cookie rearrangement. Do not expand into waste/spill/cleaning simulation.
- Whisk is the single tutorial tool; explicitly practice rotate/scroll and placement before fridge→dry-cabinet preparation. Split screens to retain compact UI. Keep the user's named Cold Ingredient Rest as preparation destination for both ingredient groups.
- Center held cookie portions on the crosshair while preserving other items' established offset and collision checks. E-release takes precedence over a parchment roll behind held food. Preserve manual layout edits and record them rather than restoring authoring defaults.

## 2026-09-08 evening Explicit measuring roles and recoverable contents

- User expanded scope to measured-content return/discard and preparation for waste grading. Supersedes earlier no-waste-foundation restriction only for measured utensil contents; full spills, cleaning and scoring remain unimplemented.
- Use the existing large _04 as Measuring Cup for flour/sugar, _01 Small Measuring Spoon for chips. Keep current recipe interaction-unit quantities; do not claim calibrated grams or silently rebalance the dough.
- A held tilt over the bowl transfers a measure; an upright touch does not. A matching-source tilt returns it. Rejected/early measures remain recoverable. Never silently overwrite a full tool with a different ingredient or delete premature chips.
- Deliberate hold-Q discard is distinct from pouring/returning. Do not turn ordinary carrying rotation in empty space into accidental waste. Cancellation and empty-tool guards prevent spurious records.
- Extend existing recipe session with structured disposition records; record units/tool/ingredient and distinguish returns from discarded measures. Do not invent a grade or treat unlike measures as grams. Source depletion and grading rules require later design.
- Keep contextual controls in one held-measure row/footer, not a new permanent action log. Transfer grains are short collider-free presentation, not food-quantity physics or persistent mess.

## 2026-09-09 Literal underbaking and parchment coverage

- Treat underbaking as a logical failed outcome with feedback, not a material treatment. Skip all cookie-base visual writes below the successful threshold so low heat cannot whiten or otherwise imply visual cooking.
- Size the existing sheet to slightly above 95% of the actual tray support on both axes. Keep the small precision margin rather than serializing a value that rounds just below the requirement. Do not change tray placement, storage, recipe quantities, or other interactions.

## 2026-09-09 Fresh-stroke cooldown for mixing and rolling

- Use the same `.35 s` tunable cooldown for whisk and rolling passes. Discard travel during that interval and reset accumulated distance after a count, so one fast or oversized gesture cannot be replayed as several actions.
- Preserve the existing distance thresholds and recipe counts. The cooldown is input debouncing, not a change to recipe quantity, dough physics, flattening scale, or visual progression.

## 2026-09-09 Usability retune after player testing

- Supersede the initial `.35 s` pacing with `1.2 s` for both cooldowns after it still felt too fast. Keep the fresh-travel and single-count protections and retain Inspector tunability.
- Size parchment to approximately 90% of the tray support after the 95% version proved too unforgiving for free-position placement. Preserve the shared placement rules and gain centering tolerance through the footprint rather than silently snapping an invalid release.
- Hold the dough knife with its long blade axis pointing forward. Its model extends mainly along local `-Z`, so use scene pickup pose `(0,180,0)` instead of the prior `(90,0,0)` vertical/crosshair-facing pose.

## 2026-09-09 Parchment magnetic placement assist

- Reduce paper another four tray-percentage points to approximately 86%, interpreting the player's “like 4%” as four points from the current 90% footprint.
- Keep exact free-position placement when it fits. Only parchment receives a fallback that centers and tray-aligns a nearby failed pose; this avoids changing shared placement behavior for dough, cookies, tools, or supplies.
- Serialize the `.35 m` vertical window, `.12 m` horizontal detection padding, and `.15 m` magnetic radius so sensitivity can be tuned without another code change. Retain the normal below-surface, obstruction, identity, occupancy, and maximum-distance guards.

## 2026-09-09 Provisional guide and approved recipe-data boundary

- Treat the manuscript/guide as working direction rather than a finalized feature specification. The current recipe scope remains chocolate-chip cookies, brownies, and cupcakes, but implementation details are not automatically authorized merely because they could be described as improvements.
- Consult the user before adding or changing any mechanic, ingredient, workflow stage, scoring rule, or feature that departs from the guide or from the currently approved request. Do not infer approval for adjacent polish or systems.
- The current approved architecture pass may introduce reusable recipe configuration and migrate the existing cookie values without changing observable gameplay. It must not implement brownies, cupcakes, icing behavior, yeast/leavening rules, scoring, modes, or cloud behavior.
- Future-facing shaping, pan-preparation, and post-bake-finish fields may exist as dormant configuration vocabulary so the base type can describe the three in-scope recipe families. A dormant `Icing` option is not an implemented cupcake stage and does not lock the cupcake formula; those details require a later user decision.

## 2026-09-09 Explicit oven preheat and retained readiness light

- Reuse the existing oven start control contextually: it requests PREHEAT while cold, shows HEATING during warm-up, becomes BAKE at readiness, and shows BAKING during the timer. Do not add a duplicate button or second oven system.
- Begin the oven at `0 C` and require the player to select a target before preheating. Use a serialized `5 s` prototype preheat duration and display current temperature during warm-up. Keep bake duration and outcome thresholds in recipe data; preheat pacing is oven interaction tuning.
- Require the oven to be empty and its door closed before preheating. Pause heating while the door is open, require readiness before baking, and invalidate readiness if the target changes.
- Keep the existing interior light/emissive surface on while preheating, ready, baking, or open under the configured door-light rule. Retain the ready/light state after a bake to make the heated oven visibly persistent.
- Add only the approved oven behavior and guide step. Do not infer authorization for extra kitchen props, ingredients, safety/scoring mechanics, or unrelated polish from the phrase “kitchen essentials.”

## 2026-09-09 Brownie base direction and physical bakeware selection

- Use Allrecipes `Quick and Easy Brownies` as the brownie basis, omit walnuts, and defer walnuts, marshmallows, and other additions until all three base dishes are complete.
- Plan for a deep rectangular metal brownie pan, a rubber/plastic spatula for physically spreading and leveling batter, and playable slicing. Treat cooling as tentatively optional and parchment-versus-greasing as unresolved until the user confirms.
- Give the cookie tray a dedicated project-owned silver bakeware material instead of editing shared `Panda Mat`. Reuse that material on the future brownie pan so they look like one equipment family.
- Recommended future selection: keep the shallow cookie tray and deep brownie pan as separate grabbable objects in storage. The chosen recipe identifies the required bakeware; the guide directs the player to it; the wrong pan receives corrective feedback. Do not morph one pan into another or silently replace it.
- A recipe-selection interaction and bakeware-type validation are not yet approved/implemented. Add them only after the user approves this physical selection design and confirms where the pans should be stored.

## 2026-09-09 Brownie portions, mixing vessel, and manual rack authority

- Use 12 playable brownie portions in a four-column by three-row grid. Make the cuttable slab 4:3 so each resulting portion is a true square. A literal full 9-by-13-inch area cannot produce a small practical grid of exact squares; four-by-three across that entire footprint would be equal near-squares instead.
- Brownies use a separate larger mixing bowl. Preserve the cookie bowl and its workflow; the approved first implementation uses `Prop_Bowel_02` at `1.4x` with its own butter receiver rather than adding a second cookie-style `BowlReceiver`.
- Supply the source recipe's butter already melted in a labeled cup. Require a deliberate held tilt over the larger bowl; do not add a microwave/stovetop melting mechanic at this stage and do not let the liquid count as cookie butter.
- Preserve the user's live rack, pan, bowl, and spatula transforms. Storage locations are presentation defaults, not hard-coded gameplay coordinates; future additions must tolerate the user's manual re-leveling.

## 2026-09-10 Exact cookie measures, hanging rack, and walnuts

- Historical 2026-09-10 decision, superseded 2026-09-11 for butter/chips: introduce the full eight-ingredient formula. Current values use one stick of butter and one 1/2-cup chocolate-chip transfer.
- Distinguish exact measuring capacities in recipe data and on the visible tools. Keep future 1/2- and 1/4-cup measures available without requiring irrelevant tools during cookie mise en place.
- Use a labeled horizontal hanging rail for storage and a wider existing countertop tool rest for active staging. Matching hooks accept returned tools; do not turn the rail into a decorative-only prop.
- Use one backed player-facing label on ingredient carriers. Remove reverse duplicates and redundant chip text rather than placing readable labels on both sides.
- Include 1/2 cup walnuts in the planned brownie base for visible texture, superseding the earlier omission. Keep the brownie poster locked until its complete runtime branch is implemented and tested; do not infer approval for other toppings.

## 2026-09-11 Brownie prototype timing and compound measures

- Use an 8-second prototype bake for Brownies, approved by the user, with the success band centered on the source recipe's 175 C target. Cooling remains optional and is not a completion gate.
- Represent 1 1/4 cups of Flour as one 1-cup requirement plus one 1/4-cup requirement. Recipe lookup must match both ingredient identity and exact tool so either transfer cannot falsely satisfy the other.
- Keep the Brownie poster locked while building and testing the branch. Recipe data and the existing large-bowl butter binding may land first; selection becomes available only after measuring, mixing, spreading, baking, slicing, guide, and reset paths work together.

## 2026-09-11 Explicitly bounded Brownie development access

- Supersede the earlier all-or-nothing poster lock at the user's request. Make Brownies selectable after the movement tutorial so the implemented large-bowl measuring and mixing slice can be validated before the rest of the dish exists.
- Stop the Brownie guide at a truthful `NEXT PART STILL IN DEVELOPMENT` boundary after completed batter. Do not fall through into Cookie-specific pan, oven, or slicing state and do not imply downstream completion.
- Keep Cupcakes locked. This development-access exception does not authorize new Brownie mechanics beyond the already approved recipe or any unapproved Cupcake details.
## 2026-09-13 — Keep the approved pan and select Brownie food art before integration

- The existing deep rectangular silver Brownie pan remains authoritative; the new photo references guide only the food inside it.
- A/B/C concepts are preview-only. A favors smooth readable swirls, B favors rustic ridges and restrained natural cracks, and C favors chunkier walnut density and stronger fissures.
- Do not replace the pan placeholder with final raw/baked geometry until the user selects A, B, C, or a combination. Bowl fullness is independent and may be applied immediately because the user explicitly requested it.
- Preserve recipe quantity and progression separately from visual volume; the larger bowl mound is presentation tuning, not an added ingredient amount.

## 2026-09-13 — Adopt the B1/B2/A3 Brownie art combination

- Use B1 for the full rustic bowl, B2 for the thick raw pan surface, and A3 for the smoother baked surface.
- Keep the existing deep silver pan unchanged. Food geometry is project-owned, procedural, collider-free, and replaceable independently of pan gameplay.
- Walnut density intentionally steps down from bowl to baked presentation: 12 clusters in B1, 10 in B2, and 8 in A3. Do not add chocolate chips to Brownies.
- `BrownieFoodVisual` is a presentation bridge only. `BrowniePanReceiver`, `PourableIngredientReceiver`, and `OvenController` remain authoritative for transfer, spreading, bake outcome, and recipe progression.

## 2026-09-15 08:59 +08:00 — Reconciled current state and chosen development order

- User chose D now, will personally perform A and send feedback, then wants C to finish Brownies before B (Cupcake planning).
- D changes documentation only. Existing historical entries and user-authored work remain preserved.
- Existing Brownie slicing/pouring/spreading/baking must be tested, not rebuilt based on stale notes. Independent serving/pickup and cooling require a concrete follow-up decision; finishing Brownies first does not by itself specify those mechanics.
- Current recipe/interaction summaries supersede old chip-spoon, automatic-replacement, selection-first, instant-restart and stop-after-mixing instructions. September 6 CodexReview is historical evidence.
- Follow Codex.md's near-feature-complete September 30 recipe target; flag the broader tracker deadline list as an unresolved schedule discrepancy rather than silently scheduling or removing capstone deliverables.
- Keep BakeIT_Notes as the existing active note set and preserve existing file links. No duplicate Project_Notes tracker is created.
- Responsible: Codex. No runtime, scene, asset, Inspector, package or settings changes.

## 2026-09-15 09:41 +08:00 — Serving endpoint and grading timing from user feedback

- User explicitly deferred rubric implementation until every dish and the VR base game are complete and ready for data collection. Do not implement a provisional score, criteria weights or a grade placeholder in this pass.
- User prefers a smaller serving. Current adjustable prototype target: three Cookies or four Brownie squares on a serving plate. Batch yields remain six/twelve.
- Plating becomes the final player-facing stage; grading will be connected later. Existing baked results remain visible.
- Guided practice colors an ingredient line green only when its full exact recipe requirement is satisfied; Brownie Flour requires both 1-cup and 1/4-cup transfers. Outside guided practice, recipe text stays unchanged. Full Free/Challenge mode implementations remain deferred.
- Hold M for three seconds leaves/discards the current batch and returns to recipe selection with tutorial completion retained. Hold R still restarts the selected recipe.
- Slightly taller dry storage is 1.4 Y scale (previously 1.18), arranged as large containers behind smaller front-row supplies; no new recipe contents are invented.

## 2026-09-15 final defaults and authoring choices

- Smaller serving defaults are three Cookies and four Brownie squares, adjustable on the serving receiver. The remaining baked batch stays available in its bakeware.
- The serving plate uses an existing licensed asset, normalized under a unit-scale parent so pickup does not shrink food. It sits at X=2.42, inside the countertop, with spaced Brownie slots.
- Wrapped-butter letter geometry was mirrored incorrectly on both sides. Unpack only its scene visual and correct the letter group, retaining the original generated prefab asset. The melted-butter jug retains existing pouring identity and uses a new hollow procedural ceramic visual.
- Future Free/Challenge mode UI remains deferred. A guided-practice flag controls only the requested green ingredient-paper feedback for future integration.

## September 15 follow-up — smaller Brownie stacks

User reference and decision supersede the original four-square serving: accept a choice of two or three Brownies. Default two; use the 2/3 keys during plating to select. Stack pieces with small offsets/rotations and measured food/plate contact. Cookies remain three. The existing result label is not grading; the rubric remains deferred.

## 2026-09-15 — Cupcake recipe source selected

The user selected https://www.lifeloveandsugar.com/easy-homemade-vanilla-cupcakes-recipe/ as the Cupcake basis. Exact formula and source method are now in cupcakes.md. This supersedes statements that no formula has been chosen. Runtime stages, game timings and the preference for making versus supplying buttercream remain separate design work. No Cupcake asset/scene branch was enabled by this documentation change. Grading remains deferred.

## 2026-09-15 — Hard Cupcakes with from-scratch buttercream

User confirmed making frosting from ingredients because Cupcakes is the Hard recipe. This settles the prior prepared-versus-made frosting question. Follow the source recipe-card sequence, including separate dry blend, creaming, individual egg additions, alternating dry/milk additions, cooling, staged buttercream mixing and piping. Hard recipe difficulty is independent of tutorial versus future Challenge mode. Source mapping is in cupcakes.md; no implemented Cupcake gameplay is claimed.

## September 15 — Cupcake foundation delivery boundary

User authorized continuing the announced ingredients/equipment and staged batter milestone. Expose Hard Cupcake batter practice through the existing paper selector with an explicit partial endpoint; do not mislabel batter preparation as serving completion. Keep the selected formula and egg/dry/milk order, use existing whisk motion plus a separate scraping action as the current desktop adaptation, and defer electric mixing/full dish equipment to later work. New batter-only recipe data blocks baking. The confirmed from-scratch buttercream remains required for the completed Hard recipe; no prepared frosting or grading was introduced.

## September 15 — Cupcake tray continuation

User asked whether individual cupcake slots existed, then authorized proceeding with the tray/liners/filling/baking/cooling milestone. Implement a 4-by-3 tray with twelve separate liners, retaining the source yield. Use the existing 10-degree oven controls at 180 C (source lists 176 C), a ten-second bake and ten-second rack cooling per cupcake as prototype tuning. The next milestone remains buttercream made from ingredients, piping and a smaller serving. Do not add a grade or claim the whole Hard dish is complete.
