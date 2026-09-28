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
    fontSize: "34px"
    fontWeight: 700
    lineHeight: 1.3
    letterSpacing: "-.03em"
  signin-title:
    fontFamily: "Poppins, \"Segoe UI\", Arial, sans-serif"
    fontSize: "36px"
    fontWeight: 700
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
    fontSize: "34px"
    fontWeight: 600
    lineHeight: 1.2
    letterSpacing: "-.025em"
rounded:
  badge: "5px"
  count: "8px"
  control: "10px"
  nav: "12px"
  panel: "16px"
  frame: "28px"
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
    padding: "14px 16px"
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
  metric:
    backgroundColor: "{colors.white}"
    textColor: "{colors.brown-dark}"
    rounded: "{rounded.panel}"
    padding: "{spacing.space-6}"
---

# Design System: BakeIT

## Overview

**Creative North Star: "A familiar instructor workspace"**

BakeIT adapts the user-selected bakery reference into a warm, practical instructor interface. Ivory surfaces, a deep cocoa navigation area, amber-orange actions, and Poppins give the existing class and learner tools a consistent identity.

Solid panels, visible table rules, and restrained blurred shadows support scanning. The original BakeIT logo remains the identity asset. Photography belongs to sign-in; records and performance views rely on real application content.

Extracted from the shipped [dashboard styles](bakeit-instructor-dashboard/public/assets/css/dashboard.css), [component details](bakeit-instructor-dashboard/public/assets/css/theme.css), and [sign-in styles](bakeit-instructor-dashboard/public/assets/css/styles.css). Frontmatter owns reusable primitives; [.impeccable/design.json](.impeccable/design.json) holds component examples and extension tokens. [PRODUCT.md](PRODUCT.md) remains authoritative for product capabilities and prototype limits.

**Key Characteristics:**

- Ivory workspace, cocoa navigation, and orange active states.
- Locally hosted Poppins with clear size and weight hierarchy.
- Rounded panels, explicit borders, and compact task controls.
- Short feedback motion with reduced-motion support.

## Colors

Warm neutrals carry the workspace; orange identifies active navigation and primary actions.

### Primary

Amber orange is `accent`, with `accent-hover` for hover and `accent-ink` for its dark text. Cocoa `brown` supports links, outlines, and secondary emphasis.

### Neutral

`canvas` surrounds the desktop frame; `page` fills the frame and scrollable content; `white` fills panels and ordinary controls. `sidebar-brown` holds navigation, with pale `nav-text` and a lighter cocoa `nav-hover`. `text` and `brown-dark` share the darkest cocoa; `muted` supports secondary copy. `line` divides content; `control-line` gives fields a stronger boundary. `brown-light` and `table-surface` distinguish selected/supporting areas.

### Status

Paired green, amber, and red tokens color status labels and error feedback. Destructive actions use `destructive`; progress uses `progress-fill`. These are semantic treatments, not additional brand accents. Status labels retain text so color is not the only cue.

**The Orange Action Rule.** Pair orange actions and active navigation with dark cocoa text; reserve semantic green, amber, and red for status feedback.

## Typography

Poppins serves headings, body text, labels, and controls, with Segoe UI, Arial, and sans-serif fallbacks. Regular, medium, semibold, and bold weights (400/500/600/700) ship locally with their license.

Use the frontmatter's headline for page titles, title for section headings, body for general text, and label for standard buttons. Dense table cells and activity text use 13px; table headers and supporting captions use 12px. Navigation uses 15px medium, changing to semibold when active. Metrics use tabular numerals. Class codes alone use Consolas/monospace at 23px with .1em tracking.

At 560px and below, page titles become 28px, metric values 30px, and sign-in titles 30px. Mobile inputs and selects become 16px at 800px and below.

## Layout

Desktop uses an inset app frame with a maximum width of 1600px, 24px outer margins, rounded 28px corners, and a 232px sidebar. The frame fits the viewport height. Main content scrolls inside it while the sidebar stays aligned with the frame; overflow is clipped to the outer corners so the ivory background remains continuous. Main content has 40px top, 32px side, and 48px bottom padding. Reused gaps and padding follow the frontmatter's 4px-based spacing scale.

Performance metrics use four equal columns. Results and activity use a 2.3:1 split with a 260px minimum secondary column. Each overview has a 368px-high, independently scrollable body with a reserved scrollbar gutter and keyboard access. Panel titles stay outside the scrolling body, and result column headers remain sticky. Class and session grids use two equal columns. Record tables retain a 620px minimum width inside their own scroll container. Reports use a single column capped at 720px.

