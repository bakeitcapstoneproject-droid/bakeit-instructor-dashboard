# Learner session history

The Learners table opens a read-only native dialog through **View history**. History is scoped to the learner's selected class section. Dates and times display in `Asia/Manila` (UTC+08:00), newest sessions first. Each row includes recipe, session state, start/end time, criterion total out of 25 and the shared Passed/Failed result. Missing ratings remain Awaiting assessment; missing timestamps remain Not recorded. The dialog supports Close, Escape, keyboard focus restoration, loading, retry and empty states.

`GET /api/learners/:learnerId/history?sectionId=:sectionId` returns `{ history }`, where history contains `learnerId`, `sectionId`, `sample`, `sessionCount` and `entries`. Each response entry contains `id`, `recipe`, `startedAt`, `endedAt`, `status`, `score` and `result`. Missing/all sections return 400; learners not enrolled in the requested section return 404. This is membership scoping, not production authorization; the prototype's existing authentication limits still apply.

The local provider reads an optional `sessionHistory` array on the enrollment. The browser provider reads the same array on its stored student. Each source entry uses this format:

```json
{
  "id": "vr-session-123",
  "recipe": "Brownies",
  "startedAt": "2026-09-29T02:00:00Z",
  "endedAt": "2026-09-29T02:30:00Z",
  "status": "Completed",
  "ratings": {
    "decorum": 4,
    "kitchen_organization": 4,
    "safety_sanitation": 5,
    "baking_skills": 4,
    "product_appraisal": 5
  }
}
```

Session IDs must be unique within the history. Status is Completed, In progress or Abandoned. Timestamps require an explicit timezone, or null when unknown. Ratings are null/omitted before assessment; otherwise all five integer ratings from 1 through 5 are required. Totals and results are derived, not entered by the instructor.

When no history array exists, only records internally marked `demo: true` receive stable fictional sessions for preview. An explicit empty array suppresses this fallback. Generated dates are sample dates, not observed gameplay. The popup omits explanatory subtitles; sample provenance remains in the data. Sample learner IDs appear beneath names as numeric display values: local hexadecimal sample namespaces convert to decimal with a three-digit learner suffix, and browser sample IDs retain their numeric portion. Sample display values use the final ten digits, padded with leading zeroes when shorter. These presentation values are not identity keys; histories and reports retain the full stored ID. Search supports the displayed ID. Real provider IDs are preserved as supplied. Stored identities are unchanged. Real records never receive fabricated timestamps or scores.

VR ingestion is still pending. The future provider must persist session history along with its latest learner summary; this change adds the history reader and presentation, not a Unity upload endpoint. Excel assessment sheets retain one latest-result row per learner. The Session history sheet exports these same available logs, with numeric display IDs, PHT dates, totals and results.
