import { createServer } from 'node:http';
import { readFile, stat } from 'node:fs/promises';
import { extname, join, normalize, resolve } from 'node:path';
import { fileURLToPath } from 'node:url';
import { ClassStore, RequestError, readJson } from './classes.js';
import { demoSessions } from './demo-sessions.js';

export class StaticWebsiteServer {
  constructor({ root = fileURLToPath(new URL('../public/', import.meta.url)), port = Number(process.env.PORT) || 3000,
    dataFile = process.env.BAKEIT_DATA_FILE || fileURLToPath(new URL('../data/classes.json', import.meta.url)) } = {}) {
    this.root = root; this.port = port; this.classes = new ClassStore(dataFile);
  }
  json(res, status, body) {
    res.writeHead(status, { 'content-type': 'application/json; charset=utf-8', 'cache-control': 'no-store' });
    res.end(JSON.stringify(body));
  }
  async api(req, res, url) {
    try {
      const route = `${req.method} ${url.pathname}`;
      if (route === 'GET /api/sections') return this.json(res, 200, { sections: await this.classes.listSections() });
      if (route === 'POST /api/sections') return this.json(res, 201, { section: await this.classes.createSection(await readJson(req)) });
      const deleteSection = req.method === 'DELETE' && url.pathname.match(/^\/api\/sections\/([^/]+)$/);
      if (deleteSection) {
        let sectionId;
        try { sectionId = decodeURIComponent(deleteSection[1]); }
        catch { throw new RequestError(400, 'Enter a valid section ID.'); }
        return this.json(res, 200, await this.classes.deleteSection(sectionId));
      }
      if (route === 'POST /api/sections/join') {
        const result = await this.classes.joinSection(await readJson(req));
        return this.json(res, result.alreadyJoined ? 200 : 201, result);
      }
      if (route === 'GET /api/learners') return this.json(res, 200, { students: await this.classes.learners(url.searchParams.get('sectionId') || 'all') });
      if (route === 'GET /api/sessions') {
        const learners = await this.classes.learners(url.searchParams.get('sectionId') || 'all');
        return this.json(res, 200, { sessions: demoSessions(learners) });
      }
      if (route === 'GET /api/activities') {
        const learners = await this.classes.learners(url.searchParams.get('sectionId') || 'all');
        return this.json(res, 200, { activities: learners.sort((a, b) => b.joinedAt.localeCompare(a.joinedAt)).slice(0, 5)
          .map(item => ({ text: `${item.name} joined ${item.section}`, time: item.joinedAt })) });
      }
      throw new RequestError(404, 'Endpoint not found.');
    } catch (error) {
      if (!error.status) console.error('Class API error:', error);
      this.json(res, error.status || 500, { error: error.status ? error.message : 'Could not save or load class data. Please try again.' });
    }
  }
  mime(path) { return ({'.html':'text/html; charset=utf-8','.css':'text/css; charset=utf-8','.js':'text/javascript; charset=utf-8','.json':'application/json; charset=utf-8','.svg':'image/svg+xml','.png':'image/png'})[extname(path)] || 'application/octet-stream'; }
  resolve(url) {
    const pathname = decodeURIComponent(new URL(url, 'http://localhost').pathname);
    const route = pathname === '/' ? '/login.html' : pathname;
    const safe = normalize(route).replace(/^(\.\.(\/|\\|$))+/, '');
    return join(this.root, safe);
  }
  async respond(req, res) {
    try {
      const url = new URL(req.url, 'http://localhost');
      if (url.pathname === '/sections.html') {
        res.writeHead(302, { location: '/students.html', 'cache-control': 'no-store' });
        return res.end();
      }
      if (url.pathname.startsWith('/api/')) return await this.api(req, res, url);
      let file = this.resolve(req.url);
      if ((await stat(file)).isDirectory()) file = join(file, 'index.html');
      const body = await readFile(file);
      res.writeHead(200, {'content-type': this.mime(file), 'cache-control':'no-cache'}); res.end(body);
    } catch { res.writeHead(404, {'content-type':'text/html; charset=utf-8'}); res.end('<h1>404</h1><p>Page not found.</p>'); }
  }
  start() { return createServer((req,res) => this.respond(req,res)).listen(this.port, () => console.log(`BakeIT running at http://localhost:${this.port}`)); }
}

if (process.argv[1] && resolve(process.argv[1]) === fileURLToPath(import.meta.url)) new StaticWebsiteServer().start();
