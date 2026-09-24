import test from 'node:test';
import assert from 'node:assert/strict';
import { ApiClient, StaticTransport } from '../public/assets/js/services/api-client.js';
import { HttpTransport } from '../public/assets/js/services/http-transport.js';
import { StaticDataService } from '../public/assets/js/static-data.js';
import { RecipeCatalog } from '../public/assets/js/domain/recipe-catalog.js';
import { RefreshController } from '../public/assets/js/app/refresh-controller.js';
import { PageController } from '../public/assets/js/app/page-controller.js';
import { ClassroomService } from '../src/services/classroom-service.js';
import { DemoLearnerSeeder } from '../src/demo.js';

function browserStorage() {
  const data = new Map();
  return { getItem: key => data.get(key) ?? null, setItem: (key, value) => data.set(key, value) };
}

class MemoryRepository {
  constructor() { this.data = { sections: [], enrollments: [] }; }
  async read() { return structuredClone(this.data); }
  snapshot() { return this.read(); }
  async mutate(change) {
    const next = await this.read();
    const result = change(next);
    this.data = next;
    return result;
  }
}

test('classroom rules and demo seeding run against an injected repository without file I/O', async () => {
  const repository = new MemoryRepository();
  const service = new ClassroomService(repository, () => 'ABCD2345');
  const section = await service.createSection({ name: '  Test class  ' });
  assert.equal(section.name, 'Test class');
  await service.joinSection({ classCode: section.classCode, learnerId: 'real-1', learnerName: 'Ana' });
  const seeder = new DemoLearnerSeeder(service);
  assert.equal(await seeder.add(), 6);
  assert.equal(await seeder.add(), 0);
  assert.equal((await service.listSections())[0].learnerCount, 7);
  assert.equal(await seeder.remove(), 6);
  assert.equal((await service.learners())[0].id, 'real-1');
  assert.equal((await service.deleteSection(section.id)).removedEnrollments, 1);
  assert.deepEqual(await repository.read(), { sections: [], enrollments: [] });
});

test('API clients use interchangeable transports and keep instances isolated', async () => {
  const first = new ApiClient(new StaticDataService(browserStorage(), null));
  const second = new ApiClient(new StaticDataService(browserStorage(), null));
  await first.request('/api/sections', { method: 'POST', body: JSON.stringify({ name: 'Only first' }) });
  assert.equal((await first.request('/api/sections')).sections.length, 3);
  assert.equal((await second.request('/api/sections')).sections.length, 2);
  const http = new ApiClient(new HttpTransport({ fetchImpl: async (path, options) => {
    assert.equal(path, '/api/sections');
    assert.equal(options.cache, 'no-store');
    assert.ok(options.signal instanceof AbortSignal);
    return new Response(JSON.stringify({ sections: [] }));
  } }));
  assert.deepEqual(await http.request('/api/sections'), { sections: [] });
});

test('static transport retries initialization after a loader failure', async () => {
  let calls = 0;
  const transport = new StaticTransport({ storage: browserStorage, load: async () => {
    if (++calls === 1) throw new Error('Unavailable');
    return { StaticDataService };
  } });
  await assert.rejects(transport.request('/api/sections'), /Could not open saved data/);
  assert.equal((await transport.request('/api/sections')).sections.length, 2);
  await transport.request('/api/sections');
  assert.equal(calls, 2);
});

test('recipe catalog instances do not depend on the global recipe definitions', () => {
  const recipe = { id: 'test', name: 'Test recipe', steps: [{ id: 'one' }, { id: 'two' }] };
  const catalog = new RecipeCatalog([recipe]);
  assert.equal(catalog.get('Test recipe'), recipe);
  assert.equal(catalog.progress('test', 'two').percent, 50);
  assert.equal(catalog.progress('test', 'two', true).percent, 100);
  assert.equal(new RecipeCatalog([]).progress('test', 'two'), null);
});

