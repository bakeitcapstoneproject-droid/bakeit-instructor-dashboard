import test from 'node:test';
import assert from 'node:assert/strict';
import { createServer, request as httpRequest } from 'node:http';
import { mkdtemp, writeFile, readFile, unlink, rmdir } from 'node:fs/promises';
import { tmpdir } from 'node:os';
import { join } from 'node:path';
import { apiRequest, AuthService, StorageService, SectionService, CloudDataService } from '../public/assets/js/core.js';
import { StaticWebsiteServer } from '../src/server.js';
import { createStaticRequest, staticStorageKey } from '../public/assets/js/static-data.js';

function storage() {
  const values = new Map();
  return { getItem: key => values.get(key) ?? null, setItem: (key, value) => values.set(key, value), removeItem: key => values.delete(key) };
}
async function listen(t, handler) {
  const server = createServer(handler);
  await new Promise(resolve => server.listen(0, '127.0.0.1', resolve));
  t.after(() => { server.closeAllConnections(); return new Promise(resolve => server.close(resolve)); });
  return { server, url: `http://127.0.0.1:${server.address().port}` };
}

test('network failures, HTTP errors, invalid JSON and retry give useful outcomes', async t => {
  let mode = 'offline';
  const { url } = await listen(t, (req, res) => {
    if (mode === 'offline') return req.socket.destroy();
    if (mode === 'html') { res.writeHead(503); return res.end('<h1>Unavailable</h1>'); }
    if (mode === 'invalid') return res.end('broken json');
    if (mode === 'null') return res.end('null');
    if (mode === 'denied') { res.writeHead(403); return res.end(JSON.stringify({ error: 'Access denied.' })); }
    res.end(JSON.stringify({ sections: [] }));
  });
  await assert.rejects(apiRequest(url), /Cannot reach/);
  mode = 'html'; await assert.rejects(apiRequest(url), /unavailable/);
  mode = 'invalid'; await assert.rejects(apiRequest(url), /unreadable/);
  mode = 'null'; await assert.rejects(apiRequest(url), /invalid/);
  mode = 'denied'; await assert.rejects(apiRequest(url), error => error.status === 403 && error.message === 'Access denied.');
  mode = 'ok'; assert.deepEqual(await apiRequest(url), { sections: [] });
});

test('timeouts bound both connection waits and stalled response bodies; cancellation works', async t => {
  const { url } = await listen(t, (req, res) => {
    if (req.url === '/body') { res.writeHead(200, { 'content-type': 'application/json' }); res.write('{'); }
  });
  for (const path of ['/headers', '/body']) await assert.rejects(apiRequest(url + path, { timeoutMs: 60 }), /too long/);
  const controller = new AbortController();
  const pending = apiRequest(url, { signal: controller.signal });
  controller.abort();
  await assert.rejects(pending, /cancelled/);
});

test('older section requests cannot overwrite newer results and invalid data keeps the last snapshot', async () => {
  const pending = [];
  const service = new SectionService(new StorageService(storage()), () => new Promise(resolve => pending.push(resolve)));
  const old = service.load(), latest = service.load();
  pending[1]({ sections: [{ id: 'new', name: 'New' }] }); await latest;
  pending[0]({ sections: [{ id: 'old', name: 'Old' }] }); await old;
  assert.equal(service.sections[1].id, 'new');
  service.request = async () => ({ sections: [null] });
  await assert.rejects(service.load(), /invalid class data/);
  assert.equal(service.sections[1].id, 'new');
  const cloud = new CloudDataService({ request: async () => ({ students: null }) });
  await assert.rejects(cloud.getStudents(), /invalid data/);
  cloud.request = async () => ({ students: [{ id: '1', sectionId: 'new' }] });
  await assert.rejects(cloud.getStudents(), /invalid data/);
  cloud.request = async () => ({ sessions: [{ student: 'Ana', sectionId: 'new', events: {} }] });
  await assert.rejects(cloud.getLiveSessions(), /invalid data/);
  cloud.request = async () => ({ activities: [{ text: 'Joined', time: 'not-a-date' }] });
  await assert.rejects(cloud.getActivities(), /invalid data/);
});

