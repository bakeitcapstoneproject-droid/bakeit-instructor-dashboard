export class PageStatusView {
  constructor(root) { this.root = root; this.lastLoadedSection = null; this.refresh = null; }
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
  }

  setBusy(busy) {
    if (busy) this.root.setAttribute('aria-busy', 'true');
    else this.root.removeAttribute('aria-busy');
    const retry = this.root.querySelector('[data-retry]');
    if (retry) { retry.disabled = busy; retry.textContent = busy ? 'Retrying…' : 'Try again'; }
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
