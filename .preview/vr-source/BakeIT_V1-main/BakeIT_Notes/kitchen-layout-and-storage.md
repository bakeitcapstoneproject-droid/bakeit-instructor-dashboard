# Kitchen Layout, Fridge Storage, and Mise en Place

## Centered onboarding correction — 2026-09-11

- The saved player start at `(0, 0, 4.5)` remains the center/reference. The user manually finalized world-text orientations; those rotations are authoritative and should not be batch-adjusted again.
- `Utensils Area` was restored from `.86` to its original `.412` X scale. The intended separate `Tool Home Mat` was enlarged from approximately `.57 x .55` to `.76 x .68`, with its sign widened to match.
- Dry storage Y scale increased from `1.0` to `1.18`. Its supported loose contents moved upward by `.073 m` without changing the user's X/Z placement. The action label no longer instructs crouching.
- Permanent labels attached to carryable ingredients/tools were removed. Direct ray hover now shows an upright screen label above the visible object, preventing names from appearing through the cabinet or reversing with object rotation.
- The dedicated chocolate-chip scoop and its hook were removed. The rack now has seven matching hooks for six measuring vessels plus the whisk.
- The BakeIT identity panel and three brown-framed recipe sheets are higher and more compact; frames are square-proportioned, and the sign uses warm brown/gold cookie details.

## Historical measuring rack and dry-storage addition — 2026-09-10 (superseded where noted above)

- Original state: a black metal-look wall rail carried eight hooks including a dedicated chocolate-chip scoop. The current state is the seven-hook layout documented above.
- Tools are real pickup bodies. Releasing the matching tool near its hook snaps it back into the hanging pose; wrong tools are ignored by that hook.
- Historical/superseded: `Utensils Area` was temporarily widened from `.412` to `.86`; the 2026-09-11 correction restored `.412` and enlarged `Tool Home Mat` instead.
- New vanilla, baking-soda, salt, and walnut jars are leveled at world Y `.405`, the measured top of `Dry Storage Shelf Support`. Existing Flour, Sugar, Melted Butter Cup, and all user-authored equipment transforms were not repositioned.
- Pantry/food packages now carry one backed player-facing label rather than front-and-back floating text.

## Separate recipe posters — 2026-09-10

- The first `Kitchen Environment/Recipe Clipboard` shared board was replaced by `Kitchen Environment/Recipe Selection Posters` at `(0, 1.56, 6.95)`, centered beneath `Kitchen Identity Panel` without moving the user's rack, pans, bowls, or spatula.
- Three separate `.70 x .74 m` dark frames sit at local X `-.76`, `0`, and `.76`. Each contains its own `.62 x .66 m` cream recipe paper, title, availability, and compact recipe/process summary.
- Each poster has its own thin trigger covering the complete paper, receiving the existing three-metre forward `E` ray. These are wall fixtures, not pickup objects, and do not obstruct loose-item placement.
- The user stated that they will manually move the `Melted Butter Cup` into dry storage. This poster revision did not move the cup, and its pour logic does not depend on a fixed storage transform.

## User-authored rack correction — 2026-09-09

- The user's current live Editor layout is authoritative. They replaced/re-leveled the holder arrangement, laid both pans on the rack, and repositioned/oriented the rubber spatula for a more realistic presentation. Future equipment additions may be manually re-leveled by the user, so systems must not depend on exact storage transforms.
- Current inspected live transforms: cookie tray `(-3.5,.425,4.12)`, rotation `(0,0,0)`; brownie pan `(-3.5,.613,4.12)`, rotation `(0,0,0)`; bowl `(-3.4939,.9979,4.1394)`; rubber spatula `(-3.2303,1.5447,4.4190)`, rotation `(270,80,0)`.
- The scene was dirty/unsaved when inspected. Codex intentionally made no scene mutation and did not save or reload it. The original authoring history below remains for traceability but no longer dictates the current visual placement.
- The approved brownie-bowl/butter pass subsequently saved those user edits unchanged. It added `Brownie Mixing Bowl` at the provisional rack position `(-3.5,1.19,4.12)`, scale `1.4`, and `Melted Butter Cup` at `(-2.35,1.19,5.60)`, scale `.65`. These new starting poses may be manually re-leveled; no gameplay rule depends on them.

## Named bowl holders — 2026-09-10

