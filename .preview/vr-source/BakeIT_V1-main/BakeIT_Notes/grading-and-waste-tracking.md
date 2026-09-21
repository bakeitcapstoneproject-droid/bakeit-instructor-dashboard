# Grading and waste tracking — partial foundation

## Current clarification — 2026-09-15

The measure ledger remains a partial, session-only foundation. The old small-spoon-for-chips unit example below is superseded by exact recipe cup/teaspoon roles and one 1/2-cup Cookie chip transfer. Do not interpret measure counts as equal ingredient mass. Grade calculation, broader waste coverage and cloud submission remain unimplemented. The current sequence is user playtest A, agreed Brownie finishing C, then Cupcake planning B; no grading work is authorized by this documentation update.


## Implemented 2026-09-08

Measured-ingredient transfers now emit structured events from MeasuringScoop. Existing CookieRecipeSessionController stores a read-only session history and DiscardedMeasureCount. This extends the existing recipe session, not a second manager.

Each MeasureTransfer contains ingredient name, tool name, unit, quantity1, destination, disposition and unscaled session time. AddedToBowl, ReturnedToSource and Discarded are distinct. Only Discarded increments the waste-unit count. Rejection, hovering, a short Q tap or an empty discard produce no record. Restart clears the list/count; no cloud/database persistence exists.

Current units are cup measure (flour/sugar) and small spoon (chips). Quantities are not grams and different units must not be treated as equal physical mass. Source stock is unlimited: a return records recovery, not a stock increment. Whole eggs/butter, dropped dough, burnt batches and spills are not in this measure-only ledger yet.

## Not implemented

- Grade calculation, rubric weights or deductions.
- Gram/volume calibration and ingredient cost.
- Source depletion, contamination rules or persistent spilled material.
- Whole-ingredient/dough/bake-result waste accounting.
- Instructor views, telemetry or cloud submissions.

## Next design work

Agree on the rubric and measurement units before connecting penalties. Count distinct disposition events once; do not punish a rejected attempt as if food was consumed. Keep returns separate from discarded food, and include other waste categories only when their gameplay semantics are approved. See ingredient-measuring-system.md for controls and test-log.md for verification limits.

## 2026-09-15 decision — defer rubric

User decision: implement grading later, after every dish and the VR-ported base game are complete, when research data can be collected. No provisional scores or weighted rubric were implemented. The new small-serving endpoint reports only existing bake outcomes and serving count. Defaults are three Cookies or four Brownie squares; see serving-and-plating-system.md.
