import { createServer } from 'node:http';
import { resolve } from 'node:path';
import { fileURLToPath } from 'node:url';
import { ClassroomService } from './services/classroom-service.js';
import { JsonFileRepository } from './repositories/json-file-repository.js';
import { ClassApiController } from './controllers/class-api-controller.js';
import { StaticFileHandler } from './http/static-file-handler.js';

export class StaticWebsiteServer {
  constructor({ root = fileURLToPath(new URL('../public/', import.meta.url)), port = Number(process.env.PORT) || 3000,
    classes, api, files, dataFile = process.env.BAKEIT_DATA_FILE || fileURLToPath(new URL('../data/classes.json', import.meta.url)) } = {}) {
    this.root = root; this.port = port; this.classes = classes ?? new ClassroomService(new JsonFileRepository(dataFile));
    this.controller = api ?? new ClassApiController(this.classes);
    this.files = files ?? new StaticFileHandler(root);
  }
  async respond(req, res) {
    try {
      const url = new URL(req.url, 'http://localhost');
      if (url.pathname === '/sections.html') {
        res.writeHead(302, { location: '/students.html', 'cache-control': 'no-store' });
        return res.end();
      }
      if (url.pathname.startsWith('/api/')) return await this.controller.handle(req, res, url);
      await this.files.respond(req, res);
    } catch { res.writeHead(404, {'content-type':'text/html; charset=utf-8'}); res.end('<h1>404</h1><p>Page not found.</p>'); }
  }
  start() { return createServer((req,res) => this.respond(req,res)).listen(this.port, () => console.log(`BakeIT running at http://localhost:${this.port}`)); }
}

if (process.argv[1] && resolve(process.argv[1]) === fileURLToPath(import.meta.url)) new StaticWebsiteServer().start();
