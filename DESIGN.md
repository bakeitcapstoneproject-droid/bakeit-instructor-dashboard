---
name: BakeIT instructor dashboard
description: Ivory, cocoa, and orange tools for baking instructors.
colors:
  accent: "#ffae38"
  accent-hover: "#f69a20"
  accent-ink: "#392010"
  brown: "#75462c"
  brown-dark: "#321d15"
  brown-light: "#fff1d4"
  sidebar-brown: "#704329"
  canvas: "#f4eddf"
  page: "#fffdf3"
  white: "#fffef9"
  text: "#321d15"
  muted: "#73614f"
  line: "#e3dccb"
  control-line: "#958268"
  nav-text: "#f9e6b8"
  nav-hover: "#865537"
  table-surface: "#fff7e4"
  progress-fill: "#ad621d"
  green: "#3e684e"
  green-bg: "#eaf2eb"
  orange: "#875d16"
  orange-bg: "#fbf0d9"
  red: "#a04437"
  red-bg: "#f9eae5"
  destructive: "#9c3028"
typography:
  headline:
    fontFamily: "Poppins, \"Segoe UI\", Arial, sans-serif"
    fontSize: "32px"
    fontWeight: 600
    lineHeight: 1.3
    letterSpacing: "-.03em"
  signin-title:
    fontFamily: "Poppins, \"Segoe UI\", Arial, sans-serif"
    fontSize: "32px"
    fontWeight: 600
    lineHeight: 1.3
    letterSpacing: "-.03em"
  title:
    fontFamily: "Poppins, \"Segoe UI\", Arial, sans-serif"
    fontSize: "18px"
    fontWeight: 600
    lineHeight: 1.3
  body:
    fontFamily: "Poppins, \"Segoe UI\", Arial, sans-serif"
    fontSize: "14px"
    fontWeight: 400
    lineHeight: 1.6
  label:
    fontFamily: "Poppins, \"Segoe UI\", Arial, sans-serif"
    fontSize: "13px"
    fontWeight: 500
    lineHeight: 1.6
  metric:
    fontFamily: "Poppins, \"Segoe UI\", Arial, sans-serif"
    fontSize: "38px"
    fontWeight: 500
    lineHeight: 1.2
    letterSpacing: "-.03em"
rounded:
  badge: "5px"
  count: "8px"
  control: "8px"
  nav: "8px"
  panel: "12px"
spacing:
  space-1: "4px"
  space-2: "8px"
  space-3: "12px"
  space-4: "16px"
  space-6: "24px"
  space-8: "32px"
  space-12: "48px"
components:
  button-primary:
    backgroundColor: "{colors.accent}"
    textColor: "{colors.accent-ink}"
    rounded: "{rounded.control}"
    padding: "10px 18px"
  button-primary-hover:
    backgroundColor: "{colors.accent-hover}"
    textColor: "{colors.accent-ink}"
  button-secondary:
    backgroundColor: "{colors.white}"
    textColor: "{colors.text}"
    rounded: "{rounded.control}"
    padding: "10px 18px"
    typography: "{typography.label}"
  button-quiet:
    backgroundColor: "transparent"
    textColor: "{colors.muted}"
    rounded: "{rounded.control}"
    padding: "10px 18px"
  button-danger:
    backgroundColor: "{colors.destructive}"
    textColor: "#fff"
    rounded: "{rounded.control}"
    padding: "10px 18px"
  input:
    backgroundColor: "{colors.white}"
    textColor: "{colors.text}"
    rounded: "{rounded.control}"
    padding: "12px 14px"
  nav-active:
    backgroundColor: "{colors.accent}"
    textColor: "{colors.accent-ink}"
    rounded: "{rounded.nav}"
    padding: "12px 16px"
  badge-good:
    backgroundColor: "{colors.green-bg}"
    textColor: "{colors.green}"
    rounded: "{rounded.badge}"
    padding: "4px 8px"
  card:
    backgroundColor: "{colors.white}"
    textColor: "{colors.text}"
    rounded: "{rounded.panel}"
    padding: "{spacing.space-6}"
  metric-band:
    backgroundColor: "{colors.white}"
    textColor: "{colors.brown-dark}"
    rounded: "{rounded.panel}"
    padding: "24px 0"
---

# Design System: BakeIT

## Overview

**Creative North Star: "A familiar instructor workspace"**

BakeIT adapts the user-selected bakery reference into a warm, practical instructor interface. Ivory surfaces, a deep cocoa navigation area, amber-orange actions, and Poppins give the existing class and learner tools a consistent identity.

