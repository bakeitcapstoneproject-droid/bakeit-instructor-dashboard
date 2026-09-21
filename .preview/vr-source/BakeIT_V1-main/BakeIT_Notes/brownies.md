# Brownies

## Current state — 2026-09-15

Brownies is selectable after movement/whisk practice. The implemented path includes preparation, exact intake, four base whisk passes, half-cup walnuts, two finishing passes, deep-pan parchment, held-bowl batter pouring, four spatula passes, recipe-aware preheat/baking and five held-knife strokes revealing twelve visual solid squares.

The guide reaches **Cut the Brownies**, then **Plate a Serving** after slicing. All twelve squares become independently grabbable; choose a stack of two or three with the **2 / 3** keys, then plate that quantity to reach **Ready to Serve / SERVING COMPLETE**. No grading rubric is implemented. Earlier endpoints, including four-square serving, are superseded.

## Approved quantities and stages

- Sugar: two 1-cup transfers.
- Flour: one 1-cup transfer plus one 1/4-cup transfer, tracked separately.
- Melted butter: one provided 1-cup pour; no new melting mechanic.
- Four whole eggs.
- Cocoa: one 1/2 cup; Vanilla: one teaspoon.
- Baking powder: one 1/2 teaspoon; Salt: one 1/2 teaspoon.
- After four base whisk passes: one 1/2-cup walnut transfer and two finishing passes.
- Parchment-only deep-pan preparation; four spreading passes; eight-second prototype bake.
- Success: 170–180 C. Below 170 C: underbaked; above 180 C and below 210 C: overcooked; 210 C and above: burnt.
- After baking, remove/set down the pan. Five knife strokes reveal a 4-by-3 grid in a 4:3 food footprint.
- Five completed cuts enable individual-square pickup. Two squares are the default serving, with a three-square option. No dedicated cooling gate exists.

## Latest presentation

September 14 supersedes the generic pre-mix B1 fill: distinct ingredients sit low inside the bowl; batter appears after whisking starts; walnut measures use kernel meshes; raw food uses #663C02 with non-emissive shading. A closed clump spreads over four passes to a rectangular layer. Baked/cut food has textured crust, darker sides and embedded walnuts. Player-distance art approval remains open.

## Evidence and limits

Earlier tests exercised pan/batter/bake states. September 14 checked public transfers, trigger callbacks, renders and reset, but directly invoked successful-bake presentation without rerunning oven timing. Five cuts and twelve-square geometry are technically checked, not yet accepted in a complete natural mouse-input run.

September 15 feedback corrections lower and narrow the bowl fill, remove the redundant completed-bowl MIX instruction, use a readable pouring jug, and scatter walnuts once per batch. Focused Play Mode checks passed the actual preheat/eight-second bake, five cuts, twelve physical squares, four-square serving, pickup scale and recipe-selection reset. See [test-log.md](test-log.md) and the [active player checklist](cookie-usability-acceptance-checklist.md). Human gesture/art acceptance remains open.

## Next work agreed September 15

User A feedback has been implemented, including the approved smaller serving. Retest these corrections to finish C before B Cupcake planning. Defer grading until all dishes and the VR base game are complete; cooling has not been added.

---

<details>
<summary>Historical notes retained on 2026-09-15 — superseded wherever they conflict with the current sections above; not current test instructions</summary>

# Brownies

## Approved recipe basis — 2026-09-09

