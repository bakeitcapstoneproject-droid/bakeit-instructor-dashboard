# BakeIT Known Issues

## Active interpretation — 2026-09-15

- **Awaiting user acceptance:** Cookies and Brownies need complete natural-input runs; focus on transfer/recovery, guide clarity, spatula/knife feel and oven-to-worktop handling. The user will perform A and send feedback before agreed Brownie finishing work C, then Cupcake planning B.
- **Implemented Brownie state:** pouring, spreading, recipe baking and five-stroke slicing into twelve visual pieces exist. Cooling and independent serving/pickup do not; they remain feature decisions, not newly found regressions.
- **Historical issues:** retain all IDs and their test evidence below, but interpret them against the [current tracker](Current%20Project%20Status.md) and [active checklist](cookie-usability-acceptance-checklist.md).
- UI-001's selection-first description is superseded: movement/whisk practice precedes selection. Cookies, Brownies and the partial Cupcake baking practice are selectable.
- MEASURE-001/002's small-chip-spoon and automatic-replacement behavior is historical. Current Cookie chips use one 1/2 Cup measure; rejected/full contents are retained, then returned to their matching source or deliberately discarded with Q.
- FLOW-002's instant restart/button wording is historical. Current restart requires a three-second R hold, with no instant HUD button.
- ENV/PREP historical board-staging instructions are superseded by ingredient rest, Utensils Area and the correct bowl in MIX.
- September 14 recorded zero final Console errors/warnings. No Console check or new runtime bug discovery occurred during this September 15 documentation-only pass.


## Brownie follow-up after 2026-09-14 visual/workflow correction

- Full keyboard/mouse end-to-end regression remains necessary, especially held-spatula reach, knife stroke feel, and the oven-to-worktop handoff. Automated callback/state rendering tests do not replace this.
- Brownie cutting now reveals twelve visual solid pieces after five strokes; independent pickup/serving and a dedicated cooling stage are not implemented.
- Reference-driven food remains stylized procedural art. Final color/presentation should be approved at normal player distance; do not treat old gray diagnostic screenshots as current output.

## GUIDE-008 Tutorial glow washed out the kitchen wall — resolved 2026-09-11

- Report: the first guided-target implementation used a pulsing point light plus a round ring, producing a large yellow bloom/scatter on the wall.
- Fix: removed tutorial Lights entirely. Guidance now draws only a thin, unlit 12-edge bounding outline around the current target, with a small expansion pulse. Yellow identifies the target/correct destination; red identifies an item left in the wrong place after 3.5 seconds.
- Verification: clean Play Mode reported 24 edge renderers for the two reusable outline boxes, zero tutorial-named Lights, one yellow Whisk target, and correct red 1/2-cup/yellow `Utensils Area` correction targets. Runtime preview roots were absent after stopping Play Mode.

## GUIDE-007 Backwards/permanent carried-object labels — resolved 2026-09-11

- Report: ingredient/tool text could face away from the centered player, lie backwards on a mat, or remain visible through closed storage.
- Fix: removed TextMesh children from carryable bodies and added direct-ray, depth-occluded screen labels. The user manually finalized remaining fixed-text orientation; those saved rotations are authoritative.
- Verification: scene audit found zero TextMesh children under Rigidbodies and direct Whisk ray hover returned `Whisk`.

## BUILD-001 Prototype scene omitted from Build Settings

- Description: `Kitchen Prototype` is open in the Editor but only `SampleScene` is enabled in Build Settings.
- Reproduction: Inspect Build Settings and compare the enabled scene with the active scene.
- Severity: Blocker.
- Affected area: Build configuration.
- Workaround: No longer required.
- Resolution: Replaced the enabled `SampleScene` entry with `Assets/BakeIT/Scenes/Kitchen Prototype.unity` and verified it is the sole enabled scene.
- Status: Resolved.
- Date resolved: 2026-09-04.

## OVEN-001 Oven controller does not resolve the oven door

- Description: The door interaction itself works, but the baking controller's serialized oven-door reference was null and its fallback search covered only descendants while the door is a sibling.
- Reproduction: Inspect the `OvenController` reference and hierarchy.
- Severity: High.
- Affected area: Oven door and baking validation.
- Workaround: No longer required.
- Resolution: Assigned the existing `FreeStove_Door` `OvenDoorController` and the existing `OvenBakeZoned` `OvenBakeZone` directly to `OvenController`. The working door behavior and prototype values were not changed.
- Status: Resolved.
- Date resolved: 2026-09-04.

## TRAY-001 Parchment receiver is missing

- Description: Parchment exists in the scene, but no `TrayLinerReceiver` is configured in the tray hierarchy.
- Reproduction: Inspect the tray hierarchy and attempt to place parchment.
- Severity: High.
- Affected area: Tray preparation.
- Workaround: No longer required.
- Resolution: Added `ParchmentPlacementZone`, its trigger collider, `TrayLinerReceiver`, and `ParchmentSnapPoint` beneath `Prop_Tray`. Play Mode verification confirmed dough is rejected before parchment, parchment snaps and stabilizes, and dough is accepted afterward.
- Status: Resolved.
- Date resolved: 2026-09-04.

## FLOW-001 Trigger-exit callbacks dereference consumed ingredients

- Description: `DoughBoardController.OnTriggerExit` and `TrayReceiver.OnTriggerExit` can receive callbacks after the related `Ingredient` has been disabled or destroyed, then dereference its transform.
- Reproduction: Move or consume ingredients through the bowl, board, and tray workflow until overlapping trigger callbacks exit after the object lifetime changes.
- Severity: High.
- Affected scene, object, and system: `Kitchen Prototype`; `DoughWorkZone` and `DoughPlacementZone`; dough transfer workflow.
- Workaround: No longer required.
- Resolution: Added destroyed-object and null guards to the board and tray trigger-exit paths. Focused Play Mode verification completed the full portion transfer without the former exception.
- Status: Resolved.
- Date resolved: 2026-09-05.