A full-height workspace, a divided metric band, horizontal class rows, and fine table rules support scanning. Permanent surfaces are flat; restrained shadows identify temporary layers. The original BakeIT logo remains the identity asset. Photography belongs to sign-in; records and performance views rely on real application content.

Extracted from the shipped [dashboard styles](bakeit-instructor-dashboard/public/assets/css/dashboard.css), [component details](bakeit-instructor-dashboard/public/assets/css/theme.css), and [sign-in styles](bakeit-instructor-dashboard/public/assets/css/styles.css). Frontmatter owns reusable primitives; [.impeccable/design.json](.impeccable/design.json) holds component examples and extension tokens. [PRODUCT.md](PRODUCT.md) remains authoritative for product capabilities and prototype limits.

**Key Characteristics:**

- Ivory workspace, cocoa navigation, and orange active states.
- Locally hosted Poppins with restrained headings and tabular metrics.
- Flat 12px panels, 8px controls, fine dividers, and compact horizontal class rows.
- Short feedback motion with reduced-motion support.

## Colors

Warm neutrals carry the workspace; orange identifies active navigation and primary actions.

### Primary

Amber orange is `accent`, with `accent-hover` for hover and `accent-ink` for its dark text. Cocoa `brown` supports links, outlines, and secondary emphasis.

### Neutral

`canvas` remains the body fallback; `page` fills the continuous full-height workspace; `white` fills panels and ordinary controls. `sidebar-brown` holds navigation, with pale `nav-text` and a lighter cocoa `nav-hover`. `text` and `brown-dark` share the darkest cocoa; `muted` supports secondary copy. `line` divides content; `control-line` gives fields a stronger boundary. `brown-light` and `table-surface` distinguish selected/supporting areas.

### Status

Paired green, amber, and red tokens color status labels and error feedback. Destructive actions use `destructive`; progress uses `progress-fill`. These are semantic treatments, not additional brand accents. Status labels retain text so color is not the only cue.

**The Orange Action Rule.** Pair orange actions and active navigation with dark cocoa text; reserve semantic green, amber, and red for status feedback.

## Typography

Poppins serves headings, body text, labels, and controls, with Segoe UI, Arial, and sans-serif fallbacks. Regular, medium, semibold, and bold weights (400/500/600/700) ship locally with their license.

Page and sign-in titles use 32px semibold, 1.3 line height, and -.03em tracking. Shared section headings use 18px semibold; class row titles use 20px and report titles 24px. Body text uses 14px/1.6. Table cells use 12px, table headers 11px, and activity text 13px. Navigation uses 14px medium, becoming semibold when active. Metrics use 38px medium, 1.2 line height, -.03em tracking, and tabular numerals. Class codes alone use 23px Consolas/monospace with .1em tracking.

At 560px and below, page and sign-in titles become 28px and metric values 30px. Mobile inputs and selects become 16px at 800px and below. This redesign preserves the font family while deliberately revising sizes and weights.

## Layout

Desktop uses a full-width, full-height shell with a 224px sidebar. Main content scrolls independently, with a stable scrollbar gutter, 40px top padding, responsive side padding of clamp(24px, 3vw, 64px), and 64px bottom padding. Direct content blocks are capped at 1360px and centered. The page header has a fine bottom divider and 32px bottom padding and margin; header buttons and selectors have at least 48px height. Reused spacing follows the frontmatter's 4px-based scale.

Performance metrics share one bordered band with four equal columns and vertical dividers. Values align at the bottom when labels wrap. Results occupy the flexible column beside a 272px activity column, separated by 24px. Each overview has a 408px-high, independently scrollable body with a reserved scrollbar gutter and keyboard access. Divided panel headers sit above the scrolling body, and table column headers remain sticky. Record tables retain a 620px minimum width inside their own horizontal scroll region, with 24px outer cell padding and reserved space for names, sections, and scores.

Class sections form a single list of horizontal rows with the section name, learner count, and open action. Session panels use two columns on wide screens. Reports use a panel capped at 920px, with title and action beside a divided contents list in a 1:1.2 split. Architecture content remains a bounded reading surface, capped at 800px with a 75ch notice.

| Maximum width | Shipped change |
| --- | --- |
| 1279px | Results and activity stack. |
| 1100px | Sidebar becomes 208px; metrics become a divided two-by-two band; sessions stack; page header wraps; main padding becomes 32px 24px 48px. |
| 900px | Sign-in hides the photograph and shows its logo above the form. |
| 800px | App uses natural document scrolling; compact brand/menu bar appears; navigation becomes a fixed drawer. |
| 560px | Main padding becomes 28px 20px 64px; class rows wrap; report content stacks; panels tighten; filters and actions wrap; header and dialog actions expand. |

The mobile drawer is at most 280px wide and 85vw, with an overlay and scroll lock while open. Metrics remain two columns on phones.

