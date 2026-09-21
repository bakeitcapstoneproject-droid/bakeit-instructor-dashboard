# BakeIT Ingredient Measuring System

## Current clarification — 2026-09-15

Brownies is selectable and uses the exact recipe capacities, including 1/2 cup Cocoa/Walnuts, separate 1-cup and 1/4-cup Flour, 1 teaspoon Vanilla and 1/2 teaspoon Baking Powder/Salt. Cookies also requires the 1/2 Cup for its one chip transfer; only the 1/4 Cup is irrelevant to the Cookie cup set. Statements below describing both extra cups as unnecessary for Cookies or Brownies/Walnuts as locked are superseded.

Full measures cannot silently replace contents. Return by tilting over the matching source; deliberately hold Q to discard. September 6 CodexReview replacement tests are historical, not current requirements. See the [active checklist](cookie-usability-acceptance-checklist.md) for both formulas and pending player tests. No runtime change or new Play Mode result is implied.


## Exact measured set — 2026-09-10

The active cookie recipe now validates exact capacities instead of treating every cup or spoon as interchangeable:

- `1 Cup Measuring Cup`: flour and white sugar.
- `1 Teaspoon`: vanilla extract.
- `1/2 Teaspoon`: baking soda.
- `1/4 Teaspoon`: salt.
- `1/2 Cup Measuring Cup`: chocolate chips; the cookie recipe uses one transfer. The former dedicated Chocolate Chip Scoop was removed.
- `1/2 Cup Measuring Cup` and `1/4 Cup Measuring Cup`: present on the rack for approved brownie quantities but not required during cookie mise en place.

Six measuring vessels and the whisk begin on `Measuring Tool Wall Rack`, whose seven matching `ToolHangingSlot` hooks accept pickup and forgiving return snaps. A hook will not accept the wrong capacity. Permanent markings on carried tools were removed in favor of direct-hover screen labels that remain upright regardless of tool orientation. `Utensils Area` is back to its original footprint; the separate `Tool Home Mat` is enlarged. Vanilla, baking soda, and salt remain carryable measured sources in dry storage; the walnut source remains for the locked brownie recipe.

## 2026-09-11 exact chip-measure correction

The cookie `RecipeDefinition` now requires one `HalfCup` measure for chocolate chips. The physical `1/2 Cup Measuring Cup` accepts chips and reuses the existing dense chip-fill meshes; the dedicated chip scoop and its rack hook were removed. Hovering directly over a tool displays its name in screen space, so no capacity text is baked into or allowed to float through the world model.

Ingredient carriers now use one small backed player-facing label. The old reverse duplicates and redundant chip label were removed; final audit found zero `Reverse` labels.

## Pre-melted butter tilt transfer — 2026-09-09

- Added a separate `PourableIngredient` interaction for the user-approved brownie butter rather than misrepresenting solid butter as melting in a room-temperature bowl or reusing `MeasuringScoop` as a liquid container.
- `Melted Butter Cup` starts with one visible cup of golden liquid. It must be picked up, tilted at least 75 degrees, and kept over the dedicated brownie-bowl receiver for `.35 s`. Its pour point uses a `.012 m` mouth overlap plus a downward search up to `.35 m`, matching the existing desktop tilt vocabulary without changing powder/chip measuring rules.
- Transfer shows short collider-free golden droplets, hides the cup fill, and shows the accepted liquid in the larger bowl. Upright contact, empty-cup attempts, the cookie bowl, a brownie bowl still in storage, and duplicate pours retain the appropriate state instead of silently adding an amount.
- This liquid container does not refill, return to a source, support Q-discard, record grading data, or count as cookie butter. Those behaviors were not requested. The current `MeasuringScoop`, `MeasureTransfer`, and cookie recipe remain unchanged except that `MeasureTransferVisual` now recognizes `MeltedButter` for golden presentation.

## Current measuring/recovery contract — 2026-09-08 evening

This supersedes the older auto-empty and stale-content replacement behavior below.