- The user renamed the two tray-mesh supports under `Kitchen Environment/Bowl Holder Rack` according to their actual purpose: `Cookie Bowl Holder` at `(-3.5,.9899,4.12)` and `Brownie Bowl Holder` at `(-3.5,1.1779,4.12)`. Both remain licensed `Prop_Tray` instances with their support colliders intact.
- The user also completed the larger bowl's manual leveling at `(-3.4973,1.1859,4.1133)`, rotation zero, scale `1.4`. These saved names/transforms are authoritative. Runtime systems use serialized component references and `Mix Bowl Rest`, not either holder name.
- Focused Play Mode hierarchy verification passed both names/parents, both support colliders, the cookie `BowlReceiver`, brownie `PourableIngredientReceiver`, and melted-butter component: `HOLDER_RENAME_PLAY_QA PASS=True`.

## Physical bakeware storage — 2026-09-09

- The existing `Bowl Holder Rack` now serves as shared physical bakeware storage without changing its Transform or mesh. The bowl remains at `(-3.5,1.056,4.12)` on its existing middle shelf.
- The existing shallow cookie `Prop_Tray` moved from the LINE & LOAD counter `(1.68,1.16135,5.76)`, rotation zero, to a lower rack rail at `(-3.5,.425,4.12)`, yaw `90`. Players retrieve it and can return it to the unchanged `Tray Worktop Rest`; its parchment/cookie receivers remain attached.
- A distinct deep `Brownie Pan` starts on the next usable rail at `(-3.5,.613,4.12)`, yaw `90`, and the `Rubber Spatula` hangs at `(-3.15,1.23,4.12)`, rotation `(90,0,0)`. They are independent grabbable roots rather than rack children, preventing nested Rigidbody behavior.
- Focused Play Mode pickup/release passed for all three equipment items. The pan is stored sideways for the narrow rack and rotates to its `.56 x .08 x .40 m` use orientation to fit the existing oven. Recipe-aware wrong-pan rejection remains future work.

## Current additions — 2026-09-08

- User-sized Mix Tool Rest was renamed Utensils Area (references preserved, no mat resize). Its release height is .50m rather than .22m. Only measuring tools/whisk receive a .035m edge allowance for a protruding handle; center must remain on the mat, with obstruction checks. Other surfaces keep .22m and zero added overhang.
- StablePlacementSurface now also observes released collision entry/stay. A tool dropped above the assistance range can fall naturally and register when it lands; held hovering still gives no credit. Rigidbody and Transform preview poses synchronize before collider bounds are measured, preventing stale held rotations from producing a floating settle.
- All marked rests share a .24s SmoothPlacement animation. X/Z/yaw are retained, player collisions are protected during settling and restored afterward, and pickup cancels anchoring. No global auto-sort or ingredient teleport slots were introduced.
- MIX TOOLS / MIX TOOL REST text now reads UTENSILS AREA. Existing scene label object names may remain historical; only the actual surface GameObject was renamed. The tutorial uses the whisk, then distinguishes fridge gathering from dry-cabinet gathering.
- A simple parchment roll sits in the existing line/load zone, with a short leading edge and E label. Existing paper starts hidden and is reused, not duplicated. An invisible Tray Worktop Rest supports near-release return of the carryable tray. User workstation positions/scales, cabinets/fridge and wood board remain intact.
- See tray/parchment, recipe session, current status and the 2026-09-08 test/development entries for precise behavior and limitations. Earlier layout sections below describe the successive historical authoring passes.

## Latest layout — 2026-09-07 evening

This supersedes the board-staging correction and first-pass layout below. Flow is ingredients/prep → mixing → wooden chopping board → tray. The user's unsaved enlarged Cold Ingredient Rest at `(-1.902,1.168,6.008)`, scale `(1.277,.012,1.207)`, and Mix Tool Rest at `(-.995,1.167,6.022)`, scale `(.412,.010,1.151)`, were preserved and saved.

