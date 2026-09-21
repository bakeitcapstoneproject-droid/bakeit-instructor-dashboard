# Deploy the static demo to Vercel

This deployment contains HTML, CSS, images and browser JavaScript only. There are no Vercel Functions, hosted databases, paid integrations, secrets, or environment variables to configure.

## Import settings

1. Commit and push the deployment changes to your GitHub repository.
2. Import the repository into your **Hobby** team on Vercel.
3. Set Root Directory to `bakeit-instructor-dashboard` and Application Preset to **Other**.
4. Disable previous dashboard overrides for Build Command, Output Directory and Install Command. The committed `vercel.json` supplies these settings:

   | Setting | Value |
   | --- | --- |
   | Build Command | `node scripts/build-static.mjs` |
   | Output Directory | `dist` |
   | Install Command | Empty (no dependencies) |

5. Deploy. The main URL opens the login page; `/login.html` also works.
6. Sign in with `instructor@mcl.edu.ph` / `demo123`.

The repository also has a root-level `vercel.json`, so leaving Root Directory at `./` works too. In that case, the build command is `node bakeit-instructor-dashboard/scripts/build-static.mjs`, and the output directory is `bakeit-instructor-dashboard/dist`.

Do not keep the earlier empty Build Command or `public` Output Directory overrides: those publish the local API version instead of the static demo. Do not use `npm start` as a build command.

## What works

- Login and browser-local demo password changes.
- Dashboard totals, learner filters, sample sessions and recipe instructions.
- Two initial fictional sections with sample learners, seeded once per browser and site origin.
- Creating new empty sections, copying sample class codes, and deleting sections with confirmation.
- Changes persist across page navigation, reloads and sign-out in the same browser. Empty workspaces stay empty after all sections are deleted.
- `/sections.html` redirects to `/students.html`.
- Direct links to HTML pages and section hashes work.

## Demo boundaries

- Each browser/site origin has its own workspace. Different devices, preview URLs and the production URL do not share records. Clearing site data resets that browser's demo.
- No local `data/classes.json` records are copied into the deployment; only the existing fictional samples in `public/assets/js/data.js` are seeded.
- Login is a presentation feature, not server-side authentication. Password recovery does not send email; the demo accepts a six-digit code and changes only this browser's password.
- Class codes are demo values. A Unity/VR client cannot join through this static site. Sessions are fixed samples, not live telemetry.
- The existing Prepare report control only displays feedback; file export is not implemented.
- Shared accounts, real student records, real password recovery, VR ingestion and durable shared storage require a separately designed backend.

## Free-plan scope

Vercel Hobby is for personal, non-commercial projects. Stay on Hobby, use the provided `vercel.app` address, and do not purchase domains, paid add-ons or external services. Hobby usage remains limited; reaching a limit may pause service. This repository cannot set or guarantee your account's future billing policy.

Official references: [Hobby plan](https://vercel.com/docs/plans/hobby), [static build configuration](https://vercel.com/docs/builds/configure-a-build), [vercel.json](https://vercel.com/docs/project-configuration/vercel-json).

## Local checks

From the application directory:

```sh
npm test
npm run build
```

The build writes `dist/`, including an `index.html` and a static runtime configuration. Serve `dist/` with any static HTTP server to preview the deployment. Do not open HTML files directly with `file://`, since the app uses JavaScript modules.

`npm start` continues to run the original local Node server and uses `data/classes.json`; building does not change that behavior. The generated `dist/` directory is ignored by Git and rebuilt by Vercel.
