# Tray and Parchment System

## Metallic cookie tray — 2026-09-09

The existing `Prop_Tray` now uses project-owned `Assets/BakeIT/Art/Bakeware Metal.mat`: URP Lit neutral silver `(.62,.65,.68,1)`, metallic `.85`, smoothness `.35`. Previously it used nonmetallic `Panda Mat`, which is shared by 27 other scene renderers; that material was deliberately left unchanged. This is appearance-only and gives the future deep brownie pan a reusable matching bakeware finish. Play Mode confirmed the actual tray still has its Rigidbody, three colliders, liner and cookie receivers, and can be picked up normally. Parchment sizing/assistance, tray geometry, cookie loading, and oven behavior are unchanged.

## Shared bakeware rack — 2026-09-09

The cookie tray now starts on the existing rack at `(-3.5,.425,4.12)`, yaw `90`, rather than already sitting in LINE & LOAD. The player physically retrieves it and the unchanged invisible `Tray Worktop Rest` remains its preparation destination. A separate deeper brownie pan uses the same metallic material on the next rail; it has no cookie liner/portion receiver and cannot replace the cookie tray in the current recipe. Focused Play Mode passed cookie-tray pickup and release to the worktop rest with all existing receiver components intact.

## Recipe-data migration — 2026-09-09

`TrayReceiver` now reads the active portion name, four cooked-result names, and product labels from the assigned cookie definition. It still requires the same actual `DoughPortion` batch metadata and parchment-first placement; no placement sensitivity, paper footprint, batch-completeness, or tray physics changed in this architecture pass. The asset records `Parchment`, but alternate pan-preparation interactions are not implemented.

## Placement sensitivity and second size retune — 2026-09-09

After further player difficulty, the sheet is reduced from approximately 90% to 86% of tray support: saved scale `(.6202,.002,.3725)` against `.7211077 x .4330797`. `TrayLinerReceiver` first preserves a valid player X/Z/yaw pose, then falls back to tray-centered, tray-aligned placement for parchment released within `.15 m` horizontally and `.35 m` above the tray. Detection padding is `.12 m` horizontal and `.35 m` vertical. These values are serialized for tuning; far-away, below-surface, occupied, wrong-item, and obstructed releases remain rejected. Play Mode preserved a valid `.02/.015 m` offset, assisted a `.10/.08 m` offset from `.30 m` high at `45°`, rejected `.18 m`-far and `.04 m`-below poses, accepted six cookies, and kept all loaded children stable when the tray moved.

## Practical tray-fit retune — 2026-09-09

Player testing found the prior 95% sheet too unforgiving to place. The saved sheet is now `(.649,.002,.3898)`, approximately 90% of the `.7211077 x .4330797` tray support on X/Z. This keeps broad visual coverage while increasing the valid off-center release tolerance on the tighter Z axis from roughly `2.4 mm` to `18.6 mm` after the shared placement inset. Receiver and collision logic are unchanged. Focused Play Mode accepted a release offset by `25 mm` X / `12 mm` Z and then accepted a complete six-cookie batch.

## Parchment coverage correction — 2026-09-09

The existing single `ParchmentPaper` sheet grew from local scale `(.55,.002,.32)` to `(.6851,.002,.4115)`. Against the saved `Prop_Tray` support footprint of approximately `.7211077 x .4330797`, this measures `95.0066%` X and `95.0172%` Z, deliberately leaving a small float-rounding margin above the requested 95%. The roll, dispenser, paper material/collider, tray transform, receivers, placement behavior, six-cookie requirement, and restart behavior are unchanged. Play Mode accepted the enlarged sheet and all six non-overlapping portions.

## Current behavior — 2026-09-08 (supersedes fixed-slot descriptions below)

- E on Parchment Roll draws the existing single recipe sheet into the player's hand. No loose initial sheet; restart replenishes the roll. Repeated use does not create extra sheets. Cylinder roll/core and a short leading edge are simple prototype geometry using existing materials; no suitable roll asset was found. The small label identifies E: PULL SHEET / SHEET TAKEN.
- Board/liner/tray receivers use shared PlacementGeometry and SmoothPlacement. E-release close above the surface preserves chosen X/Z/yaw, fits the object's footprint, rejects obstruction/overlap/under-surface placement, and settles over .24s. Held entry/stay does not auto-capture. A moving tray retains paper/cookie local positions rather than reasserting an indexed grid.
- Cookie support is the actual parchment footprint. Six same-batch portions must finish settling before oven readiness is true. Removing one returns the count to five; replacement restores it. Raw and all cooked cookie states can be picked up and repositioned. Loaded parchment cannot be pulled from underneath food.
- Prop_Tray has PreparationItem and a matching invisible Tray Worktop Rest for smooth return to the existing worktop. Tray/cookie scales and collisions remain protected through existing pickup handling.
- Looking at the roll with any held item no longer claims E: it falls through to release. Cookie visual centers use the camera ray instead of the tools' -0.15m hold-height offset. Rotation, scroll reach, sweep obstruction and player-collision recovery remain active.
- Final Play Mode checks: actual roll ray pickup, held-release over roll, chosen paper offset, six portions, overlap rejection, five/six readiness, loaded-paper protection, moving loaded tray, cooked-cookie replacement and Perfect timed bake passed. Manual oven insertion/feel and VR remain pending.

