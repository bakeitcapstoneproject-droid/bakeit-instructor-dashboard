# BakeIT Dough Board and Portioning System

## Recipe-data migration — 2026-09-09

`DoughBoardController` now reads mixed, flattened, and portion ingredient names plus the rolling-pass and target/minimum/maximum yield values from the active cookie definition. Current values are `Dough`, `FlattenedDough`, `CookieDoughPortion`, four rolls, and fixed target/minimum/maximum `6/6/6`; every accepted batch yields the six portions named by the recipe and guide. The 1.2-second cooldown, distance threshold, geometry, scaling, placement, knife behavior, and cookie visuals are unchanged. Pouring brownie batter or filling cupcake liners is configuration vocabulary only and is not implemented on this board.

## Rolling pacing retune — 2026-09-09

Player feedback found the initial `.35 s` protection still too fast. The saved and default `minimumTimeBetweenRollingPasses` is now `1.2 s`, retaining discarded cooldown motion, fresh-distance gating, and at most one count per sample. Four required rolling passes and the `.2 m` distance threshold are unchanged. Focused Play Mode held an attempted repeat at `0.65 s` to one pass, accepted the next fresh roll after `1.2 s`, completed `4/4`, and allowed the forward-held knife to create six portions; subjective mouse fluidity remains for player acceptance.

## Rolling pass cooldown — 2026-09-09

Rolling now uses serialized `minimumTimeBetweenRollingPasses=.35 s`. A movement sample can register at most one pass; movement during the cooldown and excess distance from the accepted stroke are discarded, requiring fresh rolling travel for the next count. This replaces the prior `while` loop that could consume several `.2 m` thresholds in one update. Focused Play Mode verification gave one count for a `.65 m` sample, blocked the immediate reverse stroke, then accepted separated strokes through `4/4` and produced `FlattenedDough`. Required passes, distance threshold, scale progression, pickup recovery, and portion yield are unchanged.

## Current override — 2026-09-08

- The clarified batch is **six total**, not six extra: reference/minimum/maximum yield are all 6 in Kitchen Prototype. Ingredient quantities and four rolling passes are unchanged. Each portion uses the same batch metadata and equal planar scale reduction; decorative chips never count as dough volume. Earlier four-to-nine descriptions below are historical.
- Board entry/stay never steals held dough. E-release or released contact validates nearby upright placement, preserves chosen X/Z/yaw, and settles over .24s. Full footprint and obstruction checks remain; no beneath-board or distant snapping. Rolling/cutting wait for the settle. Uncut pickup-return preserves prior rolling progress.
- New portions spread smoothly from the slab into six board positions, supported .003m above the board instead of the old floating placement-point height. Portions have PreparationItem so they can be smoothly returned to the board; tray loading is also free-position rather than indexed teleportation.
- CookieAppearance reuses the original Food_Dough model with subtle deformation/grain and one uniquely seeded chip mesh per portion (8,9,10,8,9,10 chips). Model Read/Write is enabled for this runtime geometry. No reference-image bitmap or replacement food asset was imported.
- Play Mode: held hover ignored, release X/Z preserved, uncut return at 2/4 retained progress, four passes and knife callback yielded six; one chip mesh per cookie verified after the clone-rebuild fix. Exact full manual gesture acceptance is pending.

## Latest workstation flow — 2026-09-07 evening

Board center moved from X .05 to .45 (Y1.16135/Z5.86, scale3.5/2.5/2.5 unchanged). Ingredients and mixing tools no longer need board staging: they belong on their separate rests left of MIX. SuppliesOnBoard remains an accidental-obstruction recovery check and includes the new movable bowl/large spoon. Final dough is created only after three post-chip whisk passes; then existing transfer, four rolls, uncut/flattened pickup-return and four-portion reference yield apply. Evening Play Mode passed relocated board acceptance, flattened re-pick/return at unchanged scale and 4/4 progress, four portions, tray and Perfect bake.

## Existing scene and components

- Scene: `Assets/BakeIT/Scenes/Kitchen Prototype.unity`.
- 2026-09-07 layout: existing board moved to `(0.05, 1.16135, 5.86)` with its scale `2.5` and child work-zone/snap relationships preserved. Its scene renderer uses `Kitchen_PrepWood.mat` / `BakeIT/Prep Wood`, a brown procedural fine-grain material; the vendor model/material is untouched. The chip source no longer overlaps it. Flattened pickup/rotation/physical return retained `4/4` and world scale `(1.4, 0.4, 1.4)` in the new layout, followed by actual knife contact producing four portions and a successful bake.
- `DoughWorkZone` uses `DoughBoardController`, a trigger collider, and the existing placement point on the preparation board.
- The existing rolling pin carries `RollingPinTool`; the knife carries `DoughPortioningTool`.
- Four rolling passes turn `Dough` into `FlattenedDough`. One valid knife pass creates the proportional batch of four to nine `CookieDoughPortion` objects.

## Pickup and return recovery

2026-09-07 preparation correction: board scale is now `(3.5,2.5,2.5)` (width .70 m) to accommodate five ingredients plus spoon/whisk. `StablePlacementSurface` handles these marked supplies only, preserving their drop X/Z/yaw with a small vertical settle; it does not handle dough. `DoughBoardController` defers dough placement while staged containers/tools remain, matching the CLEAR THE PREP BOARD guide. It retries through the existing trigger entry/stay flow after they are removed. The widened board still produced four portions and a Perfect bake; dough quantities and count thresholds were not changed. The earlier scale-2.5 statement describes the first layout pass.

The board accepts both raw and flattened uncut slabs. It remembers the same slab's rolling count, partial rolling distance, original scale, and target board-local scale when pickup detaches it. Returning the slab restores those values instead of resetting preparation or flattening the object a second time.

`HasDough` and `IsDoughFlattened` also check actual board parenting. A slab held away from the board cannot be cut or rolled by tools left on the work surface. Trigger entry retains the existing automatic placement behavior; trigger stay additionally catches a slab released while already overlapping the zone. Trigger stay leaves an actively held object alone, allowing normal pickup.

The same slab's saved board-local target scale is restored after carried rotation, preventing repeated parenting from shrinking or enlarging it. A new slab establishes its own preparation state. Portioning clears the remembered slab state, and cookie portions are excluded from uncut-dough placement.

## Verification on 2026-09-06

- Actual physics placed the finished raw dough on the board; early knife cutting was rejected.
- At `2/4`, real ray pickup detached the dough; held-overlap callbacks did not steal it back. Releasing while overlapping allowed actual physics to place it again with the same count and exact local scale.
- Exactly two more rolling passes completed flattening.
- Real ray pickup, carried rotation, and actual trigger re-entry returned `FlattenedDough` at `4/4`, ready to cut with its original flattened size.
- Three further rotated pickup/return cycles exercised the real pickup method and board callbacks without scale drift.
- Actual knife contact produced four portions. They remained excluded from slab placement, loaded onto parchment, and all four completed a perfect `180 C` bake.
- Full manual input feel, reach, and unusual obstruction recovery still require player acceptance; see `cookie-usability-acceptance-checklist.md`.
