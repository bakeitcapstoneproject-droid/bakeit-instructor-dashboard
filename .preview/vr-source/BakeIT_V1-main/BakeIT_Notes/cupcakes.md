# Cupcakes

## Selected source — September 15, 2026

User-selected basis: Lindsay's [Easy Homemade Vanilla Cupcakes, Life, Love and Sugar](https://www.lifeloveandsugar.com/easy-homemade-vanilla-cupcakes-recipe/), accessed September 15, 2026. Formula selection is settled. The current extension exposes **Hard | Baking Practice** through the poster, through rack cooling; the full dish is still unfinished.

## Published recipe facts

Yield: 12. Oven: 350°F (176°C listed). Bake: 15–18 minutes. Liners: approximately three-quarters full.

| Batter ingredient | Amount |
|---|---|
| All-purpose flour | 1¼ cups |
| Baking powder | 1¼ teaspoons |
| Salt | ¼ teaspoon |
| Softened unsalted butter | 6 tablespoons |
| Granulated sugar | ¾ cup |
| Vegetable oil | 2 tablespoons |
| Vanilla extract | 1½ teaspoons |
| Eggs | 2 large |
| Milk | ½ cup + 2 tablespoons |

Buttercream: 1 cup softened unsalted butter, 4 cups powdered sugar, 1½ teaspoons vanilla, 2–3 tablespoons milk or water, and 1–2 pinches salt.

Method summary: combine dry ingredients separately; cream butter/sugar/oil/vanilla; mix eggs individually; alternate dry mixture, milk, remaining dry mixture. Fill, bake, cool, then frost. These facts come from the linked recipe card; its prose and photographs are not game assets.

## Proposed BakeIT adaptation — implementation planning

- Retain the selected formula and staged mixing order. The current two-phase base/finishing abstraction will need an extension; putting everything into one bowl at once would change the method.
- Reuse existing ingredient sources, exact-measure validation, eggs, physical pickup/pouring, oven, recipe-paper progress and serving/reset framework where suitable.
- New supplies/tools needed: milk, vegetable oil, tablespoon measure, cupcake tray and liners. A from-scratch frosting branch also needs powdered sugar, frosting preparation and a piping bag. Source prep butter quantity must be represented explicitly rather than consuming the existing full Cookie stick.
- Keep the full batch yield for recipe fidelity; choose a smaller plated serving separately during design.
- Existing oven controls move in 10°C steps. A 180°C game target is a proposed approximation, not the source temperature. Compressed bake/cooling duration, outcome bands and gesture counts remain game tuning decisions, not published facts.
- Preserve a cooling step before piping. Do not implement a real-world 15–18 minute wait in the prototype without a deliberate gameplay decision.
- Optional decorations beyond vanilla frosting are not required by this source-selection decision.

## Confirmed difficulty and frosting — September 15

User confirmed **Hard difficulty**, with **buttercream made from ingredients**, then piped. This supersedes the pending frosting preference. Hard describes recipe complexity; it does not disable tutorial help or imply Challenge mode.

## Ordered source-method mapping

Paraphrased from the linked recipe card:

1. Preheat; line the tray.
2. Separately combine flour, baking powder and salt.
3. Cream softened butter, sugar, oil and vanilla until fluffy (source: 2–3 minutes).
4. Add each egg separately, mixing after each; scrape the bowl.
5. Mix in half the dry blend.
6. Gradually incorporate milk.
7. Mix in the remaining dry blend; scrape, avoiding excess mixing.
8. Fill liners three-quarters; bake 15–18 minutes, checking for a clean toothpick.
9. Transfer to a rack; cool completely before frosting.
10. For frosting, beat butter smooth; incorporate half the powdered sugar.
11. Mix in vanilla, salt and 1–2 tablespoons milk/water.
12. Incorporate remaining powdered sugar; adjust with additional liquid if needed.
13. Pipe onto cooled cupcakes.

Serving follows piping as BakeIT's established endpoint. Whisk movement, scraping, dry-blend transfers, doneness checks and rack cooling are implemented. Electric-mixer interaction and the frosting/piping/serving stages remain unfinished. Shortened timers/pass counts are game adaptations and must not change the ingredient order.

