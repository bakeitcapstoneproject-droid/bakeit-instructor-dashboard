import { createServer } from 'node:http';
import { readFile, stat } from 'node:fs/promises';
import { extname, join, normalize } from 'node:path';
import { fileURLToPath } from 'node:url';

class StaticWebsiteServer {
  constructor({ root = fileURLToPath(new URL('../public/', import.meta.url)), port = Number(process.env.PORT) || 3000 } = {}) { this.root = root; this.port = port; }
  mime(path) { return ({'.html':'text/html; charset=utf-8','.css':'text/css; charset=utf-8','.js':'text/javascript; charset=utf-8','.json':'application/json; charset=utf-8','.svg':'image/svg+xml'})[extname(path)] || 'application/octet-stream'; }
  resolve(url) {
    const pathname = decodeURIComponent(new URL(url, 'http://localhost').pathname);
    const route = pathname === '/' ? '/login.html' : pathname;
    const safe = normalize(route).replace(/^(\.\.(\/|\\|$))+/, '');
    return join(this.root, safe);
  }
  async respond(req, res) {
    try {
      let file = this.resolve(req.url);
      if ((await stat(file)).isDirectory()) file = join(file, 'index.html');
      const body = await readFile(file);
      res.writeHead(200, {'content-type': this.mime(file), 'cache-control':'no-cache'}); res.end(body);
    } catch { res.writeHead(404, {'content-type':'text/html; charset=utf-8'}); res.end('<h1>404</h1><p>Page not found.</p>'); }
  }
  start() { createServer((req,res) => this.respond(req,res)).listen(this.port, () => console.log(`BakeIT running at http://localhost:${this.port}`)); }
}

new StaticWebsiteServer().start();
