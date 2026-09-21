# VR class enrollment

The Unity/VR application is not included in this repository. Its Join Class screen should send this request to the same server as the instructor dashboard:

```http
POST /api/sections/join
Content-Type: application/json

{
  "classCode": "ABCDEFGH",
  "learnerId": "vr-account-0241",
  "learnerName": "Angela Dela Cruz"
}
```

Use an actual code generated from **Learners**. Codes ignore case and surrounding spaces. Supply the signed-in learner's stable account ID; do not generate a new ID on every attempt. On another device, use the reachable server hostname/IP, for example `http://192.168.1.10:3000/api/sections/join`. `localhost` on a headset refers to the headset itself.

## Success and retries

The first join returns HTTP `201`:

```json
{
  "section": { "id": "server-generated-section-id", "name": "BSHM 2A" },
  "enrollment": {
    "sectionId": "server-generated-section-id",
    "learnerId": "vr-account-0241",
    "learnerName": "Angela Dela Cruz",
    "joinedAt": "2026-09-15T08:00:00.000Z"
  },
  "alreadyJoined": false
}
```

Show the returned section name as confirmation and retain its ID for later session association. Retrying the same learner/section join returns HTTP `200` with `alreadyJoined: true` and the original enrollment. Retries do not duplicate learners. A learner can join multiple sections, with a separate membership in each.

The learner appears in the dashboard as **Not started**, with no score or live session. Joining records membership only; performance and session ingestion are not yet implemented.

## Validation and errors

Errors return `{ "error": "Readable message" }`. Display the message and allow a corrected request or retry.

| Status | Meaning |
| --- | --- |
| 400 | Invalid input or malformed JSON |
| 404 | Unknown class code |
| 413 | Request body exceeds 8 KiB |
| 415 | Request is not sent as `application/json` |
| 500 | Storage failure; retry after resolving the server issue |

Class codes have eight characters using uppercase letters and digits, excluding ambiguous `I`, `O`, `0`, and `1`. Learner IDs must contain 1–128 characters, and names 1–100, after trimming outer whitespace. IDs are case-sensitive.

## Dashboard endpoints

| Method | Path | Result |
| --- | --- | --- |
| GET | `/api/sections` | `{ sections: [...] }`, including codes and learner counts |
| POST | `/api/sections` | Body `{ "name": "BSHM 2A" }`; returns `201` and `{ section: {...} }` |
| DELETE | `/api/sections/ID` | Permanently removes the section and its enrollments; returns `200` and `{ section: {...}, removedEnrollments: N }`. Unknown/deleted IDs return `404`. |
| GET | `/api/learners?sectionId=ID` | `{ students: [...] }`; omit filter for all memberships |
| GET | `/api/activities?sectionId=ID` | Recent enrollment activity |
| GET | `/api/sessions?sectionId=ID` | Recipe-aligned samples for tagged demo learners; otherwise empty until VR ingestion is implemented |

Section names allow 1–80 characters and must be unique ignoring case and outer whitespace. Duplicate names return `409`. Code generation checks for collisions before saving.

Deletion and enrollment writes are serialized together. Removing a section invalidates its class code; later join attempts return `404`. The same learner's memberships in other sections are preserved. Sample sessions derived from removed enrollments disappear. Deletion has no undo; the website asks for confirmation first. The existing local/trusted-development authentication limitations below also apply to deletion.

## Storage and deployment

Data lives in `data/classes.json` or the path in `BAKEIT_DATA_FILE`. Keep it on persistent storage. Updates are serialized and replace the file atomically; run one server process per file. Fixed sample sections are not seeded into the saved roster.

The existing instructor login/reset is browser-only. The API has no server-side authentication and shares one instructor workspace. It is intended for local/trusted development. Before public deployment, add instructor authorization, authenticated learner identity, HTTPS, and rate limits. A class code identifies a section; it is not an authentication credential.

Cross-origin browser access is not enabled. Native VR HTTP clients can use the endpoint directly. Report downloads and VR score/session ingestion remain future work.
