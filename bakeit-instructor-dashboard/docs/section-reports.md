# Section Excel reports

On **Reports**, select a specific section and choose **Prepare report**. A confirmation dialog identifies the section and **Excel (.xlsx)** format; choose **Prepare report** again to download, or **Cancel** to return without requesting a report. Excel is the only download format. The selected section is checked before and after the request, so changed or removed sections do not silently export the wrong report. Preparing shows a loading state, locks the dialog controls and section selector, and prevents duplicate submissions. Failed requests keep the dialog open with a retryable error. Cancel and Escape dismiss an idle dialog and restore focus to the opener.

Each assessment sheet contains one row per enrolled learner's latest recorded result, with the existing session count. The Session history sheet contains one row per available session, grouped by learner and newest first. Learners without detailed logs have no invented history rows. All Sections is deliberately not exportable. An empty section produces headings without learner rows; its filename identifies the section. **Download Excel template** provides a blank formatted workbook without selecting a section.

## Formatted Excel report

The workbook contains exactly two sheets: **Class performance** and **Session history**. Safety and waste, Procedural accuracy, and Report data tabs are omitted from exports, templates and examples. Each has a BakeIT title, section information, colored column headings, descriptive column labels, readable widths, alternating row fills, filters, frozen headers and learner columns, and landscape print settings. Unrecorded measurements remain blank. Criterion ratings are whole numbers from 1 to 5; their unweighted sum is the total out of 25. Class performance ends with Status; Completion status, Completion (%) and Completion scope are omitted from the workbook. Supporting measurements remain in the JSON response.

Both sheets use the same 10-digit sample learner IDs as the website. Text cells preserve leading zeroes and prevent formula execution. Session history includes learner name and ID, recipe, session state, start/end dates and times in PHT, total out of 25, and Passed/Failed/Awaiting assessment. Assessment and history dates are native Excel date cells in Philippine time (UTC+08:00). All workbook fonts are pure black, including status labels. Status cells have no background fill and use plain black text without conditional color formatting. Title and identity-header backgrounds are light cream for readability. The template includes twelve empty formatted rows on each of the two tabs. Full stored IDs and supporting measurements remain available in the JSON response; removing worksheet tabs does not delete source data.

`public/assets/js/domain/section-workbook.js` generates a genuine `.xlsx` package using the small, dependency-free ZIP writer in `xlsx-package.js`; it works in the browser or Node without a CDN, Excel installation, or an AWS connection. Keep the workbook as `.xlsx` when saving edits in Excel.

## Files for the integration handoff

- [Formatted Excel template](../public/assets/reports/section-report-template.xlsx)
- [Fictional populated Excel example](../public/assets/reports/section-report-example.xlsx)
- [Matching JSON response example](../public/assets/reports/section-report-example.json)
- [JSON schema for the report object](../public/assets/reports/section-report.schema.json)
- Shared builder, validator and filename rules: `public/assets/js/domain/section-report.js`.
- Workbook serialization: `public/assets/js/domain/section-workbook.js`.

Regenerate these artifacts with `node scripts/generate-report-examples.mjs`. Example measurements are fictional and marked `data_source=demo`; they illustrate the approved five-criterion rubric and are not actual learner results.

## Report data contract v2.1

The JSON response preserves the 33 stable row fields below. The workbook exports only the Class performance and Session history fields. Version 2.1 adds a required `histories` array to the report envelope, with one validated learner-history object per report row. Each object uses the [learner-history contract](learner-history.md); membership, duplicate identities, timestamps and results are validated before export. Unknown values are blank workbook cells and `null` in JSON. A recorded zero remains `0`. Text values are stored as explicit string cells, including formula-leading text; JSON keeps the original text. The existing versioned JSON endpoint remains the AWS integration boundary.

| Category | Columns | Meaning |
| --- | --- | --- |
| Identity | `schema_version`, `generated_at` | `2.1`; UTC ISO timestamp for the snapshot, repeated on every populated row. |
| Identity | `section_id`, `section_name` | Stable section identifier and readable name. |
| Identity | `learner_id`, `learner_name` | Stable learner identifier and readable name. |
| Provenance | `data_source` | `local` for the local enrollment store, `demo` for fictional records, `aws` for the future verified provider. |
| Result | `recipe`, `session_id`, `assessed_at`, `session_count` | Latest assessed recipe/attempt and UTC assessment time; current recorded session count. Unknown attempt IDs/times remain blank. |
| Rubric | `decorum_rating`, `kitchen_organization_rating`, `safety_sanitation_rating`, `baking_skills_rating`, `product_appraisal_rating` | Each is an integer from 1 to 5. Decorum evaluates waste management. All five ratings must be present together or all absent. |
| Rubric | `total_score`, `max_score`, `score_remark` | Sum of the five criteria (5 to 25), maximum 25, and Passed at 15 or above, otherwise Failed. Total and raw remark stay blank without all five ratings; formatted sheets show Awaiting assessment. |
| Legacy | `legacy_score_percent` | Preserved older percentage when available; excluded from rubric totals and averages. |
| Class performance | `completion_status`, `completion_percent`, `completion_scope` | Explicit Not started, In progress, Completed or Abandoned; recorded 0–100 progress; description of what completion covers. A learner with zero recorded sessions is Not started. Otherwise status is unknown until recorded. |
| Safety and waste | `safety_score_percent`, `safety_checks_passed`, `safety_checks_total`, `safety_incident_count` | Supporting telemetry percentage and nonnegative recorded counts, separate from the Safety and Sanitation rating. Unknown measurements are blank, never assumed safe or zero. No safety score is inferred from incident count. |
| Safety and waste | `waste_level`, `waste_quantity`, `waste_unit` | Recorded Low/Medium/High classification; optional quantity and its unit supplied together. Do not sum cups, teaspoons, eggs, transfers or grams into one quantity. Only supply a total when its single unit and scope are valid. |
| Procedural accuracy | `procedural_correct_steps`, `procedural_assessed_steps`, `procedural_accuracy_percent` | Correct and assessed steps from the approved assessment. If accuracy is absent but both counts exist and assessed steps are positive, calculate correct ÷ assessed × 100, rounded to two decimals. Otherwise keep accuracy blank. Never substitute overall score. |

