import { dataMode } from '../runtime-config.js';
import { HttpTransport } from './http-transport.js';

export class StaticTransport {
  constructor({ storage = () => globalThis.localStorage, load = () => import('../static-data.js') } = {}) {
    this.storage = storage;
    this.load = load;
    this.transport = null;
  }
  async request(path, options = {}) {
    this.transport ??= this.load().then(({ StaticDataService }) => new StaticDataService(this.storage()))
      .catch(() => { this.transport = null; throw new Error('Could not open saved data. Allow browser storage and try again.'); });
    return (await this.transport).request(path, options);
  }
}

export class ApiClient {
  constructor(transport = dataMode === 'static' ? new StaticTransport() : new HttpTransport()) {
    this.transport = transport;
  }
  request(path, options = {}) { return this.transport.request(path, options); }
}

export const apiClient = new ApiClient();
export const apiRequest = (path, options) => apiClient.request(path, options);
