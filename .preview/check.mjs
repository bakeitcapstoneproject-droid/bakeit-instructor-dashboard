import { createServer } from 'node:http';
import { spawn } from 'node:child_process';
import { mkdir, writeFile, mkdtemp } from 'node:fs/promises';
import { tmpdir } from 'node:os';
import { join } from 'node:path';
import assert from 'node:assert/strict';
import { StaticWebsiteServer } from '../bakeit-instructor-dashboard/src/server.js';
import { addDemoLearners } from '../bakeit-instructor-dashboard/src/demo.js';

const temporary = await mkdtemp(join(tmpdir(), 'bakeit-layout-'));
const app = new StaticWebsiteServer({ dataFile: join(temporary, 'classes.json') });
const server = createServer((req, res) => app.respond(req, res));
await new Promise(resolve => server.listen(0, '127.0.0.1', resolve));
const origin = `http://127.0.0.1:${server.address().port}`;
const browser = spawn('C:/Program Files/Google/Chrome/Application/chrome.exe', [
  '--headless=new', '--no-first-run', '--no-default-browser-check', '--disable-gpu',
  '--remote-debugging-port=0', `--user-data-dir=${join(temporary, 'chrome')}`, 'about:blank'
], { windowsHide: true, stdio: ['ignore', 'ignore', 'pipe'] });
let socket;
try {
  const endpoint = await new Promise((resolve, reject) => {
    const timeout = setTimeout(() => reject(new Error('Chrome startup timed out')), 15000);
    let output = '';
    browser.stderr.on('data', chunk => {
      output += chunk;
      const match = output.match(/DevTools listening on (ws:\/\/\S+)/);
      if (match) { clearTimeout(timeout); resolve(match[1]); }
    });
    browser.on('error', reject);
    browser.on('exit', code => reject(new Error(`Chrome exited: ${code}`)));
  });
  socket = new WebSocket(endpoint);
  await new Promise((resolve, reject) => { socket.onopen = resolve; socket.onerror = reject; });
  let sequence = 0;
  const pending = new Map();
  const errors = [];
  socket.onmessage = ({ data }) => {
    const message = JSON.parse(data);
    if (message.method === 'Runtime.exceptionThrown') errors.push(message.params.exceptionDetails.exception?.description ?? message.params.exceptionDetails.text);
    if (pending.has(message.id)) {
      const { resolve, reject } = pending.get(message.id);
      pending.delete(message.id);
      if (message.error) reject(new Error(JSON.stringify(message.error)));
      else resolve(message.result);
    }
  };
  const send = (method, params = {}, sessionId) => new Promise((resolve, reject) => {
    const id = ++sequence;
    pending.set(id, { resolve, reject });
    socket.send(JSON.stringify({ id, method, params, sessionId }));
  });
  const { targetId } = await send('Target.createTarget', { url: origin + '/login.html' });
  const { sessionId } = await send('Target.attachToTarget', { targetId, flatten: true });
  const cdp = (method, params) => send(method, params, sessionId);
  await cdp('Runtime.enable');
  await cdp('Page.enable');
  await cdp('Page.bringToFront');
  await cdp('Emulation.setFocusEmulationEnabled', { enabled: true });
  const evaluate = async expression => {
    const result = await cdp('Runtime.evaluate', { expression, awaitPromise: true, returnByValue: true });
    if (result.exceptionDetails) throw new Error(JSON.stringify(result.exceptionDetails));
    return result.result.value;
  };
  const until = async expression => {
    const deadline = Date.now() + 10000;
    while (Date.now() < deadline) {
      try { if (await evaluate(expression)) return; } catch {}
      await new Promise(resolve => setTimeout(resolve, 100));
    }
    throw new Error(`Timed out: ${expression}\n${await evaluate("JSON.stringify({path:location.pathname,text:document.body.innerText})")}`);
  };
  const navigate = async page => {
    await cdp('Page.navigate', { url: origin + '/' + page + '.html' });
    await until(`location.pathname === '/${page}.html' && document.readyState === 'complete' && ${page === 'students' ? "!!document.querySelector('[data-open-create]:not(:disabled)')" : "!!document.querySelector('[data-section-select] option')"}`);
  };
  const screenshot = async filename => {
    const { data } = await cdp('Page.captureScreenshot', { format: 'png', captureBeyondViewport: true });
    await mkdir('.preview', { recursive: true });
    await writeFile(`.preview/${filename}.png`, Buffer.from(data, 'base64'));
  };
  const size = async (width, height) => cdp('Emulation.setDeviceMetricsOverride', { width, height, deviceScaleFactor: 1, mobile: false });
  await size(1440, 1000);
  await until("document.readyState === 'complete' && !!document.querySelector('#login-form')");
  await screenshot('login-desktop');
  await evaluate("document.querySelector('#email').value='instructor@mcl.edu.ph'; document.querySelector('#password').value='demo123'; document.querySelector('#login-form').requestSubmit()");
  await until("location.pathname === '/dashboard.html' && !!document.querySelector('[data-section-select] option')");
  await navigate('students');
  assert.equal(await evaluate("document.querySelector('#learner-records').hidden && !document.querySelector('.page-head [data-open-create]') && !!document.querySelector('.create-section-card')"), true);
  await screenshot('sections-empty');
  await evaluate("document.querySelector('[data-open-create]').click()");
  assert.equal(await evaluate("document.querySelector('dialog').open && document.activeElement.id === 'section-name'"), true);
  await screenshot('create-dialog');
  await evaluate("document.querySelector('#section-name').value='BSHM 2A'; document.querySelector('[data-create-section]').requestSubmit()");
  await until("!document.querySelector('dialog').open && document.querySelectorAll('.class-card').length === 1");
  assert.equal(await evaluate("!document.querySelector('.class-card .class-code') && !document.querySelector('.class-card [data-copy]')"), true);
  const code = (await app.classes.listSections())[0].classCode;
  assert.match(code, /^[A-HJ-NP-Z2-9]{8}$/);
  await evaluate("document.querySelector('[data-open-create]').click(); document.querySelector('#section-name').value='BSHM 2A'; document.querySelector('[data-create-section]').requestSubmit()");
  await until("document.querySelector('[data-create-error]').textContent.includes('already exists')");
  assert.equal(await evaluate("document.querySelector('dialog').open"), true);
  await evaluate("document.querySelector('[data-cancel-create]').click()");
  await evaluate("document.querySelector('[data-open-create]').click(); document.querySelector('#section-name').value='BSHM 2B'; document.querySelector('[data-create-section]').requestSubmit()");
  await until("!document.querySelector('dialog').open && document.querySelectorAll('.class-card').length === 2");
  assert.equal(await evaluate(`!document.body.innerText.includes(${JSON.stringify(code)})`), true);
  await evaluate("document.querySelector('[data-view]').click()");
  await until("!document.querySelector('#learner-records').hidden && !!document.querySelector('[data-detail-code]').textContent");
  assert.equal(await evaluate("document.querySelector('[data-detail-code]').textContent"), code);
  await evaluate("document.querySelector('[data-section-options]').click(); document.querySelector('[data-copy-detail]').click()");
  await until("/copied|Select and copy/.test(document.querySelector('[data-section-message]').textContent)");
  const response = await fetch(origin + '/api/sections/join', { method: 'POST', headers: { 'content-type': 'application/json' }, body: JSON.stringify({ classCode: code, learnerId: 'VR-104', learnerName: 'Ana Santos' }) });
  assert.equal(response.status, 201);
  await navigate('students');
  await screenshot('sections-desktop');
  await evaluate("document.querySelector('[data-view]').click()");
  await until("location.pathname === '/students.html' && document.querySelector('tbody')?.textContent.includes('Ana Santos')");
  assert.equal(await evaluate("document.querySelector('#section-title').textContent"), 'BSHM 2A');
  assert.equal(await evaluate("document.querySelector('.learner-sections').hidden && !document.querySelector('#learner-records').hidden"), true);
  await screenshot('learner-progress-desktop');
  assert.equal(await evaluate("document.querySelector('#section-options').hidden"), true);
  await evaluate("document.querySelector('[data-section-options]').focus(); document.querySelector('[data-section-options]').click()");
  assert.equal(await evaluate("!document.querySelector('#section-options').hidden && document.querySelector('[data-section-options]').getAttribute('aria-expanded') === 'true'"), true);
  await screenshot('section-options-desktop');
  await evaluate("document.querySelector('[data-copy-detail]').focus()");
  assert.equal(await evaluate("document.activeElement.hasAttribute('data-copy-detail')"), true);
  await evaluate("document.activeElement.dispatchEvent(new KeyboardEvent('keydown', { key: 'Escape', bubbles: true }))");
  assert.equal(await evaluate("document.querySelector('#section-options').hidden && document.activeElement.hasAttribute('data-section-options')"), true);
  await size(390, 844);
  assert.equal(await evaluate("document.documentElement.scrollWidth <= innerWidth"), true, 'Section detail: mobile overflow');
  await screenshot('learner-progress-mobile');
  await evaluate("document.querySelector('[data-section-options]').click()");
  assert.equal(await evaluate("document.querySelector('#section-options').getBoundingClientRect().left >= 0 && document.querySelector('#section-options').getBoundingClientRect().right <= innerWidth"), true);
  await screenshot('section-options-mobile');
  await evaluate("document.querySelector('#section-title').click()");
  assert.equal(await evaluate("document.querySelector('#section-options').hidden"), true);
  await evaluate("document.querySelector('[data-section-options]').click(); document.querySelector('#search').focus()");
  assert.equal(await evaluate("document.querySelector('#section-options').hidden"), true);
  await size(1440, 1000);
  await evaluate("history.back()");
  await until("document.querySelector('#learner-records').hidden");
  await evaluate("history.forward()");
  await until("!document.querySelector('#learner-records').hidden && document.querySelector('#section-title').textContent === 'BSHM 2A'");
  await cdp('Page.reload');
  await until("!!document.querySelector('[data-open-create]') && !document.querySelector('#learner-records').hidden && document.querySelector('tbody').textContent.includes('Ana Santos')");
  await evaluate("document.querySelector('#search').value='missing'; document.querySelector('#search').dispatchEvent(new Event('input'))");
  assert.match(await evaluate("document.querySelector('tbody').textContent"), /No learners/);
  await evaluate("document.querySelector('[data-back-sections]').click()");
  await until("document.querySelector('#learner-records').hidden");
  await evaluate("document.querySelectorAll('[data-view]')[1].click()");
  await until("document.querySelector('#section-title').textContent === 'BSHM 2B' && !document.querySelector('#learner-records').hidden");
  assert.equal(await evaluate("!document.querySelector('tbody').textContent.includes('Ana Santos') && document.querySelector('#search').value === ''"), true);
  await evaluate("document.querySelector('[data-section-options]').click(); document.querySelector('[data-copy-detail]').click()");
  await until("/copied|Select and copy/.test(document.querySelector('[data-section-message]').textContent)");
  await addDemoLearners(app.classes);
  await navigate('students');
  await evaluate("document.querySelector('[data-view]').click()");
  await until("document.querySelectorAll('tbody tr').length === 7");
  assert.doesNotMatch(await evaluate("document.querySelector('tbody').innerText"), /sample|demo/i);
  assert.match(await evaluate("document.querySelector('tbody').textContent"), /Needs Practice/);
  await screenshot('demo-learners-desktop');
  await navigate('sessions');
  await until("document.querySelectorAll('.live-card').length === 3");
  assert.match(await evaluate("document.querySelector('[data-session-count]').textContent"), /3 active sessions/);
  assert.equal(await evaluate("[...document.querySelectorAll('.live-card')].every(card => card.textContent.includes('Live session') && card.textContent.includes('Session time'))"), true);
  assert.doesNotMatch(await evaluate("document.body.innerText"), /sample|demo/i);
  assert.deepEqual(await evaluate("[...document.querySelectorAll('.recipe-steps')].map(list => list.children.length)"), [16, 16, 14]);
  assert.equal(await evaluate("[...document.querySelectorAll('.recipe-steps')].every(list => list.querySelectorAll('[aria-current=step]').length === 1)"), true);
  await evaluate("document.querySelector('.recipe-details summary').click(); document.querySelector('.recipe-details summary').focus(); window.dispatchEvent(new Event('focus'))");
  await new Promise(resolve => setTimeout(resolve, 500));
  assert.equal(await evaluate("document.querySelector('.recipe-details').open && document.activeElement.matches('.recipe-details summary')"), true);
  await screenshot('recipe-expanded-desktop');
  await size(390, 844);
  assert.equal(await evaluate("document.documentElement.scrollWidth <= innerWidth"), true, 'Expanded recipe: mobile overflow');
  await evaluate("document.querySelectorAll('.recipe-details')[2].open = true");
  assert.match(await evaluate("document.querySelector('.recipe-note').textContent"), /ends after cooling/);
  await screenshot('recipe-expanded-mobile');
  await size(1440, 1000);
  await evaluate("document.querySelector('[data-section-select]').value='all'; document.querySelector('[data-section-select]').dispatchEvent(new Event('change'))");
  await until("document.querySelectorAll('.live-card').length === 6");
  await screenshot('demo-sessions-desktop');
  await size(390, 844);
  assert.equal(await evaluate("document.documentElement.scrollWidth <= innerWidth"), true, 'Demo sessions: mobile overflow');
  await screenshot('demo-sessions-mobile');
  await navigate('students');
  await evaluate("document.querySelector('[data-view]').click()");
  await until("document.querySelectorAll('tbody tr').length === 7");
  await size(390, 844);
  assert.equal(await evaluate("document.documentElement.scrollWidth <= innerWidth"), true, 'Demo learners: mobile overflow');
  await screenshot('demo-learners-mobile');
  await size(1440, 1000);
  for (const page of ['dashboard', 'students', 'sessions', 'reports']) {
    await navigate(page);
    assert.doesNotMatch(await evaluate("document.body.innerText"), /sample|demo/i);
    if (page === 'reports') {
      await evaluate("document.querySelector('[data-export]').click()");
      assert.doesNotMatch(await evaluate("document.body.innerText"), /sample|demo/i);
    }
    assert.equal(await evaluate("document.querySelector('.nav a[aria-current=page]').getAttribute('href')"), `/${page}.html`);
    assert.equal(await evaluate("document.documentElement.scrollWidth <= innerWidth"), true, `${page}: desktop overflow`);
    if (page === 'dashboard') await screenshot('dashboard-desktop');
    await size(390, 844);
    assert.equal(await evaluate("document.documentElement.scrollWidth <= innerWidth"), true, `${page}: mobile overflow`);
    await until("document.querySelector('.sidebar').inert");
    await evaluate("document.querySelector('[data-nav-toggle]').click()");
    assert.equal(await evaluate("document.querySelector('.sidebar').classList.contains('is-open') && document.querySelector('.main').inert && document.activeElement.getAttribute('aria-current') === 'page'"), true);
    if (page === 'students') await screenshot('sidebar-mobile-open');
    await evaluate("document.activeElement.dispatchEvent(new KeyboardEvent('keydown', {key:'Escape',bubbles:true}))");
    await until("!document.querySelector('.sidebar').classList.contains('is-open')");
    assert.equal(await evaluate("!document.querySelector('.main').inert && document.activeElement.hasAttribute('data-nav-toggle')"), true);
    if (page === 'students') await screenshot('sections-mobile');
    await size(1440, 1000);
    await until("!document.querySelector('.sidebar').inert");
  }
  await navigate('students');
  await screenshot('section-deletion-desktop');
  assert.equal(await evaluate("!document.querySelector('.class-card .delete-section') && !document.querySelector('.class-card .class-code')"), true);
  await evaluate("document.querySelector('[data-view]').click()");
  await until("!document.querySelector('#learner-records').hidden");
  await evaluate("document.querySelector('[data-section-options]').click(); document.querySelector('[data-delete-detail]').click()");
  await until("document.querySelector('[data-delete-dialog]').open");
  assert.equal(await evaluate("document.activeElement.hasAttribute('data-cancel-delete')"), true);
  assert.match(await evaluate("document.querySelector('#delete-section-description').textContent"), /BSHM 2A.*7 learner enrollment records/);
  await screenshot('section-delete-confirm-desktop');
  await evaluate("document.querySelector('[data-cancel-delete]').click()");
  await until("!document.querySelector('[data-delete-dialog]').open && document.activeElement.hasAttribute('data-section-options')");
  assert.equal((await app.classes.listSections()).length, 2);
  await size(390, 844);
  await screenshot('section-deletion-mobile');
  await evaluate("document.querySelector('[data-section-options]').click(); document.querySelector('[data-delete-detail]').click()");
  await screenshot('section-delete-confirm-mobile');
  assert.equal(await evaluate("document.documentElement.scrollWidth <= innerWidth && document.querySelector('[data-delete-dialog]').getBoundingClientRect().right <= innerWidth"), true);
  // A failed request leaves the dialog and stored section intact, then allows retry.
  await evaluate("window.originalFetch = window.fetch; window.fetch = (url, options) => options?.method === 'DELETE' ? Promise.reject(new Error('Offline')) : window.originalFetch(url, options); document.querySelector('[data-confirm-delete]').click()");
  await until("document.querySelector('[data-delete-error]').textContent.includes('Cannot reach') && !document.querySelector('[data-confirm-delete]').disabled");
  assert.equal((await app.classes.listSections()).length, 2);
  await evaluate("window.fetch = window.originalFetch; document.querySelector('[data-confirm-delete]').click()");
  await until("!document.querySelector('[data-delete-dialog]').open && document.querySelectorAll('.class-card').length === 1 && document.querySelector('#learner-records').hidden");
  await until("document.querySelector('[data-section-message]').textContent.includes('BSHM 2A deleted')");
  assert.equal((await app.classes.listSections())[0].name, 'BSHM 2B');
  await cdp('Page.reload');
  await until("document.querySelectorAll('.class-card').length === 1");
  // Deleting from section detail returns to the empty overview.
  await evaluate("document.querySelector('[data-view]').click()");
  await until("!document.querySelector('#learner-records').hidden");
  await evaluate("document.querySelector('[data-section-options]').click(); document.querySelector('[data-delete-detail]').click(); document.querySelector('[data-confirm-delete]').click()");
  await until("!document.querySelector('[data-delete-dialog]').open && document.querySelector('#learner-records').hidden && !location.hash && document.querySelectorAll('.class-card').length === 0");
  await until("document.activeElement.hasAttribute('data-open-create')");
  assert.deepEqual((await app.classes.read()), { sections: [], enrollments: [] });
  await navigate('sessions');
  await until("document.querySelector('.empty')?.textContent.includes('No active')");
  assert.equal(await evaluate("document.querySelector('[data-section-select]').value"), 'all');
  assert.deepEqual(errors, []);
  console.log('Browser checks passed: section creation/deletion, cancel and focus restoration, deletion error/retry, persisted deletion, empty-state filters, enrollment, recipe cards, desktop/mobile layouts, and no JavaScript exceptions.');
} finally {
  socket?.close();
  browser.kill();
  server.closeAllConnections();
  await new Promise(resolve => server.close(resolve));
}
