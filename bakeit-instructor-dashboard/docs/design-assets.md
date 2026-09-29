# Design asset provenance

The visual reference was supplied by the user in `Bakery-management-system (Community).zip`. Its PNG exports informed layout, color, typography, and control styling. The reference's customer, product, order, sales, and staff features were not introduced into BakeIT.

## Sign-in ingredients photograph

- User-supplied source: `C:/Users/LENOVO/Downloads/diana-krotova--NmIsfg5DEk-unsplash.jpg` (3648 × 2736). The original source file is unchanged.
- Shipping file: `public/assets/images/signin-ingredients-brown.jpg` (1448 × 1086).
- Edited with the built-in image generation tool on 2026-09-28 to create a monochrome photograph with a warm brown tint, then encoded as JPEG at quality 90.
- Original edit output: `01a0e5fc-d61b-7a00-86e0-f952bede6715/exec-0e10c75b-70d6-45e6-8d66-a23cd850f372.png` in the local generated-images directory.
- Decorative photograph with empty alt text. The logo remains separate HTML artwork. CSS uses `object-fit: cover` at `15% 50%` to keep the left-side baking ingredients visible in the sign-in panel.

Exact edit prompt:

> Edit target: the provided baking ingredients photograph. Make a color-treatment-only edit for a BakeIT sign-in page. Preserve the exact 4:3 landscape composition, camera angle, all ingredient and utensil shapes and positions, sizes, edge crops, shadows, and original natural stone countertop texture. Do not crop, reposition, add, remove, reconstruct, or embellish any objects. Convert the original photo to black and white first, then apply a restrained, uniform warm cocoa-brown sepia tint: charcoal-brown shadows, neutral warm brown-gray midtones, softly ivory highlights. Overall it should read as a black-and-white photograph with a subtle brownish filter, not a yellow/orange color photo. Keep clean photographic detail and readable highlights without crushing shadows. No new grain, texture overlays, gradients, text, logo, watermark, border, or UI. This is the supplied photo with only tonal/color adjustments.

## Previous cake photograph (no longer used on sign-in)

- Former image file: `public/assets/images/signin-cake.jpg` (1024 × 1536).
- Created with the built-in image generation tool on 2026-09-28, then encoded as JPEG at quality 88 for a smaller download. No composition or color edits.
- Original output: `01a0e5fc-d61b-7a00-86e0-f952bede6715/exec-6237e7d5-86ed-4490-ab13-764c7523d08b.png` in the local generated-images directory.
- Decorative photograph; the HTML uses empty alt text. No text or functional interface is baked into the asset.

Exact generation prompt:

> Use case: photorealistic-natural. Asset type: a portrait bakery photograph for the left half of a sign-in page. Create a new professional food photograph of a tall unfrosted vanilla sponge layer cake with three golden sponge layers and thick pale ivory buttercream between them, smooth cream on top. Close view, cake occupies lower 75% of the portrait and extends slightly out of the right edge. Warm pale beige kitchen backdrop, natural soft window light, real fine cake crumb detail, restrained highlights. Keep the upper 20% a plain warm beige background with no objects so a logo can be placed there by the website. No text, logos, symbols, lettering, interface elements, buttons, watermarks, people, fruit, plates, utensils, gradients, or artificial texture overlays. Portrait composition, 2:3 aspect ratio. Clean photographic crop suitable for a cocoa, ivory and orange bakery interface.

## Typography

Poppins regular, medium, semibold, and bold are hosted locally under `public/assets/fonts/`. Downloaded from Google Fonts, with the SIL Open Font License retained as `OFL-Poppins.txt`. The runtime makes no external font requests.

## BakeIT logo

- User-supplied source: `C:/Users/LENOVO/Downloads/bakeit LOGO.png`, supplied on 2026-09-29.
- Shipping file: `public/assets/images/bakeit-logo-chef.png` (2000 × 2000), copied byte-for-byte from the source. The white chef's hat and BakeIT wordmark have an alpha-transparent background.
- Replaces `bakeit-logo-current.png` in every sidebar, mobile header, and sign-in logo placement. The bakery reference's logo is not used.
- CSS frames the visible artwork without changing the file or clipping the hat or lettering. Its nontransparent bounds are x 426–1614 and y 468–1336 on the square source canvas. Desktop navigation uses a 164px-wide frame, mobile headers 88px, the photo overlay 240px, and the mobile sign-in frame 160px.