- **Measuring Cup** is the existing _04 utensil formerly named Large Measuring Spoon. It measures flour and sugar only. **Small Measuring Spoon** (_01) measures chocolate chips only. GameObject names are presentation; the existing capability flag determines tool eligibility. Bowl validation checks the same roles.
- Hold the empty utensil reasonably upright (opening within55 degrees of world up) and dip into its source. Only a held utensil fills; touching a different source never overwrites a full measure. Wrong tool/source gives a correction and retains contents.
- Hold right mouse and move to tilt the utensil at least75 degrees, with its mouth over the bowl, for .35s. The destination is found from mouth overlap/downward ray within .35m. Upright touch alone does not transfer. Correct bowl preparation and ingredient/order/duplicate checks still apply. Rejected/early measures remain full instead of disappearing.
- Tilt over the matching source to return an unwanted measure. A return records ReturnedToSource, not waste. Lift the mouth at least .07m clear of that source collider before refilling; this prevents a return immediately filling the utensil again. Wrong-source returns are refused without losing ingredients.
- Hold Q for .60s while carrying a full utensil to deliberately discard. Releasing Q early cancels; an empty utensil cannot generate extra waste. Merely tilting in empty space does not spill automatically. No bin, persistent floor mess or clean-up requirement was added.
- MeasuringScoop emits ContentsTransferred with MeasureTransfer records: ingredient, tool, unit, quantity1, destination, disposition and session time. Dispositions are AddedToBowl, ReturnedToSource and Discarded. Existing CookieRecipeSessionController stores the read-only session history and DiscardedMeasureCount; restart clears both. This is preparation for grading, not grading itself or cloud persistence. Units are cup measures/small-spoon measures, not calibrated grams.
- MeasureTransferVisual displays twelve brief collider-free grains/chips from mouth to destination. They own no quantities and disappear after .40s. Source stock is still unlimited; returning records the recovery but does not increase an inventory counter.
- Scene: cup name changed, cup mouth height .023→.041 and inner radius .0205→.0365 for the larger existing mesh. Existing contents visual, models, Rigidbody, source triggers, user layout and chip fill remain. Six cookies and recipe pass counts unchanged. One extra contextual HUD row shows held contents/pour-return; footer shows RMB tilt and hold-Q discard, without a permanent waste panel.
- Verification and limitations: normal sugar/flour pours, retained duplicate sugar, matching return, wrong-source rejection, refill lock, chip pours and session events were tested in Play Mode. Background input injection did not remain pressed across automatic frames; Q accumulation/cancellation was verified with explicit input-backed scoop updates. Human keyboard/tilt comfort still needs focused Game-view testing. See dated test-log and change-documentation.

## Latest preparation change — 2026-09-07 evening

Prepare source containers plus egg/butter on Cold Ingredient Rest; the cookie-required exact measures and whisk rest separately on `Utensils Area`. Sources remain in cabinet/fridge initially and use hover names. The 1 Cup handles flour/sugar; the 1/2 Cup handles chips. The bowl must be placed in MIX to accept transfers. One chip measure requires three subsequent whisk passes before finished dough appears. Earlier board-staging instructions below are superseded.

## Purpose

Latest scene correction (2026-09-07): the three source containers are now carryable `PreparationItem` bodies, not whole `Ingredient` measures. Flour/sugar begin inside the sliding cabinet at world `(1.112,.408,3.09)` / `(1.688,.408,3.09)`; chips begin in the fridge at `(-2.35,1.544,3.13)`. Names attach to both sides of each item. Original source triggers move with their containers and actual post-transport flour, sugar and chip filling passed. Earlier pantry-row positions below are retained as first-pass history and are superseded.

The powder bags use convex MeshColliders. The chip bowl uses a compound bottom/eight low walls instead of a moving non-convex collider, preserving its cavity. Pickup ignores attached text and zero-sized renderers for centering. Release near the prep board/shelf stabilizes without any sideways slot jump. Whole bags/bowl cannot satisfy bowl quantities; the small spoon still provides the measures and only it accepts chips. Prepare all seven supplies on the board first; see `kitchen-layout-and-storage.md` for values and tests.

The intended cookie utensil is the scene instance of `Prop_MeasuringSpoon_01`, renamed `Small Measuring Spoon`. It measures only chocolate chips. Every source fills a compatible tool through `MeasuredIngredientSource`, and `BowlReceiver` validates the contents before emptying it. The `_04` utensil is labeled `Measuring Cup` and measures only flour and sugar. Eligibility does not depend on the GameObject name.

## Scene configuration

