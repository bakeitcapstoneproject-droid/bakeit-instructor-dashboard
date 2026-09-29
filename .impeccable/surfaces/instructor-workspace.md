# Instructor workspace

Mode: Operate

Primary target: `bakeit-instructor-dashboard/public/dashboard.html`
Related targets: `students.html`, `sessions.html`, `reports.html`, and `login.html` in the same directory.

Instructors sign in, choose sections, review learners and performance, inspect recipe progress, and prepare reports through the existing controls. Product capabilities and limits remain in [PRODUCT.md](../../PRODUCT.md); shared visual rules are in [DESIGN.md](../../DESIGN.md).

## Direction contract

THESIS: A calm, structured instructor workspace that makes real class information easy to scan. The 2026-09-29 redesign preserves all existing content, assets, data, and behavior across the whole website.

OWN-WORLD: Preserve the established ivory, cocoa, and orange palette and local Poppins. Use a full-height workspace, a compact cocoa sidebar, fine rules, 12px panels and 8px controls. Remove decorative elevation from permanent surfaces. Keep the full chef-hat logo and the existing sign-in photograph.

STORY: Move from section selection and summary results into learners, sessions, and report preparation. Preserve the current feature scope.

FIRST VIEWPORT: Full-height desktop shell with a 224px cocoa sidebar, a clear page heading and section controls, a single divided performance band, and results beside activity. Class sections use compact horizontal rows; reports use a two-column preparation panel. Phones use a compact brand/menu bar, stacked content, and a two-column performance band.

FORM: Evolve the user-selected bakery identity through layout and component hierarchy, without replacing its branding. Sign-in pairs the existing brown-tinted photograph, inset by 16px, with a centered 400px form. Its provenance is in [design-assets.md](../../bakeit-instructor-dashboard/docs/design-assets.md). Existing drawer, dialog, and disclosure interactions provide feedback; no new motion or content is introduced.

QUALITY BAR: A continuous working surface, consistent alignment, restrained actions, real data, readable mobile tables, visible keyboard focus, and no page overflow from 320px through 1920px. Code-led implementation; no generated comp or new raster assets. The main visual tradeoff is reduced decorative framing in exchange for more room for records.

FINISH: unreviewed and undocumented is unfinished; this build ends with the finish review, the verdict, DESIGN.md, and every shipping raster carrying its provenance

## Boundaries

Preserve class, learner, session, and report behaviors. Do not add the reference's customers, products, inventory, orders, sales, or staff-management features, invented charts, or unsupported export functionality. Confirmed visual exclusions are recorded in DESIGN.md. The user's baking ingredients photograph replaces the previous generated cake image on sign-in.

No unresolved visual direction decisions.
