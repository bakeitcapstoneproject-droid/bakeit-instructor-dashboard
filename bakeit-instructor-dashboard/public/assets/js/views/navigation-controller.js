export class NavigationController {
  constructor({ documentTarget = globalThis.document, windowTarget = globalThis.window } = {}) {
    this.document = documentTarget;
    this.window = windowTarget;
  }
  init() {
    this.dispose();
    const document = this.document;
    const window = this.window;
    const sidebar = document.querySelector('#main-sidebar');
    const toggle = document.querySelector('[data-nav-toggle]');
    if (!sidebar || !toggle) return;
    const mobile = window.matchMedia('(max-width: 800px)');
    const main = document.querySelector('.main');
    const header = document.querySelector('.mobile-header');
    const overlay = document.querySelector('[data-nav-overlay]');
    this.events = new AbortController();
    const options = { signal: this.events.signal };
    const setOpen = open => {
      open = mobile.matches && open;
      sidebar.classList.toggle('is-open', open);
      sidebar.inert = mobile.matches && !open;
      main.inert = header.inert = open;
      overlay.hidden = !open;
      toggle.setAttribute('aria-expanded', String(open));
      document.body.classList.toggle('nav-open', open);
      if (open) {
        sidebar.setAttribute('role', 'dialog');
        sidebar.setAttribute('aria-modal', 'true');
        (sidebar.querySelector('.nav a[aria-current="page"]') || sidebar.querySelector('.nav a')).focus();
      } else {
        sidebar.removeAttribute('role');
        sidebar.removeAttribute('aria-modal');
      }
    };
    const dismiss = () => { setOpen(false); toggle.focus(); };
    toggle.addEventListener('click', () => setOpen(true), options);
    overlay.addEventListener('click', dismiss, options);
    sidebar.addEventListener('keydown', event => {
      if (!sidebar.classList.contains('is-open')) return;
      if (event.key === 'Escape') { event.preventDefault(); dismiss(); }
      if (event.key === 'Tab') {
        const items = [...sidebar.querySelectorAll('a[href], button:not(:disabled)')];
        const first = items[0], last = items.at(-1);
        if (event.shiftKey && document.activeElement === first) { event.preventDefault(); last.focus(); }
        else if (!event.shiftKey && document.activeElement === last) { event.preventDefault(); first.focus(); }
      }
    }, options);
    mobile.addEventListener('change', () => {
      const focusWasInside = sidebar.contains(document.activeElement);
      setOpen(false);
      if (mobile.matches && focusWasInside) toggle.focus();
    }, options);
    setOpen(false);
  }
  dispose() { this.events?.abort(); }
}