- `FlourScoopZone` supplies `Flour`.
- `SugarScoopZone` supplies `Sugar`.
- `ChocolateChipContainer/ChocolateChipScoopZone` supplies `ChocolateChips`. Its local trigger size is `(0.30, 0.28, 0.30)` with center `(0, 0.16, 0)`, enlarged from `(0.22, 0.18, 0.22)` and `(0, 0.13, 0)` so a small spoon can enter it more reliably.
- `ChocolateChipContainer` reuses the licensed open-bowl model, has a non-convex shell collider, a trigger inside the opening, three visual-only chip layers, and a player-facing `CHOCOLATE CHIPS` label.
- The larger spoon keeps its original flour/sugar visual. Its old chip child is hidden; it cannot collect chips or replace existing powder with chips. A throttled corrective message says `Use the small measuring spoon for chocolate chips.`
- `Small Measuring Spoon` creates a powder fill fitted to its cup and instantiates the existing `ChocolateChipCluster.prefab` for chips. Eight lightweight cluster meshes form four interleaved height levels. Their actual mesh pivot and footprint are normalized so every chip stays within local radius `0.0205`, inside the measured `0.02152` inner rim. Chip geometry spans local Y `0.010-0.028`, with the highest tips only approximately `0.0043` above the cup's `0.02367` rim. No sideways layer offsets or oversized procedural spheres remain.
- Small-spoon Inspector values: `acceptsChocolateChips=true`, `chocolateChipContentsPrefab=ChocolateChipCluster`, `cupInnerRadius=0.0205`, `cupFillBottom=0.005`, `cupFillSurface=0.023`, and `chipMoundHeight=0.005` (code default). Large-spoon eligibility is false with no chip-prefab reference. Vendor models/materials and pickup physics are preserved.
- Sources attempt filling on both trigger entry and trigger stay. Entering a different source replaces stale contents (for example, rejected flour becomes chocolate chips), while remaining in the same source does not repeatedly refill or spam feedback.

The reusable source prefab is `Assets/BakeIT/Prefabs/ChocolateChipContainer.prefab`. Its root is normalized to zero position, identity rotation, and unit scale; the 2026-09-07 kitchen layout places the scene instance at `(-1.43, 1.16135, 6.28)`, clear of the board. Its tall scene label is disabled in favor of a low pantry label; the vendor/source prefab is unchanged.

Flour and sugar are now at `(-2.30, 1.16135, 6.28)` and `(-1.95, 1.16135, 6.28)`. Small/large spoons and whisk begin at the labeled cabinet tool home, not scattered across the main counter. Egg/butter begin on the fridge's middle shelf; bring them to MIX before preparation. See `kitchen-layout-and-storage.md` for all poses and storage behavior. Ingredient quantities, chip eligibility, source trigger sizes, and fill geometry did not change. Actual chip-source contact and all four gathered-object pickups passed the new-layout Play Mode check.

## Bowl requirements

The prototype batch requires:

- Flour scoop
- Sugar scoop
- Butter portion
- Egg
- Three chocolate-chip scoops after base-dough mixing

The quantities are serialized on `BowlReceiver`: the four base ingredients are set to `1`, while chocolate chips are set to `3`. Ingredient names are trimmed and compared without case sensitivity. Unsupported ingredients, duplicate measures, and inputs offered after final dough completion are rejected before the spoon or whole ingredient is consumed. Chips offered before base mixing are cleared without being counted. If a rejected base measure remains after mixing, dipping into the chip container replaces it instead of blocking progression.

## Visual behavior

- Filling the small spoon with chips hides its powder contents and shows eight interleaved cluster meshes; emptying hides all eight. The shared mesh/material reuse replaces the previous oversized sphere mound.
- Accepted ingredients appear inside the bowl.
- After four valid whisk passes, the base dough remains in the bowl while three chip measures are added. The in-bowl chip visual grows with each measure, and a `ChocolateChipVisual` cluster is attached to the finished dough after the third.
- Portion cloning carries that decoration to every cookie.
- Dough recoloring and oven result recoloring skip marked chip renderers.
- Proportional yield calculations exclude chip renderers so decorative chips do not create extra cookies.

## Current limitations

- Ingredient quantity is measured in interaction units rather than grams.
- Sources have no depletion state.
- Recipe progress and rejection messages are shown in the player-facing recipe HUD and retained as ordinary Console logs for development.
- Pressing `R` restarts the complete sequence without leaving Play Mode.

## Sugar Jar visual replacement — 2026-09-11

- `SugarContainer` now uses the project-owned `Assets/BakeIT/Generated/Sugar Jar/Sugar Jar Visual.prefab` instead of the milk-bottle placeholder.
- The visual is collider-free and does not own ingredient state. The established root `BoxCollider`, `Rigidbody`, `PreparationItem`, `HoverDisplayName`, and child `SugarScoopZone/MeasuredIngredientSource` continue to own physics, interaction, and measuring behavior.
- The jar has an open mouth and visible off-white granulated sugar. The existing scoop trigger remains at its saved position and still identifies the source as `Sugar`.
- Play Mode passed jar pickup/release and 1-cup Sugar fill/empty regression. Remaining pantry models will be handled separately, one user-approved asset at a time.

