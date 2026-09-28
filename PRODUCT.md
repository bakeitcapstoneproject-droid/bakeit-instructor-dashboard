# BakeIT instructor dashboard

<!-- impeccable:product-schema 1 -->

## Platform

Web. The existing Node.js application is in `bakeit-instructor-dashboard/`, with separate HTML pages, shared CSS, and browser JavaScript modules. A static build supports browser-local demonstration data.

## Users and purpose

Instructors review baking learners, class sections, performance results, and recipe session progress. The current sign-in identifies Mapúa Malayan Colleges Laguna, ETYCB, Hospitality Management.

## Existing capabilities

Repository evidence: `bakeit-instructor-dashboard/README.md`, `public/*.html`, and the page controllers.

- Instructor sign-in, sign-out, and a browser-local demonstration password-recovery flow.
- Dashboard metrics, recent learner results, recent activity, and section filtering.
- Class section creation, viewing learners, copying class codes, deletion with confirmation, and learner search/status/recipe filters.
- Session progress, current recipe instructions, and expandable recipe steps for Cookies, Brownies, and Cupcakes.
- A report preparation action with feedback. File export is not implemented.

## Constraints

The user's 2026-09-28 request authorizes applying the supplied bakery reference's visual style and UI treatment to the existing website. Existing BakeIT features and behavior must be preserved. Do not introduce the reference's customers, products, inventory, orders, sales, or staff-management features.

The repository describes this as a prototype: authentication is browser-local, the Node API is not authenticated, live recipe sessions are presentation samples, and VR ingestion and external report export remain future work. Visual changes must not imply these integrations have been added.

## Brand and assets

Keep the BakeIT name and existing logo. The user supplied `Bakery-management-system (Community).zip` as a visual reference and a baking ingredients photograph for sign-in, requesting a monochrome brown tint. The explicit style exclusions in the request are recorded in DESIGN.md. Asset origins and image prompts are in `bakeit-instructor-dashboard/docs/design-assets.md`.

## Product principles

- Keep class and learner tasks easy to locate and operate.
- Preserve existing records, filters, navigation, and recovery behavior.
- Keep controls usable by keyboard and on small screens.
- Use actual application data and existing capabilities; do not invent dashboard statistics or charts.