Sign-in uses equal photo/form columns with 16px outer padding and a 16px gap. The photograph has 12px corners; the centered form is at most 400px wide, with 64px 48px desktop padding, becoming 40px 24px on phones. The user-supplied baking ingredients photograph retains its monochrome brown tint and cover crop at 15% 50%. The full BakeIT logo is centered over it with a transparent background. Below 900px the single-column sign-in surface removes the outer inset and photograph.

## Elevation & Depth

Permanent workspace panels and the metric band use solid surfaces and fine borders without decorative shadows. The full-height shell and sign-in surface are flat. Soft shadows remain for temporary options, dialogs, and feedback; the table header's inset shadow supplies a sticky divider.

| Role | Shadow |
| --- | --- |
| Section options | `0 8px 28px #38291f26` |
| Dialog | `0 24px 90px #30261f20` |
| Feedback | `0 4px 20px #30261f0c` |

## Shapes

Panels and dialogs use 12px corners; controls and navigation use 8px. The desktop shell has square outer edges. Status labels use 5px and counts 8px; fields and panels have 1px borders. Learner and instructor initials use circular avatars. The mobile drawer rounds its exposed corners at 24px. The options popover retains 10px corners.

## Components

**Buttons.** Standard controls have a 44px minimum height and the frontmatter's control shape. Primary buttons use orange with semibold cocoa text; hover deepens orange, and press uses #eb8910. Secondary buttons use a pale surface and stronger border, changing to cream on hover. Quiet buttons have transparent backgrounds. Sidebar sign-out uses a transparent background with a muted light border and pale text. Destructive confirmation uses red with white text. Disabled buttons reduce opacity and use a waiting cursor.

**Inputs.** Ordinary fields and selects have a 48px minimum height. Sign-in fields and action buttons have a 52px minimum height; fields use the warm panel surface. Labels remain visible. Invalid fields use the error border and adjacent error text. Placeholder text uses the muted token.

**Navigation.** A vertical list of text links sits on cocoa. Hover uses lighter cocoa; the current page uses orange and semibold dark text. Below the mobile breakpoint, the Menu button controls the drawer and exposes its expanded state.

**Instructor profile.** A divider separates the instructor profile from navigation. A 32px cream initials avatar accompanies the name and wrapping email above Sign out. The same profile appears in the mobile drawer. It is a noninteractive account summary.

**Cards and tables.** Shared panels use the panel shape and surface. Horizontal class rows pair section names and learner counts with a visible open action; selecting a section changes its border to cocoa. Table headers have a pale cream fill and semibold text, with horizontal row dividers and a matching row-hover fill. The single divided metric band contains labels and large values without decorative icon tiles. A dashed creation row follows the class list.

**Status and progress.** Status labels use paired foreground/background colors with 11px semibold text. Count labels use the cream/cocoa pair and a softer rectangle. Progress tracks are 5px high in compact records and 6px high in session panels. Session progress also has a text caption and expandable recipe instructions.

**Session layout.** A fine rule separates learner identity from progress. On wide screens, summary headers, step titles, and instructions reserve room for wrapping so adjacent cards align with typical recipe content. Disclosures still expand independently; their summary controls have a 44px minimum height. Event status labels retain their width next to wrapping descriptions. Empty session content spans the grid.

**Dialogs and feedback.** Native dialogs are at most 440px wide, with 32px padding (24px on phones). Confirmation actions wrap when needed. Feedback appears near the viewport bottom; in-page errors retain their message and retry control. The options popover keeps the class code, copy action, and delete action together.

**Report downloads.** The report panel lists the five rubric criteria beneath the Class performance heading, without explanatory subtitle copy. Its action area contains Prepare report and an underlined Download Excel template link. Prepare report opens a native confirmation dialog naming the selected section and Excel format, with Cancel and Prepare report actions. There is no format selector. Preparing is disabled until a specific section is selected. Section changes are checked before and after requesting data. Pending exports show Preparing Excel and lock the dialog controls and page selector until success or failure. Success closes the dialog, restores focus and confirms the download; failures remain in the dialog with a retry message. Cancel and Escape dismiss idle dialogs without downloading. The action area stacks below the category list on phones. Excel workbooks use pure black fonts, light cream title bands, orange section bands, colored header backgrounds, readable column widths, frozen headers and learner columns, filters, alternating ivory rows and landscape printing. The first sheet shows learner identity, recipe, assessment time, five ratings, total out of 25, session count and Status. Completion status, Completion (%) and Completion scope are omitted. The second sheet shows Session history. Safety and waste, Procedural accuracy and Report data tabs are omitted. The JSON response preserves all version 2.1 fields. The blank template documents the same report fields without providing a website grading workflow.

