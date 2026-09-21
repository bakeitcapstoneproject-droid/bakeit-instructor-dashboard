# Recipe alignment and VR integration review

Reviewed the supplied `BakeIT_V1-main.zip` against this website. The source of truth is `BakeITCapstoneProject/Assets/BakeIT/`, especially `RecipeDefinitions/*.asset`, `Scripts/Recipes/CookieRecipeSessionController.cs`, `MixingSequence.cs`, `Interactions/CupcakeBatch.cs`, `ServingPlateReceiver.cs`, and the serialized `Scenes/Kitchen Prototype.unity`. Historical notes in the archive contain superseded behavior; this mapping follows the current code and scene. The archive was inspected as source material, not executed or modified.

## Website changes delivered

`public/assets/js/recipes.js` defines the three recipe paths, exact ingredient measures, work-pass counts and implemented endpoints. Both server samples and browser mock sessions use it. Live-session cards show the current instruction, completed-step progress, and an expandable ordered recipe list. At the user's request, the interface uses “Live session”, “Session time”, “Recorded”, and active-session counts without sample/demo labels. This presentation change does not add telemetry; underlying data remains tagged for management and removal.

| Recipe | Dashboard steps | Current game endpoint |
| --- | ---: | --- |
| Chocolate-Chip Cookies | 16 | Bake six cookies, plate three |
| Brownies | 16 | Cut twelve squares with five knife strokes, plate two or three |
| Vanilla Cupcakes | 14 | Preheat/line, nine batter phases, fill, bake, check, cool all twelve |

These are website monitoring steps derived from gameplay, not step numbers already emitted by Unity. The controls tutorial and recipe selection occur before these recipe paths. Conditional corrections (return bowl to MIX, clear the board) belong in feedback, not extra fixed steps. Cupcake checking and cooling are shown separately although the Unity guide groups them under one heading.

Cookies use four base mixing passes, three chip-combining passes and four rolling passes. Brownies use four base mixing passes, two walnut-finishing passes and four spreading passes. Cupcake phases preserve separate eggs, bowl scraping, half-dry mixture, milk, and remaining dry mixture. No brownie cooling or cupcake frosting stage is invented.

The game uses compressed bake timers: cookies 5 seconds at 170–190 °C, brownies 8 seconds at 170–180 °C, cupcakes 10 seconds at 180 °C. Cupcakes cool for 10 seconds each on the rack. These are game settings, not real-world baking directions. Underbaked, perfect, overcooked and burnt outcomes must remain distinct from task completion.

## Connection status

This supplied build is not connected to this website in the inspected code. The website's `GET /api/sessions` returns `demoSessions(learners)` and has no session write endpoint. The game's custom runtime scripts contain no HTTP/WebSocket/cloud sender or learner/class identity integration. The website does have `POST /api/sections/join`, but the game still needs a client for it. Renaming a badge or aligning recipe text does not establish a live connection.

The uploaded implementation uses keyboard/mouse controls (`FirstPersonController`, `PickupController`, `Keyboard.current`, `Mouse.current`); headset/controller behavior was not tested. This review did not run Unity.

## Changes still needed

1. **Learner identity and enrollment, both sides.** Add learner sign-in/join UI to the game, send the class code to the existing join endpoint, and retain the returned section ID with a stable learner ID. The website needs server-side instructor/learner authorization; its current login is browser-local and its API has no authentication. A class code alone is not a credential.
2. **Session submission, both sides.** Add a Unity adapter that emits session start, progress, completion, restart/abandon, bake outcomes, corrections and heartbeat updates. Add validated server endpoints and persistent session storage associated with an enrolled learner/section. Send stable `recipeId`, recipe version and step IDs; do not parse changing guide text or use unvalidated client-provided totals. The IDs in the new catalog are proposed adapter identifiers, not existing game event IDs.
3. **Connection lifecycle, website.** The existing page polls every 15 seconds while visible and on window focus, but there is currently no incoming data to display. After ingestion exists, show last update and active/disconnected/completed states, expire stale sessions, and retain history. Give restarted attempts new IDs and deduplicate retries with event IDs/sequence numbers. Use explicit completed-step IDs or equivalent verified state if gameplay can revisit earlier stages; the sample path currently assumes sequential progress.
4. **Cupcake completion scope, both sides.** The current game finishes at `BAKING PRACTICE COMPLETE` after rack cooling. Buttercream, piping and serving are unfinished. Keep practice completion distinct from full-dish completion in reports. The website now makes this limitation visible in the recipe details.
5. **Scoring and reports, both sides.** The game has no grading rubric/score submission. Current website scores and chef-hat ratings belong to sample data, not measured game results. Agree on the assessment policy before connecting final scores; store bake outcome separately so serving a burnt batch does not imply passing.
6. **Waste units, both sides.** `MeasureTransfer` records ingredient, tool, unit, destination, disposition and Unity unscaled time, with quantity 1 per transfer. `AddedToBowl`, `ReturnedToSource`, and `Discarded` are separate; only discarded measures increment the discard count. Preserve units and ingredient identity. Do not total cups, teaspoons, eggs or transfer counts as grams, costs or a waste grade. Whole ingredients, burnt batches and spills are not fully represented. `Time.unscaledTime` is not UTC and resets with scene reload; send an attempt-relative offset plus server receipt time.

## Suggested contract for the next integration task (not implemented)

```json
{
  "sessionId": "unique-attempt-id",
  "learnerId": "enrolled-learner-id",
  "sectionId": "saved-section-id",
  "recipeId": "cupcakes",
  "recipeVersion": "bakeit-v1-2026-09-15",
  "stepId": "fill-liners",
  "sequence": 12,
  "status": "active",
  "elapsedSeconds": 625,
  "detail": { "filledCount": 4, "targetCount": 12 },
  "bakeOutcome": null
}
```

Map cupcake `MixingSequence.PhaseIndex` (zero-based) to the nine ordered phase IDs. Read tray/fill/doneness/cooling state from `CupcakeBatch`; read cookie/brownie bowl, board, pan, oven and serving state from the components used by `CookieRecipeSessionController`. Capture measure events before recipe restart clears the ledger. Validate identity, membership, version, recipe/step, counter ranges and event ordering on the server; derive display titles/totals from the catalog. Use the server's reachable address on a headset rather than headset `localhost`.
