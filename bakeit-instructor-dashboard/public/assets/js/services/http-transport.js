// Fetch is injectable; the default resolves it at call time for browser instrumentation.
export class HttpTransport {
  constructor({ fetchImpl = (...args) => globalThis.fetch(...args), timeoutMs = 10000 } = {}) {
    this.fetch = fetchImpl;
    this.timeoutMs = timeoutMs;
  }
  async request(path, options = {}) {
    const { timeoutMs = this.timeoutMs, signal, ...requestOptions } = options;
    const controller = new AbortController();
    const abort = () => controller.abort();
    if (signal?.aborted) abort();
    else signal?.addEventListener('abort', abort, { once: true });
    let timedOut = false;
    const timer = setTimeout(() => { timedOut = true; abort(); }, timeoutMs);
    try {
      let response;
      try {
        response = await this.fetch(path, { ...requestOptions, cache: 'no-store', signal: controller.signal,
          headers: { 'Content-Type': 'application/json', ...options.headers } });
      } catch (error) {
        if (controller.signal.aborted) throw error;
        throw new Error('Cannot reach the server. Check your connection and try again.');
      }
      let body;
      try { body = await response.json(); }
      catch (error) {
        if (controller.signal.aborted) throw error;
        throw new Error(response.ok ? 'The server returned unreadable data. Please try again.' : 'The server is unavailable. Please try again shortly.');
      }
      if (!response.ok) throw Object.assign(new Error(typeof body?.error === 'string' ? body.error : 'Unable to load class data. Please try again.'), { status: response.status });
      if (!body || typeof body !== 'object' || Array.isArray(body)) throw new Error('The server returned invalid data. Please try again.');
      return body;
    } catch (error) {
      if (timedOut) throw new Error('The server took too long to respond. Try again. If you were saving changes, refresh first to check whether they were saved.');
      if (signal?.aborted) throw new Error('The request was cancelled. Please try again.');
      throw error;
    } finally {
      clearTimeout(timer);
      signal?.removeEventListener('abort', abort);
    }
  }
}
