# BakeIT Test Log

## 2026-09-14 — Brownie preparation, pan spreading, and slicing

- Additional cooked-detail request verified: 28 partially buried top kernels and 60 embedded cut-face fragments across the twelve squares (72 MeshFilters including the 12 brownie bodies). Each square receives a different procedural crust seed, with object-space texture coordinates; moving the tray preserves its seed and scale. Final grid and isolated cut-face renders were visually inspected. Reset passed again; final Editor state is not playing, scene dirty=false, temporary QA objects=0, Console errors=0/warnings=0. C#/shader whitespace checks passed (Unity-generated YAML retains its normal blank-field spacing).

- Unity 6000.5.0f1, Kitchen Prototype; editor-only `BrowniePresentationQA` harness, temporary Play Mode state.
- Public transfer methods accepted both Sugar cups, both Flour measures, Cocoa, Vanilla, BakingPowder, Salt, four Eggs, and MeltedButter. Before whisking, `HasAllBaseIngredients=true`, mix passes=0, batter object inactive. Render confirmed separate ivory/white powder piles, gold butter, intact yolks, brown cocoa and amber vanilla at the bowl bottom.
- Whisk-trigger callbacks progressed four base passes; half-cup accepted Walnut kernels; two finishing callbacks completed batter. Walnut scoop has shared kernel meshes and no visual physics bodies. Raw brown rendering verified, not just material color assertions.
- Dispensed Parchment accepted by the pan; held tilted bowl transferred completed batter and cleared the bowl. Four spatula callbacks advanced 1/2/3/4. Actual pickup orientation dot(outward blade, camera forward)=1. Pan stages visually inspected from clump to full rectangular coverage.
- Perfect-bake presentation was invoked directly (the oven timing loop was NOT rerun). Five synchronized held-knife callbacks progressed 1 through 5 and `IsSliced=true`. Final generated visual has twelve pieces; pan footprint aspect=1.333333, so the 4x3 cut cells are square. Reset returned batter/parchment/bake/sliced flags to false.
- Final art snapshots: `brownie-qa-final-spread-0.png` through `-4.png`, `brownie-qa-final-squares.png`. These final snapshots explicitly stage presentation states, separate from interaction checks. Earlier diagnostic PNGs document rejected gray renders and intermediate art, not the final result.
- Harness corrections: imported Reflection was rejected by the command tool (not used); initial measure positioning required a transform/physics sync; paused/background clock initially prevented cooldown progression; holdPoint is a runtime field, so the harness now finds the camera child; released knife bodies need dynamic state before test pickup. Builder asset-name warnings were resolved. Several tool calls overlapped assembly reload and were retried.
- Scope of pass: automated gameplay acceptance/callbacks plus rendered visual states. Not a full keyboard/mouse tutorial-to-oven-to-cut playthrough. Cooling, independent serving/pickup of cut squares, and player-distance art approval remain open.

## 2026-09-04 21:39 +08:00 Git ignore validation

- Unity version: 6000.5.0f1.
- Scene tested: Not applicable.
- Feature tested: Root Unity `.gitignore`.
- Test steps: Ran `git check-ignore` for representative generated directories, local files, scripts, scenes, `.meta` files, package files, project settings, and documentation; inspected `git status --short`; counted trackable files and checked their sizes.
- Expected behavior: Generated and local-only content is ignored while reconstructable Unity source remains trackable.
- Actual behavior: All representative paths matched the intended policy. Git reports 1,359 initial trackable files, no files over 50 MB, and a largest file of approximately 4.51 MB.
- Result: Pass.
- Console errors and warnings: Not applicable.
- Screenshots or recordings: None.
- Bugs discovered: None.

## 2026-09-04 22:01 +08:00 Initial Git staging dry run

- Unity version: 6000.5.0f1.
- Scene tested: Not applicable.
- Feature tested: Unity ignore rules and line-ending policy during initial staging.
- Test steps: Ran `git add --dry-run .`; searched its combined output for warnings, errors, fatal failures, `Temp`, and `.plastic`; checked representative ignore rules with `git check-ignore`; checked Unity asset attributes with `git check-attr`.
- Expected behavior: Git can enumerate the complete initial commit without opening Unity's locked temporary files, local Plastic metadata, or producing CRLF warnings.
- Actual behavior: Dry run exited successfully over 1,355 files. No problem lines were detected. `Temp` and nested `.plastic` paths are ignored, and Unity `.asset` files resolve to LF.
- Result: Pass.
- Console errors and warnings: None.
- Screenshots or recordings: User supplied a screenshot of the pre-correction Git client failure.
- Bugs discovered: The prior root-only `.plastic` pattern did not match `BakeITCapstoneProject/.plastic`; corrected during this change.

## 2026-09-04 22:34 +08:00 Confirmed prototype defect verification

- Unity version: 6000.5.0f1.
- Scene tested: `Assets/BakeIT/Scenes/Kitchen Prototype.unity`.
- Features tested: Build Settings, parchment-first tray preparation, parchment snapping and stabilization, flattened-dough acceptance, oven-door and bake-zone references, saved-scene state, and final Console state.
- Test setup: A temporary Editor-only Play Mode verifier created disposable parchment and flattened-dough test objects. The verifier and all of its source and metadata files were removed after the run; runtime objects were discarded on Play Mode exit.
- Test steps: Confirmed flattened dough is rejected before parchment; placed `ParchmentPaper` in `ParchmentPlacementZone`; waited for physics and verified it snapped, became stable, and parented to the tray; placed flattened dough in the existing dough zone and verified acceptance; inspected `OvenController` references and the existing door's closed state; exited Play Mode; refreshed and recompiled; ran a final serialized-reference and Build Settings invariant check.
- Expected behavior: The tray requires parchment before dough, accepted objects remain stable, the existing oven door and bake zone are assigned to the baking controller, the active scene is saved and is the sole enabled build scene, and no final Console errors or warnings remain.
- Actual behavior: All sequence checks passed. The verifier logged `PASS` for dough rejection before parchment, parchment snap/stability, dough acceptance after parchment, and oven integration with door closed state `True`. Final invariants reported `valid=True`, one liner receiver with a placement point, non-null oven-door and bake-zone references, a clean saved scene, and only `Kitchen Prototype` enabled in Build Settings.
- Result: Pass.
- Console errors and warnings: Final Unity MCP Console query returned 0 errors and 0 warnings.
- Failed attempts or corrections: The first background Play Mode probe did not advance enough frames, so the disposable state-machine verifier was used. Its first compile used deprecated object-finding calls; these were replaced before the passing run. A dynamic verification command initially attempted to access private fields directly and failed compilation; it was rerun successfully through `SerializedObject`. MCP temporarily disconnected during the verifier's domain reload and reconnected after compilation. Unity's MCP deletion command could not remove the temporary source without UI interaction, so the known verifier files were removed directly and Unity was refreshed.
- Bugs discovered: None beyond `BUILD-001`, `OVEN-001`, and `TRAY-001`; all three are resolved by this change.

## 2026-09-04 22:34 +08:00 Manuscript alignment verification

- Artifact tested: `BakeITCapstoneManuscript.docx`.
- Changes verified: Approved prototype recipes now read chocolate-chip cookies, brownies, and cupcakes; the implementation version now reads Unity 6.
- Structural checks: The final DOCX opens successfully with 763 paragraphs, 13 tables, 1 section, and both outdated phrases absent.
- Render checks: Exported the edited document to PDF through Microsoft Word, rendered all 110 pages to PNG, visually inspected the changed pages and page groups around them, and scanned every page for blank output and content touching the page edge. Page count remained 110; no blank pages or edge collisions were detected.
- Result: Pass.
- Failed attempts or corrections: The packaged renderer could not start because LibreOffice was not installed. Microsoft Word's local PDF export was used as the equivalent rendering fallback, followed by Poppler page rendering and image inspection.

## 2026-09-05 17:35 +08:00 Chocolate-chip-cookie dough portioning verification

- Unity version: 6000.5.0f1.
- Scene tested: `Assets/BakeIT/Scenes/Kitchen Prototype.unity`.
- Features tested: Rolling completion, invalid early cutting, proportional yield, equal portion size, combined-volume preservation, portion pickup components, parchment-first complete-batch loading, oven readiness, and bake-result propagation.
- Test setup: A temporary Editor-only Play Mode verifier created disposable reference and scaled dough batches and drove the existing trigger methods after confirming the test colliders physically overlapped. Background Play Mode was stepped explicitly because an unfocused Unity Editor does not advance reliably. The verifier, its `.meta`, and the temporary `Assets/BakeIT/Editor` folder metadata were removed after testing.
- Test steps: Verified that the knife could not portion an empty board; placed dough and completed three rolling passes; passed the knife through the board zone; checked each generated portion's batch metadata, Rigidbody, enabled collider, size, and combined rendered volume; placed parchment and every reference portion on the tray; confirmed complete-batch readiness; placed the tray in the bake zone; started a six-second perfect-temperature bake; checked every result name; repeated portion generation with a 1.25-times scaled source batch.
- Expected behavior: The reference dough produces four equal, independently pickable cookies; a larger dough quantity produces proportionally more equal cookies; total represented volume is preserved; incomplete or unprepared trays cannot bake; and a completed bake updates the full batch.
- Actual behavior: The reference batch produced four portions with combined volume `0.002505` from source volume `0.002505`. The scaled batch produced eight portions with combined volume `0.004893` from source volume `0.004893`. The complete four-cookie batch snapped to parchment, the oven accepted it, and all four results became `BakedCookie`.
- Result: Pass.
- Console errors and warnings: The passing verifier completed without feature exceptions or the former kinematic-velocity warnings. After temporary-test removal and final YAML cleanup, Unity completed a full Tundra compilation and assembly reload successfully with no C# errors or warnings in the Editor log. The final Unity MCP Console query returned 0 errors and 0 warnings.
- Failed attempts or corrections: The first verifier attempt did not advance enough background frames. Explicit `EditorApplication.Step()` calls fixed the rolling and bake timing. A later tray callback arrived too late in the automated run, so the harness first proved physical bounds overlap and then dispatched the same trigger callback deterministically. A final static check initially matched `DoughPortioningTool` while looking for saved runtime `DoughPortion` components; the assertion was narrowed to the exact component identifier and passed with zero saved runtime portions. The first Editor-log assertion assumed the Tundra success line followed the forced-reload line, but Unity logs them in the opposite order; checking the last successful build followed by the successful assembly reload passed with zero C# diagnostics. Two direct MCP probe calls used the display tool name instead of its registered transport name and logged tool-not-found errors; those self-created probe entries were cleared before the successful final 0/0 Console query. Earlier failed verifier attempts were superseded by the final passing run.
- Bugs discovered: `FLOW-001` and `PHYS-001` were reproduced before the change and resolved in this pass.

## 2026-09-05 23:03 +08:00 Chocolate-chip container and first-pastry verification

- Unity version: 6000.5.0f1.
- Scene tested: `Assets/BakeIT/Scenes/Kitchen Prototype.unity`.
- Features tested: Chocolate-chip source and label, shared-scoop visual switching, exact five-ingredient validation, rejection without consumption, mixing, chip preservation, proportional portioning, parchment-first tray loading, oven readiness, and multi-cookie bake results.
- Test setup: A temporary ignored dynamic command entered a fresh Play session and deterministically dispatched the same private trigger callbacks used by the runtime interactions through Unity `SendMessage`. Assertions inspected public state and serialized references without adding any persistent test component or scene object.
- Test steps: Filled the shared scoop from `ChocolateChipScoopZone`; transferred one measure to the bowl; attempted duplicate chips and unsupported salt; added flour, sugar, butter, and egg; attempted mixing before the final ingredient; completed four valid whisk passes; rolled and portioned the dough; checked the four reference portions for chip markers and unwanted chip colliders; prepared the tray with parchment and the complete batch; loaded the bake zone; ran a perfect bake at `180 C`; and compared chip colors before and after baking.
- Expected behavior: Chip contents are visually distinct from powder; the bowl accepts exactly one of each required ingredient without consuming invalid extras; incomplete dough cannot mix; the reference batch yields four equal chip-bearing portions; the tray requires parchment and the full batch; all portions become `BakedCookie`; and decorative chips neither inflate yield nor receive the dough's bake color.
- Actual behavior: Every assertion passed. The chip source selected `ChocolateChipContents`; the bowl recorded one visible chip measure; duplicate, unsupported, and post-completion inputs stayed in the scoop; the exact recipe completed after four whisk passes; the reference batch remained four portions with one marker and no chip collider per portion; the prepared tray baked successfully; all results were `BakedCookie`; and chip colors were unchanged.
- Result: Pass.
- Saved-state checks: After leaving Play Mode, `Kitchen Prototype` was active and clean. `ChocolateChipContainer` remained at the intended scene position with unit root scale; all source, scoop, bowl, mesh, material, and prefab references resolved; decorative chip visuals had zero colliders and zero Rigidbodies.
- Console errors and warnings: The final correctly filtered Unity MCP Console query returned 0 errors and 0 warnings. The final Editor-log window contained 0 C# error or warning diagnostics.
- Visual QA: The first scene capture exposed an oversized source and reversed label. After correction, the final view showed a compact, readable labeled bowl beside the preparation board without covering its work surface.
- Failed attempts or corrections: Reflection was disallowed in Unity's dynamic-command sandbox, so the verifier was rewritten around `SendMessage` and `SerializedObject`. The prefab root was normalized after its first save retained scene transforms. A Game-camera capture framed geometry instead of the cookies, so the scene-view inspection was used for final container layout QA. Tool-generated and authentication-related Console entries from setup were cleared before the final clean query.
- Bugs discovered: `RECIPE-001` allowed unlimited duplicate measures; it was reproduced by code review and resolved before the passing run. `FLOW-002`, the lack of a reset controller, remains open as a prototype usability limitation.

## 2026-09-06 10:03 +08:00 Manuscript realignment and render verification

- Artifact tested: `BakeITCapstoneManuscript.docx`.
- Alignment tested: `Codex.md` authority, the corrected Unity 6 implementation guide, approved recipe scope, implementation-facing wording, current three-objective research design, Appendix H research matrix, instruments summary, statistical treatment, and security-claim boundaries.
- Structural checks: The final DOCX opens successfully with 763 paragraphs, 13 tables, and 1 section. Searches found no remaining `Unity 2022 LTS`, `pandesal`, or obsolete `TLS 1.3, KMS encryption` claim.
- Render checks: Rendered the final DOCX to PDF and 103 page PNGs. Visually inspected every page through contact sheets, then inspected every changed Appendix H page at full resolution. Automated comparison and image checks found no blank pages or content touching a page edge.
- Corrections made during QA: The first render exposed inherited negative paragraph indents and missing cell formatting in appended Appendix H rows. The tables were reformatted with consistent borders, type, margins, automatic row height, and non-splitting data rows; the final render has no overlapping or borderless table content.
- Result: Pass.
- Console errors and warnings: Not applicable.
- Screenshots or recordings: Temporary render pages and contact sheets were used for inspection and removed after verification.

## 2026-09-06 10:03 +08:00 Cookie session reset and feedback verification

- Unity version: 6000.5.0f1.
- Scene tested: `Assets/BakeIT/Scenes/Kitchen Prototype.unity` in an isolated verification copy.
- Features tested: Automatic recipe-session bootstrap, player-facing feedback event propagation, `R`/button scene reset, removal of runtime-created objects, restoration of saved ingredient state, cleared bowl quantities, incomplete bowl state, idle oven state, and restoration of the initial instruction.
- Test setup: A temporary Editor-only verifier was added only to an isolated project copy, executed in Play Mode, and removed from that copy after the passing run. No verifier code or test scene object was added to the production project.
- Test steps: Opened `Kitchen Prototype`, entered Play Mode, confirmed the session controller existed, emitted a feedback event and checked the displayed message, disabled the saved `Butter` object, created a runtime-only marker, invoked `RestartRecipe`, waited for the scene reload, and asserted the marker, ingredient, bowl, oven, and instruction states. Removed the verifier and ran a final production-script batch compilation from a clean isolated copy.
- Expected behavior: A restart reloads the saved active scene in Play Mode, clears all cookie workflow state, removes runtime objects, restores scene objects, and returns the HUD to its initial instruction without duplicating reset logic across interaction systems.
- Actual behavior: The verifier logged `BAKEIT_RESET_VERIFIER_PASS`; its runtime marker was removed, saved `Butter` returned active, all bowl quantities returned to zero, the bowl was incomplete, the oven returned to `Idle`/`None`, and the initial instruction was restored. A final isolated production compile exited successfully with no C# errors or warnings.
- Result: Technical pass. Subjective pickup reach, label readability, gesture feel, and HUD comfort remain a human acceptance item under `cookie-usability-acceptance-checklist.md` and are not represented as completed.
- Failed attempts or corrections: The first headless run exposed existing null assumptions for `Keyboard.current` and `Mouse.current`; `FirstPersonController` and `PickupController` were guarded and the verifier was rerun successfully. One UnityEditor Search indexing exception occurred only while the isolated clone initialized; it did not originate from project scripts and did not recur as a C# compile diagnostic.
- Bugs discovered: `FLOW-002` and `INPUT-001`; both were resolved by this change.