test('blocked preference storage does not misreport a successful class creation', async () => {
  const store = new StorageService({ getItem: () => null, setItem: () => { throw new Error('QuotaExceeded'); } });
  const service = new SectionService(store, async () => ({ section: { id: 'saved', name: 'Saved' } }));
  await service.create('Saved');
  assert.equal(service.selected(), 'saved');
  assert.throws(() => new AuthService(store).login('instructor@mcl.edu.ph', 'demo123'), /Allow browser storage/);
});

test('corrupt login state does not bypass lockout or create a session; password reset clears lockout', () => {
  const store = new StorageService(storage());
  const auth = new AuthService(store);
  store.set('bakeit_login_attempts', 'corrupt');
  store.set('bakeit_user', {});
  assert.equal(auth.current(), null);
  for (let i = 0; i < 3; i++) assert.throws(() => auth.login('instructor@mcl.edu.ph', 'wrong'), /Invalid Password/);
  assert.ok(auth.loginCooldown() > 0);
  auth.requestPasswordReset('instructor@mcl.edu.ph');
  auth.resetPassword('123456', 'new-password', 'new-password');
  assert.equal(auth.loginCooldown(), 0);
  assert.equal(auth.login('instructor@mcl.edu.ph', 'new-password').role, 'Instructor');
});

test('structurally corrupt browser data is preserved and retry works after repair', async () => {
  const saved = storage();
  const request = createStaticRequest(saved, null);
  await request('/api/sections');
  const original = saved.getItem(staticStorageKey);
  for (const corrupt of [
    { sections: [null], students: [], sessions: [] },
    { sections: [], students: [{ id: '1', name: 'Ana', sectionId: 'missing' }], sessions: [] }
  ]) {
    const value = JSON.stringify(corrupt);
    saved.setItem(staticStorageKey, value);
    await assert.rejects(request('/api/sections'), /could not be read/);
    assert.equal(saved.getItem(staticStorageKey), value);
  }
  saved.setItem(staticStorageKey, original);
  assert.equal((await request('/api/sections')).sections.length, 2);
});

test('server contains encoded paths, rejects oversized input, and recovers after a damaged data file', async t => {
  const directory = await mkdtemp(join(tmpdir(), 'bakeit-resilience-'));
  const dataFile = join(directory, 'classes.json');
  const app = new StaticWebsiteServer({ dataFile });
  const { url } = await listen(t, (req, res) => app.respond(req, res));
  t.after(async () => { await unlink(dataFile); await rmdir(directory); });
  await app.classes.createSection({ name: 'Keep me' });
  const original = await readFile(dataFile, 'utf8');
  for (const path of ['/..%5cpackage.json', '/%2e%2e%5csrc%5cserver.js', '/%ZZ']) {
    assert.equal((await fetch(url + path)).status, 404);
  }
  const response = await new Promise((resolve, reject) => {
    const req = httpRequest(url + '/api/sections', { method: 'POST', headers: { 'content-type': 'application/json' } }, res => {
      let body = ''; res.on('data', chunk => { body += chunk; });
      res.on('end', () => resolve({ status: res.statusCode, body }));
    });
    req.on('error', reject);
    req.end(JSON.stringify({ name: 'x'.repeat(10000) }));
  });
  assert.equal(response.status, 413);
  assert.match(response.body, /too large/);
  assert.equal(await readFile(dataFile, 'utf8'), original);
  await writeFile(dataFile, '{broken', 'utf8');
  assert.equal((await fetch(url + '/api/sections')).status, 500);
  await writeFile(dataFile, original, 'utf8');
  assert.equal((await apiRequest(url + '/api/sections')).sections[0].name, 'Keep me');
});