| Maximum width | Shipped change |
| --- | --- |
| 1320px | Results and activity stack. |
| 1100px | Frame margins become 16px; sidebar becomes 208px; metrics use two columns; page header wraps; main padding becomes 32px 24px 48px. |
| 900px | Sign-in hides the photograph and shows its logo above the form. |
| 800px | The frame becomes full width with natural document scrolling; navigation becomes a fixed drawer; compact brand/menu bar appears; sessions stack. |
| 560px | Main padding becomes 28px 20px 64px; class grid stacks; panel padding tightens; filters and actions wrap. |

The mobile drawer is at most 280px wide and 85vw, with an overlay and scroll lock while open. The metric grid remains two columns on phones.

Sign-in uses a .9:1.1 photo/form split. The centered form is at most 440px wide; desktop form padding is 48px, becoming 32px 24px at 560px. The user-supplied baking ingredients photograph has a monochrome brown tint and uses a cover crop at 15% 50%, keeping the whisk, flour, and milk in the narrow image panel. The BakeIT logo is centered horizontally and vertically directly over the photograph with a transparent background.

## Elevation & Depth

Solid tonal layering and fine borders supply most structure. Soft blurred shadows distinguish the frame, metrics, transient options, dialogs, and feedback. The shipped implementation does not use hard offset shadows.

| Role | Shadow |
| --- | --- |
| App frame | `0 8px 28px #50311c12` |
| Metric panel | `0 3px 12px #50311c12` |
| Section options | `0 8px 28px #38291f26` |
| Dialog | `0 24px 90px #30261f20` |
| Feedback | `0 4px 20px #30261f0c` |

## Shapes

The desktop frame has rounded 28px corners and clips both the sidebar and scrollable content to one continuous surface. Panels and dialogs retain the shared panel radius. Controls, navigation, count labels, and status labels step down through the frontmatter's shape scale. Panels and fields use 1px borders. Learner initials sit inside circular avatars. Mobile removes the outer app rounding; the drawer has exposed corners rounded at 24px.

## Components

**Buttons.** Standard controls have a 44px minimum height and the frontmatter's control shape. Primary buttons use orange with semibold cocoa text; hover deepens orange, and press uses #eb8910. Secondary buttons use a pale surface and stronger border, changing to cream on hover. Quiet buttons have transparent backgrounds. The sidebar sign-out control uses orange. Destructive confirmation uses red with white text. Disabled buttons reduce opacity and use a waiting cursor.

**Inputs.** Ordinary fields and selects have a 48px minimum height. Sign-in fields are 56px tall and match the page background; sign-in action buttons are 52px tall. Labels remain visible. Invalid fields use the error border and adjacent error text. Placeholder text uses the muted token.

**Navigation.** A vertical list of text links sits on cocoa. Hover uses lighter cocoa; the current page uses orange and semibold dark text. Below the mobile breakpoint, the Menu button controls the drawer and exposes its expanded state.

**Instructor profile.** Above Sign out, a warm tan (#efd9b4) profile row pairs a 36px orange initials avatar with the signed-in instructor's name and email. Account text wraps within the sidebar, and the same profile appears in the mobile drawer. It is an account summary, not an interactive control.

**Cards and tables.** Shared panels use the panel shape and surface. Class cards contain section names and a visible open action; selecting a section changes its border to cocoa. Table headers have a pale cream fill and semibold text, with horizontal row dividers and a matching row-hover fill. Metric cards contain a label and a large value without decorative icon tiles.

**Status and progress.** Status labels use paired foreground/background colors with 11px semibold text. Count labels use the cream/cocoa pair and a softer rectangle. Progress tracks are 5px high in compact records and 10px high in session panels. Session progress also has a text caption and expandable recipe instructions.

**Dialogs and feedback.** Native dialogs are at most 440px wide, with 32px padding (24px on phones). Confirmation actions wrap when needed. Feedback appears near the viewport bottom; in-page errors retain their message and retry control. The options popover keeps the class code, copy action, and delete action together.

**Focus and motion.** The shared focus treatment is a 3px cocoa outline with a 4px offset; the sidebar uses pale ivory. Recipe disclosure uses a 2px outline. Color and border state changes take 140ms ease. Options and dialogs open over 160ms with a small 4px translation; backdrop feedback takes 120ms; the mobile drawer takes 200ms. Reduced motion disables animations and transitions. There are no scrolling entrances or cursor effects.

**The Visible Focus Rule.** Keep the keyboard outline visible on every interactive control, using the light outline on the cocoa sidebar.

**Assets.** Preserve the existing BakeIT logo artwork. Sign-in uses an edited version of the user's baking ingredients photograph with a monochrome brown tint. Its source, edit prompt, shipping filename, and font provenance are recorded in [design-assets.md](bakeit-instructor-dashboard/docs/design-assets.md).

## Do's and Don'ts

### Do

- Do preserve the BakeIT logo and the supplied ivory, cocoa, and orange direction.
- Do use the shared spacing scale, rounded panels, table dividers, and locally hosted Poppins.
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