## 2026-09-06 10:17 +08:00 Player recipe-guide usability test

- Unity version: 6000.5.0f1.
- Scene tested: `Assets/BakeIT/Scenes/Kitchen Prototype.unity`.
- Feature tested: Original top-left cookie feedback panel.
- Test steps: Entered Play Mode and performed normal cookie actions while using the panel for guidance.
- Expected behavior: The panel should show remaining preparation requirements and guide the player through the recipe without obstructing the workstation.
- Actual behavior: The panel behaved like an action log, repeatedly describing the current or most recent action. It did not retain a preparation checklist or clearly communicate the next stage and was more confusing than it appeared initially.
- Result: Fail.
- Console errors and warnings: Not reported by the player; the failure concerned guidance design rather than a reported runtime exception.
- Screenshots or recordings: None supplied.
- Bugs discovered: `UI-001`.

## 2026-09-06 14:57 +08:00 Staged cookie-guide and four-pass rolling verification

- Unity version: 6000.5.0f1.
- Scene tested: `Assets/BakeIT/Scenes/Kitchen Prototype.unity` in an isolated verification copy.
- Features tested: Eight guide stages, five ingredient checklist items, suppression of successful action narration, temporary corrective feedback, whisk and rolling counters, parchment and portion-transfer progress, oven requirements, successful completion, and the new four-pass rolling threshold.
- Test setup: Temporary Editor-only verifiers were added only to the isolated copy and removed after their runs. They observed the production controller and manipulated runtime component state to cover each guide branch; no verifier code or test objects were saved to the production project.
- Test steps: Confirmed the initial five unchecked ingredient items; sent a normal progress event and verified it did not replace the guide; sent a warning and verified the temporary correction; advanced bowl, board, tray, bake-zone, and oven state through all eight phases; checked numeric progress strings and completion state; then ran four actual `RegisterRollingPass` transitions on a runtime dough object and checked the ingredient state after pass three and pass four.
- Expected behavior: The guide remains a compact to-do list, advances from authoritative gameplay state, and shows only corrections from the event stream. Dough remains unflattened for three passes and flattens on pass four.
- Actual behavior: The verifier logged `BAKEIT_COOKIE_GUIDE_VERIFIER_PASS` and `BAKEIT_FOUR_ROLLING_PASSES_VERIFIER_PASS`. All eight stages, checklist states, progress counts, correction behavior, and completion checks passed. The fourth rolling pass changed `Dough` to `FlattenedDough` exactly as intended.
- Result: Technical pass; player readability and screen-obstruction retest remains pending.
- Console errors and warnings: The final isolated production compile exited successfully with no C# errors or warnings. One isolated verifier shutdown reported a package asset as unexpectedly altered by Unity's own headless package initialization; no production package file was copied back or changed.
- Failed attempts or corrections: The first isolated compile used `OvenBakeResult.Undercooked`, but the existing enum member is `Underbaked`; the guide mapping was corrected and all subsequent compilation and Play Mode verification passed.
- Bugs discovered: No additional project bug beyond `UI-001`.

## 2026-09-06 15:13 +08:00 Cookie-guide 135% sizing verification

- Unity version: 6000.5.0f1.
- Scene targeted: `Assets/BakeIT/Scenes/Kitchen Prototype.unity`; no scene asset was changed.
- Feature tested: Compilation and calculated bounds of the uniformly enlarged staged recipe guide.
- Test input: Player-supplied QHD Game-view screenshot at `0.63x` editor display scale showing that the corrected checklist was still too small.
- Test steps: Applied a `1.35` scale to all guide dimensions and fonts; reviewed the resulting maximum width and tallest preparation-state height; compiled an isolated production copy.
- Expected behavior: Guide text and controls are 30-40% larger while the normal panel remains below half of the 1440-pixel Game view.
- Actual behavior: Maximum panel width is approximately `554` pixels and the tallest normal stage is approximately `294` pixels. The isolated Unity compilation exited successfully.
- Result: Technical pass; perceived readability and obstruction require player retest at the same Game-view settings.
- Console errors and warnings: No project C# errors or warnings.
- Bugs discovered: No additional bug; this adjusts the pending `UI-001` correction.

## 2026-09-06 15:31 +08:00 Player cookie-flow follow-up

- Unity version: 6000.5.0f1.
- Scene tested: `Assets/BakeIT/Scenes/Kitchen Prototype.unity`.
- Features reviewed: Post-bake guide state, onboarding, chocolate-chip measure appearance, and ingredient order.
- Player findings: After completion the guide can return to `put tray in the oven`; no basic desktop tutorial precedes the ingredient checklist; the visible chip measure is sparse enough to count individual pieces; and chips are added before whisking even though the base dough should be formed first.
- Expected correction: Latch completion, introduce a brief movement/look/scoop-pickup tutorial, show a fuller chip scoop, require three small chip measures, and accept those measures only after four-pass base mixing.
- Result: Fail; implementation and focused regression verification initiated.
- Console errors and warnings: None reported; findings concern sequence logic and player guidance.
- Bugs discovered: Expanded `UI-001` and recipe-flow correction scope; no new identifier assigned pending verification.

## 2026-09-06 15:46 +08:00 Tutorial, post-mix chips, and completion-latch verification

- Unity version: 6000.5.0f1.
- Scene tested: `Assets/BakeIT/Scenes/Kitchen Prototype.unity` in an isolated verification copy.
- Features tested: Controls-first guide, four-base-ingredient checklist, premature-chip handling, four-pass base mixing, three post-mix chip measures, filled-scoop mound, finished-dough activation, guide progression, and post-bake completion latching.
- Test setup: A temporary Editor-only verifier was added to the isolated copy, run against the saved scene and production scripts, removed, and followed by a verifier-free production compile. It did not exist in the production project.
- Test steps: Confirmed the initial guide had three tutorial tasks and explicitly taught `E` pickup; marked those observed tasks complete; filled the saved measuring scoop with chips and checked all three visual layers; offered the premature chips to the real bowl callback and checked that the scoop emptied without increasing chip state; added flour, sugar, butter, and egg; invoked four real `RegisterWhiskPass` transitions; added three chip scoops through the real trigger callback; checked guide stages and completion state; set a successful oven result; cleared the live oven-completed flag; and refreshed the guide again.
- Expected behavior: Tutorial precedes ingredients; chips cannot count before base mixing; base dough forms only on whisk pass four; the filled scoop shows a full three-layer mound; final dough forms only on chip scoop three; and completion never regresses to oven loading.
- Actual behavior: The verifier logged `BAKEIT_TUTORIAL_CHIP_COMPLETION_VERIFIER_PASS`. Every assertion passed, including the latched successful result after the live oven flag was cleared.
- Result: Technical pass; player visual and interaction retest remains pending.
- Console errors and warnings: The final verifier-free isolated compile had no project C# errors or warnings. The follow-up Play Mode verifier produced no false bowl warning after the self-result guard.
- Failed attempts or corrections: The first passing verifier exposed the bowl reacting to its own newly activated finished dough; that false correction was fixed and the complete verifier passed again. Both successful verifier processes required termination after writing their pass marker because the temporary headless harness did not exit after Play Mode; production behavior was unaffected and a normal final compile exited successfully.
- Bugs discovered: The post-bake regression and chip-order problems reported at 15:31 are technically resolved; they remain under human acceptance with `UI-001` until the requested retest.

## 2026-09-06 15:53 +08:00 Player small-scoop chip-source retest

- Unity version: 6000.5.0f1.
- Scene tested: `Assets/BakeIT/Scenes/Kitchen Prototype.unity`.
- Feature tested: Collecting chocolate chips with the shared small measuring scoop after base mixing.
- Player finding: The scoop did not collect chocolate chips.
- Runtime evidence: The Editor log shows one earlier successful chip fill, then a base-dough completion followed by repeated base-ingredient rejection messages and no subsequent chip-fill message. Code inspection confirmed that rejected measured ingredients remain in the scoop and all sources silently refuse a non-empty scoop.
- Expected behavior: Dipping the usable measuring scoop into the chip container should reliably show the full chip mound and allow the required post-mix transfers, even if the scoop retained a different rejected ingredient.
- Result: Fail; source/refill correction initiated.
- Console errors and warnings: No C# exception or compiler diagnostic associated with the failure.
- Bugs discovered: `MEASURE-001`—stale scoop contents can block collection from the next required ingredient source.

## 2026-09-06 20:57 +08:00 Small measuring spoon and chip-source recovery verification

- Unity version: 6000.5.0f1.
- Scene tested: `Assets/BakeIT/Scenes/Kitchen Prototype.unity` in an isolated verification copy.
- Feature tested: The intended `_01` small spoon's scene identity, measuring configuration, stale-ingredient replacement, retained-overlap refill, chocolate-chip source trigger, and filled visual.
- Exact test steps: Loaded the saved scene in Play Mode; found `Small Measuring Spoon` and separately labeled `Large Measuring Spoon`; asserted the small spoon's `MeasuringScoop`, Rigidbody, and collider; filled it with flour; invoked the real chocolate-chip source trigger-stay path; asserted replacement with `ChocolateChips`; checked the 18-piece runtime cluster and all three active mound layers; emptied the spoon; invoked trigger stay again without an exit/re-entry; and confirmed it refilled. The saved trigger dimensions were also asserted at no less than `(0.30, 0.28, 0.30)`.
- Expected behavior: The small `_01` spoon collects chips reliably, replaces a stale rejected base ingredient, refills without a precision re-entry, and visibly appears full without introducing contents colliders.
- Actual behavior: The test logged `Scoop contents changed from Flour to ChocolateChips`, refilled after emptying, and emitted `BAKEIT_SCOOP_REFILL_VERIFIER_PASS`.
- Result: Pass (`1/1` Play Mode test; exit code `0`). Physical player feel remains pending.
- Console errors and warnings: No project C# error or warning occurred in the focused pass. The first custom execute-method attempt was interrupted by an unrelated UnityEditor Search-indexing `ArgumentOutOfRangeException`; the supported Play Mode test runner then completed normally. The final verifier-free compile exited with code `0` and no C# diagnostics. Intermittent Unity licensing-client `404` messages were editor-service noise and did not affect the test.
- Screenshots or recordings: None; the isolated test used headless rendering. The supplied player screenshot remains the visual reference for the workstation and guide.
- Bugs discovered: None. `MEASURE-001` is technically resolved and awaits the player's physical retest.

## 2026-09-06 21:37 +08:00 Screenshot-driven spoon and dough-recovery regression

- Unity version: `6000.5.0f1`.
- Scene: Saved `Assets/BakeIT/Scenes/Kitchen Prototype.unity` in the connected Unity Editor, using Play Mode and real runtime components. No isolated project or persistent verifier script was created.
- Original player result: Fail. `Screenshot 2026-09-06 210528.png` and `210555.png` show sparse/overflowing chip fills; `210653.png` shows held uncut dough that cannot return. Registered as `MEASURE-002` and `FLOW-003`.
- Spoon test steps: Load the imported scene; enable background simulation for the test; use the actual pickup ray on the small spoon; retain flour and physically overlap the chip source; confirm replacement; empty while overlapping and confirm actual physics refills it; verify all final chip mesh vertices; empty and check every layer hides. Reject chips in the large spoon while preserving flour, then confirm sugar measuring still works.
- Spoon expected/actual: Pass. Only the small spoon accepts chips. Final fill uses eight active shared cluster meshes, radial maximum `0.02050005`, local Y `0.00999999-0.028`, and only the original scoop collider. The actual inner rim is approximately `0.02152` and top Y `0.02367`; this leaves a shallow mound with no sideways overflow. Final code was rendered and visually inspected: [small-spoon fill](evidence/small-spoon-contained-fill-2026-09-06.png).
- Recipe test steps: Offer early chips to the real bowl callback and confirm zero count plus empty spoon; add flour, sugar, butter, and egg; register four real whisk-pass transitions; transfer three chip scoops through existing source/bowl callbacks; physically move finished dough into the board trigger.
- Dough test steps: Reject early cutting; roll twice; use actual ray pickup; rotate and hold inside the board overlap; confirm the board does not steal it back; invoke the real drop method; allow physics to recover it through trigger stay. Compare local scale and `2/4` progress, then complete exactly two more rolls. Pick up `FlattenedDough`, rotate it away from the board, and physically re-enter. Verify `4/4`, exact flattened local scale, released pickup, and readiness to cut. Exercise three further rotated pickups and callback-driven return cycles; confirm off-board cutting is rejected and actual cookie portions are excluded from slab placement.
- Dough expected/actual: Pass. Partial and flattened preparation survives return without scale drift or a blocked workflow. Actual knife contact produced four portions from the recovered slab.
- Downstream steps/results: Confirm an unlined tray rejects a portion; physically place parchment; load all four portions through the real tray callbacks; physically place the tray into the oven zone; start the normal `180 C` five-second bake. `FINAL_RECOVERED_COOKIE_BATCH_PERFECT_BAKE_PASS` confirmed all four became `BakedCookie` with `OvenBakeResult.Perfect`.
- Test method limits: Ray selection and several source/board/knife/parchment/oven contacts used actual physics. Whisk and rolling pass registration, some repeated placement cases, ingredient bowl transfer, and portion loading used deterministic calls to the existing runtime methods. This does not replace a full player-operated walkthrough or VR validation.
- Failed attempts and corrections: A diagnostic `GetInstanceID` call was rejected as obsolete; a `System.Reflection` import was rejected by Unity MCP before execution. An initial scene reload retained stale in-memory spoon data; an explicit Edit Mode scene import/reopen loaded the correct saved capability. Initial background tests had `Time.time=0` because `runInBackground=false`; temporarily enabling it allowed actual physics to advance. One flattened re-entry harness attempt using only `Rigidbody.position` did not produce the intended exit/re-entry; explicit transform movement plus `Physics.SyncTransforms`, with the outside position verified on a later frame, passed. These were recorded test-setup failures rather than counted as successful tests.
- Visual iterations: The initial contained four-mesh fill was too sparse; a vertically compressed trial flattened the chip shapes. Restored chip proportions, added the shallow mound, and doubled the interleaved packing. Preview objects were runtime-only. Two camera-capture connector calls failed, so the final render used the real Unity camera into a RenderTexture.
- Console: Final Unity Console query returned zero errors and zero warnings; current source compiled and executed in Play Mode. Tool-command assertion/setup failures above remain documented even though they were separate from the final clean Console.
- Cleanup: Exited Play Mode, restored original background behavior, and discarded temporary test cameras, runtime layer changes, and runtime interaction state. Local temporary preview PNGs remain in the OS temp folder; the final evidence PNG is retained with these notes.
- Result: Technical and rendered-visual pass; human reach, control feel, and full player-facing acceptance remain pending.

## 2026-09-07 09:20 +08:00 Kitchen storage, layout and cookie regression