The selected recipe is [Allrecipes Quick and Easy Brownies](https://www.allrecipes.com/recipe/9599/quick-and-easy-brownies/). Its written recipe uses baking spray; 2 cups white sugar; 1 1/4 cups all-purpose flour; 1 cup melted unsalted butter; 4 large eggs; 1/2 cup cocoa powder; 1 teaspoon vanilla extract; 1/2 teaspoon baking powder; 1/2 teaspoon salt; and optional walnuts. It preheats to 175 C / 350 F, spreads batter in a prepared 9-by-13-inch pan, bakes 20–30 minutes, cools completely, and slices into squares.

## User-approved direction

- Include the source recipe's 1/2 cup walnuts; the user approved them on 2026-09-10 for added texture and charm, superseding the earlier no-walnut decision.
- Defer other optional additions such as marshmallows until the base cookie, brownie, and cupcake dishes are complete.
- Use a deep rectangular metal brownie pan so the baked slab can be cut into rectangular or square portions.
- Use a rubber/plastic spatula as the physical leveling tool for spreading the batter in the pan.
- Make slicing a playable action.
- Cut the baked slab into 12 equal square portions using four columns by three rows. The playable batter/slab area should be 4:3 so the portions are true squares rather than merely equal rectangles.
- Use a separate larger mixing bowl for brownies; do not resize or replace the cookie bowl.
- Supply the recipe's one cup of butter already melted in a labeled pour cup. The player must hold and deliberately tilt the cup over the brownie bowl to transfer it.
- Cooling is tentatively optional and is not yet an implemented required gate.
- Use parchment as the prototype pan preparation; do not add a separate greasing action to the base brownie workflow.

## Pending workflow decisions

- Resolved 2026-09-11: Brownies starts from its framed recipe paper after the movement tutorial. The paper is deliberately available during development so the implemented portion can be play-tested.
- Decide how the cookie tray and brownie pan are stored and selected. The recommended physical approach is to keep both pans in the kitchen storage area and let the active recipe guide name the required pan; the wrong pan should be rejected with corrective feedback rather than magically changing shape.
- Map real quantities through the new exact measuring set: 1 cup, 1/2 cup, 1/4 cup, 1 teaspoon, 1/2 teaspoon, and 1/4 teaspoon tools. Compound quantities such as 1 1/4 cups use multiple matching transfers.
- Resolved 2026-09-11: use an 8-second prototype bake with a success band centered on the 175 C source target. Cooling remains optional.

## Implementation status

Milestone 1 is complete. The equipment foundation includes the stored deep rectangular metal `Brownie Pan`, green `Rubber Spatula`, separate larger `Brownie Mixing Bowl`, labeled `Melted Butter Cup`, exact measuring set, and a visible walnut jar/source in dry storage. The cup contains one visible liquid portion and transfers it only while held and tilted at least 75 degrees over the larger bowl for `.35 s`. The bowl must first be placed on `Mix Bowl Rest`; upright contact, pouring while stored, the cookie bowl, and a duplicate pour are rejected without consuming the cup.

`Assets/BakeIT/RecipeDefinitions/Brownies.asset` now records the approved ingredient/measuring plan, parchment preparation, pour/spread shaping, exactly 12 squares, an 8-second prototype bake, and a 170–180 C success range centered on the approved 175 C target. The existing large-bowl receiver references this definition. Compound Flour measurement is represented as separate 1-cup and 1/4-cup requirements, supported by tool-specific recipe lookup. The working Cookie definition remains unchanged.

Milestone 2 is also complete. The large bowl accepts all exact Brownie measures plus four whole Eggs, renders progressive chocolate batter, requires four base whisk passes with the 1.2-second cooldown, then accepts the half-cup Walnuts and requires two finishing passes. The framed Brownie paper is selectable after the movement tutorial and its guide covers this implemented path.

Brownies now has its own mise-en-place stage before measurement. The player stages four Eggs; all eight dry-storage ingredients/sources; the five required measuring capacities and Whisk; then the large Brownie bowl. The current-step outline follows this Brownie list rather than the Cookie list and continues through measuring and mixing targets.

Milestone 3 is complete: the playable branch continues from finished batter through parchment placement in the deep pan, held-bowl batter transfer, four Rubber Spatula passes, Brownie-specific preheat/loading, and the 8-second bake with all four result states. It then stops truthfully at `NEXT PART STILL IN DEVELOPMENT`; cooling presentation, slicing into 12 squares, and the completed-result model remain unfinished. Cupcakes remain locked.

## Equipment values — 2026-09-09

- `Brownie Pan`: reused licensed `Prop_Tray` mesh; scale `(1.65,7,1.63)`; `Bakeware Metal`; `.8 kg` Rigidbody; thin bottom/four-wall BoxCollider layout; `PreparationItem` and `PickupPose`.
- `Rubber Spatula`: reused licensed solid-head `Prop_Spatula_01`; dark-green nonmetallic `Rubber Spatula.mat`; `.15 kg` Rigidbody; convex MeshCollider; `PreparationItem` and `PickupPose`.
- `Brownie Mixing Bowl`: licensed `Prop_Bowel_02` at `1.4x`, approximately `.306 x .131 x .310 m`; `.5 kg` Rigidbody; disabled vendor MeshCollider plus base and 12-wall compound collision; `PreparationItem`, `PickupPose`, and a dedicated melted-butter trigger. The user finalized its saved rack position at `(-3.4973,1.1859,4.1133)`, rotation zero, above the renamed `Brownie Bowl Holder`.
- `Melted Butter Cup`: licensed `FreeCup` at `.65x`, user-moved dry-storage position `(1.3241,.4030,3.1055)`; `.22 kg` Rigidbody; `PreparationItem`, `PickupPose`, one backed player-facing label, visible liquid fill, and `PourableIngredient` configured for 75 degrees / `.35 s`.
- `Melted Butter.mat`: project-owned URP Lit golden-yellow liquid presentation, metallic `0`, smoothness `.62`; reused for cup fill, transfer droplets, and accepted bowl contents.
- Play Mode: both objects passed pickup and release through the real `PickupController`; pan interior depth measured `.0655 m`; pan rotated to `.56 x .08 x .40 m` fit completely inside the `.92 x .52 x .51 m` oven zone.

## User-authored storage correction — 2026-09-09

The user replaced/re-leveled the bowl-holder arrangement and manually corrected the trays so they rest on the rack rather than appearing to float. The user also chose a more realistic spatula placement and orientation. These manual scene edits supersede the earlier Codex-authored storage rotations and must be preserved. They were inspected while unsaved during consultation and were subsequently preserved and saved alongside the approved brownie bowl/butter additions: cookie tray `(-3.5,.425,4.12)`, rotation `(0,0,0)`; brownie pan `(-3.5,.613,4.12)`, rotation `(0,0,0)`; cookie bowl `(-3.4939,.9979,4.1394)`; rubber spatula `(-3.2303,1.5447,4.4190)`, rotation `(270,80,0)`.

## Brownie contents presentation — 2026-09-11

Measured Sugar and the other pale dry ingredients appear off-white in the Brownie bowl until Cocoa has actually been added. The progressive contents visual retains its small saved bowl-relative height, fixing the temporary oversized brown column without replacing the final batter mesh. The ingredient and Brownie batter model pass remains deferred until the user supplies references.

### Reference and volume revision — 2026-09-13

The user supplied photo references for a thick raw mixture, a fuller bowl, and a walnut-topped baked surface while approving the current pan. They selected B1 for the bowl, B2 for raw pan batter, and A3 for the baked top. The final B1 local position/scale is `(0,.088,0)` / `(.105,.032,.105)`, producing an approximately `.294 m` horizontal fill inside the approximately `.31 m` vessel without changing recipe quantity.

The selected presentation is implemented with separate procedural meshes: `Brownie_B1_Bowl`, `Brownie_B2_RawPan`, and `Brownie_A3_BakedPan`. B1 contains 12 walnut clusters, B2 contains 10, and A3 contains 8 plus three subtle crack groups. `BrownieFoodVisual` swaps only presentation meshes/materials when the existing bake result completes; the current pan, recipe quantities, transfer, spreading, oven, and result logic remain authoritative.

### Progression and containment correction — 2026-09-13

B1 no longer appears fully formed after the first accepted dry ingredient. Its saved minimum/full poses interpolate with ingredient completion, expanding from approximately `.153 m` to `.294 m` while remaining bottom-anchored. The raw-detail root stays hidden until batter completion, so Walnuts appear only after the existing four base and two finishing whisk passes. Raw colors are warm brown `(.46,.17,.055,1)` before completion and dark brown `(.34,.105,.035,1)` after completion.

B1 now has a closed skirt and bottom, and B2/A3 have closed perimeter walls and bottoms. Their Walnut clusters use three offset, rotated, asymmetric lobes rather than paired heart silhouettes. B2's saved local height places its bottom `.0024 m` above the liner. These are presentation corrections only; ingredient quantities, whisk thresholds, transfer trigger, four spreading passes, bake range, and results are unchanged.

</details>