- Five ingredients are prepared on Cold Ingredient Rest; both measuring spoons and whisk on Mix Tool Rest. Existing stable placement holds at the chosen release X/Z, not a prescribed horizontal slot. All supplies must be correctly resting concurrently, together with the bowl, before readiness latches.
- New slate Mix Bowl Rest: center `(-.4,1.167,5.91)`, size `(.56,.01,.92)`, cloned from the existing supported mat. Bowl must be placed here for ingredients/whisking; lifting it pauses actions without resetting quantities.
- Board X `.05 → .45`; pin X `.04 → .45`; knife X `.57 → 1.02`. Other coordinates and scales retained. No storage ingredient positions or user mat scales changed.
- Reused vendor `Prop_TrayHolder` at `(-3.5,0,4.12)`, unit scale, static hollow MeshCollider. Reused `Prop_Tray` as Bowl Holder Shelf at `(-3.5,1.045,4.12)`, scale `(1.4,1,1.7)`, with its existing BoxCollider reused for stable support. Bowl starts at `(-3.5,1.056,4.12)` and remains unit scale.
- Bowl now has Rigidbody/PreparationItem and compound hollow collision; large spoon also gains PreparationItem. Nine preparation carriers, ten stable surfaces. Original vendor assets are not edited.
- Station text/backplates aligned to ingredient X -1.90 (width1.20), MIX X -.4 (width.64), board X .45 (width.90), existing tray X1.70. Tool-home hint now targets Mix Tool Rest; no required tool placement on the board.
- Explicit one-time authoring helper: `Editor/WorkstationFlowSetup.cs`; refuses repeat full Apply to preserve manual adjustments. Cleaning remains **planned only** in its dedicated note.
- Play Mode: correct/wrong rest checks, rack pickup/release/re-pick, carry/rotation/scroll, filled-bowl return, final combining, board recovery, four-cookie Perfect bake and reset passed. Natural player traversal/feel/VR remain pending. Evidence: `prepared-rests-guide-2026-09-07.png`, `place-bowl-guide-2026-09-07.png`, `bowl-holder-rack-2026-09-07.png`.

## Scope and status

Revised 2026-09-07 after the user's mise-en-place correction, in `Assets/BakeIT/Scenes/Kitchen Prototype.unity`. Ingredients start in storage and all five ingredients plus the small spoon/whisk must be released on the prep board before mixing. Existing models, recipe amounts, bake duration and bindings are preserved. Technical Play Mode and rendered QA passed; full player-operated handling/visual acceptance remains open.

## Latest preparation/storage correction

- Flour and sugar are now carryable bags inside the existing `Prop_StorageCabinet_02`; E slides its existing doors sideways to access either half. The shelf is the original internal platform, with a thin support collider. Ctrl uses the existing crouch action if a lower view helps.
- Egg, butter and the carryable chip bowl start in the fridge. Ingredient names are attached on both sides of the items, not on the counter or shelf. Labels and empty renderers are excluded from pickup-center calculations.
- E release near the board uses `StablePlacementSurface`: retain exactly the release X/Z and yaw, settle upright/flat by at most .22 m vertically, and make the body stable/kinematic. No horizontal slot jump or reparenting. A complete footprint must fit; occupied, high or edge positions decline assistance and use normal drop physics. Pick up again at any time.
- The board is .70 m wide instead of .50 m: scale changes from `(2.5,2.5,2.5)` to `(3.5,2.5,2.5)`, preserving thickness/depth and dough world-volume behavior. Seven supplies fit together in tested free-placement positions; no authored grid is used at runtime.
- `PreparationItem` marks the seven supplies; containers do NOT get an `Ingredient` component, so whole bags/bowls cannot be consumed as one measure. Initial bodies are kinematic/no-gravity, mass .2, speculative collision detection; both powder sources use convex hulls. The chip bowl's static non-convex shell is disabled and replaced by a bottom plus eight low box wall segments, preserving an open scoopable cavity.
- `MiseEnPlaceStation` now reads deliberate board rests. Hovering and early bowl use never count. Before completion, lifting any staged supply unchecks it; once all seven are together the workspace-ready state latches for the batch. Bowl additions are rejected without consuming/emptying anything until preparation is ready. No new recipe manager was added.
- Guide phase 2 says `PLACE ON PREP BOARD`: five ingredient rows name their storage locations, then two tool rows. Existing 135% sizing remains. Phase 6 first asks to move containers/tools beside the board; the board refuses dough while staged supplies occupy it, then accepts it through the existing entry/stay recovery path.
- The former broad MIX hover trigger and both fixed egg/butter slot components/triggers are disabled but retained for recovery. Return placement on fridge/cabinet shelves now uses the same release-local rest assistance; no automatic trigger pickup/re-storage and no fixed-slot teleport.
- Authoring: `Editor/PreparationStorageSetup.cs` is an explicit one-time follow-up to the first pass; it refuses an already-configured scene. It does not execute automatically.
- Current evidence: [prepared board](evidence/prepared-board-2026-09-07.png), [actual guide](evidence/board-preparation-guide-2026-09-07.png), [dry cabinet](evidence/dry-cabinet-storage-2026-09-07.png), [revised fridge](evidence/revised-fridge-storage-2026-09-07.png). The earlier preview links below document the first pass.