## PHYS-001 Kinematic velocity assignments produce Console warnings

- Description: Bowl, board, and parchment placement code assigns linear and angular velocity even when the affected Rigidbody is already kinematic.
- Reproduction: Move already-kinematic ingredients through the bowl, preparation board, or parchment receiver.
- Severity: Medium.
- Affected scene, object, and system: `Kitchen Prototype`; bowl, dough board, and tray-liner transitions.
- Workaround: No longer required.
- Resolution: Bowl, board, tray, and liner transitions now clear velocity only while the Rigidbody is dynamic, before making it kinematic. The focused portioning workflow completed without the former physics warnings.
- Status: Resolved.
- Date resolved: 2026-09-05.

## RECIPE-001 Bowl accepts unlimited duplicate measures

- Description: Before the cookie ingredient pass, every supported scoop was consumed and counted even after the required amount was already present.
- Reproduction: Add the same supported ingredient repeatedly before adding the final missing recipe ingredient.
- Severity: High.
- Affected scene, object, and system: `Kitchen Prototype`; `IngredientTrigger`; cookie recipe validation.
- Workaround: No longer required.
- Resolution: The bowl now maps every canonical ingredient name to its serialized requirement and rejects an extra measure before emptying the scoop or consuming a whole ingredient.
- Status: Resolved.
- Date resolved: 2026-09-05.

## FLOW-002 Cookie sequence has no reset controller

- Description: After completing or disrupting a cookie batch, the prototype has no in-world command that restores every ingredient, tool, tray, oven, and recipe state for another attempt.
- Reproduction: Complete one batch and attempt to begin a second batch without leaving Play Mode.
- Severity: Medium.
- Affected scene, object, and system: `Kitchen Prototype`; complete chocolate-chip-cookie workflow.
- Current workaround: No longer required.
- Resolution: Added `CookieRecipeSessionController`, which restarts the active `Kitchen Prototype` scene through `R` or the HUD restart button. Focused Play Mode verification confirmed that the restart removes runtime-only objects and restores consumed ingredients, bowl quantities, oven state, and the opening instruction without leaving Play Mode.
- Status: Resolved.
- Date resolved: 2026-09-06.

## INPUT-001 Desktop controllers dereference missing input devices

- Description: `FirstPersonController` and `PickupController` directly read `Keyboard.current` and `Mouse.current`. In a headless verification session with no desktop input device, their update loops repeatedly threw `NullReferenceException`.
- Reproduction: Run `Kitchen Prototype` in a headless Unity Play Mode verification environment without a registered keyboard or mouse.
- Severity: Medium.
- Affected scene, object, and system: `Kitchen Prototype`; player first-person movement, crouching, looking, and pickup input.
- Current workaround: No longer required.
- Resolution: Added keyboard and mouse availability guards while preserving normal desktop behavior when those devices are present. The repeated project-script exceptions were absent from the passing follow-up reset verification, and the final isolated compilation produced no C# errors or warnings.
- Status: Resolved.
- Date resolved: 2026-09-06.

## UI-001 Recipe HUD behaves like an action log instead of a guide