- Unity: `6000.5.0f1`; connected Editor Play Mode; `Assets/BakeIT/Scenes/Kitchen Prototype.unity`. Tested the authored/saved scene and again after a runtime restart. Background simulation temporarily enabled; restored afterward.
- Baseline: Player screenshot showed chips overlapping the green board and crowded ingredient/tool placement. Existing fridge had a full solid root box and no door/storage logic. New scene reloaded from disk with the brown material and no missing-script references.
- Storage steps: Read both initial slots; aim the real pickup camera at the left door and call the same public action E dispatches. After physics reaches -100 degrees, aim at egg and invoke the existing pickup action. Wait for frames and confirm the item stays held, slot empties, and WasRetrieved is true. Aim at the second door while retaining egg and operate it. Offer egg to butter slot (reject). Release the egg outside its old slot, synchronize/wait, then move it inside the actual trigger and let physics store it. Attempt closing with the CharacterController at `(-2.65,0,3.95)`, then move clear to `(-1.95,0,4.7)`. Aim/pick at the egg behind the closed door.
- Storage expected/actual: PASS. Both initial foods are stable and keep original scale. Doors reach +/-100. E door action retains held food. Egg is not stolen back, wrong type is rejected, physical reentry stores it under Storage Snap, closing stops at the obstructed pose without moving the player, clearing the path completes closing, and the closed door blocks food pickup. Open/closed renders confirm shelf labels no longer draw through the door.
- Setup steps: Start fresh Play Mode. Ray-pick the small spoon from TOOLS. Inject one W keyboard state and mouse delta through Input System, run the existing tutorial observer, then release both states. Observe `2 / 11 MISE EN PLACE` and four false rows. Release the spoon onto the MIX tool mat. Ray-pick egg/butter from the open fridge and whisk from TOOLS; release them on their matching MIX mats using deterministic placement, then let physics settle. Drop large spoon on the empty shelf at approximately local Y .92 and let it settle before actual ray pickup.
- Setup expected/actual: PASS. Tutorial completed and all four preparation flags latched; guide advanced to `3 / 11 PREPARE BASE`. Egg, butter, spoon, and whisk settled with zero velocity (egg Y 1.24, butter 1.20, spoon 1.18, whisk 1.21). Loose large spoon settled on the shelf at Y .83 and could be ray-picked again. Neither slot retained consumed food later. The chip container's visible bounds do not intersect the board.
- Cookie steps: Ray-pick staged egg/butter and transfer through existing bowl callbacks; fill/empty flour and sugar using the existing scoop/bowl methods. Register four real whisk-pass transitions. Physically overlap the relocated `ChocolateChipScoopZone`; observe small spoon filled with ChocolateChips. Transfer three post-mix measures through bowl callbacks. Move spawned dough into the actual relocated board trigger; register four roll transitions. Ray-pick flattened slab, rotate it outside the board, release through placement API, re-enter the board trigger, and wait for physics. Move actual knife into the work zone. Physically overlap parchment with tray liner zone, then load all portions through the existing tray callbacks.
- Cookie expected/actual: PASS. Base mixed at 4 passes, chip count 3, dough spawned at the relocated bowl `(-1.72,1.19,5.70)`. Returned slab was flattened, retained 4/4, and kept world scale `(1.4,.4,1.4)`. Actual knife contact made four portions. Physical parchment placement succeeded, and the tray reported all four portions ready.
- Oven steps: Aim actual player camera at new plus/minus/BAKE faces, raycast and call the existing control action. Confirm empty-start rejection. Move prepared tray into the existing oven trigger with a kinematic test placement, wait for physics registration, then select the rounded BAKE face through its actual ray. Wait normal five-second prototype duration. Remove registration through the existing zone exit callback and refresh the guide.
- Oven expected/actual: PASS. Rays hit the intended button colliders; temperature changed 180 -> 190 -> 180 C. Empty BAKE did not start. Loaded BAKE started, reached Perfect, and all four ingredients became BakedCookie. Guide remained RECIPE COMPLETE after tray removal.
- Restart steps/result: Call the same RestartRecipe method used by R/HUD; await scene load. PASS: both food slots restored and WasRetrieved=false; both doors closed; all setup flags false; bowl egg quantity zero. Existing tutorial latch remained complete, so the repeat batch started at `2 / 11 MISE EN PLACE`. Fresh Play Mode had started at BASIC CONTROLS as expected.
- Visual QA: Rendered/reviewed [worktop](evidence/kitchen-worktop-2026-09-07.png), [overview](evidence/kitchen-overview-2026-09-07.png), [oven](evidence/kitchen-oven-2026-09-07.png), [open fridge](evidence/kitchen-fridge-2026-09-07.png), and [closed fridge](evidence/kitchen-fridge-closed-2026-09-07.png). The first text material visibly bled through geometry (FAIL); depth-tested world text fixed it in final renders (PASS). Reduced fridge-light hotspot and corrected player-facing minus/plus order.
- Test setup failures: Two command-only component lookups expected a root chip Renderer/Collider that actually lives on children; corrected and retried only the unfinished checks. QueueDeltaStateEvent on a keyboard bitfield failed; whole KeyboardState events succeeded. These are recorded harness failures, not production runtime failures. A malformed PowerShell path query was corrected. No failed diagnostic was counted as a feature pass.
- Console: Final Play Mode and Edit Mode queries returned zero warnings/errors/exceptions. Saved scene validation found zero missing scripts; both new shaders had zero compiler messages. Scene saved clean in Edit Mode and `PlayerSettings.runInBackground=false`. Final `git diff --check` passed after mechanical scene-whitespace normalization. Tool-command exceptions above remain documented separately.
- Method limits: Real ray selection, several physical trigger contacts, shelf/mat settling, obstruction, rendering and timed baking were tested. Whisk/roll counts, several food/portion transfers, carried travel placement, and tray insertion were deterministic calls/moves through the existing runtime systems. This is not a complete user-operated traversal or gesture test. Held rotation/scroll, crouch reach, all awkward releases, closing around every loose object, manual oven door/tray insertion, subjective UI/layout acceptance and VR remain in the player checklist.
- Result: Technical workflow and rendered-visual pass. Final player usability acceptance remains open; no claim of a finalized kitchen or cookie recipe.

## 2026-09-07 14:15 +08:00 Storage-first board preparation correction

- Unity/scene: connected Editor `6000.5.0f1`, saved `Assets/BakeIT/Scenes/Kitchen Prototype.unity`, two Play Mode passes. Initial Console zero warnings/errors. Previous first-pass tests remain historical; this entry supersedes the MIX-volume/fixed-slot behavior.
- Original behavior: user rejected arrow-style MIX instructions and requested true board preparation, cabinet dry storage, fridge chips and release-local rests. Inspection confirmed fixed source containers, a solid cabinet root box, two separate sliding door meshes/platform, and hover/consumption-based setup.
- Cabinet steps/results: From a low camera at `(1.4,.72,4.4)`, aim at flour behind closed doors and call the normal pickup action: nothing picked (PASS). Dispatch the normal E-ray action to `_p03`: it slid from local X -.33 to .41 and exposed flour. Ray-pick flour, close that track, aim at sugar and slide `_p02` from .33 to -.41: sugar ray pickup succeeds (PASS). Place a sugar body in the sliding path and close: door remained moving/blocked without displacing food (PASS). Return sugar on the open shelf using the release placement path: Y settled to .41 and X/Z delta was zero (PASS).
- Early preparation steps/results: Send egg to bowl before setup: egg remained active, bowl amount zero (PASS). Ray-pick each of flour/sugar/egg/butter/chips, move the held body over a chosen free board location, verify hover did not mark prepared, then invoke the same DropObject method as E. Each body became stable with unchanged release X/Z (delta exactly 0); no preset slot or parent teleport (PASS). Pick/release small spoon and whisk too; all seven fit concurrently on the .70 x .75 m board, and the guide advanced to PREPARE BASE (PASS).
- Release safety steps/results: Ray-pick staged egg. Try assistance over the occupied chip bowl: declined and direct TryPlace kept it held (PASS). Try well above the board at Y 2.2: declined (PASS). Move to a new clear point, rotate `(25,30,5)`, release: settled at `(-.16,1.19,5.85)`, same X/Z, yaw retained, original scale unchanged and body stable (PASS). This was repeated placement at a user-selected point, not a fixed slot.
- Carrying checks: On the final code/labels, ray-pick sugar from the cabinet with the camera at crouch-like height .9 and wait for normal LateUpdate/physics. Actual visual center reached `(1.55,.55,3.74)`, about .765 m from the camera; stepping back carried it out to about Z 4.22. Rotation and scroll changed held orientation/hold distance .75 -> .95 without scale change; sugar remained `(.69,.69,.69)` (PASS). Text and zero-sized renderer bounds are excluded from centering.
- Source/recipe regression: After moving the containers, actual physical trigger overlap filled the small spoon with ChocolateChips, Flour and Sugar (PASS). Large spoon still rejected chips. Entire flour bag passed to bowl did not count as a measure; early chips after setup still emptied without being counted, before base mixing. Existing callbacks accepted egg/butter, one flour/sugar scoop, four whisk transitions and three chip scoops to complete dough (PASS).
- Board/bake regression: Guide showed CLEAR THE PREP BOARD while supplies rested there; dough placement was rejected. Move supplies aside, place dough, register four roll transitions, cut using actual board callback, line tray, transfer portions and register the loaded oven tray. Wider board still made four portions, and the unchanged 180 C / 5-second bake reached Perfect with four BakedCookie results (PASS).
- Reset: RestartRecipe restored dry sources to `(1.112,.408,3.09)` / `(1.688,.408,3.09)`, chips to `(-2.35,1.544,3.13)`, egg/butter to fridge, tools to their home, all seven WasPrepared/rest-surface values clear, workspaceReady false and both sliding panels closed/not moving (PASS). Retained in-session controls tutorial resumed at PLACE ON PREP BOARD; a fresh Play Mode pass began at BASIC CONTROLS.
- UI/visual QA: Five explicit ingredient-location rows fit the existing compact panel; after ingredient staging only two tool rows are needed. Actual Game-view screenshot reviewed in `evidence/board-preparation-guide-2026-09-07.png`; final prepared-item render in `evidence/prepared-board-2026-09-07.png`; storage in `dry-cabinet-storage-2026-09-07.png` and `revised-fridge-storage-2026-09-07.png`. First single-sided names were not visible from the prep side, so reverse labels were added and inspected (final PASS). Cabinet hint was moved forward/lighter for contrast; the storage preview predates that small hint-only adjustment.
- Failed attempts/tooling: One initial multi-file patch was rejected before changing files because it used delete/add for the same script; reapplied as an update. Intermediate scene diff checks reported Unity-generated trailing whitespace, normalized at handoff. A broad collider audit flagged the unchanged stationary kinematic FreeStove; inspected/classified separately as PHYS-BASELINE-001 rather than altering the appliance. All seven preparation carriers have zero active non-convex MeshColliders; zero missing scripts and final Console errors/warnings.
- Limits: Real ray pickup, delayed held motion, source trigger physics, E-dispatched release logic, guide input state and actual timed baking were exercised. Deterministic transforms positioned held items for most releases; whisk/roll/count, some transfers and tray insertion used existing runtime callbacks. Full user-controlled traversal, every edge/throw/rotation case, multi-item obstruction reversal, preferred placement feel and VR remain unaccepted. No persistent verifier was installed.
- Cleanup: Exited Play Mode, restored `PlayerSettings.runInBackground=false`, saved scene/assets and removed transient runtime cameras through normal test teardown. Old fixed slot objects are disabled/recoverable, not deleted. Notes and pending changes reviewed without committing. Technical pass; player acceptance still required.

## 2026-09-07 19:27–19:39 +08:00 Separate rests, movable bowl and final chip mixing

- Environment: connected Unity 6000.5.0f1, Kitchen Prototype, Codex/Unity MCP. Original enlarged user mats preserved. Initial Console had six external Unity account-service errors; final fresh Play/Edit query returned zero errors/warnings.
- Wrong/early setup: place egg physically on the chopping-board StablePlacementSurface before readiness; EggPrepared=false and IsPrepared=false (PASS). Send egg entry before setup and while holding the bowl: egg amount0, ingredient intact, whisk count0 (PASS).
- Correct setup: position five ingredients above clear Cold Ingredient Rest points and invoke the real stable-release path; both spoons/whisk on Mix Tool Rest. All eight settled with no X/Z shift; no readiness until bowl placed in MIX (PASS). Input-state move/look plus actual small-spoon ray pickup completed tutorial; guide showed MIX TOOL REST, then PLACE BOWL IN MIX; final bowl placement produced ADD BASE TO BOWL (PASS). Saved Game-view screenshots inspected.
- Bowl handling: real camera-ray pickup from rack, release at (-3.49,1.056,4.14), re-pick, delayed normal held movement, mouse rotation and scroll .75→.95 hold distance (PASS). Player position remained stable; bowl scale stayed1. Tilted (15,40,10) near-MIX release settled to yaw40 at (-.4,1.175,5.83), unchanged X/Z. High release assistance and occupied chip-container area were refused (PASS). Bowl returned to the corrected shelf support after the first authoring iteration failed support selection (PASS after fix).
- Filled-bowl recovery: add flour through existing scoop callback; pick and move bowl to another clear MIX point (-.36,1.175,6.07). Flour quantity1 and local ingredient-visual position preserved; carried whisk callback did not advance; return restored readiness (PASS).
- Cookie mixing: actual recipe callbacks accepted flour/sugar/egg/butter and four base RegisterWhiskPass transitions. Three chip scoops left result inactive and ChipMixPassCount0. Place whisk collider overlapping the bowl and move it .075 m right/left/right over separate real physics frames. Observed final counts1,2,3: result inactive at2 and active at3 (PASS). Final dough appeared at moved/yawed bowl position, not the original counter position.
- Downstream: ray-pick final dough, move/release to relocated board and invoke its actual receiver callback. Register four rolls. Ray-pick flattened slab, release while overlapping, invoke OnTriggerStay: flat state4/4 and world scale(1.4,.4,1.4) retained. Knife receiver made4 portions, parchment receiver lined tray, portion receiver loaded4, oven-zone callback accepted tray and actual timed180 C/5-second bake reached Perfect and RECIPE COMPLETE (PASS).
- Reset: RestartRecipe restored bowl to rack, ingredients to storage, all preparation/rest references clear, both whisk counts0, chip count0, result inactive and readinessfalse (PASS). Fresh scene reload preserves all authored references and user mat scales.
- Safety audit: nine preparation carriers and ten stable supports, zero enabled non-convex meshes attached to those moving bodies, zero missing scripts. Existing stationary FreeStove baseline remains separately documented. No permanent settings/packages changed; background execution temporarily enabled for physics then restored false.
- Visual QA: inspected prepared-rests-guide, place-bowl-guide and bowl-holder-rack PNGs. Earlier separate-workstations PNG retains a badge-alignment iteration; final badges and correct compact instructions are visible in the two guide screenshots. Reused holder asset preview retained as evidence.
- Failed attempts: direct DropObject after blocked pickup produced three harness-triggered exceptions; corrected harness did not call DropObject without a held body. Wrong MixtureResult hierarchy lookup produced one temporary-command error. Over-high/stale-bounds spoon release and edge-position whisk staging declined; corrected near-surface/centered positions passed. First holder migration used disabled original plus new duplicate BoxCollider; reused original and removed newly added duplicate, then verified rack return. No failures are counted as successes.
- Limits: ray pickup, actual delayed carry/input, stable releases, final whisk trigger motion and timed baking were real Play Mode. Many supply placements, base whisk counts, board/portion/tray transfers and oven insertion used deterministic transforms/callbacks. This does not establish full player-controlled traversal, every collision/throw/edge case, subjective feel or VR acceptance. No permanent verifier installed.
- Cleanup: exited Play Mode, restored runInBackground=false, saved scene/assets, normalized serializer-only trailing whitespace and reimported/reopened saved scene (dirty=false). No stale scene save after formatting. Pending changes reviewed; no commit/reset.

## 2026-09-08 11:20 +08:00 Flour containment handoff (tests begun 2026-09-07 19:51)

- Unity6000.5.0f1, Kitchen Prototype, Codex/Unity MCP. Baseline visual reproduction used real BowlReceiver quantity/presentation callbacks: flour bottomY-.03148, no flour Collider, visibly protruding below bowl (FAIL reproduced; flour-leak-before PNG).
- Corrected normal ingredient path: stage all supplies on their proper StablePlacementSurfaces, place bowl in MIX, fill large spoon with flour and invoke the normal bowl trigger callback. Flour amount1, scoop emptied (PASS). Remaining ingredient callbacks accepted sugar/egg/butter, with unchanged base requirements.
- Interior audit: temporary static collider copied from actual bowl mesh; downward ray at each rendered vertex X/Z compared vertex height with inner-shell surface. Final flour515 vertices: zero outside/below, minimum clearance.00115m. Sugar/butter/egg white/yolk and forming dough515 vertices each: zero after fitting; chip mesh576 vertices: zero. Temporary collider destroyed within the diagnostic command, before physics advance.
- Iteration results: initial raised flour floated14mm above the interior; lowered to1.15mm minimum clearance. First inward sugar adjustment still had9 protruding vertices, minimum gap-.00254m; final X offset-.08D produced zero with.01133m clearance. No physics/material/quantity workaround.
- Mixing callbacks: four base passes and three chip measures retained incomplete dough; two final passes remained incomplete and the third created dough (PASS). First/final forming-dough and chip presentation containment passed. No new full baking rerun was needed for this presentation-only change; earlier four-cookie bake regression remains historical evidence.
- Fresh final-source test: flour local offset(.00431,-.02586,0); ray pickup of flour-filled bowl, rotated near-MIX release, quantity1 and unit scale retained. Re-audit still zero protruding flour vertices (PASS).
- Continuation 2026-09-08: saved-code fresh Play Mode reproduced corrected minimum flourY.01258; final rack-view render inspected. RestartRecipe restored flour quantity0, invisible mound, incomplete bowl and rack initial pose (PASS).
- Evidence: flour-leak-before-2026-09-07.png; flour-contained-preview-2026-09-07.png; flour-contained-final-2026-09-08.png. Views show actual Unity meshes, not edited/generated mockups.
- Console: zero at original start. Two existing connector certificate-metadata warnings at Sep8 continuation, no game error; final query zero errors/warnings. No failed command or runtime exception in this focused correction; two intermediate visual fits were deliberately refined.
- Cleanup: Edit Mode, scene/assets saved, background execution false, no permanent audit cameras/colliders. Serialized contents center saved; scene whitespace normalized and reloaded. Pending changes reviewed; no commit/reset. Full user-controlled scoop/gesture visual acceptance remains pending.

