# BakeIT Bowl and Mixing System

## Brownie batter fullness — 2026-09-13

The existing `Brownie Batter In Bowl` sphere presentation was too small and low for the approved recipe batch. It is now the selected B1 radial mound at local position `(0,.088,0)` and scale `(.105,.032,.105)`, with a roughly `.294 m` horizontal footprint and 12 walnut clusters. `PourableIngredientReceiver` still derives progressive height from the saved Y scale, so ingredient progress remains visible while the completed batch reads as fuller. No ingredient counts, mixing gates, transfer rules, colliders, Rigidbody values, or pan behavior changed.

Play Mode confirmed the B1 mesh/material and walnut hierarchy under the real bowl. The raw material is double-sided because Unity culled the radial mesh despite upward recalculated normals. The melted-butter surface and B1 batter remain mutually exclusive through the existing receiver logic. Evidence: `BakeIT_Notes/evidence/brownie-B1-bowl-final.png`.

## Exact cookie base ingredients — 2026-09-10

`BowlReceiver` now requires flour, sugar, one stick of butter, egg, vanilla, baking soda, and salt before the first of four base whisk passes can count. Vanilla appears as a small dark liquid pool; baking soda and salt appear as separate small light mounds, then fold into the existing dough visual during mixing. Chocolate chips remain the finishing ingredient: one 1/2-cup transfer followed by three whisk passes. The 1.2-second whisk cooldown and all existing dough colors/low-temperature behavior remain unchanged.

## Separate brownie bowl and melted-butter receiver — 2026-09-09

- Added `Brownie Mixing Bowl` from licensed `Prop_Bowel_02` at `1.4x`, approximately `.306 x .131 x .310 m` versus the cookie bowl's `.218 x .122 x .222 m`. The cookie bowl and its sole `BowlReceiver` remain unchanged.
- The new `.5 kg` bowl uses `PreparationItem`, `PickupPose`, a thin base plus 12-wall compound collision, and a dedicated trigger with `PourableIngredientReceiver`. It accepts only `MeltedButter`, only after this bowl is placed on `Mix Bowl Rest`, and only once.
- Accepted butter activates a golden liquid surface in the larger bowl. This is a brownie equipment/ingredient foundation, not a second full `BowlReceiver`: it does not yet accept flour, sugar, cocoa, eggs, vanilla, baking powder, or salt; mix batter; or produce a result.
- The user finalized the rack position at `(-3.4973,1.1859,4.1133)`, rotation zero, above `Brownie Bowl Holder`. The cookie bowl's support is named `Cookie Bowl Holder`. Gameplay validation depends on `PreparationItem.RestingSurface`, not either holder name or a hard-coded storage Transform.

### Recipe binding update — 2026-09-11

- The existing `PourableIngredientReceiver` now references `Brownies.asset` and validates the poured ingredient through recipe data while retaining its original held, tilt, destination, MIX-surface, and duplicate protections.
- Focused Play Mode passed a real held 90-degree melted-butter transfer and a second-transfer rejection that retained the extra cup contents. Other measured Brownie ingredients and mixing are the next milestone; the Cookie `BowlReceiver` was not reassigned or replaced.

## Mixing pacing retune — 2026-09-09

Player feedback found the initial `.35 s` protection still too fast. The saved and default `minimumTimeBetweenPasses` is now `1.2 s`. Movement during the cooldown is still discarded, and fresh travel is still required after the interval. Required base/chip passes remain `4/3`. Focused Play Mode held an attempted repeat at `0.65 s` to one pass, accepted the next fresh stroke after `1.2 s`, and completed base `4/4` plus chip `3/3`; subjective mouse fluidity remains for player acceptance.

## Mixing pass cooldown — 2026-09-09

Whisk passes now have a serialized `minimumTimeBetweenPasses=.35 s` (previously `.15 s`). After one valid pass, movement observed during the cooldown is discarded and accumulated travel resets to zero, so excess motion cannot remain banked and become another pass without a fresh stroke. The same tracker covers both four-pass base mixing and three-pass post-chip combining. Focused Play Mode verification held immediate repeats at `1`, then accepted fresh strokes after each interval through base `4/4` and chip `3/3`. Ingredient requirements, `whiskDistancePerPass=.06`, visuals, and completion counts are unchanged.

## Cookie presentation update — 2026-09-08

Evening measuring update: scoop contact no longer auto-empties. MeasuringScoop drives deliberate held tilt; BowlReceiver.TryPourMeasure validates preparation, destination, utensil role and ingredient/order/quantity before accepting a transfer. Duplicate sugar or premature chips stay in the utensil for matching-source return or deliberate Q-discard. Cup is flour/sugar-only; small spoon is chips-only. Whole egg/butter entry and both mixing counters remain unchanged. ContentsTransferred records accepted measures separately from returns/waste; no bowl quantity changes on rejection or discard.

