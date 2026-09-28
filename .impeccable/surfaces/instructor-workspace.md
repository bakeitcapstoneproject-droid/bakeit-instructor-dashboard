# Instructor workspace

Mode: Operate

Primary target: `bakeit-instructor-dashboard/public/dashboard.html`
Related targets: `students.html`, `sessions.html`, `reports.html`, and `login.html` in the same directory.

Instructors sign in, choose sections, review learners and performance, inspect recipe progress, and prepare reports through the existing controls. Product capabilities and limits remain in [PRODUCT.md](../../PRODUCT.md); shared visual rules are in [DESIGN.md](../../DESIGN.md).

## Direction contract

THESIS: A familiar instructor workspace using the user's supplied bakery interface as visual authority.

OWN-WORLD: Ivory workspace, cocoa navigation, orange active links and actions, local Poppins, rounded panels, and soft blurred shadows. Keep BakeIT's existing logo.

STORY: Move from section selection and summary results into learners, sessions, and report preparation. Preserve the current feature scope.

FIRST VIEWPORT: Inset rounded desktop frame with a cocoa sidebar and scrolling content inside the continuous ivory surface, bold page title, section controls, four text-only metrics, results, and activity. Phones use a brand/menu bar with stacked content and two-column metrics.

FORM: User-selected `Bakery-management-system (Community).zip`; direct adaptation, with no alternative direction or random seed. Sign-in pairs the user's baking ingredients photograph, edited to monochrome with a brown tint, with the form. Its provenance is in [design-assets.md](../../bakeit-instructor-dashboard/docs/design-assets.md). Motion communicates focus, selection, and opening states.

FINISH: unreviewed and undocumented is unfinished; this build ends with the finish review, the verdict, DESIGN.md, and every shipping raster carrying its provenance

## Boundaries

Preserve class, learner, session, and report behaviors. Do not add the reference's customers, products, inventory, orders, sales, or staff-management features, invented charts, or unsupported export functionality. Confirmed visual exclusions are recorded in DESIGN.md. The user's baking ingredients photograph replaces the previous generated cake image on sign-in.

No unresolved visual direction decisions.
