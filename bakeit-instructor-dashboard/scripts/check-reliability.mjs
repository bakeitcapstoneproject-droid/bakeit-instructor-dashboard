import assert from 'node:assert/strict';

// Called by .preview/check.mjs against disposable servers, browser profiles and data.
export async function checkReliability({ staticMode, cdp, evaluate, until, navigate, size, screenshot, origin, server, setFailure }) {
  const settled = () => until("!document.querySelector('.main')?.hasAttribute('aria-busy')");
  await settled();
  if (!staticMode) {
    // Every page must finish wiring controls even if the initial API request fails.
    for (const page of ['dashboard', 'students', 'sessions', 'reports']) {
      setFailure('unavailable');
      await navigate(page);
      await until("!!document.querySelector('[data-retry]')");
      assert.match(await evaluate("document.querySelector('[data-page-error]').textContent"), /unavailable/);
      setFailure('');
      await evaluate("document.querySelector('[data-retry]').click()");
      await until("!document.querySelector('[data-page-error]') && !document.querySelector('.main').hasAttribute('aria-busy')");
      if (page === 'students') assert.equal(await evaluate("!!document.querySelector('[data-open-create]')"), true);
    }
    await navigate('dashboard');
    const before = await evaluate("document.querySelector('[data-metric=enrolled]').textContent");
    // Actually stop the listening server after loading the page, then restart it.
    const port = server.address().port;
    server.closeAllConnections();
    await new Promise(resolve => server.close(resolve));
    await evaluate("window.dispatchEvent(new Event('focus'))");
    await until("!!document.querySelector('[data-page-error]')");
    assert.match(await evaluate("document.querySelector('[data-page-error]').textContent"), /Cannot reach.*last loaded/);
    assert.equal(await evaluate("document.querySelector('[data-metric=enrolled]').textContent"), before);
    await screenshot('server-offline-desktop');
    await new Promise(resolve => server.listen(port, '127.0.0.1', resolve));
    await evaluate("window.dispatchEvent(new Event('online'))");
    await until("!document.querySelector('[data-page-error]') && !document.querySelector('.main').hasAttribute('aria-busy')");
    setFailure('malformed');
    await evaluate("window.dispatchEvent(new Event('focus'))");
    await until("document.querySelector('[data-page-error]')?.textContent.includes('unreadable')");
    setFailure('');
    await evaluate("document.querySelector('[data-retry]').click()");
    await until("!document.querySelector('[data-page-error]') && !document.querySelector('.main').hasAttribute('aria-busy')");
  } else {
    const key = 'bakeit_static_workspace_v1';
    const saved = await evaluate(`localStorage.getItem('${key}')`);
    await evaluate(`localStorage.setItem('${key}', '{broken')`);
    await navigate('students');
    await until("!!document.querySelector('[data-retry]')");
    assert.match(await evaluate("document.querySelector('[data-page-error]').textContent"), /could not be read/);
    assert.equal(await evaluate(`localStorage.getItem('${key}')`), '{broken');
    await evaluate(`localStorage.setItem('${key}', ${JSON.stringify(saved)}); document.querySelector('[data-retry]').click()`);
    await until("document.querySelectorAll('.class-card').length === 2 && !document.querySelector('[data-page-error]')");
  }

  await navigate('students');
  if (staticMode) {
    // Install after navigation: a new document has its own Storage prototype.
    await evaluate("window.originalSetItem = Storage.prototype.setItem; Storage.prototype.setItem = function(key, value) { if (key === 'bakeit_static_workspace_v1') throw new DOMException('Full', 'QuotaExceededError'); return originalSetItem.call(this, key, value); }");
  }
  await evaluate("document.querySelector('[data-open-create]').click(); document.querySelector('#section-name').value='  '; document.querySelector('[data-create-section]').requestSubmit()");
  assert.match(await evaluate("document.querySelector('[data-create-error]').textContent"), /Enter a section name/);
  const name = '長い名前 <script>alert(1)</script> ' + 'X'.repeat(45);
  await evaluate(`document.querySelector('#section-name').value=${JSON.stringify(name)}`);
  if (!staticMode) setFailure('unavailable');
  await evaluate("document.querySelector('[data-create-section]').requestSubmit(); document.querySelector('[data-create-section]').requestSubmit()");
  await until("!!document.querySelector('[data-create-error]').textContent && !document.querySelector('[data-create-section] [type=submit]').disabled");
  assert.equal(await evaluate("document.querySelector('#section-name').value"), name);
  assert.equal(await evaluate("document.querySelector('[data-create-dialog]').open"), true);
  if (!staticMode) {
    setFailure('hang');
    await evaluate("document.querySelector('[data-create-section]').requestSubmit()");
    assert.equal(await evaluate("document.querySelector('[data-create-section] [type=submit]').disabled"), true);
    await until("document.querySelector('[data-create-error]').textContent.includes('too long') && !document.querySelector('[data-create-section] [type=submit]').disabled");
    assert.equal(await evaluate("document.querySelector('#section-name').value"), name);
  }
  if (staticMode) await evaluate('Storage.prototype.setItem = window.originalSetItem');
  else setFailure('');
  await evaluate("document.querySelector('[data-create-section]').requestSubmit(); document.querySelector('[data-create-section]').requestSubmit()");
  await until("!document.querySelector('[data-create-dialog]').open");
  assert.equal(await evaluate(`Array.from(document.querySelectorAll('.class-card h3')).filter(el => el.textContent === ${JSON.stringify(name)}).length`), 1);
  assert.equal(await evaluate("!!document.querySelector('.class-card script')"), false);
  await size(320, 740);
  assert.equal(await evaluate('document.documentElement.scrollWidth <= innerWidth'), true);
  await evaluate("document.querySelectorAll('[data-view]')[document.querySelectorAll('[data-view]').length - 1].click()");
  await until("!document.querySelector('#learner-records').hidden");
  assert.equal(await evaluate('document.documentElement.scrollWidth <= innerWidth'), true);
  await evaluate("document.querySelector('[data-section-options]').click(); document.querySelector('[data-delete-detail]').click(); document.querySelector('[data-confirm-delete]').click()");
  await until("!document.querySelector('[data-delete-dialog]').open && document.querySelector('#learner-records').hidden");
  await settled();

  // Reduced motion and rapid interruption must keep the drawer and keyboard state coherent.
  await cdp('Emulation.setEmulatedMedia', { features: [{ name: 'prefers-reduced-motion', value: 'reduce' }] });
  await evaluate("document.querySelector('[data-nav-toggle]').click()");
  assert.equal(await evaluate("getComputedStyle(document.querySelector('.sidebar')).transitionDuration"), '0s');
  await evaluate("document.querySelector('[data-logout]').focus(); document.activeElement.dispatchEvent(new KeyboardEvent('keydown', { key: 'Tab', bubbles: true }))");
  assert.equal(await evaluate("document.activeElement === document.querySelector('.sidebar a')"), true);
  for (let i = 0; i < 3; i++) {
    await evaluate("document.querySelector('[data-nav-overlay]').click(); document.querySelector('[data-nav-toggle]').click()");
  }
  await evaluate("document.activeElement.dispatchEvent(new KeyboardEvent('keydown', {key:'Escape',bubbles:true}))");
  assert.equal(await evaluate("!document.querySelector('.main').inert && document.activeElement.hasAttribute('data-nav-toggle')"), true);
  await cdp('Emulation.setEmulatedMedia', { features: [] });
  await size(1440, 1000);

  // Exercise recovery and lockout through the actual login forms.
  await evaluate("document.querySelector('[data-logout]').click()");
  await until("location.pathname === '/login.html' && !!document.querySelector('#login-form') && document.readyState === 'complete'");
  await evaluate("document.querySelector('#email').value='instructor@mcl.edu.ph'; document.querySelector('#password').value='wrong'; for(let i=0;i<3;i++) document.querySelector('#login-form').requestSubmit()");
  assert.equal(await evaluate("document.querySelector('#login-form [type=submit]').disabled"), true);
  await cdp('Page.reload');
  await until("!!document.querySelector('#login-form [type=submit]')?.disabled");
  await evaluate("document.querySelector('[data-forgot]').click(); document.querySelector('#request-reset-form').resetEmail.value='instructor@mcl.edu.ph'; document.querySelector('#request-reset-form').requestSubmit()");
  await until("!document.querySelector('[data-reset-step=confirm]').hidden");
  await evaluate("const form=document.querySelector('#confirm-reset-form'); form.resetCode.value='123456'; form.newPassword.value='reliable-pass'; form.confirmPassword.value='different-pass'; form.requestSubmit()");
  assert.match(await evaluate("document.querySelector('[data-confirm-error]').textContent"), /do not match/);
  await evaluate("document.querySelector('#confirm-reset-form').confirmPassword.value='reliable-pass'; document.querySelector('#confirm-reset-form').requestSubmit()");
  await until("!document.querySelector('[data-reset-step=success]').hidden");
  assert.equal(await evaluate("document.activeElement.hasAttribute('data-return-login')"), true);
  await evaluate("document.querySelector('[data-return-login]').click(); document.querySelector('#password').value='reliable-pass'; document.querySelector('[data-toggle-password]').click()");
  assert.equal(await evaluate("document.querySelector('#password').type"), 'text');
  await evaluate("document.querySelector('[data-toggle-password]').click(); document.querySelector('#login-form').requestSubmit()");
  await until("location.pathname === '/dashboard.html' && !!document.querySelector('[data-section-select] option')");
  await settled();
  // Restore the documented demo password for the remaining original feature checks.
  await evaluate("localStorage.removeItem('bakeit_demo_credentials')");
  console.log(`Reliability browser scenarios passed (${staticMode ? 'static storage' : 'server shutdown/restart'}): initial-load recovery, failed-save retry, duplicate submit, long/escaped names, 320px layout, keyboard drawer, reduced motion, login lockout and password reset.`);
}