## Batter foundation — September 15 (historical milestone)

- `Cupcakes.asset` records the exact batter formula and nine ordered phases. `MixingSequence` owns phase quantities, work passes and the two dry-blend portions; the existing `PourableIngredientReceiver` handles physical ingredient intake and tool motion.
- A separate blue-grey Dry bowl and tan Batter bowl have labelled rests. New milk/oil props use the licensed food assets, with measuring zones above their openings. A distinct softened **84 g / 6 tbsp** butter portion, two eggs, tablespoon measure and bowl scraper support the selected formula. Existing dry sources, cups, teaspoons and whisk are shared.
- Order: dry ingredients (2 whisk passes), cream butter/sugar/oil/vanilla (4), egg one (1), egg two (1), scrape (1), half dry mixture (2), milk (2), remaining half dry mixture (2), final scrape (1). These pass counts are desktop tuning, not the source's real-time mixer instructions. Creaming currently uses the shared whisk; an electric mixer is not implemented.
- Dry mixture requires a sustained 1.4-second tilt per half. The second half stays in the Dry bowl until the milk stage is mixed. Wrong bowl, wrong step, duplicates and missing ingredients cannot advance the sequence. Rejected measures retain their contents for existing return/discard controls.
- The wall recipe and guide explicitly identify batter practice. All nine ingredient lines become green only at their exact aggregate amounts in guided practice. The endpoint is **BATTER PRACTICE COMPLETE**, without dish completion, a serving result or grade. The oven rejects baking for this explicitly incomplete recipe asset.
- Cupcake-specific supplies appear when selected. Other recipe bowls are hidden during this branch; scene reload restores them. R repeats the selected practice; M returns to selection.
- The separate dry/batter bowls start on their rests for this foundation. A full Cupcake gathering/preheating/liner workflow is not implemented yet. Existing dry ingredients still come from storage.

## Tray and baking extension — September 15

- A purpose-built 4-by-3 metal tray has twelve recessed wells, side grips and compound solid colliders. It is one movable body. Twelve separate fluted paper liners come from a finite stack through the existing E dispenser/pickup interaction.
- Preheat the empty oven and place every liner before the batter sequence unlocks. Release liners above individual wells; occupied wells reject another liner. The two mixing bowls still start on their rests; full Cupcake gathering is not a separate stage.
- Keep the tray on Tray Worktop Rest. Tilt the completed batter bowl over each lined well: a 0.9-second transfer raises the visible fill to three-quarters. Partial fills remain in their own well, the bowl volume decreases, and a full well stops accepting batter. All twelve fills are required before baking.
- The game target is **180°C**, approximating the source's listed **176°C** using the existing 10°C controls. The prototype bake lasts **10 seconds**, followed by **10 seconds of cooling per cupcake while on the rack**. These are game tuning choices, not real baking instructions or a calibrated thermal simulation.
- After baking, return the tray to the worktop and touch the wooden tester tip into a cupcake centre. An underbaked result leaves visible wet batter on the tip; other results retain their outcome feedback. Cupcakes become individually grabbable after checking. Transfer all twelve to separate cooling-rack positions.
- Cooling ends at **BAKING PRACTICE COMPLETE**, with no full-dish completion or grade. The poster, HUD and tutorial target follow these stages. R replenishes the batch; M returns to recipe selection. Cookie/Brownie serving behavior is retained.
- Authored assets live in `Assets/BakeIT/Generated/Cupcake Tray`; `CupcakeTraySetup` rebuilds only this extension. The foundation builder invokes it after creating the batter equipment.

### Next implementation milestone

Implement the confirmed buttercream ingredient sequence, piping and a smaller serving. Powdered sugar, frosting butter portions and piping equipment belong to that branch; there is no prepared-frosting shortcut. Grading remains deferred until all dishes and the VR base game are complete.

Focused Play Mode test details and limitations belong in `test-log.md`; player keyboard/mouse acceptance remains open.

Latest verification (September 15, 13:02 +08): the full Cupcake controlled sequence through cooling and both resets passed; Cookie/Brownie regression passed. Final scene/assets saved with zero missing scripts and a clean Console. See the final tray extension entry in test-log.md for controlled-fixture and natural-input limitations.