function refreshFixture(overrides = {}) {
  const events = [], intervals = new Map();
  const windowTarget = new EventTarget(), documentTarget = new EventTarget();
  documentTarget.hidden = false;
  windowTarget.location = {};
  let nextInterval = 0;
  const view = {
    setBusy: busy => events.push(['busy', busy]),
    loaded: name => events.push(['loaded', name]),
    showError: error => events.push(['error', error.message])
  };
  const options = {
    sections: { load: async () => {}, selected: () => 'all', name: () => 'All Sections' },
    selector: { refresh() {} }, view, auth: { current: () => ({ role: 'Instructor' }) },
    refresh: async () => events.push(['render']), windowTarget, documentTarget,
    timers: { setInterval: callback => { intervals.set(++nextInterval, callback); return nextInterval; }, clearInterval: id => intervals.delete(id) },
    ...overrides
  };
  return { controller: new RefreshController(options), ...options, events, intervals };
}

test('refresh failures recover on reconnect; disposal removes timers and subscriptions', async () => {
  const fixture = refreshFixture();
  let offline = true;
  fixture.sections.load = async () => { if (offline) throw new Error('Offline'); };
  await fixture.controller.start();
  assert.ok(fixture.events.some(event => event[0] === 'error'));
  offline = false;
  fixture.windowTarget.dispatchEvent(new Event('online'));
  await fixture.controller.pending;
  assert.ok(fixture.events.some(event => event[0] === 'loaded'));
  fixture.controller.stop();
  assert.equal(fixture.intervals.size, 0);
  assert.equal(fixture.view.refresh, null);
  const count = fixture.events.length;
  fixture.windowTarget.dispatchEvent(new Event('focus'));
  fixture.documentTarget.dispatchEvent(new Event('bakeit:section-change'));
  await Promise.resolve();
  assert.equal(fixture.events.length, count);
});

test('refresh controller coalesces overlapping events without concurrent rendering', async () => {
  let release, count = 0;
  const gate = new Promise(resolve => { release = resolve; });
  const fixture = refreshFixture({ refresh: async () => { if (++count === 1) await gate; } });
  const started = fixture.controller.start();
  await Promise.resolve();
  fixture.windowTarget.dispatchEvent(new Event('focus'));
  fixture.windowTarget.dispatchEvent(new Event('online'));
  assert.equal(count, 1);
  release();
  await started;
  assert.equal(count, 2);
  fixture.controller.stop();
});

test('stopping and restarting while loading does not let an obsolete refresh render', async () => {
  let release, loads = 0;
  const gate = new Promise(resolve => { release = resolve; });
  const fixture = refreshFixture();
  fixture.sections.load = async () => { if (++loads === 1) await gate; };
  const old = fixture.controller.start();
  fixture.controller.stop();
  await fixture.controller.start();
  release();
  await old;
  assert.equal(fixture.events.filter(event => event[0] === 'render').length, 1);
  fixture.controller.stop();
});

test('hidden pages skip scheduled refreshes and refresh when visible again', async () => {
  const fixture = refreshFixture();
  await fixture.controller.start();
  fixture.documentTarget.hidden = true;
  fixture.intervals.values().next().value();
  assert.equal(fixture.events.filter(event => event[0] === 'render').length, 1);
  fixture.documentTarget.hidden = false;
  fixture.documentTarget.dispatchEvent(new Event('visibilitychange'));
  await fixture.controller.pending;
  assert.equal(fixture.events.filter(event => event[0] === 'render').length, 2);
  fixture.controller.stop();
});

test('protected page lifecycle initializes controls before starting recoverable loading', async () => {
  const calls = [];
  const shell = { init: async () => calls.push('shell'), watch: async refresh => { calls.push('watch'); await refresh(); }, dispose: () => calls.push('dispose') };
  class ExamplePage extends PageController {
    setup() { calls.push('controls'); }
    async refresh() { calls.push('data'); }
  }
  const page = new ExamplePage({ shell });
  assert.equal(await page.init(), page);
  page.dispose();
  assert.deepEqual(calls, ['shell', 'controls', 'watch', 'data', 'dispose']);
});