Raw/formed dough now uses pale (.93,.80,.57), not the flour presentation color. CookieAppearance decorates the existing final MixtureResult with subtly varied geometry, grain and 24 scattered chips; bowl/scoop intermediate chip clusters remain unchanged. Four base passes, three chip measures and three combining passes still gate final dough. The flour/sugar interior correction is preserved. Initial non-readable mesh error was corrected by enabling Read/Write on the original Food_Dough model; final-source Play Mode returned no such errors. Preparation feedback now names Utensils Area.

## Purpose

`BowlReceiver` owns the current chocolate-chip-cookie ingredient state, in-bowl presentation, mixing progress, and creation of the raw dough object. It extends the existing bowl trigger rather than introducing a parallel recipe manager during the one-dish prototype stage.

## Scene configuration

- Scene: `Assets/BakeIT/Scenes/Kitchen Prototype.unity`.
- Receiver object: `IngredientTrigger` with a trigger collider and `BowlReceiver`.
- Result object: Existing `Dough` GameObject referenced by `mixtureResult` and inactive until mixing completes.
- Mixing tool: Existing whisk carrying `MixingTool`.
- Current layout: bowl starts on the holder shelf at `(-3.5,1.056,4.12)` and is carried to the central Mix Bowl Rest. Trigger, contents and inactive result remain attached during transport. Whisk begins in the tool area; cold ingredients begin in the fridge.
- Chocolate presentation: `Assets/BakeIT/Prefabs/ChocolateChipCluster.prefab`, assigned to `chocolateChipVisualPrefab`.

## Contents containment — corrected 2026-09-08

- Root cause of apparent flour leakage: the visual anchor was derived from the raised/offset scoop trigger and then lowered again. Accepted flour extended to bowl-local Y `-.03148`, below the vessel; the flour visual has no collider, so this was clipping, not simulated spilling.
- `ingredientVisualCenter=(0,.06,0)` is now serialized in bowl-parent coordinates and converted to the existing visual hierarchy. Trigger position and physics remain unchanged.
- Flour width multiplier `.72 → .62`, X offset `.08 → .02`, Y offset `-.13 → -.12` (relative to trigger diameter). Quantity-one flour now starts at local Y `.01258`, with minimum measured inner-shell clearance `.00115`.
- Sugar offsets X `-.18 → -.08`, Z `.08 → .04` keep its mound inside the sidewall. No quantity, material, recipe count or mixing threshold change.
- Play Mode geometry checks found zero protruding vertices for flour/sugar/butter/egg/base dough/chips after fitting. A fresh-source filled-bowl ray pickup, rotated return and recipe reset retained correct presentation/state. Before, iteration and final PNG evidence is in `evidence/flour-*`.

## Recipe state

2026-09-07 evening correction: `BowlReceiver` uses the existing `MiseEnPlaceStation`. Five ingredients must rest on Cold Ingredient Rest, both measuring spoons/whisk on Mix Tool Rest, and the movable bowl on Mix Bowl Rest before readiness latches. Whole-ingredient/scoop entry is rejected non-destructively until then, and while the bowl is lifted or returned elsewhere. Containers still do not carry `Ingredient`; entire bags never count as measures.

The serialized requirements are one flour scoop, one sugar scoop, one butter portion, one egg, and three chocolate-chip scoops. Names are trimmed, canonicalized, and compared without case sensitivity. The four base ingredients must be present before whisking, and chocolate chips are accepted only after base dough forms. Unsupported ingredients, duplicate measures, and all inputs after completion are rejected before the scoop is emptied or a whole ingredient is consumed. A premature chip scoop is the deliberate exception: it is cleared without incrementing the bowl so the single shared scoop remains usable.

The current recipe state is kept in ingredient references and a case-insensitive quantity dictionary. Visual objects read that state but do not define it.

## Mixing behavior

