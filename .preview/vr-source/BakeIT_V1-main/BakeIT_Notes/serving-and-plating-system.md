# Serving and plating

## Current behavior — September 15, 2026

The player approved a smaller final serving. Cookies use **3 of 6**. Brownies now offer **2 or 3 of 12 squares**, defaulting to two; at the plating stage press **2** or **3** to choose. This supersedes the initial four-square serving. Completing baking alone no longer completes a recipe. Brownies first require all five knife cuts; then every square is individually grabbable. Release finished food over the plate to arrange a slightly rotated stack. Picking a plated piece back up removes it from the count and closes the stack; replacing it adds it back on top. Reducing the target below the current count requires removing the extra piece first.

`ServingPortion` records recipe identity and the existing oven outcome. `ServingPlateReceiver` rejects raw/unmarked food, the wrong recipe, food still held, a held plate, full servings and transfers while the active bakeware is still loaded in the oven. Existing pickup and smooth-placement behavior is reused. Food keeps its world scale when attached to or removed from the plate. Completion is derived from the current count.

No score, rubric, criterion weights or research-data collection was added. The user deferred grading until every dish and the VR base game are complete. The existing bake-outcome text is retained; it is not a grade.

## Scene setup

- Scene: `Assets/BakeIT/Scenes/Kitchen Prototype.unity`.
- Serving Plate Root: `(2.42, 1.172, 5.90)`, unit scale, Rigidbody mass 0.4, PreparationItem and PickupPose. Its visual reuses the licensed `Prop_Plate_01` prefab at 0.36 m width.
- Physical support collider: center `(0, .008, 0)`, size `(.32, .016, .32)`.
- Serving Plate Acceptance trigger: center `(0, .07, 0)`, size `(.32, .12, .32)`; receiver references the unit root.
- Surface local Y `.016`, measured from the plate's central well (about `.0145`, with contact clearance); the outer rim is about `.0384` and is not used as the stack floor. Acceptance radius `.12` plus `.04` tolerance. Brownies stack at small center offsets and alternating -8/12-degree yaw. Heights come from the main food mesh bounds in plate coordinates; decorative kernels do not raise the next layer. Cookies retain three positions on a `.071` radius.
- Plated portions use kinematic bodies with interpolation off. Layout synchronizes their poses after the plate's smooth movement, retaining contact when carried and set down. World scale is preserved.
- Brownie top walnuts follow their square after cutting. Scatter is generated once per batch, retaining its arrangement when carried.

## Guide and reset

The final stages are **Plate a Serving / PLATE THE FINISHED FOOD** and **Ready to Serve / SERVING COMPLETE**. Hold R for three seconds to repeat the current recipe. Hold M for three seconds to clear the batch and choose a recipe; completed control practice is retained.

Brownie guide rows show the **2 / 3** serving choices and selected target. Either size is acceptable; Cookies stay at three. Grading remains deferred.

## Verification and remaining acceptance

`PlayerFeedbackQA` is an Editor-only Play Mode driver. It passed both actual bake timers and recipe serving paths, including held-food rejection, removal/replacement, pickup scale and resets. It uses controlled transforms, public APIs and trigger callbacks; natural E placement, plate carrying and gesture feel remain on the player acceptance checklist. See `test-log.md` and `evidence/feedback-fixed-2026-09-15-*-serving.png`.