**Loading.** Initial metrics use static neutral placeholders; results, activity, class sections, and sessions show named loading messages. A small bottom-right status appears after 150ms while page data is pending. Refreshes retain existing content and show "Updating data" instead of replacing records. The live status sits outside the busy content region. Pending actions use a 16px current-color spinner and an action-specific label; related form controls are disabled until completion and restored on failure. Spinner rotation takes 800ms and stops under reduced motion, leaving the text visible. Fast local work has no artificial delay. Report preparation retains its existing unavailable-download feedback.

**Focus and motion.** The shared focus treatment is a 3px cocoa outline with a 4px offset; the sidebar uses pale ivory. Recipe disclosure uses a 2px outline. Color and border state changes take 140ms ease. Options and dialogs open over 160ms with a small 4px translation; backdrop feedback takes 120ms; the mobile drawer takes 200ms. Reduced motion disables animations and transitions. There are no scrolling entrances or cursor effects.

**The Visible Focus Rule.** Keep the keyboard outline visible on every interactive control, using the light outline on the cocoa sidebar.

**Assets.** Use the user's 2026-09-29 white chef-hat and BakeIT wordmark PNG unchanged. The 1.3:1 logo frame trims only the square file's transparent margins, displaying the full hat and lettering. Frame widths are 128px in the sidebar, 64px in the mobile header, 240px over the sign-in photo, and 120px above the mobile sign-in form. Sign-in uses an edited version of the user's baking ingredients photograph with a monochrome brown tint. Asset sources, edit prompts, shipping filenames, and font provenance are recorded in [design-assets.md](bakeit-instructor-dashboard/docs/design-assets.md).

**2026-09-29 redesign.** This pass preserves the palette, Poppins family, assets, all content, and application behavior while changing the layout and typography hierarchy. No new raster assets were introduced. The current evidence and earlier refinement history are separated in the [interface review](bakeit-instructor-dashboard/docs/interface-review.md).

## Do's and Don'ts

### Do

- Do preserve the BakeIT logo and the supplied ivory, cocoa, and orange direction.
- Do use the shared spacing scale, 12px panels, 8px controls, fine table dividers, and locally hosted Poppins.
- Do retain visible labels, keyboard focus, readable status text, and reduced-motion behavior.
- Do keep record tables usable through their own horizontal scrolling region on narrow screens.
- Do record image origins and generation prompts in the asset provenance document.

### Don't

The following exclusions are the user's confirmed direction.

- Don't use purple-to-blue gradients or text gradients.
- Don't add heading emojis, headline badges, or three icon boxes.
- Don't use Inter, italic serif accents, or Space Grotesk with Instrument Serif.
- Don't introduce colored left-border cards, glassmorphism, or low-contrast dark mode.
- Don't add ubiquitous Lucide icons or default shadcn styling.
- Don't add scroll fades, cursor beams, or fading button hovers.
- Don't introduce inconsistent spacing, repeated em-dash prose, or buzzword copy.
- Don't add grain over gradients.

**VR result monitoring.** Learner progress and dashboard tables display a single total out of 25, with no standalone criterion column or Grade/Edit action. All five criterion ratings remain in Excel reports. Instructors monitor results from the game; no score editor or website score mutation endpoint is provided.

**Learner table alignment.** The Learner progress table uses fixed column proportions and consistent cell padding. Sessions, totals and history actions are centered beneath their headings; names, sections, recipes and status labels align left. View history is an underlined cocoa text button with a transparent resting surface, a cream hover surface and a 44px target. Keep its keyboard focus outline. The table scrolls horizontally within a keyboard-focusable region on narrow screens.

**Subtitle policy.** Omit explanatory page and section subtitles throughout the website, including sign-in branding text. Preserve learner identities, field instructions, confirmation details, errors, loading indicators and status messages.

**Excel consistency.** Class performance is the first worksheet. Instructor-facing sheets use the website's 10-digit sample learner IDs, five integer ratings, totals out of 25, and Passed/Needs practice/Awaiting assessment. Session history exports the popup records with native Excel dates in PHT. Source IDs and UTC timestamps remain in the JSON response; no raw data worksheet is exported.

**Excel text color.** Use pure black (#000000) for every workbook font, including titles, metadata and status labels. Status cells have no background fill and no conditional color formatting; title/header surfaces stay light enough for black text. This applies to exports, templates and examples.

**Report feedback and class selectors.** Excel download confirmations disappear after two seconds. Starting another report or changing sections clears the previous notification timer. Class section selectors and notification borders use the light 1px line token (#e3dccb); keyboard focus outlines remain visible.