- Mixing cannot advance until the four exact base-ingredient requirements are present.
- Entering the bowl with the whisk logs any missing quantities.
- Whisk movement is accumulated into four distinct prototype passes, with a `.35 s` cooldown and fresh travel required between counts.
- The intermediate mixture visual changes position, scale, color, and smoothness as progress advances.
- On the fourth valid pass, the ingredients become a visible base-dough mound while the final `Dough` result remains inactive.
- Three small chocolate-chip measures are then accepted. The mound grows with transfers but finished dough stays inactive. `requiredChipMixPasses=3` adds a separate final-combining counter; the third valid whisk pass activates chip-bearing raw dough.
- The new counter cannot advance before all chips are added. Whisk tracking measures bowl-local movement and clears tracking while the bowl is not placed, preventing carrying from counting as stirring. The last scoop resets tracking so earlier whisk travel is not credited.
- The original bowl mesh remains unchanged. Its static MeshCollider is disabled; a kinematic .35 kg Rigidbody, PreparationItem, .13 × .016 × .13 base and twelve .052 × .076 × .013 wall boxes preserve a hollow movable vessel. Walls sit at radius .091, Y .05, in 30-degree steps.
- Ingredient visuals remain children of the bowl trigger and quantities survive transport. Placement preserves release X/Z/yaw and scale. The bowl starts on the existing tray-holder asset's added tray shelf and can be returned there; only the central MIX rest enables recipe actions.

## Downstream contract

The output keeps the canonical ingredient name `Dough`. `ChocolateChipVisual` separates decorative chocolate renderers from the dough base. The preparation board therefore excludes chips from represented-volume yield calculations, and the oven excludes them from result recoloring.

## Earlier verification (before final-combining revision)

- A focused Play Mode sequence accepted exactly one flour, sugar, butter, and egg through the existing whole-ingredient and shared-scoop paths.
- A premature chip measure was cleared, did not increment chip state, and left the scoop reusable.
- Mixing before the final missing base ingredient was rejected and did not advance the pass count.
- Four valid passes formed base dough without completing the recipe; the third subsequent chip measure activated finished chip-bearing raw dough.
- The downstream reference batch remained four portions, with one visible chip marker per portion.

## Current limitations

- Quantities are interaction units rather than grams.
- Missing-ingredient, rejection, progress, and completion messages appear in the player-facing recipe HUD.
- Ingredient sources do not deplete.
- `CookieRecipeSessionController` can restart the entire saved sequence while Play Mode remains active.
- Final player-facing pickup, reachability, collision-recovery, and whisk-gesture acceptance remains open.

## Recipe-data migration — 2026-09-09

- `IngredientTrigger/BowlReceiver` now holds the scene's active `ChocolateChipCookies` `RecipeDefinition` reference and exposes it to existing downstream systems. No second manager was introduced.
- The asset supplies the five requirements and their Base/Finishing stages, exact quantities/tool roles, four base passes, and three finishing passes. The legacy serialized cookie fields remain unchanged as fallbacks for an unassigned isolated receiver.
- Validation now looks up the active requirement rather than accepting from a hardcoded ingredient switch. Whole-item consumption follows the requirement's `WholeItem` role. Current ingredient visuals remain intentionally cookie-specific.
- Runtime receiver tests rejected early chips, unlisted Cocoa, duplicate flour, and a fourth chip; accepted the approved base quantities; formed base dough at 4 passes; and completed at 3 finishing passes. Existing 1.2-second gesture cooldown behavior is unchanged.

## 2026-09-15 player feedback — completed batter and containment

Brownie full-fill local position changed from (0,.088,0) to (0,.065,0); scale changed from (.105,.032,.105) to (.088,.025,.088). Play Mode vertex bounds and renders keep the completed batter below the rim. The guide and highlight director require MIX placement only while batter is incomplete, so lifting completed batter to pour no longer asks the player to return it. Four base and two finishing passes still apply.

## September 15 follow-up — tapered-wall clearance and empty-bowl appearance

The earlier rim-only correction missed the tapered side wall. Full-fill local position is now (0,.073,0), scale (.075,.018,.075), previously (0,.065,0)/(.088,.025,.088). A dedicated non-emissive Sage Mixing Bowl material replaces the very bright atlas appearance only on the Brownie bowl. The shared Panda material and other props are preserved. Once batter transfers, tutorial targeting advances to pan tasks and clears any obsolete bowl-placement correction. Ordinary crosshair hover feedback remains a separate interaction.

Walnuts appear on the batter once the finishing ingredient is accepted, and the larger falling-kernel effect accompanies transfer before folding. See the follow-up evidence and test log.

## September 15 — Cupcake staged bowls

Two Cupcake bowls extend PourableIngredientReceiver with an optional MixingSequence reference and Dry/Batter role. Ingredient physics, measured transfers and whisk tracking remain shared. Current-phase requirements enforce separate dry mixing, creaming, individual eggs, scraping, half dry mixture, milk and remaining dry mixture. Only held tools can count staged work. MixingTool now distinguishes Mix from Scrape; old tools retain Mix by default. StagedMixingVisual controls a contained pale fill independently from Brownie visuals. Brownie reference resolution explicitly selects its recipe-bound receiver so multiple bowls cannot redirect existing guides/tests. Cupcake bowls begin on labelled rests; the shared placement system handles returns.