## 2026-09-08 13:10–13:19 +08:00 Final six-cookie / tutorial / placement regression

- Environment: Unity6000.5.0f1, Kitchen Prototype, fresh Play Mode after all runtime refinements. Desktop controller disabled for deterministic fixture positioning; normal PickupController, Input System, physics, receiver methods, timer and scene reload remained active. This is technical integration testing, not an end-to-end human walk-through.
- Tutorial: queue WASD/mouse look input; actual camera ray picks whisk; queue right-button mouse delta and scroll; dispatch normal update. Rotation/scroll rows true, tutorial false before release. Release whisk with its supporting bottom approximately .45m above Utensils Area. Immediate motion did not teleport; after settling, lower collider gap .002999902m, tutorial true, COLD INGREDIENTS stage. PASS.
- Physical landing: release large spoon from bottomY1.90 over mat (outside .50m assist). Immediately dynamic and unprepared; allow actual physics to fall. RestingSurface became Utensils Area automatically. PASS. Earlier edge-slip attempt failed and informed the .035m tool-only allowance. Held overlap never credited as placement.
- Cold/dry progression: stage only egg/butter/chips on Cold Ingredient Rest, wait for settling; HUD becomes DRY INGREDIENTS with only cabinet/flour/sugar rows. Stage remaining supplies plus bowl; workspace ready only after all correct surfaces settle. PASS. Storage doors themselves were not retested through a full carrying route in this pass.
- Recipe: fill existing large scoop with flour/sugar and use BowlReceiver's normal trigger callback; use egg/butter ingredient callbacks; four base-whisk callbacks, three small-chip measures. Dough inactive until third combining pass (false after2, true after3). Large scoop still rejects chips. PASS. These gesture-count tests invoke gameplay callbacks rather than seven complete manual whisk motions.
- Board: ray/pickup path plus explicit E-release fixture, held OnTriggerEnter does not capture; immediate release displacement0. Return uncut slab at2/4 and preserve passes, then remaining two passes and knife callback yield6. One active chip mesh per cookie, counts8/9/10/8/9/10; world portion dimensions about(.57,.40,.58) scale after carried yaw. PASS. Existing nonuniform board parenting can produce small orientation-dependent scale differences; no additional shrink was observed through tray transfer.
- Roll/tray: actual camera ray invokes dispenser and holds the existing sheet. Place paper at(1.70,~1.18,5.77), not tray center; no instant positional jump. Six chosen non-overlapping positions accepted, readiness false during settle and true afterward. Removing one gives5/false; overlapping replacement rejected, original position accepted. Loaded paper pickup rejected. PASS. Intermediate wrong-tool and repeated-roll requests produced correction without extra paper; final held-roll ray bypass tested separately.
- Cookie aiming: held cookie visual-bounds center projected to viewport(.50,.50) at .75m distance. Aim at roll while holding cookie: interaction dispatcher returns false, queued E invokes release and places cookie on tray. PASS (reproduced prior low HoldPoint=-.15 and roll interception through code/scene inspection).
- Baking: move entire tray into actual OvenBakeZone and allow physics registration; TryStartBake at180C starts five-second timer. All6 become BakedCookie, Perfect. Pick up/return loaded tray through smooth worktop placement; count6 remains, guide stays RECIPE COMPLETE. Pick up and replace cooked cookie accepted. PASS. Door was closed for this test; full manual oven insertion is not certified here.
- Alternate outcomes: while actual oven registry holds complete tray, set runtime temperatures160,200,230,180 and invoke CompleteBake. Underbaked / Overcooked / Burnt / Perfect states and distinct base colors verified; ChocolateChipMaterial unchanged. PASS for result branches, not four separate timed runs.
- RestartRecipe:0 runtime portions, roll HasDispensed=false, flour0, preparationfalse, ovenfalse, yield6, COLD INGREDIENTS (completed tutorial persists within session). PASS. Fresh Play retains opening whisk tutorial behavior.
- Visual QA: actual Unity renders cookie-raw-dough-final-2026-09-08.png, cookie-six-raw-tray-2026-09-08.png, cookie-six-golden-final-2026-09-08.png inspected. Pale raw/golden baked and different chip layouts visible. Stylized low-poly surface retained. Subjective reference likeness awaits player approval.
- Failures retained: initial mesh Read/Write errors, repeated chip meshes, reversed label, stale-pose test fixture/floating whisk, edge-slip non-credit, four deprecated API warnings, intermediate source-reload collection loss, diagnostic FBX lookup and note-heading mismatch. Corrected and final checks rerun. Final Play/Edit Console query: zero warnings/errors. No permanent test components or cameras; test input released, Edit Mode restored, background false, assets saved.

## 2026-09-08 21:08–21:28 +08:00 Recoverable measuring / transfer accounting

- Fresh Play Mode with deterministic preparation/pickup positioning, real source physics and scoop Update timers. Controller temporarily disabled to hold fixture poses; no permanent test components. This is technical integration testing, not a complete human keyboard/mouse walkthrough.
- Roles: Measuring Cup rejects chips; Small Measuring Spoon rejects sugar. Actual upright cup contact fills sugar. Trying flour while full refuses replacement and preserves sugar. Upright bowl contact adds nothing; held 100-degree tilt over bowl runs the .35s timer and adds one Sugar measure, empties cup, records AddedToBowl.
- Duplicate sugar rejected intact. Tilt above matching sugar source returns it, records ReturnedToSource and adds no waste. Remaining inside that source does not immediately refill; lifting clears the return lock. Wrong-source return to flour refused without loss or extra event.
- Input-test limitation: one queued Q state and two background sustained-injection attempts did not persist reliably across automatic frames; a delayed helper command also failed to execute. Explicit queued keyboard state plus repeated scoop Update calls accumulated deltaTime (~.00824s) across the .60s threshold and produced exactly one Sugar discard. Empty repeat refused; a one-update Q tap followed by release canceled progress without waste. Manual focused-Game-view hold/release Q remains required; no runtime exception or input setting workaround introduced.
- Flour subsequently poured successfully. Early chocolate chips rejected intact before base mixing. Egg/butter callbacks and four base-whisk callbacks unlock chips. Actual small-spoon source contact refills chips; three accepted tilted transfers recorded, including timer-driven pours and one direct validated TryPourMeasure invocation. Two combining passes not complete; third complete.
- Ledger before restart: seven records = Sugar Added, Sugar Returned, Sugar Discarded, Flour Added, three ChocolateChips Added. DiscardedMeasureCount=1, measured in prototype utensil units, not grams or grading points.
- Recovered recipe: board release/settle, four rolling callbacks, knife callback, six portions; parchment dispensed and six normal placement releases accepted. Full tray registered at oven and five-second 180C bake exercised. Detailed six-cookie result branches already covered in earlier regression; this pass focuses measuring recovery.
- Visual QA: actual Unity render measure-tilt-transfer-2026-09-08.png inspected: tilted small spoon, short chip transfer and contained bowl mixture. Particles are collider-free presentation only, not physical spills.
- Restart confirmed fresh ledger0, waste0, both measuring tools empty, runtime portions0. Play Console query zero logs/warnings/errors. Stopped Play, restored background=false; save/diff/final Edit Console checks at handoff. No source edits during final Play verification.

## 2026-09-09 09:31–09:39 +08:00 Focused underbaked and parchment correction

- Environment: Unity `6000.5.0f1`; `Assets/BakeIT/Scenes/Kitchen Prototype.unity`; deterministic Editor-command Play Mode fixture. Baseline Git clean and Console zero errors/warnings. The externally reverted scene was initially dirty in the open Editor, so it was reloaded from disk before any edit.
- Scope: only `OvenController` underbaked visual behavior and `ParchmentPaper` scale. No storage, workstation, pickup, recipe quantity, oven threshold/timer, shader/material asset, package, or ProjectSettings change.
- Steps: measured the real tray BoxCollider support; activated and placed the existing sheet through `TrayLinerReceiver`; created six disposable same-batch `CookieDoughPortion` cubes with deliberately non-default Cookie Surface values (`_BaseColor=(.61,.52,.43)`, `_BakeAmount=.37`, `_Smoothness=.42`); loaded them through `TrayReceiver`; registered the actual tray with `OvenBakeZone`; started/completed `160 C`, recorded names/result and material properties; then started/completed `180 C` and checked the normal visual branch. Runtime fixtures disappeared on Play Mode exit.
- Expected: paper is at least 95% of support X/Z and still accepts six portions; `160 C` produces logical underbaking with exactly zero material changes; `180 C` still applies normal baked properties.
- Actual/pass: paper `95.0066%` X / `95.01715%` Z; paper placement true; complete six-cookie batch true; all six became `UnderbakedCookie` with every recorded property unchanged; subsequent Perfect bake gave all six `BakedCookie`, `_BakeAmount=1`, `_Smoothness=.25`. Final focused result PASS.
- Failed attempts/corrections: first disposable fixture moved the inactive sheet without `Physics.SyncTransforms`, so liner placement and all downstream checks correctly failed. Fresh Play and synchronized transforms fixed it. Second run proved the underbaked zero-change path but exposed X float rounding at `94.99999%`; sheet scale increased by less than `.00005` m for a safe margin. That run's Perfect assertion watched the original material reference rather than each renderer's runtime instance; the final test inspected `Renderer.sharedMaterial` and passed. These were fixture/precision corrections, not hidden product errors.
- Console/evidence/limits: final successful Play and reloaded Edit Mode queries both returned zero errors/warnings. Final scene audit found paper inactive, zero disposable cookies, coverage above 95%, and `dirty=false`. No screenshot recorded and no persistent verifier/helper asset created. Full human visual comparison and subjective paper/rim fit remain open; this fixture does not certify VR or the entire recipe walkthrough.

## 2026-09-09 09:45–13:35 +08:00 Mixing and rolling cooldown verification

- Environment: Unity `6000.5.0f1`; `Assets/BakeIT/Scenes/Kitchen Prototype.unity`; disposable Play Mode objects at remote coordinates driving the actual `BowlReceiver` and `DoughBoardController` callback paths. Runtime `Application.runInBackground` was enabled only between timed phases and restored false; no ProjectSettings change.
- Configuration: saved bowl cooldown `.15 → .35 s`; new saved rolling cooldown `.35 s`. Whisk distance remains `.06 m`, rolling distance remains `.2 m`, and required counts remain base `4`, chip `3`, rolling `4`.
- Immediate-repeat steps/result: supplied an isolated receiver with the four required base ingredients, entered with a whisk, moved `.18 m`, then immediately moved another `.18 m`; count stayed `1 → 1`. An isolated board accepted/settled raw dough; one `.65 m` rolling sample counted exactly one instead of three, and an immediate `.65 m` reverse movement stayed at one. PASS.
- Cooldown recovery/result: after each interval, fresh `.18 m` whisk and `.25 m` rolling strokes advanced exactly one count. Base mixing completed `4/4` with `IsBaseDoughMixed=true`; rolling completed `4/4` with `FlattenedDough`. Added three chip units, then verified the same immediate-repeat block at `1 → 1` and separated progression to `3/3` with recipe completion. PASS.
- Expected/actual: one gesture/sample must not create multiple logical passes, but fresh strokes after the cooldown must still complete every existing stage. Actual matched exactly. Unity compilation and successful Play query had zero errors/warnings. Final Edit Mode query had zero errors and one existing external Codex executable-signature warning (`SERVICE-002`), unrelated to project scripts or gameplay.
- Failed attempts/bugs/evidence: no gameplay-test failure. No screenshot and no persistent test helper; all `Cooldown QA` objects are runtime-only and were removed on Play exit. Full human mouse gesture feel, unusual frame-rate/device behavior, and VR tuning remain unaccepted.

## 2026-09-09 13:45–14:05 +08:00 Parchment, knife, and 1.2-second retune verification

- Environment/scope: Unity `6000.5.0f1`, `Assets/BakeIT/Scenes/Kitchen Prototype.unity`; existing paper, tray, player pickup controller, and knife plus disposable remote bowl/board/cookie fixtures. No persistent verifier asset, package, ProjectSettings, or recipe-count change.
- Saved configuration: paper `.649 x .3898` gives `90.000%` X / `90.007%` Z of support; knife pickup pose `(0,180,0)`; bowl/board cooldowns `1.2 s`; counts remain base/chip/rolling `4/3/4` and distances `.06/.2 m`.
- Parchment/tray result: actual `TrayLinerReceiver` accepted the paper from `25 mm` X / `12 mm` Z off center, then actual `TrayReceiver` accepted all six non-overlapping portions and reported a complete batch. PASS.
- Knife result: actual pickup controller held the knife; the mesh's local `-Z` blade axis had `dot=1.0000` against camera forward. After four rolls, its actual collider callback created six cookie portions. PASS.
- Cooldown result: first and immediate duplicate whisk/roll movements stayed `1 → 1`. A separate timing fixture attempted fresh movement at `0.65 s` and stayed mix `1`, roll `1`; after the full `1.2 s`, both advanced to `2`. Separated strokes completed base `4/4`, rolling `4/4`, and chip combining `3/3`/recipe complete. PASS.
- Fixture corrections: first knife log used the wrong axis sign; both directions were then measured. One chip stroke occurred after the disposable whisk left its synthetic trigger and correctly re-established tracking without counting; recenter/re-entry completed the final pass. The Unity bridge rejected reflection in one read-only audit, which was replaced with `SerializedObject`. None indicates a saved product failure.
- Cleanup/Console: Play Console zero errors/warnings. After exit, `Application.runInBackground=false`, scene `dirty=false`, zero QA objects, and Edit Console zero errors/warnings. Manual Game-view appearance and subjective `1.2 s` fluidity remain unaccepted.

## 2026-09-09 14:15–14:35 +08:00 Parchment sensitivity and 86% footprint verification

- Environment/scope: Unity `6000.5.0f1`, `Kitchen Prototype`; actual paper, liner, support, tray receiver, and disposable six-cookie fixtures. Paper scale `.6202 x .3725` measured `86.007%` X / `86.012%` Z. Assist settings: max height `.35 m`, horizontal padding `.12 m`, vertical padding `.35 m`, magnetic radius `.15 m`.
- Exact/safety result: a valid paper release `.020 m` X / `.015 m` Z off center and `.18 m` high settled with the offset preserved. A rotated release `.18 m` from center was rejected outside the magnet radius; a centered pose with its bottom `.04 m` below the support was rejected. PASS.
- Assisted result: release `.10/.08 m` off center, `.30 m` above, and yawed `45°` failed exact fit but was accepted by the parchment fallback. It settled at the snap center and tray yaw with center/yaw deltas `0/0`. PASS.
- Load/movement result: six non-overlapping same-batch portions were accepted on the 86% sheet, oven-ready completeness became true, and paper plus all six cookie local poses remained unchanged after moving the tray `.20/-.10 m`. PASS.
- Failed run/correction: the first fallback preview assigned only Rigidbody pose, producing stale Transform bounds and a false rejection; consequent cookie/movement assertions also failed because no liner existed. Direct centered geometry passed, identifying preview synchronization as the cause. Updated to set/restore both Transform and Rigidbody, recompiled outside Play Mode, and reran every case successfully from a fresh scene.
- Cleanup/Console: successful Play and final Edit Console queries returned zero errors/warnings. Final Edit audit found scene `dirty=false`, zero QA objects, background false, and all saved values correct. Human Game-view placement ease remains unaccepted.

## 2026-09-09 14:51–15:14 +08:00 Recipe-data foundation and cookie migration