### Cabinet and collider values

- Cabinet remains at `(1.4,0,3)`, scale `(.72,1,1)`. Disable its old broad BoxCollider; static non-convex MeshCollider uses the actual hollow shell.
- Existing `_p02` / `_p03` doors: add mesh-bounds boxes and kinematic/no-gravity bodies; `SlidingCabinetDoor.openOffset=(-.74,0,0)` / `(.74,0,0)` local, speed `.65` local units/s. Separate depth tracks remain. Sliding one over the other exposes one half; change the overlap to reach the other half.
- Door motion becomes trigger-only while checking candidate penetration; stop above .002 m against bodies/player and allow E reversal. Return to a solid collider at rest. Fixed cabinet shell and the other sliding panel are excluded from obstruction checks.
- New support under the existing shelf: local `(0,.397,-.015)`, size `(1.48,.012,.56)`; top `.403`. Flour/sugar start at cabinet-local `(-.4,.408,.09)` / `(.4,.408,.09)` (world `(1.112,.408,3.09)` / `(1.688,.408,3.09)`). Chip bowl starts fridge-local `(-.4,1.544,.13)` (world `(-2.35,1.544,3.13)`).
- Chip compound base: center `(0,.008,0)`, size `(.10,.016,.10)`; eight walls centered at radius `.076`, Y `.036`, sizes `(.064,.056,.012)`, yaw increments 45 degrees. Original visual model/source trigger/fill content are unchanged.
- Rest surfaces: board, both existing MIX mats, four existing fridge shelf supports and the new dry shelf (eight total). Only the board flags preparation. Surface rest does not alter object scale. No permanent settings or package change.

## First-pass layout baseline

The remaining sections record the 09:20 first pass. Source positions, fixed return slots, table labels and MIX-based preparation described there are superseded by the correction above. Earlier before/after data is retained for audit rather than erased.

## Workstation layout

The main counter is split into pantry/MIX (left), ROLL & CUT (middle), and LINE & LOAD (right). Fridge storage and the separate tool home are across the aisle. A missing rear prep wall is filled in with warm plaster, sage trim, and a small BakeIT sign. The counter itself, fridge, sink, actual stove, and player spawn stay in their original positions.

Exact root before/after world positions, rotations, scales, and active states are retained in [transform audit](evidence/kitchen-layout-transforms-2026-09-07.json). Important final positions:

| Object | World position | Other changes |
| --- | --- | --- |
| Bowl | `(-1.72, 1.16135, 5.70)` | Child trigger/result follow the bowl |
| Flour / sugar | `(-2.30, 1.16135, 6.28)` / `(-1.95, 1.16135, 6.28)` | Pantry row; low labels |
| Chocolate chips | `(-1.43, 1.16135, 6.28)` | Tall label inactive; low label; existing source trigger unchanged |
| Wood board | `(0.05, 1.16135, 5.86)` | Existing scale `2.5`, work-zone/snap hierarchy preserved |
| Rolling pin / knife | `(0.04, 1.186, 6.42)` / `(0.57, 1.18, 5.94)` | Clear of board and tray |
| Tray / parchment | `(1.68, 1.16135, 5.76)` / `(1.68, 1.162, 6.36)` | Scale and child receivers preserved |
| Small / large spoon | `(-0.91, 1.046, 3.14)` / `(-0.71, 1.046, 3.14)` | Both yaw `180`; on TOOLS cabinet |
| Whisk | `(-0.53, 1.09, 3.03)` | Yaw `0`; on TOOLS cabinet |
| Auxiliary sideboard | `(1.40, 0, 3.0)` | Yaw `0`, scale `(.72,1,1)`; existing model reused |
| Cup / cover | `(1.22,.735,3.02)` / `(1.47,.630,3.02)` | Rehomed on sideboard; cover mesh has offset pivot |

`Prop_Oven_01` is an unused duplicate appliance and is now inactive, not deleted; it can be re-enabled. The actual `StoveWithExtractorHood (1)` stays active. The active scene has no new recipe managers.

## Materials and authoring