## Flour, Cocoa, and Baking Powder visuals — 2026-09-12

- `FlourContainer`, `Cocoa Powder Container`, and `Baking Powder Container` now use independent reference-driven prefabs from `Assets/BakeIT/Generated/`.
- Visual children contain no colliders and no persistent TextMesh names. Existing root body colliders, Rigidbodies, hover labels, and measured-source triggers continue to own gameplay behavior.
- Exact source IDs remain `Flour`, `Cocoa`, and `BakingPowder`. Brownie-tool regression passed 1 Cup Flour, 1/2 Cup Cocoa, and 1/2 Teaspoon Baking Powder, plus wrong-tool rejection for Cocoa and Baking Powder.
- Closed-body size order is Flour tin > Cocoa tin > Baking Powder can. The accepted Sugar Jar remains wider than the two small cylindrical cans. Open lids extend presentation bounds without enlarging physical colliders.

## Salt, Vanilla, and Chocolate-Chip Bowl visuals — 2026-09-12

- `Salt Container`, `Vanilla Extract`, and `ChocolateChipContainer` now use independent local prefabs under `Assets/BakeIT/Generated/`, derived from the supplied turnaround references.
- Salt uses a compact ceramic cellar with open lid, spoon, and visible salt. Vanilla uses a taller clear/amber bottle with a green cap and flower label. This preserves the requested realistic hierarchy: the Salt body is `.082 m` high while Vanilla is `.154 m` high.
- The Chocolate-Chip source now presents a white ceramic bowl containing three interleaved layers of the existing dark-chip mesh. The prior visible bowl/chip layers are disabled, while `ChocolateChipScoopZone`, `Chip Bowl Base`, and all eight rim colliders remain authoritative.
- All new presentation hierarchies are collider-free. Root physics/pickup behavior, hover-only names, preparation state, and measured-source IDs remain unchanged: `Salt`, `Vanilla`, and `ChocolateChips`.
- Play Mode passed correct and incorrect exact-tool routing: Brownie Salt uses 1/2 Teaspoon, Brownie Vanilla uses 1 Teaspoon, and Cookie Chocolate Chips use 1/2 Cup. Filled contents also emptied/reset successfully.

### Flour and Chocolate-Chip visual correction — 2026-09-12

- The Flour Tin's visible off-white fill now sits just below its open rim, so the source no longer appears empty from normal player angles. Its package panel also carries a modeled `FLOUR` wordmark; this is not a contextual hover label and adds no TextMesh or collider.
- The Chocolate-Chip Bowl now combines a contained packed bed with six overlapping instances of the existing chip cluster. Every cluster is grounded from its actual mesh minimum rather than vertically stacked, increasing visible fullness without floating chips, new physics geometry, or scoop-source changes.
- Visual prefabs remain collider-free. Flour continues to identify as `Flour`; the chip source continues to identify as `ChocolateChips` and retains its base plus eight rim colliders.
- Recipe regression passed Brownie 1 Cup Flour, Cookie 1/2 Cup Chocolate Chips, and wrong-tool rejection of Chocolate Chips by the Cookie 1 Cup measure.

### Physical ingredient identifiers — 2026-09-13

Ambiguous source packages now carry small, depth-tested, cream-backed front names for Sugar, Melted Butter, Cocoa, Vanilla, Baking Powder, Salt, Walnuts, and Chocolate Chips. Flour keeps its modeled `FLOUR` wordmark, and Baking Soda uses its corrected dedicated carton label. These visual children have no colliders and do not alter source IDs, hover names, quantities, scoop triggers, pickup physics, or recipe routing. Eggs remain recognizable whole items with hover naming rather than attached plaques.

The Cocoa and Baking Powder plates were subsequently moved beyond their front badge geometry and enlarged after the first placement proved physically present but visually buried. Flour now has a shallow uneven powder top, Sugar has a brighter mound plus 18 visible grains, and the Walnut bowl contains 34 collider-free crinkled pieces. Play Mode revalidated exact `Flour`, `Sugar`, `Walnuts`, `Cocoa`, and `BakingPowder` source IDs.

### Walnut Bowl and Wrapped Butter visuals — 2026-09-12

