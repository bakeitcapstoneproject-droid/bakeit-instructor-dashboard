import { RequestError } from './request-error.js';

export class JsonBodyReader {
  constructor(limit = 8192) { this.limit = limit; }
  async read(req) {
    if (req.headers['content-type']?.split(';')[0].trim().toLowerCase() !== 'application/json') {
      throw new RequestError(415, 'Send a JSON request with Content-Type: application/json.');
    }
    const chunks = [];
    let size = 0;
    for await (const chunk of req) {
      size += chunk.length;
      if (size > this.limit) throw new RequestError(413, 'Request body is too large.');
      chunks.push(chunk);
    }
    try {
      const value = JSON.parse(Buffer.concat(chunks).toString('utf8'));
      if (!value || typeof value !== 'object' || Array.isArray(value)) throw new Error();
      return value;
    } catch { throw new RequestError(400, 'Send a valid JSON object.'); }
  }
}

const reader = new JsonBodyReader();
export const readJson = req => reader.read(req);
