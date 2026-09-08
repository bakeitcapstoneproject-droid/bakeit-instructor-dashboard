# BakeIT Instructor Dashboard

Static Node.js prototype refactored from the supplied single-file HTML and aligned with the BakeIT capstone manuscript.

The interface intentionally uses a simple college-project style: standard Arial typography, a restrained brown-and-cream palette, flat panels, minimal icons, and no decorative animation.

The visible website is limited to instructor monitoring: class performance, learner records, active VR sessions, and reports. Technical architecture and cloud-integration information are intentionally excluded from the interface.

The product title and monitoring terminology follow the capstone manuscript: **BakeIT Instructor Dashboard**, with learner task completion, procedural accuracy, performance results, session history, criterion-level scores, safety events, waste indicators, chef-hat ratings, and stage progress.

## Run

Requires Node.js 18 or newer. No package installation is required.

```bash
npm start
```

Open `http://localhost:3000`. The static login accepts any non-empty email and password.

## Structure and OOP design

- `src/server.js` — `StaticWebsiteServer`, a small Node HTTP server.
- `public/*.html` — separate Login, Overview, Learners, Live Sessions, and Reports pages.
- `public/assets/js/core.js` — reusable `StorageService`, `AuthService`, `CloudDataService`, `DashboardMetrics`, `TableView`, and `AppShell` classes.
- Page controllers (`login.js`, `dashboard.js`, etc.) encapsulate page behavior.
- `CloudDataService` isolates the mock data source. Replace its methods with authenticated AWS API Gateway requests later.

## Future AWS integration

1. Configure Cognito sign-in and role claims in `AuthService`.
2. Replace `CloudDataService` mock methods with `fetch()` calls to API Gateway.
3. Validate and process Unity events in Lambda, then persist minimal learner/session records in DynamoDB.
4. Add polling or WebSocket updates for live sessions; log access and backend errors in CloudWatch.
5. Enforce server-side authorization, validation, HTTPS, retention rules, and export auditing. Client-side guards are only a prototype convenience.

## Notes

The prototype is an instructor monitoring and reporting tool, not a full LMS. It deliberately does not start, stop, or control Unity sessions. Demo exports show UI feedback but do not create files.
