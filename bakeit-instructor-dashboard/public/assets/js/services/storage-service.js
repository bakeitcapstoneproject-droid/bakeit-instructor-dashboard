export class StorageService {
  constructor(storage) {
    try { this.storage = storage ?? globalThis.localStorage; } catch { this.storage = null; }
  }
  get(key, fallback = null) {
    try { return JSON.parse(this.storage.getItem(key)) ?? fallback; }
    catch { return fallback; }
  }
  set(key, value) {
    try { this.storage.setItem(key, JSON.stringify(value)); }
    catch { throw new Error('Could not save changes. Allow browser storage and check available space, then try again.'); }
  }
  remove(key) {
    try { this.storage.removeItem(key); }
    catch { throw new Error('Could not clear saved data. Allow browser storage, then try again.'); }
  }
}