- Environment/scope: Unity `6000.5.0f1`; `Assets/BakeIT/Scenes/Kitchen Prototype.unity`; new `RecipeDefinition` source and `ChocolateChipCookies.asset`; existing bowl, measuring, board, tray, oven, and session consumers. No brownie/cupcake asset, new gameplay stage, score, mode, package, ProjectSettings, or cloud change.
- Baseline: active scene clean, recipe unassigned, Console zero errors/warnings. Unity synchronous command compilation passed after source changes.
- Edit Mode asset audit: bowl reference matched the new asset; ID/display/product labels correct; five requirements were Flour/Cup/1/Base, Sugar/Cup/1/Base, Butter/Whole/1/Base, Egg/Whole/1/Base, ChocolateChips/SmallSpoon/3/Finishing. Process was mix `4+3`, RollAndPortion `4`, yield `6 [6–9]`, Parchment, finish None, bake `5 s`, perfect `170–190 C`, burnt `>=220 C`. PASS.
- First Play Mode consumer audit: bowl, board, tray, oven, and session resolved the same definition instance. Actual Measuring Cup accepted flour/sugar and rejected chips; Small Measuring Spoon accepted chips and rejected flour. Outcome-name mapping and unlisted Cocoa lookup passed. Runtime configured counts/profile were 4+3, roll4, target6, 5s, 170–190. PASS.
- Runtime-only bowl receiver callbacks: removed only the runtime `MiseEnPlaceStation` component to isolate recipe validation; disposable `RecipeDataQA_*` Ingredients drove the existing receiver. Early chips and Cocoa were rejected, one each of flour/sugar/butter/egg accepted, duplicate flour rejected, base mixed at 4 callbacks, exactly 3 chips accepted, fourth chip rejected, and recipe completed at 3 finishing callbacks. PASS. This certifies data gates/state transitions, not manual stirring or a second full cookie walkthrough.
- Failed attempt: an initial read-only deterministic gate command imported `System.Reflection`; the Unity bridge rejected the command before compilation/execution as an unauthorized namespace. It changed no scene, asset, or runtime state. The callback-based test above replaced it.
- Final-source Play Mode regression after unlisted-ingredient tightening: all six consumers shared the asset, all configured values/outcome names matched, both tools also rejected absent Cocoa. PASS.
- Cleanup/final: exited Play Mode, reloaded the saved scene rather than saving runtime dirty state, zero `RecipeDataQA_*` objects, recipe still assigned, scene `dirty=false`. Final Console query: zero errors/warnings. No persistent verifier/helper, screenshot, package, ProjectSettings, or commit.

## 2026-09-09 15:21–18:43 +08:00 Oven preheat and retained-light verification

- Environment/scope: Unity `6000.5.0f1`; `Assets/BakeIT/Scenes/Kitchen Prototype.unity`; existing oven controller, door, bake zone, light/emissive renderer, display, start control/label, tray/liner, and cookie session guide. No extra kitchen prop, recipe value, package, or ProjectSettings change.
- Saved configuration audit: ambient `25 C`, preheat duration `5 s`, start control references the existing oven and label, saved label `PREHEAT`, scene `dirty=false`, and `Application.runInBackground=false`.
- Play Mode lifecycle: confirmed initial cold `25 C`, light off, PREHEAT control and SET TEMP display. `TryStartBake()` rejected the cold oven. An actual parchment-lined tray with six same-batch portions was registered and correctly blocked preheat; after removing it, preheat started.
- Heating/readiness: after two seconds, current temperature was strictly between 25 and 180 C, light was on, control read HEATING, and display showed PREHEAT. At target, temperature was 180 C, control read BAKE, display showed READY, and the light remained on. PASS.
- Bake/regression: re-registered the same complete tray, began the existing five-second 180 C bake, observed BAKING/countdown/light, and completed all six portions as `BakedCookie` with Perfect outcome. After completion the oven remained preheated at target, the light stayed on, and BAKE/READY returned. PASS.
- Exact verifier result: `OVEN_PREHEAT_QA_COMPLETE: PASS=True; initial=True; bakeBlocked=True; trayPrepared=True; loadedBlocks=True; removed=True; started=True; heatingUI=True; readyUI=True; reloaded=True; bakingUI=True; retainedAfterBake=True`.
- Fixture/compiler corrections: a read-only command using `GetInstanceID()` was rejected by Unity 6 before execution. The first multi-command timed attempt lost transient Play state on dynamic assembly refresh, so a temporary single-lifecycle verifier replaced it. Two `CS0618` warnings from a deprecated `FindObjectsByType` overload were fixed. One immediate post-refresh command hit a transient Unity bridge macro-evaluator lookup error; retry succeeded.
- Cleanup/final: temporary verifier `.cs`/`.meta` removed, Play Mode stopped, saved scene reloaded, zero preheat QA objects/markers remained, and compilation succeeded with zero project errors/warnings. Final Console had no errors and one already documented external Codex executable-signature warning (`SERVICE-002`), unrelated to gameplay. Technical logic is verified; normal player judgment of five-second pacing, label fit, and light readability remains open.

## 2026-09-09 19:01–19:05 +08:00 Metallic cookie tray visual/regression pass

- Environment/scope: Unity `6000.5.0f1`; `Assets/BakeIT/Scenes/Kitchen Prototype.unity`; existing `Prop_Tray`. Baseline scene clean. Its only renderer used shared `Panda Mat` (`metallic=0`, `smoothness=.5`) alongside 27 other scene renderers, so the shared asset was not edited.
- Change under test: new project-owned URP Lit `Bakeware Metal.mat`; base color `(.62,.65,.68,1)`, metallic `.85`, smoothness `.35`; assigned only to the cookie tray. No tray geometry, physics, receiver, parchment, cookie, oven, package, or ProjectSettings change.
- Play Mode steps/result: located the real tray, checked `Bakeware Metal` and `.85` metallic, found its Rigidbody, three colliders, `TrayLinerReceiver`, and `TrayReceiver`, then passed the body through the real `PickupController.TryPickupBody` path. Held-object identity matched the tray. Result marker: `METALLIC_TRAY_PLAY_QA PASS=True`.
- Visual result: framed Scene-view inspection showed a readable reflective silver tray distinct from the light parchment and surrounding counter. No screenshot asset was saved; player aesthetic acceptance remains open.
- Cleanup/final: exited Play Mode without saving runtime position and retained no QA object/helper. A final disk reload passed assignment, exact material values, all tray components, `dirty=false`, and stopped Play Mode. Console had zero errors and one transient Editor capture-tool warning (`Releasing render texture that is set to be RenderTexture.active!`) generated after the Scene-view preview returned; no runtime render texture or asset was retained and no project/gameplay stack was implicated.

## 2026-09-09 19:14–19:26 +08:00 Stored cookie tray, deep brownie pan, and rubber spatula

- Environment/scope: Unity `6000.5.0f1`; `Assets/BakeIT/Scenes/Kitchen Prototype.unity`; existing tray rack/bowl/cookie tray/worktop rest/utensils area; reused vendor `Prop_Tray` and final solid-head `Prop_Spatula_01`; new `Rubber Spatula.mat`. No brownie recipe/runtime stage or project/package setting changed.
- Saved layout: bowl unchanged on shelf; cookie tray `(-3.5,.425,4.12)`, yaw90; Brownie Pan `(-3.5,.613,4.12)`, yaw90, scale `(1.65,7,1.63)`; Rubber Spatula `(-3.15,1.23,4.12)`, rotation `(90,0,0)`. Visual inspection showed distinct shallow/deep metal pans on separate rails and visible green spatula beside the rack.
- Structure/material checks: both pans use `Bakeware Metal`; spatula uses nonmetallic `Rubber Spatula`. Brownie Pan has `.8 kg` Rigidbody, `PreparationItem`, `PickupPose`, bottom plus four-wall BoxColliders. Spatula has `.15 kg` Rigidbody, convex MeshCollider, `PreparationItem`, and `PickupPose`. Center ray hit the pan bottom `.0655 m` below the rim, confirming the intended open interior.
- Play interaction result: real `PickupController.TryPickupBody` and its private release dispatch were exercised for each item. Cookie tray and brownie pan released successfully through `StablePlacementSurface` onto `Tray Worktop Rest`; spatula released onto `Utensils Area`. Marker: `BAKEWARE_EQUIPMENT_QA PASS=True; materials=True; structures=True; openInterior=True; cookie=True/True; pan=True/True; spatula=True/True`.
- Failed first fit assertion: compared the sideways stored pan (`.40 x .08 x .56 m`) against the oven zone (`.92 x .52 x .51 m`) and failed Z containment. The saved pan was not oversized; the test omitted the physical rotation required when removing it from the narrow rack.
- Corrected oven fit: fresh Play Mode rotated the pan to the oven-zone orientation, centered its renderer bounds, and passed full containment at `.56 x .08 x .40 m`. Marker: `BROWNIE_PAN_OVEN_FIT PASS=True`. This certifies physical size, not a brownie bake/receiver that does not yet exist.
- Spatula visual correction: the first rack preview revealed that `Prop_Spatula_05` was a slotted turner rather than a solid batter-leveling utensil. It was replaced before handoff with solid spoon-style `Prop_Spatula_01`; the material/physics/pose stayed unchanged. Fresh Play Mode verified source mesh, configuration, pickup, and release: `SOLID_RUBBER_SPATULA_QA PASS=True`.
- Cleanup/limits: exited Play Mode and reloaded saved state; no preview/QA object retained. Scene-view capture produced only the known Editor active-render-texture cleanup warning and saved no screenshot asset. Human pickup feel and final aesthetic approval remain open; recipe selection and wrong-pan feedback are future work.
- Final disk-reload audit: exactly one cookie tray, one Brownie Pan, and one Rubber Spatula; zero preview/QA objects; exact positions, materials, and required components; `dirty=false`; Play Mode/background execution false. Final Console query returned zero errors/warnings.

## 2026-09-09 20:13–20:36 +08:00 Brownie bowl and pre-melted butter tilt transfer

- Environment/scope: Unity `6000.5.0f1`; `Assets/BakeIT/Scenes/Kitchen Prototype.unity`; new larger brownie bowl, pre-filled butter cup, dedicated liquid receiver, and golden transfer presentation. Existing cookie `BowlReceiver`, quantities, measuring tools, and recipe remained unchanged.
- Structure/visual checks: brownie bowl renderer bounds `.306 x .131 x .310 m` versus cookie bowl `.218 x .122 x .222 m`; new bowl uses `.5 kg` Rigidbody, `PreparationItem`, `PickupPose`, open base/12-wall collision and one trigger. Cup uses `.22 kg` Rigidbody, `PreparationItem`, `PickupPose`, existing solid collider, two labels, pour point, and visible `Melted Butter.mat` fill.
- Play Mode steps: pick up the real cup through `PickupController`; attempt upright contact over the receiver; tilt over the stored bowl; mark the bowl on `Mix Bowl Rest`; complete one transfer; refill only in the disposable runtime test and try a duplicate; position the same tilted cup over the cookie bowl; release cup and bowl through the pickup controller. Expected: only the single tilted pour into the brownie bowl on MIX succeeds.
- Actual: upright contact, stored-bowl pour, duplicate, and cookie-bowl target all rejected without improper consumption. The valid pour hid cup fill, showed golden transfer droplets and bowl contents, and latched exactly one received ingredient. Cup and bowl pickup/release passed. Marker: `BROWNIE_BUTTER_QA_COMPLETE: PASS=True; structure=True; larger=True ((0.306, 0.131, 0.310) vs (0.218, 0.122, 0.222)); visuals=True; cupPickup=True; uprightRejected=True; tilt=True; storageRejected=True; transfer=True; duplicateRejected=True; cookieRejected=True; cupRelease=True; bowlPickup=True; bowlRelease=True`.
- Failed verifier iterations: first lookup could not find the deliberately inactive bowl-fill child through `GameObject.Find`; corrected to hierarchy lookup. Second run stalled only at an automation `WaitForSeconds` because background Editor time stayed at zero; it was stopped and replaced by immediate release-path checks. Neither run changed the saved scene or indicated a gameplay failure.
- Cleanup/final: temporary verifier source/meta and preview camera removed; saved scene retained the user's manual rack corrections plus the two approved additions; final audit passed with one cookie bowl/receiver, no QA objects, `dirty=false`, Play Mode stopped, and zero Console errors/warnings. Human judgment of tilt feel, label readability, and the user's final bowl leveling remains pending.

## 2026-09-10 16:19 +08:00 Unity crash recovery check

- After the user reported and recovered from a Unity Editor crash, reopened `Kitchen Prototype` and inspected the saved scene without mutation.
- Expected: approved brownie bowl/butter components and user-authored equipment transforms remain saved; scene is clean and Console has no project errors.
- Actual: all expected objects/components and exact tray/pan/spatula transforms were present; `dirty=false`, Play Mode stopped, zero Console errors/warnings. `CRASH_RECOVERY_AUDIT PASS=True`. No data restoration was required.

## 2026-09-10 16:33 +08:00 Bowl-holder rename regression

- Scene: Unity `6000.5.0f1`, `Assets/BakeIT/Scenes/Kitchen Prototype.unity`. User-authored names under `Bowl Holder Rack`: `Cookie Bowl Holder` and `Brownie Bowl Holder`.
- Play Mode steps: resolve both holders by their saved names, verify both parent to the rack and retain colliders, then verify the cookie `BowlReceiver`, brownie `PourableIngredientReceiver`, and cup `PourableIngredient` still exist.
- Expected/actual: both holder names, parent relationships, supports, and both recipe foundations remained valid. `HOLDER_RENAME_PLAY_QA PASS=True; names=True; supports=True; systems=True`. Play Mode exited without saving runtime state; no gameplay change was needed.

## 2026-09-10 16:49–17:04 +08:00 Recipe clipboard and guide UI verification

- Baseline/scope: Unity `6000.5.0f1`; saved clean `Assets/BakeIT/Scenes/Kitchen Prototype.unity`; zero Console errors/warnings. Added only the approved wall recipe paper, its `E` selection path, a cookie-selection gate, plain-language guide headings, and the requested modest guide enlargement. Existing user-authored equipment transforms and recipe values were not changed.
- Visual inspection: rendered the scene from the work area. The clipboard is centered beneath the BakeIT sign; its wood/paper/clip silhouette is clear; cookies read `READY`; brownies/cupcakes read `IN DEVELOPMENT`. The two lower rows were raised slightly so the cupcake line clears the worktop edge. Preview camera/texture were temporary and no scene preview object was retained.
- Play Mode interaction: confirmed the initial session had no selection, showed `Choose a Recipe / RECIPE SELECTION`, and had not completed the tutorial. A real `PickupController` forward ray aimed at Brownies consumed the interaction, kept selection empty, and produced the unavailable warning. The same ray aimed at cookies selected `chocolate-chip-cookies` and changed the guide to `Movement Tutorial / 1 / 13 BASIC CONTROLS`.
- Result: `RECIPE_CLIPBOARD_QA PASS=True; structure=True; initialGate=True; lockedRay=True; cookieRay=True; position=True; heading='Movement Tutorial'; stage='1 / 13  BASIC CONTROLS'`.
- Restart regression: selected cookies, invoked the production `RestartRecipe()` scene reload, then confirmed the persistent session retained `chocolate-chip-cookies` and returned to `Movement Tutorial` after reset. `RECIPE_SELECTION_RESTART_QA PASS=True; selected=chocolate-chip-cookies; reset=False; heading=Movement Tutorial`.
- Final saved-scene audit: clipboard present, exactly three rows/one available, zero preview or QA objects, `dirty=false`, Play Mode stopped, and Console zero errors/warnings. A rejected preliminary QA command imported the disallowed `System.Reflection` namespace; the bridge refused it before execution and no state changed. A later `ScreenCapture.CaptureScreenshot` request did not create its Library PNG in this background Editor session; the direct scene render remained the visual evidence and neither attempt changed saved state. Human selector discovery/aim and 150% HUD readability remain for player acceptance.

## 2026-09-10 17:11–17:16 +08:00 Separate framed recipe-poster revision

- User visual clarification: use individual framed recipe papers like the supplied wall-art reference instead of one large clipboard. The image was used only for visual structure; it supplied no gameplay or recipe authority.
- Scene change: removed exactly `Kitchen Environment/Recipe Clipboard` and added `Kitchen Environment/Recipe Selection Posters` at the same center. Three independent `.70 x .74 m` dark frames with `.62 x .66 m` cream papers were spaced at local X `-.76/0/.76`. Cookie and approved Brownie summaries were shown; Cupcakes says its recipe details are still to be finalized.
- Visual render: all three frames read as distinct pieces beneath BakeIT, with titles/statuses and compact summaries visible above the worktop. No camera was saved to the scene.
- Play Mode: verified old board absent, new root/marker present, exactly three independent button posters with frame/paper children, one available recipe, expected horizontal spacing, initial selection gate, locked Brownie warning, and Cookie transition to `Movement Tutorial`. Marker: `SEPARATE_RECIPE_POSTERS_QA PASS=True; structure=True; spacing=True; initialGate=True; lockedRay=True; cookieRay=True; oldRemoved=True`.
- The user said they will put the Melted Butter Cup in dry storage. Codex did not move it; the poster revision does not depend on its storage transform. Existing live dirty scene state was preserved and saved with the requested replacement.

## 2026-09-10 20:55–22:10 +08:00 Exact measurements, wall rack, labels, and walnut foundation

