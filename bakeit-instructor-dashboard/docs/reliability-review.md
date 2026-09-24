# Reliability review — September 24, 2026

## Changes

- Pages install their controls and recovery handlers before requesting class data. A failed first request no longer strands the page.
- Failed refreshes show a retry action and identify previously loaded data as potentially outdated. Refresh resumes on reconnection, window focus, and returning to a visible tab.
- Requests time out after 10 seconds, including responses that send headers but never finish the body. Failed forms retain their input and re-enable controls. Mutations are never automatically retried; timeout messages explain that a save may already have reached the server.
- Invalid responses and damaged browser workspaces produce readable errors. Saved workspaces are not silently erased or reseeded after corruption.
- Overlapping section loads cannot replace newer results. Failure to save a selected-section preference does not misreport a successfully saved section as failed.
- Unchanged table rows, section-selector options, and session cards retain their DOM, reducing redraws and preserving interaction state during polling.
- Password reset clears login lockout, restores password masking, and moves keyboard focus through recovery steps. Malformed saved login state is handled safely. Login remains a browser-only prototype.
- Storage changes from another tab trigger workspace refresh; sign-out in another tab returns the page to login.
- Reports now state that downloads are unavailable instead of claiming a report was prepared.
- The server explicitly checks that requested files stay inside its public directory. This is defensive hardening; an existing file-exposure exploit was not reproduced.
- Drawer, dialog, and section-menu feedback uses short CSS transitions (120–200 ms), respects reduced motion, and does not replay during polling. Mobile fields use 16 px text; secondary text contrast and long-name wrapping were improved.

## Verification

All 41 Node tests passed. The static build passed. Both headless Chrome browser runs passed with no uncaught JavaScript exceptions. Tests used disposable data files and browser profiles, not the project's class records.

| Area | Scenarios exercised | Result |
| --- | --- | --- |
| Login | Valid/invalid credentials, email normalization, password visibility, three-attempt lockout, persistence after reload, sign-out | Passed |
| Password recovery | Invalid code/password checks, mismatched confirmation, successful change, lockout reset, keyboard focus | Passed |
| Dashboard | Metrics/scoring boundaries, empty records, section selection, recent activity, preservation during outages | Passed |
| Class sections | Creation, duplicate/blank/overlong names, long multilingual names, HTML escaping, repeated submit, class-code copying/fallback | Passed |
| Enrollment API | Joining, duplicate joins, bad codes, persistence, concurrent creation/join/deletion | Passed |
| Learners | Search, each status and recipe filter, section isolation, back/forward navigation, reload, unchanged table preservation | Passed |
| Deletion | Cancel, confirmation, failed request/retry, focus restoration, related-record removal, last-section deletion | Passed |
| Sessions | All three recipe paths, completed-step calculation, expandable instructions, preserved expansion/focus, section filters, empty state | Passed |
| Reports | Navigation, section selector, truthful unavailable-download feedback | Passed; export remains unimplemented |
| Initial API outage | Dashboard, Learners, Live Sessions, and Reports load their UI while API returns HTTP 503, then recover via retry | Passed |
| Server shutdown | Stop the actual local listener after loading Dashboard, retain visible data, restart on the same port, reconnect without reloading | Passed |
| Slow/bad responses | Hanging form request, stalled response headers/body, cancellation, dropped connection, invalid JSON and payload shapes | Passed |
| Storage | Full quota, blocked storage, corrupt data preserved, repaired data retry, separate-tab write locking | Passed |
| Server errors | Malformed/oversized requests, encoded paths, damaged data-file error and recovery after repair | Passed |
| Responsive interaction | Desktop, 390 px mobile, 320 px long-name layout, drawer keyboard trap/Escape, rapid open/close, reduced motion | Passed |
| Static deployment | Build, initial sample data, create/delete/reload persistence, empty workspace behavior, no backend requests | Passed |

## Reproduce

From `bakeit-instructor-dashboard/`:

```powershell
npm.cmd test
npm.cmd run build
```

From the repository root, with Chrome installed at the path configured in `.preview/check.mjs`:

```powershell
node .preview/check.mjs
node .preview/check.mjs --static
```

The browser harness requires a Node version providing global `WebSocket` (the review used Node 24). Browser scenarios are implemented in `scripts/check-reliability.mjs`; screenshots are written to `.preview/review/`.

## Scope and remaining limitations

- Browser verification used Chrome on Windows with emulated viewport sizes and reduced motion. Other browser engines, physical mobile devices, assistive technologies, and sustained production load were not tested.
- Recovery works for an already loaded page, or when page assets load but the API is unavailable. A fresh visit while the entire host is offline still depends on the browser's network-error page; this app does not install an offline service worker.
- Existing prototype boundaries remain: browser-local authentication/password recovery, sample session/results data, no Unity telemetry ingestion, and no downloadable reports. Static data is stored per browser and origin.
- No deployment or external service was changed. Passing these checks does not establish production authentication or real VR connectivity.