- `Kitchen_PrepWood.mat` uses `Art/PrepWood.shader` (`BakeIT/Prep Wood`): brown base `(.48,.245,.105)`, grain `(.28,.12,.045)`, density `180`; code-generated fine grain, no new texture or image dependency.
- Other new URP materials: slate `(.055,.12,.135)`, cream `(.89,.86,.75)`, amber `(.88,.52,.14)`, sage `(.22,.43,.34)`, plaster `(.77,.79,.73)`.
- `Kitchen_WorldText.mat` / `Art/KitchenWorldText.shader` use the existing font atlas with depth testing. Default TextMesh labels initially showed through the fridge/other geometry; final labels respect occlusion.
- `Kitchen Environment` groups new fixed badges, pantry labels, ingredient/tool mats, setup region, rear wall, trim/sign, and soft fill light. Decor has no collision except the rear wall and the two MIX mats, which support released food/tools.
- Main MIX mat: center `(-1.05,1.167,5.80)`, size `(.30,.010,.64)`. Cold-food mat: `(-2.32,1.168,5.65)`, size `(.44,.012,.49)`. TOOLS mat is visual only over its existing cabinet collider.
- Prep wall: center `(0,1.5,7.12)`, size `(8,3,.16)`. Existing three walls use plaster. Directional light changed to warm `(1,.94,.84)` at `1.15`; added warm point fill at `(0,2.8,4.9)`, range `7`, intensity `2.2`, no shadows.
- `Editor/KitchenPolishSetup.cs` is an explicit, Undo-aware one-time authoring command. It never runs automatically, refuses Play Mode/wrong scene, and refuses a second application once `Kitchen Environment` exists. Do not rerun to overwrite subsequent manual edits.

## Fridge interaction and collision

- Existing `Prop_Fridge_01` broad root BoxCollider is disabled, not removed. A static non-convex MeshCollider uses the actual hollow shell mesh. There is no moving Rigidbody on that shell.
- Existing right/left pivoted meshes `Prop_Fridge_01_p02` / `_p03` receive tight mesh-bounds BoxColliders, kinematic/no-gravity Rigidbodies, and `FridgeDoorController`.
- Configured opening angles are `+100` / `-100` degrees; speed `110 deg/s`, maximum step `3 degrees`, obstruction tolerance `.002`, correction interval `2s`. E toggles/reverses the targeted door.
- `PickupController.TryInteractWithFridgeDoor` checks sorted ray hits before pickup/release, ignores held/player colliders and non-door triggers, and respects solid occlusion. E can operate a door while retaining a held item. No additional keyboard reader was introduced.
- Moving door colliders become triggers. Candidate penetration checks stop motion against the player, held objects, dynamic loose objects, and stored ingredients. At the target pose the door collider is solid again. This prevents the kinematic door from shoving the player during motion. Other fridge doors and the fixed shell are excluded.
- Four invisible support boxes cover the existing wire shelves: local X/Z center `(0,.01)`, size `(1.43,.014,.68)`, Y centers `.469`, `.825`, `1.167`, `1.532`. Existing bars remain visible, but small tools cannot fall through their gaps.
- Interior point light: local `(0,1.85,.05)`, range `1.9`, intensity `.3`, cool color `(.73,.86,1)`, no shadows. The first `.75` preview was reduced after visual QA. This first pass leaves the light on; it is not a fridge-temperature simulation.

## Ingredient slots and ordinary storage

Two `IngredientStorageSlot` components live under `Fridge Storage`:

| Slot | Ingredient | Local position under fridge | Access door |
| --- | --- | --- | --- |
| Egg Storage | `Egg` / saved `Food_Egg` | `(-.40,1.181,.13)` | `_p03` |
| Butter Storage | `Butter` / saved `Food_Butter` | `(.40,1.207,.13)` | `_p02` |

Each slot has a trigger of size `(.38,.22,.35)`, center `(0,.075,0)`, and a `Storage Snap` child. The saved food begins at the shelf pose with gravity off and a kinematic body. Start attaches it to the snap. Picking up clears occupancy and retains original world scale. A retrieved item is protected from automatic re-storage until it leaves the slot. Correct return requires a fully open corresponding door and a free matching slot, releases the pickup safely, then snaps and stabilizes the ingredient. Wrong ingredient/full/closed access gives a throttled correction. Trigger stay can recover a released item without stealing a held item.

Other shelves support normal physical drop/pickup of loose objects; this is not a limitless inventory UI or an auto-sort system. A large measuring spoon was dropped on an empty shelf, settled at Y `.83`, and ray-picked again. Only egg and butter have typed snap slots in this recipe pass. Food consumed by the existing bowl clears slot occupancy; restarting restores it from the saved scene.

## Mise-en-place guide