- Scope: approved cookie formula; exact measuring set; hanging tool storage; a wider active tool rest; one-sided ingredient labels; and the now-approved brownie walnuts. The supplied hanging-utensil image was used only as a visual/layout reference. Existing user-authored pan, bowl, tray, spatula, and Melted Butter Cup transforms were preserved.
- Data result: `ChocolateChipCookies.asset` now has eight requirements—1 cup flour, 1 cup sugar, 1/2 cup softened butter, 1 egg, 1 teaspoon vanilla, 1/2 teaspoon baking soda, 1/4 teaspoon salt, and 3 chip scoops. Mix/shaping/yield/parchment/bake values stayed 4+3/4/6/unchanged/5 seconds at 170–190 C.
- Scene result: seven marked measuring vessels and the whisk hang on eight role-specific snap hooks. `Utensils Area` X scale is `.86`. Vanilla, baking soda, salt, and visibly modeled walnut pieces are in carryable jars leveled at shelf Y `.405`. Ten affected ingredient carriers have one backed face; reverse-label count is zero. Cookie and brownie poster summaries fit their individual papers; brownies remain `IN DEVELOPMENT`.
- Play Mode tool gates: 1 cup accepted Flour and rejected Vanilla; 1 teaspoon accepted Vanilla; 1/2 teaspoon accepted BakingSoda; 1/4 teaspoon accepted Salt; chip scoop accepted ChocolateChips and rejected Flour. Recipe count was eight; rack count was eight; expanded rest, walnut source, and one-sided labels passed.
- Functional rack regression: the first return test exposed a stale `Rigidbody.worldCenterOfMass` after a kinematic tool detached from its rack parent. `ToolHangingSlot` now measures snap distance from the stable Rigidbody pivot. Fresh Play Mode pickup/return passed for the 1/2 Cup Measuring Cup and whisk through the real `PickupController` and matching slot.
- Mise-en-place audit: all eight ingredient carriers, the five cookie-relevant measured tools, whisk, and bowl reported their correct prepared flags. The 1/2- and 1/4-cup future tools remain optional for cookie setup. The latch itself was not frame-advanced by the synchronous bridge command; all inputs to it were true.
- A first deterministic full-bowl test command imported `System.Reflection`; the Unity bridge rejected it before execution. No state changed. Exact gates and ingredient metadata were verified through public runtime APIs instead.
- Final: Play Mode stopped, scene `Assets/BakeIT/Scenes/Kitchen Prototype.unity` clean, 7 measuring tools, 8 rack slots, 0 reverse labels, all four new jars shelf-level, and Console 0 errors / 0 warnings. No persistent QA object/script, package, ProjectSettings change, or commit.
- A final fresh Play Mode stability check let physics advance, confirmed all four new pantry jars remained above Y `.35` on their support, then exited without saving runtime state. Final compile/scene audit again reported stopped Play Mode, `dirty=false`, and Console 0/0.
- Vanilla source check: the held 1 Teaspoon overlapped the source trigger, remained upright, and reported both `CanFillFrom=true` and `CanMeasure(Vanilla)=true`. This background bridge did not advance a trigger-stay frame while the held-position controller was disabled, so state remained empty during two frame-dependent attempts. Invoking the actual `OnTriggerStay` callback with the overlapping utensil filled `Vanilla` and passed. This is callback verification; natural dip feel remains for a normal Game-view pass.

## 2026-09-10–11 Centered onboarding, hover labels, exact chip cup, and clean outlines

- Scene: `Assets/BakeIT/Scenes/Kitchen Prototype.unity`; Unity 6000.5.0f1.
- Edit audit: PASS. Player start remained `(0,0,4.5)`; `Utensils Area` X returned to `.412`; `Tool Home Mat` reached `.76 x .68`; dry-storage Y reached `1.18`; posters moved above Y `1.7`; 23 initial/29 final hover targets existed; carryable-body TextMesh count was zero; six measuring vessels and seven hooks remained; 1/2 Cup accepted chocolate chips; cookie data reported `1 stick of butter`, one chip measure, and `HalfCup`.
- Initial guided state: PASS. Before any recipe selection the guide reported `Movement Tutorial / 1 / 13 BASIC CONTROLS`, three checklist rows, and yellow target `Prop_WhipMixer`. Attempting the available Cookie selector was handled but did not select while the tutorial was incomplete.
- Outline correction: the first point-light/ring preview visibly scattered across the wall and was rejected by the user. It was replaced and old runtime preview roots were removed. Clean Play Mode reported exactly 24 reusable outline edges (yellow + red boxes), zero tutorial Lights, and no preview roots after exit.
- Hover/exact-measure test: PASS. From interaction range, the direct camera ray returned the upright `Whisk` label. The real pickup controller held the 1/2 Cup measure; `TryFill("ChocolateChips")` succeeded and emptied the same ingredient. Early recipe selection remained blocked.
- Misplacement state: PASS. With the production delayed-correction state evaluated deterministically at a zero-duration test override, a released 1/2 Cup showed red while `Utensils Area` showed yellow. The saved delay remains 3.5 seconds; the runtime override was discarded on Play exit.
- Restart threshold: PASS. A `.2 s` tap did not trigger; releasing reset progress; `2.99 s` remained below threshold; an additional `.02 s` crossed the three-second threshold. The instant HUD button is absent.
- Final state: Play Mode stopped, active scene clean, zero runtime preview roots, and Unity Console 0 errors / 0 warnings. The user's final fixed world-text orientations were preserved and not rewritten during continuation.

## 2026-09-11 Held-item hover suppression

- Compilation: PASS after refreshing `HoverLabelController.cs`.
- Focused Play Mode control: direct Whisk targeting with empty hands returned `Whisk`.
- Focused Play Mode behavior: pickup through `PickupController.TryPickupBody` succeeded, then `HoverLabelController.RefreshNow()` returned an empty label while the item was held.
- Runtime collider/camera positioning used only for deterministic QA and was discarded on Play Mode exit; no scene or world-text transform was saved.

## 2026-09-11 Recipe-sheet text contrast

- Saved-scene check: `RECIPE_TEXT_QA labels=9; black=9; outsideUntouched=19; sceneClean=True; playing=False`.
- Scope check passed: only TextMesh descendants of `Recipe Selection Posters` were recolored; no title/body transforms or non-recipe UI text changed.

## 2026-09-11 Recipe difficulty labels

- Saved-scene inspection confirmed three orange `Recipe Title` labels and the exact status strings `DIFFICULTY: EASY`, `DIFFICULTY: MODERATE`, and `DIFFICULTY: INTERMEDIATE`.
- Fit check: status widths were `.252 m`, `.335 m`, and `.384 m`; all remain within each `.62 m` paper. Scene transforms and selector logic were unchanged.

## 2026-09-11 Expanded recipe wording

- Saved-scene inspection confirmed one fully written ingredient/process item per line and no ` / ` separators in any of the three recipe summaries.
- Bounds check passed against each `.620 x .620 m` paper: Cookie `.310 x .279 m`, Brownie `.296 x .301 m`, Cupcake `.378 x .116 m`. Detail character size is `.0043`. The user later moved the Cookie/Brownie summaries to local Y `.126`; Codex saved that live dirty scene before Brownie work.

## 2026-09-11 Brownie recipe-data and butter-receiver foundation

- Unity 6000.5.0f1; `Assets/BakeIT/Scenes/Kitchen Prototype.unity`; Play Mode.
- Data check: `BROWNIE_DATA_QA measures=True; quantities=True; process=True; binding=True; cookieSafe=True`. Brownie Flour accepts the exact 1 Cup and 1/4 Cup tools but rejects 1/2 Cup; Cocoa and Walnuts accept 1/2 Cup. Sugar requires 2, Eggs require 4, and the definition contains 10 entries.
- Process check: Brownie data reports 8 seconds, 170–180 C, 12 portions, pour-into-pan shaping, and parchment. The existing large-bowl receiver resolves `Brownies.asset`. Cookie remains 5 seconds/6 portions and still rejects 1/4 Cup for Flour.
- Physical transfer: placed the existing large bowl logically on the real `Mix Bowl Rest`, picked up the saved `Melted Butter Cup` with `PickupController`, tilted it 90 degrees with its pour point in the receiver, and invoked the production transfer path. `BROWNIE_BUTTER_QA held=True; tilted=True; destination=True; transferred=True; state=True`.
- Negative path: refilled the runtime cup after the accepted transfer and attempted a duplicate. `BROWNIE_DUPLICATE_QA rejected=True; sourceRetained=True`; the extra butter was not consumed.
- Test transforms/state were runtime-only and discarded on Play Mode exit. Brownie poster stayed locked.
- Final audit first used an over-rounded `.126` equality expectation for the user-edited Cookie summary and reported `userLayout=False`; direct read showed the saved values are `.125740` for Cookie and `.126100` for Brownie. This was an audit assertion error, not a scene change or gameplay defect. The corrected final audit passed asset, receiver binding, locked poster, stopped Play Mode, and clean scene.

## 2026-09-11 Brownie measured mixing and development selection

- Measured-intake verification passed the existing held/tilted transfer paths for Sugar, both exact Flour tools, Cocoa, Vanilla, Baking Powder, Salt, and Walnuts, plus deliberate melted Butter and four whole Eggs. Base readiness, four base passes, Walnut gating, two finishing passes, progressive batter presentation, and Cookie-rule isolation all passed.
- Selection verification passed with the real movement/whisk tutorial conditions: Brownies was rejected before tutorial completion, accepted afterward, became the active recipe, and propagated Brownie capacity rules to the measuring set. Brownie was available and Cupcakes remained locked.
- Boundary verification repeated the full Brownie ingredient intake through completed batter and confirmed the guide reported `Brownie Batter Ready`, `NEXT PART STILL IN DEVELOPMENT`, and `Pan spreading and baking will be added next`.
- QA-only corrections: the Unity command sandbox rejected a reflection-based shortcut, and direct state mutation of the keyboard bitfield was unsupported. Neither attempt changed a saved asset or scene. The successful tutorial test used queued Input System states and public pickup/placement behavior.

## 2026-09-11 Brownie tutorial and outline correction

- Reproduced the player's mismatch: Brownie UI requested the large bowl while the existing normal target was still determined by Cookie mise-en-place state.
- Clean Play Mode check `BROWNIE_TUTORIAL_FINAL_QA pass=True` completed the actual movement/whisk gate, selected Brownies, and verified: `Brownie Mise En Place` with fridge outline; opening-to-first-Egg transition; four-Egg count; dry-storage stage; Brownie-only tool stage; large-bowl stage; `2 cups` Sugar copy; and the first 1-cup measure target.
- Held-measure targeting passed separately and in the final sequence: empty 1-cup measure outlined Sugar, then a filled Sugar measure outlined the Brownie bowl.
- Cookie regression `COOKIE_TUTORIAL_REGRESSION pass=True` confirmed Cookies still enter their original `2 / 13 COLD INGREDIENTS` guide with the fridge outlined.

## 2026-09-11 Contour, held-object, crosshair, and sugar QA

- Shader import/compile: PASS. `BakeIT/Tutorial Silhouette Outline` was found and supported by the active renderer.
- Runtime contour structure: PASS. The highlighted Egg produced two close mesh-outline parts and zero tutorial `LineRenderer` boxes.
- Held-target transition: PASS. Picking up the guided Egg immediately removed the Egg outline and moved the yellow target to `Cold Ingredient Rest`.
- Held Egg alignment: PASS. The combined visible Egg bounds aligned to the camera forward vector at `1.00000`; Rigidbody and Transform position delta was `0.00000`.
- Crosshair inspection: PASS at `10 x 10`, increased from `6 x 6`.
- Brownie Sugar transfer: PASS through the production held, tilted 1-cup measuring path. The receiver recorded one Sugar measure; its visible contents were active, compact (`localScale.y = .001713` at that progress), and off-white `RGBA(.980,.970,.930,1)` before Cocoa.

## 2026-09-11 Teacher playtest usability regression

- Flat destination guidance: PASS. After completing the real movement/whisk input checks while holding the Whisk, `Utensils Area` used one looping four-point perimeter, zero mesh shells, and a `.021 m` pulse peak. The mat face was not duplicated or filled.
- Scoop assistance: PASS. A held upright 1-cup measure collected Flour with its mouth `.040 m` outside the original source collider. Overlapping incompatible Cocoa was ignored; the compatible Flour source filled the cup.
- Stuck recovery: PASS. A held Egg was deliberately scrolled from `.75 m` to the maximum `2.50 m` and blocked by a temporary obstacle. The production recovery returned both its scroll distance and visible center to `.60 m` in front of the player.
- QA objects and transforms were runtime-only and are discarded on Play Mode exit.

## 2026-09-12 Cookie usability and guide consistency verification

- Unity 6000.5.0f1; `Assets/BakeIT/Scenes/Kitchen Prototype.unity`; Edit Mode structural audit and connected Play Mode verification.
- Held poses: PASS. A live sweep picked up and released all 27 active `PreparationItem` bodies. Every body matched either its explicit `PickupPose` or the corrected front-facing fallback; failures: zero.
- Baking Soda: PASS. The saved open-carton visual contains 12 renderers and zero colliders, while its root retains the exact `BakingSoda` measured-source trigger. Direct pickup confirmed its front-facing held rotation.
- Recipe/UI configuration: PASS. The initial live stage is `BASIC CONTROLS` without a phase fraction; source audit found no remaining numbered stage headers or grouped cup/spoon shorthand. The progressive tool copy lists each required Cookie tool separately.
- Portions: PASS. Live `DoughBoardController.TargetCookieCount` and active-recipe maximum both reported six; the saved board minimum/target/maximum are all six.
- Oven: PASS. Live selected/current temperature started at `0 C`; preheat at zero was rejected; the first positive control step selected `100 C`.
- Guidance: PASS for live initial target (`Prop_WhipMixer`) and structural state routing through the later Cookie stages, including tray/parchment/portion targets that were previously absent.
- Final verifier-free Console query: zero errors and zero warnings. Play Mode stopped after testing.

## 2026-09-11 Reference-driven Sugar Jar asset

- Unity 6000.5.0f1; `Assets/BakeIT/Scenes/Kitchen Prototype.unity`; Edit Mode structural audit plus corrected final Play Mode verification.
- Asset/structure: PASS. `Sugar Jar Visual.prefab` resolves beneath `SugarContainer`, contains 20 renderers and zero colliders, and its supporting glass/sugar/metal/seal materials plus rounded meshes resolve from `Assets/BakeIT/Generated/Sugar Jar/`.
- Preservation: PASS. Root `Rigidbody`, `PreparationItem`, `HoverDisplayName`, `SugarScoopZone`, and `MeasuredIngredientSource` remain present; source name remains exactly `Sugar`. Old milk-bottle rendering is disabled and the old moving MeshCollider is absent.
- Pickup/release: PASS through `PickupController.TryPickupBody` and `ReleaseForPlacement` for the production `SugarContainer` Rigidbody.
- Measuring: PASS. The held `1 Cup Measuring Cup` accepted `Sugar`, reported the correct filled contents, emptied the same ingredient, and released successfully.
- Visual review: PASS for the isolated three-quarter preview. The jar reads as clear rounded glass containing off-white sugar with an open hinged lid and metal front clamp; the open mouth was refined by omitting the body's top face.
- Diagnostic corrections: one earlier pickup QA cleanup wrote linear/angular velocity to a kinematic body and generated two warnings; the corrected final Play Mode test omitted those writes and passed. A compile-only audit replaced obsolete `GetInstanceID` with `GetEntityId`. A console-clear experiment using `System.Reflection` was rejected before execution. No saved runtime state or production behavior was affected.
- Result: PASS. Play Mode stopped after verification; only Sugar was changed in this asset pass.

## 2026-09-12 Flour, Cocoa, and Baking Powder model pass

- Unity 6000.5.0f1; `Assets/BakeIT/Scenes/Kitchen Prototype.unity`; isolated three-quarter render review, structural audit, and Play Mode.
- Visual review: PASS. Flour reads as an ivory vintage rectangular tin with red raised lid and wheat badge; Cocoa reads as a red silver-rimmed tin with cocoa-bean badge; Baking Powder reads as a smaller cream/red can with navy badge and translucent raised lid. Each shows the correct ingredient inside its open mouth.
- Relative sizing: PASS. Closed bodies are approximately Flour `.138 x .200 x .110 m`, Cocoa `.104 x .160 x .104 m`, and Baking Powder `.094 x .150 x .094 m`. Open lids intentionally extend the total visual bounds.
- Structure: PASS. Prefabs resolve beneath the intended source roots; Flour has 26 renderers, Cocoa 14, Baking Powder 15, and all three visual hierarchies have zero colliders. Root body BoxColliders remain the only non-trigger physical shell for these models.
- Preservation: PASS. Each source retains Rigidbody, preparation state, hover labeling, and its original measured trigger. Ingredient identifiers remain exact: `Flour`, `Cocoa`, and `BakingPowder`.
- Pickup/release: PASS for `FlourContainer`, `Cocoa Powder Container`, and `Baking Powder Container` through the production pickup controller.
- Correct-tool measurement: PASS with `Brownies.asset`: 1 Cup filled/emptied Flour, 1/2 Cup filled/emptied Cocoa, and 1/2 Teaspoon filled/emptied BakingPowder.
- Negative path: PASS. 1 Cup rejected Cocoa and remained empty; 1 Teaspoon rejected BakingPowder and remained empty.
- Corrections: the experimental reversed flour mesh winding was reverted after its preview hid the rounded body/fill. One QA script had a compile-only duplicate variable-name error; its corrected version produced `THREE_PANTRY_FINAL_QA pass=True`. No runtime state from tests was saved.
- Final result: PASS. Play Mode stopped and no temporary preview root remained. The only baseline Console warning was the pre-existing Unity AI MCP executable-signature diagnostic, unrelated to project scripts.

