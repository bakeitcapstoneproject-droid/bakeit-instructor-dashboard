# BakeIT Instructor Dashboard

Node.js prototype for creating class sections, enrolling learners by class code, and monitoring learner results.

The interface uses the original full-height left sidebar with a mobile navigation drawer, white cards on a light gray background, and warm brown navigation and accents. Page headers contain titles and controls without subtitles.

The website includes class enrollment, class performance, learner records, active VR sessions, and reports.

The product title and monitoring terminology follow the capstone manuscript: **BakeIT Instructor Dashboard**, with learner task completion, procedural accuracy, performance results, session history, criterion-level scores, safety events, waste indicators, chef-hat ratings, and stage progress.

## Run

Requires Node.js 18 or newer. No package installation is required.

```bash
npm start
```

Open `http://localhost:3000`. The demo instructor login is `instructor@mcl.edu.ph` / `demo123` (unless changed in this browser).

## Demo learners

Run `npm run demo:add` to add six fictional learners to each existing section. Repeating it skips existing samples. Sample scores cover Passed, Needs Practice, and Not started across Brownies, Cookies, and Cupcakes. Each sample has a Demo label and a DEMO-prefixed ID. Demo learners contribute to section counts and dashboard metrics while present.

Run `npm run demo:remove` to remove only the tagged demo learners and keep real enrollments and class sections. Stop the server before running these data commands, then restart it; they use the same data file as the server (including `BAKEIT_DATA_FILE` when set). New class sections stay empty until learners join or you run the add command again.

Live sessions displays up to three sample sessions per section from the tagged demo learners. Each card is labeled **Demo session**, with fixed sample steps, elapsed times, and activity events for Brownies, Cookies, or Cupcakes. The section selector filters these samples. These are presentation snapshots; removing the demo learners also removes their sample sessions.

## Create a class section

Open **Learners**, choose the dashed **+ Create class section** card, and enter a name in the dialog. Copy the generated eight-character code to share with learners. Click a class card to open its learner progress within Learners; use **Class sections** to return to the overview. Browser Back/Forward and reload preserve the section view. New sections start empty. Learners join when the VR client submits the code and their account details to the server. Dashboard and learner views refresh every 15 seconds while visible and when the window regains focus.

Sections and enrollments are saved in `data/classes.json`, outside the public directory and ignored by Git. They survive server restarts. Set `BAKEIT_DATA_FILE` to use another location. Run one server process per data file.

See [VR enrollment integration](docs/vr-enrollment.md) for the request contract and prototype limitations. The Unity/VR application is not included in this repository and must be connected to the join endpoint separately.

Run `npm test` (`npm.cmd test` in PowerShell if script execution is restricted) to check creation, persistence, joining, concurrent requests, code collisions, validation, filters, scoring, and login behavior.

## Structure and OOP design

- `src/server.js` — `StaticWebsiteServer`, a small Node HTTP server.
- `public/*.html` — separate Login, Dashboard, Learners, Live Sessions, and Reports pages.
- `public/assets/js/core.js` — reusable `StorageService`, `AuthService`, `CloudDataService`, `DashboardMetrics`, `TableView`, and `AppShell` classes.
- Page controllers (`login.js`, `dashboard.js`, etc.) encapsulate page behavior.
- `src/classes.js` validates and persists sections and enrollments; `public/students.html` provides section creation and code copying.
- `CloudDataService` loads learners and enrollment activity from the Node server. Explicit mock mode retains sample performance data for tests.

## Future AWS integration

1. Configure Cognito sign-in and role claims in `AuthService`.
2. Move the local class endpoints to authenticated API Gateway requests. `CloudDataService` already loads saved classes and enrollments from the Node server; mock performance data is used only when explicitly requested by tests.
3. Validate and process Unity events in Lambda, then persist minimal learner/session records in DynamoDB.
4. Add polling or WebSocket updates for live sessions; log access and backend errors in CloudWatch.
5. Enforce server-side authorization, validation, HTTPS, retention rules, and export auditing. Client-side guards are only a prototype convenience.

## Notes

The prototype supports section creation and enrollment, alongside instructor monitoring. It does not start, stop, or control Unity sessions. VR score/session ingestion remains future work. Demo exports show UI feedback but do not create files. The existing login is browser-only; the new API has no server-side authentication and shares one instructor workspace. Use it for local/trusted development until authenticated instructor and learner access is implemented.
