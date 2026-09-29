import assert from 'node:assert/strict';
import { mkdir, writeFile } from 'node:fs/promises';

// Browser integration checks with deliberately held responses; no production delays.
export async function checkLoading({cdp,evaluate,until,navigate,size,origin}) {
  const capture = async name => {
    await evaluate('document.fonts.ready');
    await evaluate('Promise.all(document.getAnimations().filter(animation => animation.effect.getTiming().iterations !== Infinity).map(animation => animation.finished.catch(() => {})))');
    const { data } = await cdp('Page.captureScreenshot', {format:'png',captureBeyondViewport:true});
    await mkdir('.preview/review',{recursive:true});
    await writeFile(`.preview/review/loading-${name}.png`,Buffer.from(data,'base64'));
  };
  await until("document.readyState==='complete' && !!document.querySelector('#login-form')");
  await evaluate("document.querySelector('#email').value='instructor@mcl.edu.ph';document.querySelector('#password').value='demo123';document.querySelector('#login-form').requestSubmit()");
  await until("location.pathname==='/dashboard.html' && !!document.querySelector('.main') && !document.querySelector('.main').hasAttribute('aria-busy')");
  const { identifier } = await cdp('Page.addScriptToEvaluateOnNewDocument',{source:`
    const originalFetch=window.fetch.bind(window);
    const gate=new Promise(resolve=>window.releaseLoading=resolve);
    window.fetch=async (...args)=>{if(String(args[0]).startsWith('/api/')) await gate;return originalFetch(...args);};
  `});
  for(const width of [1440,390]) {
    await size(width,width===390?844:1000);
    for(const page of ['dashboard','students','sessions','reports']) {
      await cdp('Page.navigate',{url:origin+'/'+page+'.html'});
      await until("document.readyState==='complete' && document.querySelector('.main')?.getAttribute('aria-busy')==='true' && document.querySelector('.page-loading')?.hidden===false");
      assert.equal(await evaluate('document.documentElement.scrollWidth<=innerWidth'),true);
      assert.equal(await evaluate("document.querySelector('.page-loading').closest('[aria-busy=true]')===null"),true);
      await capture(page+'-'+width);
      await evaluate('releaseLoading()');
      await until("!document.querySelector('.main').hasAttribute('aria-busy') && document.querySelector('.page-loading').hidden");
      assert.equal(await evaluate("document.querySelectorAll('[data-loading-placeholder]').length"),0);
    }
  }
  await cdp('Page.removeScriptToEvaluateOnNewDocument',{identifier});
  await size(1440,1000);
  await navigate('students');
  await evaluate(`window.originalFetch=window.fetch;window.writes=0;window.fetch=async (...args)=>{
    if(args[1]?.method==='POST'){window.writes++;await new Promise(resolve=>window.releaseWrite=resolve);throw new Error('Offline');}
    return originalFetch(...args);
  };document.querySelector('[data-open-create]').click();document.querySelector('#section-name').value='Pending class';document.querySelector('[data-create-section]').requestSubmit();document.querySelector('[data-create-section]').requestSubmit()`);
  assert.equal(await evaluate('window.writes'),1,'Duplicate save must not start a second write');
  assert.equal(await evaluate("document.querySelector('[data-create-section]').getAttribute('aria-busy')"),'true');
  assert.equal(await evaluate("document.querySelector('#section-name').disabled"),true);
  await capture('create-1440');
  await cdp('Emulation.setEmulatedMedia',{features:[{name:'prefers-reduced-motion',value:'reduce'}]});
  assert.equal(await evaluate("getComputedStyle(document.querySelector('button[data-loading]'),'::before').animationName"),'none');
  await cdp('Emulation.setEmulatedMedia',{features:[]});
  await evaluate('releaseWrite()');
  await until("!document.querySelector('[data-create-section]').hasAttribute('aria-busy')");
  assert.equal(await evaluate("document.querySelector('#section-name').value"),'Pending class');
  assert.equal(await evaluate("document.querySelector('#section-name').disabled"),false);
  assert.equal(await evaluate("document.querySelectorAll('button[data-loading]').length"),0);
  await evaluate("window.fetch=originalFetch;document.querySelector('[data-cancel-create]').click()");
  await navigate('dashboard');
  const original=await evaluate("document.querySelector('[data-metric=enrolled]').textContent");
  await evaluate(`window.originalFetch=window.fetch;const gate=new Promise(resolve=>window.releaseRefresh=resolve);window.fetch=async (...args)=>{await gate;throw new Error('Offline');};window.dispatchEvent(new Event('focus'))`);
  await until("document.querySelector('.page-loading').hidden===false");
  assert.equal(await evaluate("document.querySelector('[data-metric=enrolled]').textContent"),original);
  assert.equal(await evaluate("document.querySelector('.main').hasAttribute('data-initial-loading')"),false);
  await capture('refresh-1440');
  await evaluate('releaseRefresh()');
  await until("!!document.querySelector('[data-retry]') && !document.querySelector('.main').hasAttribute('aria-busy')");
  assert.equal(await evaluate("document.querySelector('.page-loading').hidden"),true);
  await evaluate("window.fetch=originalFetch;document.querySelector('[data-retry]').click()");
  await until("!document.querySelector('[data-page-error]') && !document.querySelector('.main').hasAttribute('aria-busy')");
  await evaluate("document.querySelector('[data-logout]').click()");
  await until("location.pathname==='/login.html' && document.readyState==='complete'");
  // Hold injected auth methods to cover future async providers as well as local auth.
  await evaluate(`(async()=>{window.Auth=(await import('/assets/js/services/auth-service.js')).AuthService;
    window.originalLogin=Auth.prototype.login;window.loginCalls=0;
    Auth.prototype.login=function(){window.loginCalls++;return new Promise((resolve,reject)=>window.failLogin=()=>reject(new Error('Sign-in unavailable')));};
    document.querySelector('#email').value='instructor@mcl.edu.ph';document.querySelector('#password').value='demo123';document.querySelector('#login-form').requestSubmit();document.querySelector('#login-form').requestSubmit()})()`);
  assert.equal(await evaluate('loginCalls'),1);
  await capture('signin-1440');
  await evaluate('failLogin()');
  await until("!document.querySelector('[data-auth-view=login]').hasAttribute('aria-busy')");
  assert.equal(await evaluate("document.querySelector('#email').disabled"),false);
  await evaluate(`Auth.prototype.login=originalLogin;document.querySelector('[data-forgot]').click();window.originalReset=Auth.prototype.requestPasswordReset;Auth.prototype.requestPasswordReset=function(email){return new Promise(resolve=>window.releaseReset=()=>resolve(originalReset.call(this,email)));};document.querySelector('#request-reset-form').requestSubmit()`);
  assert.equal(await evaluate("document.querySelector('[data-auth-view=recovery]').getAttribute('aria-busy')"),'true');
  await size(390,844);
  await capture('recovery-390');
  await evaluate('releaseReset()');
  await until("!document.querySelector('[data-reset-step=confirm]').hidden");
  assert.equal(await evaluate("document.activeElement.id"),'reset-code');
  console.log('Loading checks passed: 8 slow page loads, refresh retention/failure/retry, duplicate save and sign-in prevention, form recovery, async password recovery focus, mobile layout and reduced motion.');
}