## 2026-09-12 Salt, Vanilla, and Chocolate-Chip Bowl model pass

- Unity 6000.5.0f1; `Assets/BakeIT/Scenes/Kitchen Prototype.unity`; isolated three-quarter renders, Edit Mode structural audit, and Play Mode interaction verification.
- Visual review: PASS. Salt reads as a compact ivory ceramic cellar with an open lid and wooden spoon; Vanilla reads as a tall clear/amber bottle with green cap and flower label; Chocolate Chips read as dark chip clusters held by a white ceramic prep bowl.
- Relative sizing: PASS. Salt body collider `.066 x .082 x .066 m`; Vanilla bottle collider `.050 x .154 x .050 m`; chip bowl visual approximately `.18 x .11 x .18 m`. Salt remains substantially shorter than Vanilla even with its raised lid.
- Structure: PASS. Salt has 10 visual renderers, Vanilla 24, and Chocolate-Chip Bowl 4; each new visual hierarchy contains zero colliders. The chip source retains nine active non-trigger physical colliders from its established hollow compound shell.
- Preservation: PASS. Root Rigidbodies, `PreparationItem`, `HoverDisplayName`, saved scene transforms, and measured-source children remain present. Ingredient identifiers remain exactly `Salt`, `Vanilla`, and `ChocolateChips`.
- Pickup/release: PASS for `Salt Container`, `Vanilla Extract`, and `ChocolateChipContainer` through `PickupController.TryPickupBody` and `ReleaseForPlacement`.
- Correct tools: PASS. Brownies 1/2 Teaspoon filled/emptied Salt; Brownies 1 Teaspoon filled/emptied Vanilla; Cookie 1/2 Cup filled/emptied Chocolate Chips.
- Negative paths: PASS. Brownies 1 Teaspoon rejected Salt, Brownies 1/2 Teaspoon rejected Vanilla, and Cookie 1 Cup rejected Chocolate Chips; rejected tools remained empty.
- Corrections: a missing rotated-cube helper produced one compile-only builder error before scene mutation. Two later attempts to clear historical Console entries were compile/sandbox rejected and made no changes. Final production builder and validation commands compiled and executed successfully.
- Final result: PASS. Play Mode stopped, scene saved/clean, and no temporary preview roots remain. Walnut bowl and wrapped Butter were not modified.

## 2026-09-12 Screenshot-driven Flour and Chocolate-Chip correction

- Visual review: PASS. The Flour Tin now shows an off-white flour surface directly below its open rim instead of reading as empty, and its cream panel visibly says `FLOUR` using collider-free modeled strokes. The Chocolate-Chip Bowl now has a packed base and six overlapping chip layers rather than a sparse three-layer presentation.
- Builder rerun regression: PASS. Flour body, flour fill, flour lid, and ceramic bowl custom meshes all resolve after reapplication and forced import; no stale custom-mesh references remain.
- Structure: PASS. Flour and chip presentation children contain zero colliders. `ChocolateChipContainer` retains exactly nine active non-trigger physical colliders in its established hollow compound shell.
- Play Mode recipe regression: PASS. `Brownies.asset` 1 Cup filled and emptied `Flour`; `ChocolateChipCookies.asset` 1/2 Cup filled and emptied `ChocolateChips`; Cookie 1 Cup rejected `ChocolateChips`.
- Preservation: PASS. Both roots retain their Rigidbody and measured-source children with exact IDs `Flour` and `ChocolateChips`.
- Final state: PASS. Play Mode stopped, scene saved/clean, zero temporary preview roots, and Unity Console 0 errors / 0 warnings.

## 2026-09-12 Chocolate-Chip Bowl grounding follow-up

- The user's review identified that the upper chip clusters appeared to float. Updated the prefab and its rerunnable builder so all six visible cluster bottoms meet and slightly intersect the packed bed at local Y `.056`.
- Numerical containment check: each cluster uses `positionY = .056 - meshBounds.min.y * scale`, with shared mesh minimum Y `.2025781`; therefore each lowest transformed chip vertex is exactly `.056` before rotation around Y and cannot hover above the bed.
- This is a presentation-only correction. The measured source, pickup Rigidbody, and established bowl collision hierarchy were not modified.
- Disk audit passed six active grounded layers, two inactive retired layers, zero visual colliders, and the scene's existing prefab reference. The first relay queue stopped draining after a domain reload; after a clean Unity restart, the live builder rerun and isolated render both passed and showed a supported shallow mound. The previous chip fill/empty Play Mode regression remains applicable because no gameplay component or scene collider changed.

## 2026-09-12 Walnut Bowl and Wrapped Butter model pass

- Unity 6000.5.0f1; `Assets/BakeIT/Scenes/Kitchen Prototype.unity`; independent builder reruns, isolated render review, and Edit Mode structural audit.
- Visual review: PASS. `Walnut Bowl Visual.prefab` presents a clear glass prep bowl with an uneven supported bed, sixteen individually arranged chopped kernels/halves, and no floating detail geometry. `Wrapped Butter Visual.prefab` presents a 120 x 30 x 30 mm warm wax-paper stick with overlapping panels, end folds, crease lines, and a modeled blue `BUTTER` package mark on both long sides.
- Relative sizing: PASS. Walnut bowl body is approximately `.145 x .070 x .145 m`; wrapped Butter resolves through its preserved root scale to approximately `.120 x .030 x .030 m`.
- Preservation: PASS. `Walnut Container` remains at `(0.97,0.48,3.08)` with `Walnuts` measured-source identity; `Food_Butter` remains at `(-1.55,1.21,3.13)` with local scale `(.12,.05,.07)` and `Butter` ingredient identity. Both retain Rigidbody, `PreparationItem`, and `HoverDisplayName`.
- Structure: PASS. Both generated visual hierarchies contain zero colliders. Walnut legacy jar/fill/lid/half presentation children are inactive and its measured trigger remains present. Butter's primitive root renderer is disabled and its fitted root collider is local size `(1,.60,.4286)`.
- Cleanup: PASS. Scene saved/clean, Play Mode stopped, zero temporary preview cameras/lights, and no hosted model generation used.

## 2026-09-12 Measuring-tool and work-area visibility pass

- Unity 6000.5.0f1; `Assets/BakeIT/Scenes/Kitchen Prototype.unity`; Edit Mode structural audit plus a 1920 x 1080 standing-height render.
- Tool contrast: PASS. All six `MeasuringScoop` roots use `MeasuringTool_Copper`; `_EMISSION` is disabled on every tool. Fill child renderers retain their separate ingredient materials.
- Work-area identity: PASS. Exactly five active permanent TextMesh labels identify `INGREDIENTS`, `TOOLS`, `MIXING`, `ROLL + CUT`, and `LINE + LOAD`. The earlier tiny duplicate tool-area caption is disabled, and the four fascia labels use bold `.0062` character sizing.
- Render review: PASS after one revision. The first two-line captions were too fine at normal distance, so they were simplified to larger single-line area names. `ROLL + CUT` and `LINE + LOAD` were raised above their physical surfaces to avoid depth overlap.
- Preservation: PASS. No measuring capacities, pickup/physics components, content visuals, tutorial-highlight logic, placement colliders, or recipe progression were changed. Scene saved; no temporary preview object remained; final Unity Console query reported zero errors and zero warnings.

## 2026-09-12 Baking-tool rack return pass

- Edit Mode: PASS. Exactly seven `ToolHangingSlot` components retain `.55 m` close snap, `.22 m` crosshair tolerance, and `2.5 m` aim-assist distance. The scene contains one `BAKING TOOLS` heading and one `AIM AT RACK + PRESS E TO RETURN` hint, and saved cleanly.
- Play Mode positive path: PASS. Whisk, 1 Cup, 1/2 Cup, 1/4 Cup, 1 Teaspoon, 1/2 Teaspoon, and 1/4 Teaspoon all returned to their matching placement transforms while deliberately held `1.05 m` from the hook, proving the crosshair path rather than the near-snap path accepted them.
- Play Mode negative paths: PASS. A measuring tool aimed at a different hook remained held; a ray intentionally missing its correct hook by more than `.22 m` remained held; aiming back at its matching hook then returned it successfully.
- Post-snap state: PASS for all seven tools. Each tool was parented to its matching placement point, kinematic, and gravity-disabled after return.
## 2026-09-12 Brownie pan, spreading, and oven stages

- Unity 6000.5.0f1; `Assets/BakeIT/Scenes/Kitchen Prototype.unity`; focused Play Mode state verification and final Edit Mode audit.
- Structure: PASS. The saved deep pan contains one `BrowniePanReceiver`, parchment anchor, trigger work zone, inactive temporary slab, and the existing Rubber Spatula contains `BrownieSpreadingTool`. Scene was clean after testing.
- Pan state: PASS. Parchment placement was accepted and stabilized; completed batter transferred once from the held/tilted Brownie bowl; four required spreading passes produced the oven-ready state.
- Oven state: PASS. The Brownie pan registered as loaded, the shared oven started with `Brownies.asset`, and an approved-temperature completion produced `Perfect` / `BakedBrownie` at the 8-second recipe duration and 170-180 C band.
- Outcome/profile regression: PASS. Brownie result names resolved as `UnderbakedBrownie`, `BakedBrownie`, `OvercookedBrownie`, and `BurntBrownie`. Switching to `ChocolateChipCookies.asset` restored 5 seconds and 170-190 C, then switching back restored the Brownie profile.
- Guide-state correction: PASS by compiled state audit. Completed Brownies are recognized as still loaded and the guide checks `BakeCompleted` before preheat/load routing; the contour returns no target after completion.
- Final compile/audit: PASS. `BROWNIE_EDITMODE_AUDIT playing=False; compiling=False; receiver=True; spatulaMarker=True; dirty=False`.
- Final Unity Console query: zero errors and zero warnings.
- Human acceptance still required for natural bowl-tilt duration, physical four-stroke recognition, pan handling/loading, UI wording/readability, and the temporary visual. The final art will be replaced from user-supplied references without changing gameplay logic.

## 2026-09-13 Fuller Brownie bowl presentation

- Unity version: 6000.5.0f1.
- Scene: `Assets/BakeIT/Scenes/Kitchen Prototype.unity`.
- Feature: fuller saved Brownie-batter presentation and containment; A/B/C concept generation was preview-only and not treated as gameplay validation.
- Steps: audited the inactive visual and bowl bounds; changed the batter local transform; entered Play Mode; temporarily activated the presentation and moved only the runtime bowl to an isolated render setup; verified the saved transform after `Awake`; captured the focused view; exited Play Mode; checked saved state, scene dirtiness, temporary-object cleanup, and the Unity Console.
- Expected: completed batter reads fuller without clipping through the approximately `.31 x .13 x .31 m` bowl; no runtime preview objects or state persist.
- Actual: `BROWNIE_BOWL_PLAY_QA pass=True`; batter bounds approximately `.19 x .056 x .19 m`; bowl bounds approximately `.31 x .13 x .31 m`. Final audit reported `playing=False`, `sceneDirty=False`, and `previewObjects=0`.
- Result: PASS for saved transform, runtime preservation, and visual containment. Player aesthetic acceptance remains pending.
- Console: the Unity camera helper rejected a rounded entity ID and logged one tool-side error; the successful RenderTexture fallback replaced that attempt. After cleanup, the final Console query reported zero errors and zero warnings.
- Evidence: `BakeIT_Notes/evidence/brownie-bowl-fuller-2026-09-13.png`.
- Bugs discovered: no product bug. Final surface detail remains intentionally pending the user's A/B/C selection.

## 2026-09-13 Selected B1/B2/A3 Brownie food art

- Unity version: 6000.5.0f1; scene: `Assets/BakeIT/Scenes/Kitchen Prototype.unity`.
- Expected: B1 fills the green bowl with rustic dark batter and walnuts; B2 is the raw pan state; a successful bake switches to the smoother A3 mesh/details; the pan and recipe mechanics remain unchanged.
- Edit Mode: PASS. Assets resolve as `Brownie_B1_Bowl`, `Brownie_B2_RawPan`, and `Brownie_A3_BakedPan`; B1/B2/A3 detail counts are 12/10/11 root children respectively; scene saved cleanly.
- Play Mode: PASS. `BROWNIE_ART_ACCEPTANCE pass=True; B1=True; B2raw=True; A3baked=True`. B2 raw details deactivate when A3 baked details activate, and switching back restores B2.
- Visual correction: the initial B1 radial surface was back-face culled. An isolated double-sided test exposed the cause; `_Cull = 0` fixed it. A separate early preview incorrectly left the mutually exclusive melted-butter surface active; the final capture matches the recipe state with butter hidden and batter shown.
- Evidence: `BakeIT_Notes/evidence/brownie-B1-bowl-final.png`; earlier side-by-side state evidence remains at `BakeIT_Notes/evidence/brownie-B1-B2-A3-playmode.png`.
- Preservation: no colliders, Rigidbodies, recipe quantities, mixing gates, pan geometry, transfer rules, spreading requirements, oven values, or result identifiers changed.

## 2026-09-13 Brownie progression, labels, and fitted liner

- Unity 6000.5.0f1; scene `Assets/BakeIT/Scenes/Kitchen Prototype.unity`.
- Visual inspection: PASS. The isolated Baking Soda carton displayed one correctly sized front label. The Brownie bowl appeared full rather than hollow; the fitted liner covered the pan bottom and four sides; raw batter rested on it; Walnut silhouettes were irregular rather than heart-shaped.
- Play Mode progression: PASS. Early ingredient state width was `.153 m`, complete state width was `.294 m`, and no Walnut detail was active early. Completion retained the existing six-pass gate and used `RGBA(.340,.105,.035,1)`.
- Geometry: PASS. B1 and B2 meshes were closed, and every sampled Walnut cluster contained three asymmetric lobes.
- Parchment: PASS. Placement activated five liner parts; translating and rotating the held pan retained `0.0000 m` anchor error; B2 bottom gap was `.0024 m`; reset restored the original parchment state.
- Regression: PASS. `OvenBakeZone.OnTriggerExit` no longer emitted the pre-existing null teardown error.
- Final assertion: `FINAL_BROWNIE_ACCEPTANCE_PASS early=0.153 full=0.294 color=RGBA(0.340,0.105,0.035,1.000) linerParts=5 anchorError=0.0000 batterGap=0.0024`.
- Failed QA attempts: one duplicate builder variable, one incorrect sibling receiver lookup, two diagnostic-script type/lookup errors, and one camera-helper failure were corrected before final validation.
- Final state: Play Mode stopped; final Unity Console contained zero errors and zero warnings.

## 2026-09-13 Ingredient label, fill, and Walnut follow-up

- Visual QA: PASS after revision. Cocoa and Baking Powder front names render outside their decorative panel bounds. Flour and Sugar have visible contained surfaces. The final Walnut source uses faceted golden crinkled chunks rather than the rejected smooth coffee-bean-like first pass; temporary isolated review objects were removed.
- Play Mode: `INGREDIENT_VISUAL_PLAYMODE_PASS`.
- Regression: Flour/Sugar/Walnuts/Cocoa/BakingPowder retained exact source IDs, Rigidbodies, and `PreparationItem`; Flour/Sugar mound tops stayed below their rims; Sugar contained 18 visible crystals; the Walnut visual contained 34 chopped pieces, 34 craggy mesh instances, zero colliders, and no paired left/right lobes; all 84 revised Brownie Walnut lobes remained hidden before completion.
- Final state: stopped Play Mode, clean saved scene, zero review objects, and zero Console warnings/errors.

## 2026-09-15 09:01 +08:00 — Documentation reconciliation validation (not Play Mode)

- Tester: Codex. Objective: D; prepare accurate notes for the user's A playtest, then C before B.
- Unity version/scene: Not executed; latest recorded gameplay environment remains Unity 6000.5.0f1 / Kitchen Prototype.
- Checks: reviewed current recipe assets, Brownie slicing and guide source; compared active checklist with quantities, counts, selection order, recovery, restart and bake thresholds; verified seven active local Markdown links; checked preservation of original status/checklist/Brownie note text; inspected Git documentation diff and whitespace check.
- Expected/actual: active summaries match inspected data/source and recorded test limits; original history retained; links resolve; Git diff --check reported no whitespace errors.
- Result: PASS for documentation checks only. A remains pending with the user. No gameplay acceptance, Console health, scene save or device test is newly claimed.
- Tool diagnostics: two orchestration calls failed before shell execution due to JavaScript template quoting/interpolation; corrected calls completed. No files changed in those failed calls. Link validation passed; that shell call exited 1 only because optional Get-Command cm found no executable.
- Version control: existing note/image modifications observed before edits were preserved. Git emitted CRLF-to-LF normalization notices. A .plastic directory exists but cm was unavailable on PATH, so Unity Version Control pending changes could not be reviewed through its CLI. No commit, check-in or repository settings change.
- Bugs discovered: documentation contradictions reconciled; no new runtime bugs observed.

