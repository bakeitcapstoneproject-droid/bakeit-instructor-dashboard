export class PageStatusView {
  constructor(root) {
    this.root = root;
    this.lastLoadedSection = null;
    this.refresh = null;
    this.indicator = root.ownerDocument.createElement('p');
    this.indicator.className = 'page-loading';
    this.indicator.setAttribute('role', 'status');
    this.indicator.setAttribute('aria-live', 'polite');
    this.indicator.setAttribute('aria-atomic', 'true');
    this.indicator.hidden = true;
    // Outside the busy region so its loading announcement is not deferred.
    root.after(this.indicator);
  }
  showError(error) {
    const document = this.root.ownerDocument;
    let notice = this.root.querySelector('[data-page-error]');
    if (!notice) {
      notice = document.createElement('div');
      notice.dataset.pageError = '';
      notice.className = 'error page-error';
      notice.setAttribute('role', 'alert');
      const message = document.createElement('span');
      message.dataset.errorMessage = '';
      const retry = document.createElement('button');
      retry.type = 'button';
      retry.className = 'btn';
      retry.dataset.retry = '';
      retry.textContent = 'Try again';
      retry.addEventListener('click', () => this.refresh?.());
      notice.append(message, retry);
      this.root.prepend(notice);
    }
    notice.querySelector('[data-error-message]').textContent = error.message
      + (this.lastLoadedSection ? ` Showing the last loaded data for ${this.lastLoadedSection}; it may be out of date.` : ' Data has not loaded yet.');
    notice.querySelector('[data-retry]').hidden = !this.refresh;
    this.root.querySelectorAll('[data-loading-placeholder]').forEach(element => {
      element.classList.remove('loading-block');
      element.textContent = 'Unable to load data.';
    });
    if (!this.lastLoadedSection) this.root.querySelectorAll('[data-session-count], [data-section-count]').forEach(element => {
      element.textContent = 'Unavailable';
    });
  }

  setBusy(busy) {
    clearTimeout(this.busyTimer);
    if (busy) this.root.setAttribute('aria-busy', 'true');
    else this.root.removeAttribute('aria-busy');
    this.root.toggleAttribute('data-initial-loading', busy && !this.lastLoadedSection);
    if (busy) {
      this.root.querySelectorAll('[data-loading-placeholder]').forEach(element => {
        element.classList.add('loading-block');
        element.textContent = element.dataset.loadingPlaceholder;
      });
      // Fast cached responses never flash a loading notice.
      this.busyTimer = setTimeout(() => {
        this.indicator.textContent = this.lastLoadedSection ? 'Updating data…' : 'Loading page data…';
        this.indicator.hidden = false;
      }, 150);
    } else {
      this.indicator.hidden = true;
      this.indicator.textContent = '';
    }
    const retry = this.root.querySelector('[data-retry]');
    if (retry) { retry.disabled = busy; retry.textContent = busy ? 'Retrying…' : 'Try again'; }
    retry?.toggleAttribute('data-loading', busy);
  }
  loaded(sectionName) {
    this.lastLoadedSection = sectionName;
    const notice = this.root.querySelector('[data-page-error]');
    if (notice?.contains(this.root.ownerDocument.activeElement)) {
      const heading = this.root.querySelector('h1');
      heading?.setAttribute('tabindex', '-1');
      heading?.focus({ preventScroll: true });
    }
    notice?.remove();
  }
}
