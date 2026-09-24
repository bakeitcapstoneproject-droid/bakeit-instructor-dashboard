import { readFile, stat } from 'node:fs/promises';
import { extname, join, relative, isAbsolute, resolve } from 'node:path';
import { RequestError } from './request-error.js';

export class StaticFileHandler {
  constructor(root) { this.root = root; }
  mime(path) { return ({'.html':'text/html; charset=utf-8','.css':'text/css; charset=utf-8','.js':'text/javascript; charset=utf-8','.json':'application/json; charset=utf-8','.svg':'image/svg+xml','.png':'image/png'})[extname(path)] || 'application/octet-stream'; }
  resolve(url) {
    const pathname = decodeURIComponent(new URL(url, 'http://localhost').pathname);
    const route = pathname === '/' ? '/login.html' : pathname;
    const file = resolve(this.root, '.' + route);
    const withinRoot = relative(resolve(this.root), file);
    if (withinRoot === '..' || withinRoot.startsWith('..\\') || withinRoot.startsWith('../') || isAbsolute(withinRoot)) {
      throw new RequestError(404, 'Page not found.');
    }
    return file;
  }
  async respond(req, res) {
    let file = this.resolve(req.url);
    if ((await stat(file)).isDirectory()) file = join(file, 'index.html');
    const body = await readFile(file);
    res.writeHead(200, { 'content-type': this.mime(file), 'cache-control': 'no-cache' });
    res.end(body);
  }
}