## 2026-09-15 09:20 +08:00 — User A playtest feedback received (partial acceptance)

- Tester: user; Kitchen Prototype; screenshots show QHD Game view. Exact runtime version/time not supplied; current Editor baseline is recorded separately.
- Cookie screenshot shows all six baked cookies and Recipe Complete with successful result. This supports completion of a normal Cookie attempt, not an exhaustive edge-case pass.
- Brownie screenshots reach ingredient intake and walnut-fold stage; no full Brownie bake/slice player pass is claimed.
- Reported failures/preferences: see current development entry and evidence/player-feedback-2026-09-15-1.png through -8.png.
- Pickup failure is intermittent and user-reported, pending reproduction. Still images alone do not establish its cause. Serving/grade/recipe-return/green-poster requests are additions.
- New agent baseline: Edit Mode, saved scene, Console zero errors/warnings. Fix verification pending.

## 2026-09-15 10:00 +08 — Focused player-feedback verification (Codex)

- Unity: 6000.5.0f1. Scene: Assets/BakeIT/Scenes/Kitchen Prototype.unity. Driver: Editor-only PlayerFeedbackQA, using public ingredient APIs, trigger callbacks, controlled transforms and synthetic practice input. This is a focused integration pass, not a full natural keyboard/mouse acceptance run.
- Expected: retain both recipe loops while correcting the reports; complete only after smaller servings, allow recipe reselection, and suppress paper assistance outside guided practice.
- Steps: open the dry cabinet and ray-pick the visible Baking Soda carton; progress control practice; select Brownies; add exact quantities and four eggs using actual receivers. Check partial Sugar and compound Flour; green-paper lines only at complete quantities; disable/re-enable guided practice. Pour the held melted-butter jug; perform four base and two finishing whisk callbacks. Verify bowl vertex containment and no return-to-MIX request when completed batter is lifted. Line/pour/spread the pan; run actual preheat and eight-second bake at 180 C. Remove the pan, make five knife strokes, inspect twelve physical squares. Pick up/release four onto the plate; verify held-food rejection, scale preservation, removal and replacement, and completion count. Return to selection and verify no leaked food/recipe ingredients. Select Cookies; complete formula, four base/three finishing passes, chip-row wording, four rolls, six cuts, parchment and tray loading. Run actual preheat and five-second bake at 180 C; plate three. Repeat the selected recipe and verify generated servings clear.
- Actual: both actual oven runs produced Perfect; both serving endpoints and reset routes passed. Green-paper partial/full and non-guided checks passed. Baking Soda visible-body ray pickup passed. Brownie completion requires four plated squares and Cookies three; cooking alone does not complete either. Driver final status PASS, including the rerun after plate spacing/position changes.
- Later focused checks: final saved butter has 44 correctly oriented letter strokes and successful pickup; final jug held render has readable text and contained fill. Raw and wrong-recipe serving guard checks passed using temporary Play Mode fixtures. Temporary objects/camera state were discarded when leaving Play Mode.
- Failed attempts: input buffer/overload, transfer target and kinematic knife fixture issues described in change-documentation.md. Initial art renders exposed mirrored butter geometry, stale jug mesh buffers, cramped serving spacing and a plate too close to the edge; these were corrected and recaptured. Tool-compilation/reload failures are not recorded as gameplay failures.
- Console: final inspected errors 0, warnings 0. The successful combined recipe run reported no runtime errors. Scene and assets saved in Edit Mode; no fixture was saved.
- Evidence: evidence/feedback-fixed-2026-09-15-storage.png, -butter-held.png, -melted-butter-jug.png, -green-recipe.png, -contained-batter.png, -brownie-serving.png, -cookie-serving.png. Original player images remain separate.
- Limits: does not certify human gesture feel, natural E placement/carrying, all incorrect bake outcomes, VR behavior or a grading rubric. The active acceptance checklist tracks the remaining user run.

### Guard-check clarification

The final temporary raw/wrong-recipe fixture ran with no recipe selected, so both calls exited at the no-selection guard. It verifies that serving cannot start before recipe selection; it does not independently exercise raw/wrong-recipe rejection during an active recipe. Those branches were inspected in source. The combined active-recipe run did exercise held-food rejection and removal/replacement. This clarification supersedes the broader guard-pass wording above.

Final verification: Edit Mode, scene dirty=false, Console errors=0/warnings=0. Source and Markdown diff checks pass. Scene diff reports only Unity-serialized empty m_Name trailing spaces; retained Unity serialization.

## 2026-09-15 10:34 +08 — Follow-up Play Mode verification

Unity 6000.5.0f1; Kitchen Prototype. Expected: smoother full-cup pour, visible walnut transfer, contained/non-neon bowl, no empty-bowl tutorial target, acceptable two/three stacks that retain plate contact after carrying, Cookie regression intact.

Driver: updated Editor-only PlayerFeedbackQA. Used existing tutorial synthetic input, ingredient APIs and controlled gesture callbacks; real runtime Update drove butter flow, real elapsed timers drove both ovens. The driver temporarily disables player movement during the controlled pour and restores it afterward. No scene fixture is saved.

Steps/results:
1. Visible Baking Soda pickup and exact ingredient/green-paper regression passed. Positioned the held jug spout above the bowl; after .45 s, contents remained with active stream and partial progress. Leveled .25 s: stream stopped and progress remained. Resumed tilt for 1.7 s: contents emptied once and receiver accepted the full cup. PASS.
2. Completed four base whisk passes; transferred half-cup walnuts; at .18 s the effect contained visible kernel meshes. Rendered fall and subsequent contained bowl fill. Completed two finishing passes and transferred batter. Empty bowl was not the yellow tutorial target. PASS.
3. Actual preheat, eight-second Brownie bake at 180 C, five knife cuts and twelve independent pieces. Plated two and checked completion, switched target to three and checked incomplete until third, rejected target reduction while three remained, removed/replaced a piece. PASS.
4. Verified primary-piece bounds clear the central plate well and preceding layer. Picked up loaded plate, rotated it, released it and used actual SmoothPlacement onto a new worktop position. Three portions remained and bottom contact matched the moved plate within .002 m. Side/after-carry renders inspected. PASS.
5. Returned to recipe selection, completed Cookie formula/mixing/rolling/six portions/parchment and actual five-second bake, plated three, repeated same recipe and checked reset. PASS.

Two integration runs completed: initial logic pass identified floating contact in side render; contact refinement and moving-plate test then passed on final rerun. Final driver status PASS. No runtime errors/warnings in inspected Console. Scene was returned to Edit Mode; assets/scene saved, no test fixtures retained.

Evidence: feedback-followup-2026-09-15-butter-stream, walnut-fall, contained-batter, empty-bowl, brownie-stack-two, brownie-serving, brownie-stack-side, brownie-stack-after-carry, cookie-serving PNGs. Original player screenshots/reference are player-followup-2026-09-15-1..4. Earlier run evidence remains under its original prefix.

Limits: this verifies controlled integration, not a natural player gesture/carry acceptance run, physical liquid simulation, new grading criteria or VR behavior. The 2/3 target setter was exercised; key-selection wiring is source reviewed and remains on the natural checklist.

## 2026-09-15 — Cupcake source documentation verification

Reviewed the selected web recipe card and existing RecipeDefinition/oven code. Recorded exact batter and frosting quantities, separated published bake instructions from proposed game settings, and verified current 10-degree oven steps and missing tablespoon tool role. Source cited in cupcakes.md. Documentation-only update: no gameplay or Unity scene changes and no Play Mode test claimed. Frosting preparation preference requested separately.

## 2026-09-15 10:45 +08 — Cupcake method review

Rechecked the selected recipe card's numbered/order-dependent batter and frosting instructions. Expanded the documented mapping to include scraping, staged dry additions, gradual milk, toothpick doneness, complete cooling and buttercream's two powdered-sugar additions. Recorded confirmed Hard/from-scratch scope. No Unity changes or Play Mode tests: this is a design/source verification pass. Gameplay implementation remains pending.

## 2026-09-15 — Cupcake batter foundation Play Mode verification

Environment: Unity 6000.5.0f1, Kitchen Prototype. Initial Edit Mode scene clean; initial Console errors=0/warnings=0. CupcakeFoundationQA uses real pickup/receiver APIs, tool-trigger callbacks, synthetic movement-practice input and actual transfer timers, with controlled body positions. It is not a complete natural mouse/keyboard run.

Final Cupcake run: PASS. Verified movement-practice gating and selection; two authored bowl rests; tablespoon rack return; baking rejected for the partial branch; missing-input mixing blocked; wrong bowl/excess flour retain measure contents; exact compound flour green state; dry blending before creaming; early egg rejected; first egg mixed before the second; whisk cannot bypass scraping; two sustained 1.4-second dry-mixture halves with milk/mixing between; holding the first pour longer does not consume the second half; Dry bowl returns through StablePlacementSurface; Milk opening physically fills both measure types; final scrape; all nine exact ingredient lines green only in guided practice; completed bowl no longer yellow; partial endpoint does not latch full-recipe completion; M restores selection, empty state and the original recipe bowls. Final Console errors=0/warnings=0.

Presentation inspected: recipe text fits its paper, worktop labels resized, contained pale batter, powder transfer with the pouring lip above the destination, final paper progress. Evidence: cupcake-foundation-2026-09-15-equipment, recipe, dry-half-0, dry-half-1, finished-batter and green-recipe PNGs in evidence/.

Iteration history: first driver failed after pickup reparented an egg outside its original root; fixed the fixture's lookup. Initial screenshots caught oversized text and an upward-facing fixture pour pose intersecting the bowls; corrected labels and downward lip orientation. A subsequent complete run passed; final rerun also passed after adding tablespoon return, the partial-bake guard, falling powder and progressive dry fill. Earlier fixture failure is recorded here and is not counted as a passing run.

Limits: no Cupcake gathering/liner/filling/baking/cooling/frosting/serving test is claimed. Creaming uses existing whisk gestures, not an electric mixer or a real-time 2–3-minute simulation. Ingredient and tool reach, natural scraping/pouring feel, performance and VR remain for player/device testing. No grading implemented.

### Cookie/Brownie regression after Cupcake receiver integration

PlayerFeedbackQA completed on a fresh Play Mode session: PASS. Existing exact measures, green recipe paper, Baking Soda ray pickup, continuous melted-butter pause/resume, walnut transfer, Brownie mixing/spreading, actual eight-second bake, slicing, two/three serving and loaded-plate movement all passed. Recipe selection reset, complete Cookie preparation, actual five-second bake, three-cookie serving and same-recipe reset also passed. Console errors=0/warnings=0. Evidence uses cupcake-regression-2026-09-15- so prior feedback evidence was retained. This is controlled integration, not a new natural player acceptance pass.

### Cupcake guide layout check

After adding per-row wrapped-text heights for Cupcake instructions and the shorter CUPCAKES stage prefix, the Cupcake integration run passed again. Captured the actual 2560x1440 Game view during creaming; ingredient amounts, work progress, correction message and restart/selection footer were readable without clipped rows. Evidence: cupcake-foundation-2026-09-15-guide.png. The bowl-rest checklist now reflects actual placement rather than presenting a permanently pending instruction. Cookie/Brownie row sizing stays unchanged.

An isolated guide-only fixture attempt incorrectly treated the nonserialized tutorialComplete field as a SerializedProperty and failed before changing it. Reused the existing input-driven Cupcake test setup instead; no gameplay fix was required for that tool-script error.

Final placement-indicator check: PASS for placed, picked-up and returned state. The current bowl row turns complete only when its PreparationItem reports the required rest. Captured the updated green rest indicator in cupcake-foundation-2026-09-15-rest-guide.png.

Final rerun after placement-row refinement: CupcakeFoundationQA PASS. Returned to Edit Mode and saved Kitchen Prototype/assets. Final audit: dirty=false, missing scripts=0, nine Cupcake phases, batterPracticeOnly=true; Console errors=0/warnings=0. No Play Mode fixtures retained. Source/editor/Markdown diff checks passed. Final full-dish/VR/player-acceptance limitations remain as stated above.

## 2026-09-15 13:00 +08 — Cupcake tray-through-cooling verification

Environment: Unity 6000.5.0f1, Kitchen Prototype scene. Initial Console: zero errors/warnings; initial scene was saved. User authorized continuing with the individual-slot tray and its baking stage.

### Implemented and exercised

- CupcakeFoundationQA plus CupcakeTrayQA completed the full controlled Play Mode path twice, including a run with the final release-only placement guard: tutorial/selection; empty/incomplete tray rejection; twelve dispenser liners; occupied-well rejection; tray carry/rotation and child-body alignment; actual empty-oven preheat; all nine exact batter phases; gradual individual fills; duplicate-fill rejection; eleven-fill bake rejection; actual ten-second perfect bake; pickup restrictions in the oven/before testing; physical tester-centre check; twelve individual rack transfers; per-item cooling; partial-practice endpoint; R replenishment and M selection reset. Both reported PASS.
- Existing batter checks retained wrong-bowl/wrong-phase rejection, excess measure retention, individual eggs, scrape gates, two sustained dry transfers, milk opening intake, and exact tutorial-only green recipe lines.
- Final mesh-only refinements were then checked in a separate Play Mode fixture: twelve release-only placements (hovering while held does not snap), contained raw fill, partial raw pickup guard, saved side/top views of the metal tray and liners, baked cakes contained inside their paper, and twelve stable rack placements. No moving non-convex mesh colliders were introduced.
- Non-perfect outcome presentation was tested by applying controlled result states, not by running three additional oven timers: underbaked retains raw cake and leaves wet batter on the tester; perfect clears the tester; overcooked/burnt update all twelve ingredient IDs and cake presentation. The perfect result was also reached through the actual timer in the full scenario.
- Controlled handling invoked the shared pickup/rotation/scroll paths for tray, tester and cupcake. Tray/tester moved and rotated; a cupcake retained unit scale and rotated 6.7 degrees. Its immediate position delta was constrained to about 1.4 mm in that fixture, so this is not a natural-input hold-distance/recovery acceptance pass. Full keyboard/mouse feel, occlusion/recovery near every kitchen object, and VR remain unaccepted.

### Findings and corrections

- Initial interface migration produced two reference-comparison compiler warnings; explicit ReferenceEquals checks removed them. A QA call omitted the existing dispenser's pickup argument; corrected before gameplay verification.
- The dispenser's return value means the interaction was handled, including exhaustion. The initial QA incorrectly expected false for an empty stack; the check now verifies zero remaining and an empty hand.
- Render review caught rectangle-corner seams and coplanar liner floors. Added exact rectangular mesh corners, explicitly updated persisted mesh data, and raised liner anchors 1 mm above the metal floor. Final review added outer/underside metal faces, attached the side grips, and reduced baked cake side radius to stay inside the fluted paper.
- A source import during Play Mode interrupted an intermediate QA run; that run was discarded and restarted. Another fixture checked wall-clock elapsed time while Unity was behind its render step, reporting fill 11 just before it completed. Fill/bake/cooling checks now wait for actual state with a game-time deadline.
- A focused single-command handling fixture initially required more than 1 cm of immediate movement for every object; that assumption failed for a constrained cupcake. State was inspected and the held object released before continuing. A retry encountered that retained held object; no scene fixture state was saved. Subsequent rack placement reached all twelve after the placement frame completed.

Evidence: `evidence/cupcake-tray-2026-09-15-*.png` contains the full timed scenario. The `final-` images supersede its earlier geometry close-ups and include lined tray, tray side, rack, wet tester and non-perfect result presentation. Final save/Console and Cookie/Brownie regression results follow below.

### 2026-09-15 13:02 +08 — Final verification

PlayerFeedbackQA reported PASS for Cookies and Brownies after the Cupcake shared-system changes, including their actual bake timers, transfer/recovery checks, mixing, Brownie slicing, smaller servings and resets. Regression evidence uses `cupcake-tray-regression-2026-09-15-` so earlier evidence is preserved. A final wording-only edit changed the Cupcake bake instruction to `Press BAKE (10 seconds)`; compilation remained clean.

Final Unity state: Edit Mode, Kitchen Prototype scene saved and not dirty, twelve Cupcake wells and twelve rack positions, Cupcake supplies inactive until selection, zero missing scripts, Console zero errors and zero warnings. Source/editor/Markdown `git diff --check` passed. Mesh/material assets were saved; no test fixture state or commit was saved. Natural keyboard/mouse acceptance and frosting/piping/serving remain open.