The current local/static data has scores, recipes, session counts and waste classifications. It has no measured safety, waste quantities or procedural assessments. Its legacy `progress` display field is intentionally not exported as measured completion. Rubric ratings do not prove completed recipe work. In particular, Cupcake baking practice through cooling must remain distinct from full-dish completion.

## Provider contract

`GET /api/reports/sections/{sectionId}` returns `{ "report": { "schemaVersion": "2.1", "generatedAt": "...", "section": { "id": "...", "name": "..." }, "rows": [...], "histories": [...] } }`. The schema describes the `report` property. The Node endpoint reads section membership and results from one repository snapshot; the static implementation reads one browser-workspace transaction. Missing/deleted sections fail; `all` is rejected. No report request mutates stored data.

`CloudDataService.getSectionReport(sectionId)` is the frontend integration boundary. It rejects a wrong section, duplicate learner rows, invalid numbers, missing columns or unsupported schema version before downloading. Cross-field checks supplement the JSON schema: row metadata must match the envelope, counts cannot exceed their totals, waste quantity needs a unit, and totals must equal the sum of five valid criterion ratings. Files use `bakeit-{section-name}-{section-id}-{UTC-timestamp}.xlsx` with filename-unsafe characters removed.

The builder accepts existing learner fields plus an optional `assessment` object containing the snake_case assessment columns from this contract. Assessment fields override legacy score/recipe/waste fields. It never reads live-session presentation cards as finalized assessments. An AWS adapter can construct this input and call `buildSectionReport(section, learners, { source: 'aws' })`, or return the validated report shape directly. The pure `sectionReportWorkbook(report)` function can run on the server if generation later moves there.

## AWS handoff plan

This change prepares the contract and working local downloads; it does not provision AWS or connect VR telemetry.

1. Have the authenticated backend resolve the requested section within the instructor's allowed sections. Browser selection is not authorization.
2. Read that section's enrollment roster and join each learner's latest finalized assessment. Choose latest by assessment timestamp, with a stable attempt-ID tie-break; do not choose highest score. Learners without results still receive a row with unknown measurements blank. Do not silently truncate paginated records.
3. Map the approved score rubric, explicit completion scope, safety checks, homogeneous waste units and procedural counts to the report contract. Preserve the attempt ID and assessment timestamp; mark provider rows `aws`.
4. Return the versioned endpoint response through the existing request boundary. A future server-side Excel download can reuse the workbook serializer with `application/vnd.openxmlformats-officedocument.spreadsheetml.sheet` and a safe attachment filename.
5. Verify section ownership, row completeness, missing values, empty sections, timestamps and formula escaping with integration tests before replacing the local provider. Define an explicit schema migration when changing columns or meanings.

The current Node API remains a local prototype without server authentication. Production identity, authorization, telemetry persistence and assessment policy are separate integration work.

## Verification

The Node suite covers section isolation, unknown versus measured-zero results, assessment mapping, invalid values, Unicode, filenames, empty sections, schema validation, deleted sections and the local endpoint. Workbook checks cover the ZIP package, category sheets, frozen headers, styling, unchanged provider data, omitted completion columns, literal text and blank templates. The browser suite exercises report confirmation, cancellation and focus restoration, Excel downloads, pending and duplicate-submit protection, failed-request retry, section changes before and after requesting data, the template link and responsive layouts in server and static modes. It also verifies that retired CSV files are unavailable. Generated workbooks were opened in Microsoft Excel to verify compatibility, numeric display and formatting. Tests use disposable data and do not connect to AWS.

## Monitoring VR results

The baking VR game supplies assessment ratings. The website is read-only for scoring: no Grade/Edit controls and no POST /api/assessments endpoint. Learner progress and dashboard results show only the total out of 25. All five criterion ratings remain available in the Excel report. The blank Excel template describes the report layout; it is not a grading input or import workflow.

A future VR provider supplies assessment.ratings with keys decorum, kitchen_organization, safety_sanitation, baking_skills, product_appraisal, each an integer from 1 to 5. The report builder derives the unweighted total and validates the complete set. Include the game assessment timestamp and attempt identifier when available. Current data is local or demonstration data; this change does not connect the VR game or AWS.

Scale: 5 Excellent / exceeds standard; 4 Good / meets standard; 3 Satisfactory / acceptable; 2 Needs improvement; 1 Poor / below standard. Totals remain out of 25; 15 or above is Passed, otherwise Failed. Existing records remain intact; missing game results are not fabricated.
