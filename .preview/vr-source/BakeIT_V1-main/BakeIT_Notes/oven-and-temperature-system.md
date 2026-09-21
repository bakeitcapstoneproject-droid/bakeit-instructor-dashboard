# Oven and Temperature System

## Brownie bakeware and recipe profile — 2026-09-12

The bake zone now recognizes either the established complete Cookie tray or a lined, filled, evenly spread Brownie pan. Loading a ready Brownie pan selects `Brownies.asset` for the shared oven: 8-second prototype duration, 170-180 C successful range, and its four Brownie result names. The existing Cookie path remains 5 seconds at 170-190 C and passed a focused recipe-data regression after this generalization.

Brownie results are stored on `BrowniePanReceiver` and recolor the temporary slab only for visually cooked outcomes. Underbaked, successful, overcooked, and burnt logical outcomes all remain distinguishable. The slab is a replaceable presentation layer; future raw/baked model references do not need changes to pan preparation, spreading, bake classification, or guide state.

Focused runtime verification covered parchment acceptance, completed-batter transfer, four-pass spread state, Brownie pan registration, an 8-second Perfect bake, all four result names, and restoration of the Cookie duration/range. The normal timed bowl tilt and four physical spatula gestures still require player acceptance in Game view.

## Preheat workflow — 2026-09-09

The existing oven now separates heating from baking. Its target and current displays begin at `0 C`; preheat is rejected until the player chooses a target. The first positive input selects the `100 C` minimum and later inputs use 10-degree steps. With the door closed and no tray registered, the start control reads `PREHEAT` and heats to the selected target over the serialized prototype duration of `5 s`. The control reads `HEATING` during warm-up. Opening the door pauses progress and closing it resumes. A prepared tray must be removed before preheating can start.

At the target temperature, the oven enters `READY`: the display shows the current temperature and `READY`, the start control changes to `BAKE`, and the interior light/emissive surface remains on. Baking is rejected until this ready state is reached. After a completed bake, the oven stays heated and the light remains on so readiness is visually persistent. Changing the target temperature invalidates readiness and requires another preheat. The recipe asset still owns the target range, bake duration, thresholds, outcome names, and cookie material behavior; the 5-second preheat is oven interaction tuning rather than a recipe quantity.

Focused Play Mode verification exercised the actual tray liner, tray receiver, bake zone, oven, display, start label, and light. It confirmed cold-bake rejection, loaded-tray preheat rejection, an intermediate temperature between 25 and 180 C with `HEATING`/`PREHEAT`, `180 C` readiness with the light on, a five-second 180 C bake of all six portions to `BakedCookie` / Perfect, and retained `READY`/light state afterward. Player-facing timing, label fit, and perceived light readability still need normal Game-view acceptance.

## Recipe-data migration — 2026-09-09

`OvenController` now receives the bake duration, perfect-temperature range, burnt threshold, product labels, and four result ingredient names from the active cookie definition. Values remain exactly 5 seconds, 170–190 C perfect, 220 C and above burnt, and `UnderbakedCookie` / `BakedCookie` / `OvercookedCookie` / `BurntCookie`. The earlier underbaked zero-material-change rule remains intact. The guide reads the oven's active min/max properties instead of repeating 170/190 literals.

## Underbaked no-visual-change correction — 2026-09-09

Below `170 C`, the oven still records `Underbaked` and renames every portion `UnderbakedCookie`, but it now skips every base-renderer material write. Existing `_BaseColor`, `_BakeAmount`, and `_Smoothness` values remain exactly as they were before baking; low heat no longer turns dough white or changes its finish. Perfect, overcooked, and burnt visual branches are unchanged. Focused six-cookie Play Mode verification preserved deliberately non-default raw values at `160 C`, then confirmed a `180 C` bake still applies bake amount `1` and smoothness `.25`.

## Visual update — 2026-09-08

Successful-bake base color changed from (.78,.42,.14) to (.88,.67,.36): golden rather than dark orange. Cookie Surface shader supplies subtle grain and baked-edge detail (`_BakeAmount=1` for visually cooked outcomes). As of 2026-09-09, underbaked results do not write any shader property. ChocolateChipVisual renderers remain excluded from base recoloring. Ingredient names, thresholds, five-second prototype timer, temperature buttons and door logic are unchanged. Six cookies completed a real 180 C timed Perfect bake, retained their positions when the tray returned to the counter, and left the guide at RECIPE COMPLETE. See final rendered evidence and dated test log; user appearance acceptance is still required.

