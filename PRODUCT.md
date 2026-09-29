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
- Per-section Excel downloads with one row per learner's latest recorded result, plus a blank template. Prepare report opens a confirmation popup identifying the section and Excel format, with section validation, cancellation, duplicate-submit protection and retryable errors. Excel provides colored headings, readable columns and separate scores/completion, safety/waste and procedural-accuracy sheets. A complete data sheet preserves the integration fields; missing measurements remain blank. The versioned JSON data contract is documented in `bakeit-instructor-dashboard/docs/section-reports.md` for future AWS integration.

## Constraints

The user's 2026-09-28 request authorizes applying the supplied bakery reference's visual style and UI treatment to the existing website. Existing BakeIT features and behavior must be preserved. Do not introduce the reference's customers, products, inventory, orders, sales, or staff-management features.

The repository describes this as a prototype: authentication is browser-local, the Node API is not authenticated, and live recipe sessions are presentation samples. Local/static Excel downloads are implemented; AWS integration, server-side export jobs, and VR assessment ingestion remain future work. Uncollected safety and procedural results must not be invented.

## Brand and assets

Keep the BakeIT name and existing logo. The user supplied `Bakery-management-system (Community).zip` as a visual reference and a baking ingredients photograph for sign-in, requesting a monochrome brown tint. The explicit style exclusions in the request are recorded in DESIGN.md. Asset origins and image prompts are in `bakeit-instructor-dashboard/docs/design-assets.md`.

## Product principles

- Keep class and learner tasks easy to locate and operate.
- Preserve existing records, filters, navigation, and recovery behavior.
- Keep controls usable by keyboard and on small screens.
- Use actual application data and existing capabilities; do not invent dashboard statistics or charts.