- `Walnut Container` now uses a clear glass prep-bowl presentation with grounded chopped kernels; its existing `Measured Source Trigger` still reports the exact recipe key `Walnuts`.
- `Food_Butter` now uses a wrapped 120 x 30 x 30 mm stick presentation with modeled two-sided package artwork; its whole-item `Ingredient.ingredientName` remains exactly `Butter`.
- Both presentation prefabs are collider-free. Walnut measuring continues through the existing trigger while root colliders/Rigidbodies continue to own pickup and placement. No measuring quantities or recipe routing were changed.

## 2026-09-06 evening verification

- Real Play Mode ray pickup and physics-trigger contact passed for the small spoon, including stale-flour replacement and refill while remaining inside the source.
- Large-spoon chip rejection preserved its flour contents; subsequent sugar measurement succeeded.
- Every final chip-mesh vertex stayed within the configured envelope; the only active spoon collider was the original model collider.
- Three post-whisk chip transfers produced dough that subsequently survived board pickup/recovery, portioning, tray loading, and a successful four-cookie bake.
- The final saved-code render was inspected: [contained small-spoon fill](evidence/small-spoon-contained-fill-2026-09-06.png). Desktop reach/rotation comfort remains a player acceptance item.

## Recipe-data migration — 2026-09-09

- `MeasuringScoop` resolves the active definition from `BowlReceiver`. Flour and sugar are configured for `MeasuringCup`; ChocolateChips is configured for `SmallMeasuringSpoon`; butter and egg are whole items.
- With an active definition, an ingredient absent from that asset is rejected rather than falling back to cookie assumptions. Wrong-tool feedback names the configured required tool.
- Play Mode verified the actual cup accepts flour/sugar and rejects chips/Cocoa, while the actual small spoon accepts chips and rejects flour/Cocoa. Transfer timers, return/discard accounting, contents geometry, and source behavior were not changed.

## 2026-09-15 feedback — measuring instructions and recipe paper

The Cookie finishing row now says Chocolate chips and half-cup measures explicitly. Guided practice colors recipe-paper ingredient lines green only after all matching exact requirements are satisfied, including both Brownie Flour capacities. Partial Sugar and Flour checks passed; disabling guided practice restores plain text. Source collider alignment now follows the visible body/mouth and excludes open lids. Recovery rules and measuring quantities remain unchanged.

## September 15 follow-up — smoother pouring and walnut transfer

Melted butter pours over 1.6 seconds of valid tilt, lowered from a 75-degree threshold to 55 degrees and extended from the earlier .35-second transfer. Keep the spout above the bowl: downward detection supports .40 m. A continuous golden stream tracks the spout and receiver, while the jug's visible liquid level drains. Leveling or moving away stops the stream and pauses progress; the same bowl can resume it. Quantities commit once, only when the full cup completes. A rejected destination does not show a pour. No physical spill accounting was added.

Walnut transfers now show 24 tumbling chunks using the existing kernel meshes over approximately one second; Cookies keep their existing short chip effect. Accepted walnuts are visible on the batter before the finishing whisk passes. Measuring units and recipe quantities are unchanged.

## September 15 — Cupcake measurements and dry blend

Added exact tablespoon capacity (enum 10), with a matching rack slot; active-recipe resolution also handles a tool first awakened after selection. Milk and oil sources expose measuring zones above their openings, and their measured contents have distinct colors. Cupcake softened butter is a separate 84 g / 6 tbsp whole portion; it cannot substitute the Cookie stick. Milk uses one half cup plus two tablespoons, and the recipe retains all compound flour/leavening/sugar/vanilla measures. The staged receiver checks phase quotas and bowl identity before emptying a measure.

PourableIngredient optionally obtains two half-bowl portions from MixingSequence. A valid 1.4-second tilt transfers one half, then waits for the later dry-addition stage. Powder falls through the shared transfer visual; the Dry bowl fill falls progressively. Melted butter retains its continuous liquid stream and existing duration. This is visual presentation with exact discrete recipe portions, not a fluid or powder simulation.

## September 15 — Shared pour destinations and Cupcake fills

PourableIngredient now targets IPourDestination, implemented by the existing bowl receiver and new Cupcake wells. Existing melted butter and dry-blend transfers retain their receiver validation. The Cupcake batter source fills each liner over 0.9 seconds, remembers that liner's partial progress, displays the rising fill and drains the bowl by aggregate transferred volume. A completed well cannot accept more batter. Twelve exact three-quarter fills unlock the oven; raw or partially filled portions cannot be picked up.