## Current scene setup

- Scene: `Assets/BakeIT/Scenes/Kitchen Prototype.unity`.
- Baking controller: `OvenBakeZoned` with `OvenController` and `OvenBakeZone`.
- Door: Existing `FreeStove_Door` with `OvenDoorController`.
- Serialized integration: `OvenController.bakeZone` references the local bake zone and `OvenController.ovenDoor` references the existing sibling door.
- 2026-09-07 presentation: retained the real stove, door, controller, zone, and marker components. The three small cubes are now rounded button faces on a slate bezel with an inset temperature readout, minus/plus labels, and amber BAKE control. Existing E selection and hold-E door dragging remain unchanged. Hitboxes grew with the visual buttons. The unrelated spare `Prop_Oven_01` is inactive, not deleted, and its space now holds the existing sideboard. See `kitchen-layout-and-storage.md` for exact transforms/materials.
- Play Mode verification: rays from the player camera hit the new plus, minus, and BAKE controls; plus/minus changed `180 -> 190 -> 180 C`, empty-tray start was rejected, and the same BAKE ray started the loaded four-cookie batch with a Perfect result. No recipe timing, temperature, or door-mechanic changes were made.

## Runtime behavior

The desktop prototype uses the existing oven control flow. Preheating requires an empty oven and closed door; baking requires a preheated oven, a tray holding every portion from one cookie batch, and a closed door. An incomplete batch cannot start. Opening the door pauses active preheating or baking; closing it resumes. Every bake outcome updates the logical result/name. Successful, overcooked, and burnt outcomes also update the cookie base appearance; undercooked applies no visual/material mutation. Renderers marked with `ChocolateChipVisual` are intentionally excluded from result recoloring so the chips remain visibly chocolate.

## Prototype values

The current short bake duration, interaction distance, temperature step, and result thresholds are intentionally retained to make iteration fast. They are tuning parameters, not production targets.

| Setting | Current value |
| --- | ---: |
| Selectable temperature | `100-250 C` |
| Temperature step | `10 C` |
| Bake duration | 5 seconds |
| Undercooked | Below `170 C` |
| Successful | `170-190 C` inclusive |
| Overcooked | Above `190 C` and below `220 C` |
| Burnt | `220 C` and above |

`OvenController.OnValidate` preserves at least one configured temperature step between the successful ceiling and burnt threshold, keeping the overcooked band reachable when values are tuned in the Inspector.

## Verified behavior

- The baking controller resolves a non-null bake zone and the working oven door.
- The linked door reports the expected closed state during the focused Play Mode verification.
- A complete four-cookie batch started successfully at the prototype perfect temperature and all four portions became `BakedCookie`.
- Every baked portion retained its chip marker and original chocolate color.
- Focused outcome verification passed for all four portions at `160 C` undercooked, `180 C` successful, `200 C` overcooked, and `220 C` burnt.
- An incomplete batch was not treated as a loaded tray.
- The existing door component and its drag behavior were not modified.

## Known limitations

- Results use cookie-specific prototype names but are not yet connected to recipe scoring or final presentation.
- Player-facing textual bake progress and correction feedback is connected. Data-driven recipe sequencing, scoring, audio polish, and device interaction remain future work.
- A full user-driven bake cycle should be repeated after the recipe-session architecture is introduced.

## September 15 — Cupcake baking integration (current)

OvenBakeZone now recognizes the dedicated Cupcake tray through its well components. All twelve lined wells must be filled before the existing bake control accepts it. Any recognized empty/incomplete bakeware also blocks empty-oven preheating. Cupcakes use a 180 C game target, a ten-second prototype bake, and the shared underbaked/perfect/overcooked/burnt outcome routing. This approximates the selected source's 176 C with the existing ten-degree controls; it is not a real baking duration. Remove the tray, touch the tester tip into a cupcake, and transfer each to the rack for ten seconds of cooling. Frosting and full dish completion remain future work.