`MiseEnPlaceStation` observes the four original egg, butter, small-spoon, and whisk Transform references plus the existing bowl. Its mixing trigger is centered at `(-1.68,1.43,5.90)`, size `(1.92,.70,1.35)`. It latches each item once inside the region; accepted egg/butter also satisfy preparation so early bowl use cannot deadlock the guide. This is a gathering checklist, not a scored organization system and not a new ingredient gate.

The guide now waits for the recipe-paper choice, then has thirteen phases: basic controls, mise en place, base ingredients, whisk, chips, final combining, board transfer, rolling, cutting, tray preparation, oven preheating, oven loading, and baking. The 150% UI scale, plain-language yellow part headings, and completion latch remain. Restart clears setup and restores fridge/tool homes while retaining the in-session recipe choice. The already-completed in-session basic tutorial stays complete; fresh Play Mode starts at `Choose a Recipe`.

## Oven control presentation

Existing button markers are preserved. The start control now changes between PREHEAT, HEATING, BAKE, and BAKING as the oven state changes. Under the actual stove root:

- Slate bezel: local `(0,.80,.324)`, scale `(.70,.22,.035)`, no collider.
- Sage screen inset: `(-.04,.822,.346)`, scale `(.27,.12,.006)`, no collider.
- Context-sensitive start control: `(-.252,.805,.363)`; minus: `(.278,.805,.363)`; plus: `(.172,.805,.363)`. The latter order reads minus then plus from the player's view.
- Existing `.03,.03,.02` cube faces become flattened built-in sphere meshes at scale `(.076,.076,.032)` with BoxCollider size `(1.18,1.18,1.35)` and cream/amber finishes. Labels sit `.02` forward of each button.
- Existing display: local `(-.04,.825,.355)`, yaw `180`, scale `.62`; centered readout; existing `OvenDisplay` continues updating it.
- Door dragging and the `180 C`, five-second prototype bake remain unchanged. The oven begins at `0 C`, requires the player to select a target, then preheats over five seconds with an empty closed oven and retains the light at READY and after baking. Plus/minus/start-control rays and a six-cookie Perfect preheat/bake lifecycle passed.

## Verification and limitations

See `test-log.md`, 2026-09-07, for exact test steps and harness failures. Saved previews: [overview](evidence/kitchen-overview-2026-09-07.png), [worktop](evidence/kitchen-worktop-2026-09-07.png), [fridge open](evidence/kitchen-fridge-2026-09-07.png), [fridge closed](evidence/kitchen-fridge-closed-2026-09-07.png), [oven](evidence/kitchen-oven-2026-09-07.png).

Manual carrying paths around the door/player, full rotation/scroll and crouch reach, reading small physical labels at the user's resolution, oven-door tray insertion, and subjective visual acceptance remain. VR, scoring, recipe generalization, additional recipes, ceiling/final lighting, and a full appliance remodel were not added.

## 2026-09-15 player feedback — storage and readable sources

The dry cabinet Y scale increased from 1.18 to 1.40, retaining X=.72 and Z=1. Nine sources now sit upright on the shelf in two spaced rows. Back-row X/Z: Flour .99/2.88; Sugar 1.27/2.88; Cocoa 1.54/2.88; Baking Powder 1.81/2.88. Front-row X/Z: Melted Butter 1.00/3.13; Baking Soda 1.22/3.13; Walnuts 1.45/3.13; Salt 1.68/3.13; Vanilla 1.83/3.13. Renderer bottoms determine shelf seating. Raised lids are excluded when fitting pickup colliders. Baking Soda and Vanilla visual offsets were centered on their roots; a visible-carton pickup ray passed in Play Mode.

Food_Butter uses an explicit zero held rotation and corrected block lettering on both sides. Its scene visual was unpacked to group its 44 letter strokes with mirrored X; the original generated prefab remains intact. Melted Butter Cup keeps its identity and pour behavior but displays an ivory ceramic jug, golden fill, front name and handle. The old mug renderer/label are retained disabled. A serving plate is on the right counter; see serving-and-plating-system.md. Natural layout/handling acceptance remains pending.

## September 15 — conditional Cupcake foundation supplies

Cupcake-specific supplies activate only for its batter-practice branch: blue-grey Dry bowl and small rest on the ingredient counter, tan Batter bowl in MIX, milk/oil at the back of the ingredient worktop, a labelled 84 g butter portion and two eggs, tablespoon with rack return, and scraper in the tools area. Existing pantry ingredients and measures remain shared. Cookie/Brownie bowls are hidden during this branch and restored by the normal scene reset. No existing storage prop was moved by this authoring pass. Full Cupcake gathering, tray/liner, cooling and frosting equipment remain later work.
