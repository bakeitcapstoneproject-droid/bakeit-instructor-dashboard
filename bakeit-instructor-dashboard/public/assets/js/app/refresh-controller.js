// Owns refresh scheduling and subscriptions, independently of page rendering.
export class RefreshController {
  constructor({ sections, selector, view, auth, refresh, windowTarget = globalThis.window,
    documentTarget = globalThis.document, timers = globalThis, intervalMs = 15000 }) {
    Object.assign(this, { sections, selector, view, auth, refresh, windowTarget, documentTarget, timers, intervalMs });
    this.listeners = [];
    this.started = false;
    this.pending = null;
    this.queued = false;
    this.generation = 0;
  }

  listen(target, event, callback) {
    target.addEventListener(event, callback);
    this.listeners.push(() => target.removeEventListener(event, callback));
  }

  start() {
    if (this.started) return this.pending ?? Promise.resolve();
    this.started = true;
    this.generation += 1;
    this.view.refresh = () => this.run();
    this.listen(this.documentTarget, 'bakeit:section-change', () => this.run());
    this.listen(this.windowTarget, 'focus', () => this.run());
    this.listen(this.windowTarget, 'online', () => this.run());
    this.listen(this.windowTarget, 'storage', event => {
      if (event.key === 'bakeit_static_workspace_v1' || event.key === null) this.run();
      if ((event.key === 'bakeit_user' || event.key === null) && !this.auth.current()) {
        this.windowTarget.location.href = '/login.html';
      }
    });
    const refreshVisible = () => { if (!this.documentTarget.hidden) this.run(); };
    this.listen(this.documentTarget, 'visibilitychange', refreshVisible);
    this.timer = this.timers.setInterval(refreshVisible, this.intervalMs);
    return this.run();
  }

  run() {
    if (!this.started) return Promise.resolve();
    if (this.pending) { this.queued = true; return this.pending; }
    const generation = this.generation;
    this.pending = this.drain(generation).finally(() => {
      if (generation === this.generation) this.pending = null;
    });
    return this.pending;
  }

  async drain(generation) {
    const active = () => this.started && generation === this.generation;
    do {
      this.queued = false;
      this.view.setBusy(true);
      try {
        await this.sections.load();
        if (!active()) return;
        this.selector.refresh();
        const selected = this.sections.selected();
        await this.refresh();
        if (!active()) return;
        if (selected !== this.sections.selected()) this.queued = true;
        else this.view.loaded(this.sections.name());
      } catch (error) {
        if (active()) this.view.showError(error);
      } finally {
        if (active()) this.view.setBusy(false);
      }
    } while (active() && this.queued);
  }

  stop() {
    this.started = false;
    this.generation += 1;
    this.pending = null;
    this.queued = false;
    this.timers.clearInterval(this.timer);
    this.listeners.splice(0).forEach(remove => remove());
    this.view.refresh = null;
    this.view.setBusy(false);
  }
}