## Current scene setup

- Scene: `Assets/BakeIT/Scenes/Kitchen Prototype.unity`.
- Tray root: `Prop_Tray`.
- Kitchen layout 2026-09-07: tray moved to `(1.68,1.16135,5.76)` and loose parchment to `(1.68,1.162,6.36)` in LINE & LOAD. Existing scales, tray child receivers, snap points, and runtime code remain unchanged. Actual parchment trigger contact passed at the new position; the four-portion callback-loaded tray completed a Perfect bake.
- Parchment trigger: `ParchmentPlacementZone` with a trigger `BoxCollider` and `TrayLinerReceiver`.
- Parchment snap target: `ParchmentSnapPoint`, positioned just above the tray surface.
- Dough trigger: Existing `DoughPlacementZone` with `TrayReceiver`.

## Runtime behavior

`TrayLinerReceiver` accepts an `Ingredient` whose `ingredientName` is `ParchmentPaper`. It releases the held Rigidbody, parents it to the tray, snaps it to the placement point, disables gravity, makes it kinematic, and ignores tray-to-liner collisions.

`TrayReceiver` discovers the liner receiver from the shared tray hierarchy during `Awake`. While the receiver exists, cookie portions are rejected until parchment has been placed. Parchment is explicitly ignored by the dough receiver so the two trigger zones can overlap safely.

The tray rejects a whole `FlattenedDough` slab and accepts only `CookieDoughPortion` objects with valid `DoughPortion` metadata. The first accepted portion establishes the active batch ID and expected yield. All later portions must belong to that batch. Accepted portions snap into an even grid, remain attached while the tray moves, and only form an oven-ready load when every expected portion is present.

## Verified sequence

1. A cookie portion entering the dough zone before parchment is rejected.
2. `ParchmentPaper` entering the parchment zone snaps and remains stable on the tray.
3. A whole `FlattenedDough` slab is rejected with an instruction to divide it first.
4. All four portions from the reference batch are accepted, arranged, and remain aligned while the tray moves.
5. Oven readiness stays false for an incomplete batch and becomes true when the final matching portion is loaded.

## Known limitations

- The placement zone currently accepts one parchment object at a time.
- The portion layout is capped at nine cookies for the current prototype tray dimensions.
- The current interaction uses prototype object names rather than recipe-step validation.
- Tray progress and incorrect-step messages appear in the recipe HUD. Scoring and data-driven step validation are not yet connected.

## Brownie fitted liner — 2026-09-13

The deep Brownie pan retains the single loose `ParchmentPaper` interaction, but accepted placement now displays a fitted liner made from a bottom and four raised side pieces. `BrowniePanReceiver` locks the source sheet to `Brownie Parchment Anchor`, removes its collision/physics influence, hides its flat renderer, and enables the fitted visual. This prevents the sheet from flying out when the pan is picked up while presenting realistic side coverage for expanding batter.

Reset restores the sheet's original parent, local transform, scale, renderer states, colliders, Rigidbody settings, and velocities. Play Mode verified five fitted pieces, zero anchor error after moving/rotating the pan, and a `.0024 m` raw-batter-to-liner gap.

## 2026-09-15 feedback endpoint

After the actual bake completes, Cookie portions gain serving metadata; remove the tray and release three cookies onto the serving plate. Baking alone no longer completes the session. Parchment and loading rules remain unchanged. Brownies require five cuts before individual portions can be served. See serving-and-plating-system.md.

## September 15 — Cupcake tray extension (current)

Cupcakes now use twelve independent wells and a finite stack of twelve fluted paper liners. The shared ParchmentDispenser accepts additional sheets/liners while existing rolls still supply one parchment sheet. CupcakeSlot handles released-liner placement and implements the shared IPourDestination interface for batter. CupcakeBatch owns readiness, counts and cooling; individual CupcakePortion bodies stay anchored while the tray moves. Occupied wells reject another liner. Filled portions cannot be picked up until baked and checked. Geometry is authored by CupcakeTraySetup under Generated/Cupcake Tray; no existing model was replaced. See cupcakes.md for controls and current boundary.
