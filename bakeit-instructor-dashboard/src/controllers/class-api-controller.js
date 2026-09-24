import { RequestError } from '../http/request-error.js';
import { JsonBodyReader } from '../http/json-body-reader.js';
import { DemoSessionFactory } from '../demo-sessions.js';

export class ClassApiController {
  constructor(classes, { bodyReader = new JsonBodyReader(), sessions = new DemoSessionFactory() } = {}) {
    Object.assign(this, { classes, bodyReader, sessions });
  }
  json(res, status, body) {
    res.writeHead(status, { 'content-type': 'application/json; charset=utf-8', 'cache-control': 'no-store' });
    res.end(JSON.stringify(body));
  }
  async handle(req, res, url) {
    try {
      const route = `${req.method} ${url.pathname}`;
      if (route === 'GET /api/sections') return this.json(res, 200, { sections: await this.classes.listSections() });
      if (route === 'POST /api/sections') return this.json(res, 201, { section: await this.classes.createSection(await this.bodyReader.read(req)) });
      const deleteSection = req.method === 'DELETE' && url.pathname.match(/^\/api\/sections\/([^/]+)$/);
      if (deleteSection) {
        let sectionId;
        try { sectionId = decodeURIComponent(deleteSection[1]); }
        catch { throw new RequestError(400, 'Enter a valid section ID.'); }
        return this.json(res, 200, await this.classes.deleteSection(sectionId));
      }
      if (route === 'POST /api/sections/join') {
        const result = await this.classes.joinSection(await this.bodyReader.read(req));
        return this.json(res, result.alreadyJoined ? 200 : 201, result);
      }
      if (route === 'GET /api/learners') return this.json(res, 200, { students: await this.classes.learners(url.searchParams.get('sectionId') || 'all') });
      if (route === 'GET /api/sessions') {
        const learners = await this.classes.learners(url.searchParams.get('sectionId') || 'all');
        return this.json(res, 200, { sessions: this.sessions.create(learners) });
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
}