- Description: The original top-left panel replaced its content with the latest successful interaction message. It told the player what had just happened but did not preserve ingredient requirements, current progress, or the next task.
- Reproduction: Enter `Kitchen Prototype` Play Mode and perform normal ingredient and preparation actions while watching the original recipe panel.
- Severity: Medium.
- Affected scene, object, and system: `Kitchen Prototype`; runtime `BakeIT Recipe Session`; player-facing cookie guidance.
- Current workaround: Follow the Console or remember the recipe sequence while the revised guide awaits player retesting.
- Implemented correction: Replaced action narration with a state-derived checklist, now thirteen phases after the mise-en-place, final-combining, and explicit preheat additions. A short controls tutorial appears first; gathering and base ingredient rows check off individually; whisking precedes three counted chip transfers and final combining; rolling and tray transfer show counts; preheating is separate from tray loading/baking; the completed result is latched; normal progress stays out of the panel; and corrections appear temporarily without replacing the guide.
- Readability adjustments: The player's first screenshot of the staged checklist showed that the structure was appropriately compact but too small at the QHD Game-view scale. The panel, text, spacing, and controls were first enlarged to `135%`, then to `150%` on 2026-09-10. The yellow title now says the current learning part (for example `Movement Tutorial` or `Let's Practice Mise En Place`) instead of repeating the recipe. The guide waits for the new physical recipe-paper choice before counting tutorial actions.
- Technical verification: Isolated Play Mode verifiers passed the original checklist behavior, the fourth-pass rolling threshold, and the revised tutorial/chip/completion flow. The latest verifier confirmed three full-scoop visual layers, rejected premature chips, base dough on pass four, final dough on chip scoop three, and a completion state that remains latched after the oven's live flag changes. The final production compile had no C# errors or warnings.
- Status: Fix, recipe-start gate, stage headings, and 150% readability adjustment implemented; awaiting player readability, selection discovery, and obstruction retest.

## MEASURE-001 Stale scoop contents block the next ingredient source

- Description: A rejected measured ingredient remains in the shared scoop. `MeasuringScoop.TryFill` refuses every fill while non-empty, so dipping that scoop into the next required source appears to do nothing.
- Reproduction: Form base dough, retain a rejected flour or sugar measure in the scoop, then dip it into the chocolate-chip source.
- Severity: High for recipe progression because the intended small spoon could not advance to the required ingredient.
- Affected scene, objects, and system: `Kitchen Prototype`; `Small Measuring Spoon`; `MeasuringScoop`; `ChocolateChipScoopZone`; measured ingredient flow.
- Current workaround: No longer required for the technically verified path.
- Resolution: Configured `Prop_MeasuringSpoon_01` as `Small Measuring Spoon` with `MeasuringScoop`; allowed a different source to replace stale contents; added trigger-stay refill; enlarged the chocolate-chip trigger from `(0.22, 0.18, 0.22)` to `(0.30, 0.28, 0.30)`; and added a dense collider-free runtime fill for the `_01` model. A focused isolated Play Mode test passed stale-flour replacement, empty-spoon retained-overlap refill, component/collider setup, and visible chip-layer assertions.
- Status: Resolved technically; physical player retest pending.
- Date resolved: 2026-09-06.

## MEASURE-002 Chip fill does not fit the spoon and the large spoon accepts chips

- Description: The player's 21:05 screenshots show sparse chips on the large cup and an oversized floating chip pile above the small cup. The small fallback used a `0.047`-radius spread, `0.014`-wide spheres, and offsets above a `0.0375` height even though the small cup's inner rim radius is approximately `0.02152` and its top is only `0.02367` high. The larger spoon also accepted the same chip measure.
- Reproduction: Dip each measuring spoon into `ChocolateChipScoopZone` and inspect the held contents from the side.
- Severity: Medium; incorrect tool acceptance and confusing visual feedback.
- Affected scene/objects/system: `Kitchen Prototype`; small and large measuring spoons; `MeasuringScoop` and its contents visuals.
- Resolution: Explicit chip eligibility is enabled only on the small spoon. The large spoon rejects chips with corrective feedback and retains powder. Eight interleaved instances of the existing chip-cluster mesh replace the oversized sphere fallback. Mesh pivots and radial bounds are normalized to a `0.0205` radius, with final chip Y `0.010-0.028`, a shallow mound approximately `0.0043` above the rim. The small powder fill was also fitted to the cup.
- Verification: Actual pickup/source contact and retained-overlap refill passed; wrong-tool rejection preserved flour; every final chip vertex was checked; the final saved-code render was visually inspected in `evidence/small-spoon-contained-fill-2026-09-06.png`.
- Workaround: Use `Small Measuring Spoon` for chips; large spoon remains usable for flour and sugar.
- Status: Fix implemented and technically/visually verified; human handling acceptance pending.
- Date implemented: 2026-09-06. See the 21:37 completion entry in `change-documentation.md`.

## FLOW-003 Picked-up uncut dough cannot return to the board

- Description: The fourth roll renames the slab `FlattenedDough`, but placement formerly accepted only `Dough`. Detaching it also discarded preparation progress. Returning the slab then failed and the recipe could not progress.
- Reproduction: Finish rolling, pick up the uncut slab, and attempt to put it back before using the knife. Partly rolled dough also lost its progress after pickup.
- Severity: High; blocks the current recipe batch.
- Affected scene/objects/system: `Kitchen Prototype`; `MixtureResult`, preparation board, and `DoughBoardController`.
- Resolution: Accept raw and flattened uncut slabs, retain same-slab preparation state across pickup, restore board-local scale after rotation, and recover released dough during retained trigger overlap without snatching an actively held object. Off-board dough cannot be rolled or cut; portions are not treated as slabs.
- Verification: Actual `2/4` pickup/release recovery retained progress and scale; two further passes completed rolling. Actual flattened pickup/rotation/re-entry and three additional callback-driven return cycles retained `4/4` and size. Actual knife contact then produced four portions, and the recovered batch completed a perfect bake.
- Workaround: With the fix, bring the slab back onto the board; if still held while overlapping, press `E` to release it there.
- Status: Fix implemented and Play Mode verified through baking; human interaction acceptance pending.
- Date implemented: 2026-09-06. See the 21:37 completion entry in `change-documentation.md`.

## ENV-001 Cluttered counter and unusable fridge prevent mise en place

- Description/reproduction: In the supplied kitchen screenshot, the chip bowl overlaps the green board, tools/ingredients cluster at one end, fridge doors have no interaction and a broad solid collider blocks storage, and tiny oven control cubes look temporary.
- Severity: Medium; learning clarity and environment usability.
- Affected: Kitchen Prototype scene layout, fridge, prep board, tool/ingredient homes, guide and oven presentation.
- Implemented 2026-09-07: Separate work zones, brown wood board, supported ingredient/tool mats, usable existing fridge doors with typed egg/butter returns and spare-shelf loose storage, four-item setup guide, modest room finishes, rounded oven controls. Duplicate oven and old tall chip label are inactive/recoverable, not deleted.
- Verification: Ray/physics/storage/guide/rendered checks and four-cookie Perfect-bake/reset regression passed; details in the 2026-09-07 test/development entries.
- Status: Implemented and technically verified; human layout, readability, full carrying-path and gesture acceptance pending.
- Follow-up: The original four-item MIX setup was rejected by the user; use the revised board-preparation workflow in PREP-001 below. Record carrying/placement/label/door issues in the player checklist.

## ENV-002 World labels render through fridge doors

- Description/reproduction: First kitchen-preview TextMesh material displayed EGG/BUTTER and the neighboring TOOLS sign through the closed/open door surfaces.
- Severity: Low; visual clarity. Affected new kitchen/fridge labels.
- Resolution: Added depth-tested BakeIT/World Text shader and shared font-atlas material; applied to new labels. Closed/open camera renders no longer show occluded shelf/tool labels.
- Status: Resolved 2026-09-07 during visual QA. No workaround required. See the matching layout handoff entry.

## PREP-001 MIX arrows and hover detection do not represent preparation

- Description/reproduction: First pass tells the player `Egg: fridge -> MIX`, counts merely passing through a volume or consuming food, leaves dry sources on the counter, uses fixed fridge return slots and table labels. It does not stage all supplies before mixing as requested.
- Severity: Medium; instructional sequence and object organization. Scene: Kitchen Prototype; guide, preparation observer, source containers, storage and pickup.
- Correction 2026-09-07: Carry flour/sugar from sliding cabinet and egg/butter/chips from fridge; attached double-sided names; board-width increase .50 -> .70 m. Seven supplies use released, footprint-checked, collision-checked stable placement with no X/Z translation. Old fixed return slots and broad MIX trigger are disabled. All seven must be staged together; early bowl use is non-destructively blocked; clear the board before dough.
- Verification: Seven real ray pickups and actual DropObject calls stabilized supplies with zero horizontal movement; hover/occupied/high attempts did not count or snap; rotated egg re-placement preserved scale; shelf return, sliding access/obstruction, transported source scooping, ready-state/clear-board guidance, four-cookie bake and reset passed. Details in 14:15 test entry.
- Status: Implemented and technically verified. Final user carrying/placement feel and label readability acceptance pending.
- Current guidance: Lower the held item close to a free board/shelf spot and press E. If the footprint does not fit, move/rearrange it rather than expecting a fixed-slot jump. Sliding cabinet exposes one half at a time; Ctrl is the existing crouch control.

## PHYS-BASELINE-001 Existing stationary stove body uses a non-convex mesh

- Found during 2026-09-07 collider audit: the existing nested `FreeStove` has a non-convex MeshCollider on a kinematic, no-gravity Rigidbody (constraints None), with no movement script on that object. This was not added or changed by the source-storage correction.
- Severity: Low while stationary; review before making that assembly movable/VR-grabbable. No current Console warning/error or failing bake was observed.
- Status: Recorded for future physics/VR audit, not changed in this scoped request. All seven new/updated preparation carriers have zero active non-convex MeshColliders. Keep the existing stove assembly stationary.

## PREP-002 Board-staging instructions and fixed bowl conflict with workstation flow

- Reported: 2026-09-07. Severity: usability/high; Kitchen Prototype, preparation guide and bowl interaction.
- Reproduction before fix: prepare tools/ingredients on the board as instructed, although dedicated enlarged rests exist; attempt to carry the static bowl; add the third chip scoop and see finished dough immediately.
- Resolution: separate ingredient/tool/MIX surface references; movable hollow bowl with rack return; twelve-phase guide; three post-chip combining passes before dough. Technical Play Mode checks passed, including wrong-board rejection, correct rests, bowl movement, final mixing and four-cookie bake/reset.
- Status: implemented and technically verified 2026-09-07; full player usability/gesture acceptance pending. No workaround required for the verified sequence. See evening development/test entries.
- Authoring iteration: first rack migration added a second BoxCollider while StablePlacementSurface selected the disabled first one. Corrected to reuse the original box and removed only the newly added duplicate. Rack release/re-pick passed afterward; original assets remain recoverable.

## SERVICE-001 Existing Unity Connect / AI account-service errors

- Observed 2026-09-07 before edits: six token-exchange/refresh failures in Unity Editor account-service stacks. Severity: external service/editor integration; no cookie-gameplay fault established.
- Reproduction: account service refresh in the current editor session; not forced or investigated by accessing credentials.
- Status: intermittent external baseline, not fixed by this kitchen task. Tools remained usable and final fresh Play Mode/Console query returned zero errors/warnings. Re-authentication, if needed again, should be performed by the user; no account settings or credentials were changed/logged.

## TEST-HARNESS-001 Evening deterministic command mistakes (resolved)

- Some diagnostic commands guessed an incorrect script path. A first placement test attempted DropObject with no held body after a cabinet-obstructed ray; three NullReferenceExceptions came from that unsupported direct callback invocation, not the normal input branch (which checks HeldObject).
- A result lookup used IngredientTrigger/MixtureResult instead of the bowl-root sibling path and failed in the temporary command. Corrected lookup verified chips=3, final count=0 and inactive result.
- A held-spoon test used stale immediate collider bounds and an over-high release; a normal near-surface reposition and release passed. The first whisk staging point extended past the tool mat; center-based placement of all three tools passed without changing user mat sizes.
- Status: corrected tests and re-run; final fresh Play Mode Console zero errors/warnings. No permanent test script added. Historical failures are retained here rather than hidden as passing tests.

## VIS-004 Flour appears to leak through the mixing bowl

- Reported 2026-09-07; Kitchen Prototype / BowlReceiver ingredient presentation; severity medium (visible clipping).
- Reproduction: prepare supplies, transfer one flour scoop to the placed bowl, view the bowl bottom/side. Baseline flour vertices reached localY-.03148 below the bowl base. No flour Rigidbody/Collider exists: not an ingredient-loss or spilling simulation.
- Resolution: bowl-local contents anchor(0,.06,0), narrower/recentered flour and inward sugar mound; trigger and physics unchanged. Flour minimumY now .01258 and minimum interior clearance .00115. Geometry checks for all active ingredient/mixing visuals passed, as did bowl pickup/return and reset.
- Status: technically resolved 2026-09-08, pending user appearance retest. Workaround no longer required. See 11:20 development/test entry and flour-contained-final-2026-09-08.png.
- Iterations: first corrected flour height floated too high; lowered it to sit near the cavity floor. First sugar placement left9 vertices intersecting the sidewall by up to.00254m; moved it inward and rechecked zero intersections. These failed intermediate fits were not accepted as final results.

## SERVICE-002 Unity MCP executable-signature warning

- Observed at continuation start 2026-09-08: two editor connector warnings that Windows signature metadata could not be read as a certificate. No project-script error or gameplay failure; connection remained usable.
- Status: external connector baseline, not fixed or bypassed. No security/account setting changed. Final Play/Edit Console query returned zero warnings/errors; revisit connector validation if it recurs.

## VIS-005 Cookie appearance and integration iterations

- Reported2026-09-08; medium, Kitchen Prototype final dough/portions/baked materials. Original raw dough/chips looked repetitive and successful bake too dark. Six total confirmed by user.
- Implemented pale raw/golden baked variation and six uniquely decorated portions. Initial runtime variation failed because source mesh was non-readable; importer Read/Write enabled. Clone rebuild briefly produced three active chip meshes; remove-all cleanup corrected to one. Roll label initially mirrored, corrected. These failed intermediate visuals are not final evidence.
- Status: technically verified13:19 Sep8, player visual acceptance pending. No workaround needed for tested flow. See13:20 change entry and final golden PNG.

## PREP-003 Tool lands on mat but is not credited

- Reported2026-09-08; medium/high usability, Utensils Area / StablePlacementSurface. Release above old .22m threshold, let tool land, observe missing prep credit.
- Fix: .50m tool-area allowance, released collision registration and .035m tool-handle edge allowance while center stays on mat. Explicit Rigidbody/Transform synchronization avoids stale rotated bounds. No credit for held hovering or a tool centered outside the mat.
- Status: technically resolvedSep8; actual high drop became dynamic, landed, then acquired correct RestingSurface. Player retest pending. Manual near-release workaround no longer required for this tested case.

## UI-002 Ambiguous spoon tutorial / mixed preparation checklist

- Reported2026-09-08; medium, session guide. Two measuring spoons make generic pickup instructions ambiguous; rotate/scroll omitted, cold and dry ingredients mixed together.
- Fix: whisk-specific two-screen tutorial, observed rotation/scroll and settled Utensils Area gate, then cold-food and dry-cabinet substeps. Thirteen main phases and 150% size retained, at most four setup rows; the yellow heading now names the current learning part.
- Status: input/ray/placement and cold→dry transitions passedSep8; full readability and beginner acceptance pending.

## PICKUP-004 Cookie below crosshair and roll intercepts release

- Reported2026-09-08; high usability, cookie portions / PickupController. Saved HoldPoint localY=-.15; E first dispatches roll interaction even with food held. User temporarily moved roll to get past it.
- Fix: cookie center targets camera ray; other tool offsets unchanged. Roll ray does not intercept when holding an object. Existing sweep, rotation, scroll and player-collision handling retained.
- Status: technically resolvedSep8. Final viewport center(.50,.50), queued E with roll targeted placed cookie successfully, cooked-cookie replacement passed. User's saved roll relocation preserved, no workaround needed for verified case.

## LIMIT-COOKIE-001 Remaining acceptance

- Shared placement deliberately requires supported, non-overlapping footprints and mostly level surfaces; no free-form tilted shelf snapping. Nonuniform board parenting can cause small orientation-dependent scale differences; complete tray-transfer scale/placement passed, but broad extreme-rotation QA remains open.
- Runtime script recompilation is not a supported gameplay checkpoint: an intermediate domain reload cleared nonserialized tray collections. Restart after editing scripts in Play Mode. Fresh saved-code recipe/reset passed.
- Full human walk-through, natural tool gestures, door/oven carrying route, worst-case collision recovery and VR/device performance are still required before declaring the recipe complete under Codex.md.

## MEASURE-003 Unwanted sugar traps measuring utensil

- Reported2026-09-08 evening, high progression/usability, MeasuringScoop/BowlReceiver. Fill a utensil with sugar when bowl already has enough; rejection leaves it full, with no explicit return/discard. Previous different-source overwrite and early-chip deletion also lose ingredient provenance.
- Fix: explicit cup/spoon roles, deliberate held tilt into bowl or matching-source return, hold-Q discard, no silent overwrites, no deletion of rejected chips. Return blocks refill until mouth clears source. Transfer events distinguish AddedToBowl / ReturnedToSource / Discarded.
- Technical checks passed: surplus sugar rejected intact then returned; wrong-source return refused; upright contact retained contents; correct tilt transferred; Q-backed update accumulation emptied once, short tap canceled and empty repeat did not count; ledger separated return and waste. Recovered six-cookie batch baked Perfect. Focused Game-view mouse/Q usability remains to be accepted by player.
- Workaround: restart remains available but is not needed for the verified recovery paths. See evening measuring test/change entries and grading-and-waste-tracking.md. Counts are prototype units, not grams or a score.

## TEST-INPUT-002 Background Q injection limitation

- In the background editor fixture, a queued Q press did not remain active across automatic game frames; two sustained injection attempts still showed0 waste. No runtime exception or gameplay data loss occurred. A first delayed-release helper command also failed to execute.
- Corrected verification explicitly applied keyboard state before each scoop Update; dt~.00824, repeated updates crossed the .60s threshold and yielded exactly one Sugar discard. A one-update tap then release canceled with no new waste. This validates the state/timer/event path but is not a human-held-key acceptance result.
- Final manual test must focus Game view and physically hold/release Q. No permanent input settings or background-input workaround was added to the product.

## VIS-006 Underbaked branch whitens raw dough

- Reported 2026-09-09; medium visual/semantic defect in `OvenController`. Baking below `170 C` blended the base toward white, changed smoothness, and wrote bake amount even though lower temperature should produce no cookie appearance change.
- Reproduction: load a complete tray, bake at `160 C`, and compare the cookie base material before/after.
- Fix: retain the logical `UnderbakedCookie` result and feedback but skip every non-chip renderer material write for that branch. Removed the obsolete underbaked color/smoothness settings. Perfect/overcooked/burnt branches remain active.
- Status: technically resolved 2026-09-09. Six-cookie Play Mode verification used non-default pre-bake values and confirmed all remained exact; the following `180 C` regression changed the expected visual properties. Human side-by-side appearance acceptance remains recommended.

## TRAY-002 Parchment covers less than requested tray area

- Reported 2026-09-09; low/medium presentation issue in `Kitchen Prototype` / `ParchmentPaper`. Saved sheet scale `.55 x .32` covered about 76.27% X and 73.89% Z of the tray support.
- Fix: enlarged the existing sheet to `.6851 x .4115`, measuring 95.0066% X and 95.0172% Z. No tray, roll, collider, receiver, or placement-logic change.
- Status: technically resolved 2026-09-09. Enlarged paper and six-cookie placement passed in Play Mode; subjective rim fit remains for normal Game-view review.
- Reopened by player 2026-09-09: the 95% footprint looked too large and made free-position tray placement impractical. Retuned to approximately 90%, increasing the tight-axis centering tolerance while retaining broad coverage. A `25 mm` X / `12 mm` Z off-center release and six-cookie load passed; subjective fit remains for player acceptance.
- Reopened again by player 2026-09-09: 90% precise placement remained difficult. Retuned to approximately 86% and added a parchment-only nearby magnetic center/alignment fallback with tunable `.15 m` horizontal and `.35 m` vertical reach. Precise, assisted, rejection, six-cookie, and tray-movement tests passed; human placement feel remains for player acceptance.

## INTERACT-005 One whisk or roll can count multiple passes

- Reported 2026-09-09; high interaction/progression issue in `BowlReceiver` and `DoughBoardController`. Whisk travel retained excess distance behind a short `.15 s` timer, while rolling used an uncapped `while` loop with no cooldown; one gesture could therefore advance multiple logical passes.
- Reproduction: move the whisk rapidly through the bowl or move the rolling pin a large distance through the dough work zone and watch the pass counter.
- Fix: both interactions now use `.35 s` serialized cooldowns, reset travel after a valid count, discard travel during cooldown, and accept at most one count per movement sample.
- Status: technically resolved 2026-09-09. Immediate repeats stayed at one; fresh separated strokes completed base `4/4`, chip `3/3`, and rolling `4/4` with correct base-dough/recipe/flattened states. Human gesture-feel tuning remains open.
- Reopened by player 2026-09-09: `.35 s` still felt too fast. Both saved/default cooldowns are retuned to `1.2 s`. A `0.65 s` repeat stayed blocked and post-interval strokes completed `4/3/4`; human fluidity acceptance remains pending.

## TOOL-006 Knife points toward crosshair while held

- Reported 2026-09-09; medium usability/orientation issue in the dough-portioning knife pickup pose.
- Cause: the scene-specific `(90,0,0)` held rotation maps the model's long local `-Z` blade axis upward toward the crosshair from the lowered tool hold point.
- Fix: change only the knife pickup pose to `(0,180,0)`, mapping the blade axis forward from the camera while preserving right-click rotation and all cutting behavior.
- Status: technically resolved. Play Mode measured local `-Z` blade direction exactly aligned with camera forward (`dot=1`) and the held knife created six portions; subjective first-person appearance remains for player acceptance.

## ARCH-001 Remaining recipe orchestration is cookie-specific

- Recorded 2026-09-09; architecture limitation, not a current cookie defect. Recipe facts now live in `RecipeDefinition`, but `CookieRecipeSessionController` still owns a thirteen-stage cookie sequence; `MiseEnPlaceStation`, ingredient visuals, chip decoration, and some feedback are also cookie-specific.
- Impact: a new brownie or cupcake asset alone will not create a complete playable recipe. Reusing those mechanics without an approved workflow could silently apply cookie assumptions.
- Current state: the approved Brownie workflow now has explicit preparation, measuring, mixing, pan, spreading, and oven branches driven by `Brownies.asset`; it stops before cooling and slicing. Cookie-specific dough-board, chip decoration, and portion/tray behavior remain deliberately separate.
- Status: partially reduced, still open by design. Generalize cooling/slicing only after the user approves the corresponding Brownie presentation and workflow, and generalize Cupcakes only after its recipe is approved. Do not invent icing, yeast/leavening, scoring, or stages from the provisional guide.

## OVEN-002 Preheat presentation awaits player acceptance

- Recorded 2026-09-09; acceptance limitation rather than a known logic defect. The oven now begins at `0 C`, requires the player to choose a target, then heats to that target over `5 s`; it exposes `PREHEAT`/`HEATING`/`READY`/`BAKE` states and retains its light when heated.
- Technical verification passed the complete state and bake lifecycle, including the negative gates and six-cookie Perfect result. The remaining question is whether five seconds feels natural and whether the longer control labels and retained light are clear in the normal Game view.
- Status: technically implemented; await player feedback before retuning timing or presentation. No workaround is required.

## EDITOR-003 One-time Unity Editor crash after brownie equipment pass

- Reported 2026-09-10; low current severity because no saved project data was lost. The Editor crashed once after the brownie bowl/pre-melted-butter work; no reproduction sequence, crash log, or recurring symptom was supplied.
- Recovery check reopened `Kitchen Prototype` cleanly and confirmed the new bowl/cup/components plus the user's tray, pan, and spatula transforms. Console contained zero errors/warnings and `CRASH_RECOVERY_AUDIT PASS=True`.
- Status: open for monitoring, not reproduced. Current workaround is simply reopen Unity; investigate crash logs only if it happens again or a reproducible action emerges.

## MEASURE-004 Exact measuring set and storage capacity

- Reported/approved 2026-09-10: the expanded cookie formula needs distinct cup/teaspoon capacities, and the previous small countertop tool zone cannot clearly store the whole set. Ingredient labels also duplicated text on both sides and looked detached from their models.
- Final fix after 2026-09-11 feedback: exact 1 cup, 1/2 cup, 1/4 cup, 1 teaspoon, 1/2 teaspoon, and 1/4 teaspoon vessels remain. The 1/2 Cup now handles chips; the dedicated scoop/hook was removed. The staging-area enlargement was moved from `Utensils Area` to `Tool Home Mat`, and carried-object markings were replaced by hover labels.
- Validation: Play Mode accepted the 1/2-cup/chip pairing, direct Whisk hover with empty hands, complete hover-name suppression while holding an item, tutorial gate, and rack/layout invariants. Final scene audit: 6 measuring vessels, 7 hooks, 0 carryable-body TextMesh, stopped Play Mode, clean scene, and zero Console warnings/errors.
- Status: technically resolved. Normal-player readability, reach, and preferred tool spacing remain for subjective acceptance.

## INTERACT-007 First-time held-object recovery and scooping precision

- Reported from the 2026-09-11 teacher playtest. A snagged held item could remain far away until the player looked sufficiently clear of the obstruction, and an accidentally large scroll distance made recovery harder. Flour/Sugar collection also demanded more precise cup placement than a first-time player expected.
- Fix: halve the stuck-gap trigger from `.15 m` to `.075 m`; move a recovered item only one `.20 m` scroll step toward the player so their chosen reach is preserved; and add a `.045 m` compatible-source assist around the opening of an empty, upright, held measuring cup.
- Status: technically resolved and regression-tested. Normal human acceptance remains useful, particularly to confirm that `.045 m` feels forgiving without creating accidental scoops.

## UI-008 Thin preparation mats fill yellow when guided

- Reported from player screenshots and confirmed again during the teacher-playtest review. Expanding the back faces of a very thin mat could reveal a solid yellow underside instead of a clean outline.
- Fix: preparation rests use a dedicated four-edge perimeter line. Loose tools and ingredients retain the close mesh silhouette. Guidance was also strengthened to `9.75 ± 1.25` pixels for objects and `.015–.021 m` for pad perimeters without adding lights.
- Status: technically resolved and structurally verified; await normal Game-view acceptance.

## ASSETGEN-001 Unity reference-image ID rejection

- Reported 2026-09-11; low workflow severity, Unity Asset Generation integration. The installed image-conditioned Tripo P1 model rejected both the imported Sugar reference Texture2D ID and its Sprite ID as nonexistent. Prompt-only generation was also refused because that model requires a reference image.
- Impact: no generation started and no model-generation usage was consumed, but the hosted 3D path could not produce the requested Sugar Jar from the supplied image in this Editor/package version.
- Current workaround: use a local, rerunnable procedural Unity asset builder. `SugarJarAssetSetup` produced and integrated the Sugar Jar successfully without changing ingredient mechanics.
- Status: open tool/package compatibility issue; product work is not blocked. Retry the hosted path only if the Unity AI package/reference-ID handling is updated or a later asset specifically benefits from it.

## TOOL-008 Dropped measuring tools blend into the floor

- Reported from the teacher playtest on 2026-09-12. The six measuring vessels shared a neutral gray material close to the floor value, so a dropped teaspoon could appear lost even when it remained physically reachable.
- Fix: all measuring-tool shells now use a saturated warm-copper, metallic, non-emissive finish. Ingredient contents and all measuring interactions are unchanged.
- Related navigation fix: the five work areas now carry permanent large surface names and enlarged fascia labels, so their identity does not depend on an active tutorial outline.
- Status: technically resolved through structural and rendered visual checks; human in-game acceptance remains pending.

## TOOL-009 Wall-rack return is difficult to discover and execute

- Reported from the teacher playtest on 2026-09-12. The mixed rack was labeled `MEASURING TOOLS` despite also storing the Whisk, and returning a held item required bringing its pivot within only `.34 m` of the exact hook.
- Fix: rename the rack `BAKING TOOLS`, add an explicit aim-and-E return hint, expand the physical snap radius to `.55 m`, and accept crosshair aim within `.22 m` of the matching hook from up to `2.5 m` away.
- Status: technically resolved. All seven correct returns, wrong-hook rejection, off-aim rejection, and recovery to the correct hook passed in Play Mode; human in-game discoverability and preferred tolerance remain for acceptance.

## ART-010 B1 radial Brownie surface was culled in Play Mode

- Discovered 2026-09-13 during isolated visual acceptance of the selected B1/B2/A3 Brownie art.
- Reproduction: enable the first B1 radial mesh with the default URP Lit back-face culling; only its walnut children render from above.
- Severity: medium presentation issue; gameplay state remained correct.
- Fix: the shared raw Brownie food material explicitly uses `_Cull = 0`. The mesh remains collider-free and its upward normals are retained.
- Status: resolved 2026-09-13. Isolated rendering and final recipe-state Play Mode capture both show the full dark B1 surface.

## BROWNIE-011 Premature, black, hollow, and floating batter presentation

- Reported 2026-09-13 from the first B1/B2/A3 in-game review. A single Sugar transfer exposed the full near-black surface; Walnuts appeared before the finishing whisk passes; open meshes looked hollow; and transferred B2 batter floated above the liner.
- Fix: drive B1 scale from ingredient progress, reveal raw details only at batter completion, use warm/dark brown colors, close B1/B2/A3 geometry, replace heart-like Walnut pairs with irregular three-lobe clusters, and lower B2 to a `.0024 m` liner gap.
- Status: resolved technically. Final Play Mode progression/geometry assertions passed; player aesthetic acceptance remains pending.

## PAN-012 Brownie parchment detached and lacked side coverage

- Reported 2026-09-13. The loose flat sheet could react physically when the deep pan was picked up and did not line its walls.
- Fix: freeze and parent the accepted source sheet, disable its colliders/flat renderer, and show a fitted bottom-plus-four-sides liner owned by the pan. Reset fully restores the source sheet.
- Status: resolved. Five-piece coverage, moved-pan stability, and reset passed in Play Mode.

## UI-013 Ingredient names and malformed Baking Soda label

- Reported 2026-09-13. Several sources lacked Flour-like visible names and the Baking Soda carton rendered stacked, duplicated text.
- Fix: add compact depth-tested package-front nameplates to the ambiguous ingredient sources and rebuild the dedicated Baking Soda front/back wordmarks at `.00235` character size with corrected rotations.
- Status: resolved structurally and visually. Eggs remain hover-named and visually recognizable rather than carrying artificial attached plaques.

## OVEN-014 Null teardown in `OvenBakeZone.OnTriggerExit`

- Found in the pre-change Console as 12 repeated entries when both tracked and exiting bakeware references were null.
- Fix: guard the tray and Brownie-pan comparisons before accessing their transforms.
- Status: resolved; final Play Mode and Console checks were clean.

## UI-015 Cocoa and Baking Powder names were buried in the front artwork

- Reported 2026-09-13. The generic name objects existed, but their local Z positions placed them behind the cans' raised badges and emblems.
- Fix: move both complete plates beyond the outer decorative bounds and enlarge their plates/type while retaining depth-tested text.
- Status: resolved; isolated render and bounds-based Play Mode verification passed.

## ART-016 Pantry contents lacked readable material surfaces

- Reported 2026-09-13 for Flour and Sugar; the first Walnut rework also remained too smooth and coffee-bean-like.
- Fix: add contained powder/granule mound geometry to Flour and Sugar. Replace the Walnut source with 34 golden crinkled pieces using three shared deformed, flat-shaded meshes and embedded folds/creases; update Brownie Walnut clusters to seven uneven lobes.
- Status: resolved technically. Visual and collider/source-identity checks passed; subjective player acceptance remains pending.

## FEEDBACK-20260915 — Player A findings and bounded corrections

| ID | Report / reproduction | Severity | Affected system | Workaround | Current status |
|---|---|---|---|---|---|
| FB-01 | Butter package appears flipped when held/prepared | Medium | Food_Butter held pose/letter geometry | Rotate manually | Resolved technically September 15: pose and mirrored lettering corrected; held render/pickup passed |
| FB-02 | Dry storage cramped; Vanilla intersects wall/tilts | Medium | Dry cabinet, pantry props | Reposition items | Resolved technically September 15: taller storage, upright aligned roots and organized rows; rendered check passed |
| FB-03 | E sometimes fails on visibly targeted Baking Soda | High | Baking Soda pickup collider | Aim/move until collider hit | Found displaced visual vs root collider; corrected and visible-body ray pickup passed in Play Mode |
| FB-04 | Chip-transfer row names half-cup tool without ingredient | Medium | Cookie guide | Read yellow heading | Resolved September 15: row names Chocolate chips and quantity; active recipe assertion passed |
| FB-05 | Melted butter model disliked; name backwards/below ground | Medium | Melted Butter Cup | Rotate/reposition | New ivory jug with readable raised label and corrected held pose; final render/pour checks passed; art acceptance pending |
| FB-06 | Brownie batter appears above/over the rim | Medium | BrownieFoodVisual bowl fill | None | Resolved technically September 15: lower/narrower fill; vertex containment and render passed |
| FB-07 | Guide demands returning completed Brownie bowl to MIX | Medium | Session and highlight direction | Put bowl back | Resolved September 15: MIX gate applies only before batter completion; held completed-bowl check passed |
| FB-08 | Final walnut layout looks regular | Low | Brownie cooked/cut art | None | Per-batch scatter and cut-face variation implemented and rendered; art acceptance pending |

- Evidence: evidence/player-feedback-2026-09-15-1.png through -8.png.
- New feature requests tracked separately: smaller serving, recipe-selection return and guided-only green ingredient rows. Grading remains deferred per user.
- Implementation and test details: [September 15 development entry](change-documentation.md), [test log](test-log.md), [serving system](serving-and-plating-system.md). Natural player retest remains open for all corrected interactions.
- Test-driver failures: synthetic input initially targeted the wrong input-state buffer; corrected to queued input/update. A transfer fixture put its pour origin below the receiver; corrected to collider center. These do not establish player-control failures.

## September 15 follow-up playtest — pouring, bowl and serving

| ID | Report | Severity/system | Resolution and verification |
|---|---|---|---|
| FB-09 | Melted-butter transfer looks abrupt and too small | Medium / PourableIngredient | Replaced .35-second burst with 1.6-second continuous stream and draining contents. Pause/resume and exactly one full-cup completion passed in Play Mode. Tilt threshold 55 degrees; pour above bowl. |
| FB-10 | Batter clips through the tapered bowl wall despite clearing rim | Medium / bowl visual | Fit narrowed/raised to (.075,.018,.075) at Y=.073. Side and overhead renders confirm containment; initial rim-only acceptance was insufficient. |
| FB-11 | Walnut measure lacks recognizable falling pieces | Medium / MeasureTransferVisual | Uses 24 tumbling existing kernel meshes over about one second; accepted walnuts show before folding. Chunk presence and rendered fall checked. |
| FB-12 | Bowl remains bright/highlighted after batter transfer | Medium / bowl material and tutorial target | Dedicated matte, non-emissive green material; transferred bowl skips earlier preparation targets and stale correction. Empty-bowl target check passed. Normal crosshair hover is separate. |
| FB-13 | Served brownies intersect plate; four-piece layout disliked | High / ServingPlateReceiver | Two/three stack choice, measured central-well height, local main-mesh bounds and Rigidbody pose sync after plate movement. Two/three completion, removal/replacement, overlap bounds and loaded-plate pickup/rotation/set-down passed. Initial rim-based correction floated; corrected before final save. |

Resolved technically September 15; subjective pouring/stack feel remains for natural player retest. Evidence: player-followup-2026-09-15-1..4 and feedback-followup-2026-09-15 PNGs. See the follow-up development/test entries.

## September 15 — Cupcake foundation boundary

Cupcakes now exposes a deliberately partial batter-practice branch. Full gathering/preheat/liner setup, electric-mixer interaction, filling, baking/doneness, cooling, from-scratch buttercream, piping and serving remain unimplemented. The current whisk pass counts are desktop adaptations. This does not fulfill the complete Hard dish or add grading. Natural keyboard/mouse testing is still required for reaching the milk/oil openings, retrieving/replacing the Dry bowl and scraper feel. Controlled integration checks do not establish those as fully accepted.

During foundation QA, an egg fixture initially searched only children of the Cupcake root; normal pickup reparents objects and invalidated that test lookup. The fixture now finds live Cupcake-labelled eggs. Render inspection caught oversized new labels and a test-only upward-facing pour pose that intersected the bowls. Text was resized and the fixture corrected to tilt the pouring lip down above the destination; final evidence replaces the initial Cupcake captures. These fixture/presentation findings do not change prior Cookie/Brownie acceptance status.

## September 15 — Cupcake tray boundary (current)

The tray/liners, preparation gate, individual gradual fills, timed baking, centre tester and rack cooling are now implemented, superseding the foundation's pending list. Remaining: full gathering stage, electric mixer, from-scratch frosting, piping, smaller serving and natural input/art acceptance. Cooling is a fixed timer rather than a thermal model; the 180 C target and ten-second bake are prototype adaptations. The new 3D meshes use simple materials and require user art acceptance. No full manual player/VR pass is claimed.
