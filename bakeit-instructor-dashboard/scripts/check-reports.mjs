import assert from 'node:assert/strict';
import { mkdir, readFile, writeFile } from 'node:fs/promises';
import { join } from 'node:path';
import { workbookParts } from '../test/helpers/xlsx.js';

export async function checkReports({cdp,evaluate,until,size}) {
  const directory = join(process.cwd(),'.preview/review/downloads');
  await mkdir(directory,{recursive:true});
  await cdp('Browser.setDownloadBehavior',{behavior:'allow',downloadPath:directory});
  const originalSection = await evaluate("document.querySelector('[data-section-select]').value");
  const select = async id => {
    await evaluate(`document.querySelector('[data-section-select]').value=${JSON.stringify(id)};document.querySelector('[data-section-select]').dispatchEvent(new Event('change'))`);
    await until("!document.querySelector('.main').hasAttribute('aria-busy')");
  };
  const capture = async name => {
    await evaluate('document.fonts.ready');
    await evaluate('Promise.all(document.getAnimations().filter(animation => animation.effect.getTiming().iterations !== Infinity).map(animation => animation.finished.catch(() => {})))');
    const {data} = await cdp('Page.captureScreenshot',{format:'png',captureBeyondViewport:true});
    await writeFile(join(process.cwd(),`.preview/review/report-${name}.png`),Buffer.from(data,'base64'));
  };
  await select('all');
  assert.equal(await evaluate("document.querySelector('[data-export]').disabled"),true);
  assert.equal(await evaluate("document.querySelectorAll('[data-report-format]').length"),0);
  await evaluate("document.querySelector('[data-export]').click()");
  assert.equal(await evaluate("document.querySelector('[data-report-dialog]').open"),false);
  const id = await evaluate("document.querySelector('[data-section-select] option[value]:not([value=all])').value");
  await select(id);
  await evaluate(`(async()=>{
    window.ReportCloud=(await import('/assets/js/services/cloud-data-service.js')).CloudDataService;
    window.originalReport=ReportCloud.prototype.getSectionReport;window.reportCalls=0;
    ReportCloud.prototype.getSectionReport=function(){reportCalls++;return new Promise((resolve,reject)=>window.failReport=()=>reject(new Error('Report connection interrupted.')));};
    window.originalLinkClick=HTMLAnchorElement.prototype.click;window.reportFiles=[];
    HTMLAnchorElement.prototype.click=function(){if(this.download) reportFiles.push(this.download);return originalLinkClick.call(this);};
    document.querySelector('[data-export]').click();document.querySelector('[data-export]').click();
  })()`);
  assert.equal(await evaluate('reportCalls'),0,'Opening a confirmation must not prepare a report');
  assert.equal(await evaluate("document.querySelector('[data-report-dialog]').matches(':modal')"),true);
  assert.equal(await evaluate("document.activeElement.matches('[data-confirm-report]')"),true);
  const sectionName=await evaluate("document.querySelector('[data-section-select] option:checked').textContent");
  assert.ok((await evaluate("document.querySelector('[data-report-description]').textContent")).includes(sectionName));
  assert.match(await evaluate("document.querySelector('[data-report-description]').textContent"),/Excel \(\.xlsx\)/);
  await evaluate("document.querySelector('[data-cancel-report]').click()");
  await until("!document.querySelector('[data-report-dialog]').open && document.activeElement.matches('[data-export]')");
  assert.equal(await evaluate('reportCalls'),0,'Cancel must not contact the report provider');
  await evaluate("document.querySelector('[data-export]').click();document.querySelector('[data-report-dialog]').requestClose()");
  await until("!document.querySelector('[data-report-dialog]').open && document.activeElement.matches('[data-export]')");
  assert.equal(await evaluate('reportCalls'),0,'Native cancellation must not download');
  await evaluate("document.querySelector('[data-export]').click()");
  await select('all');
  await evaluate("document.querySelector('[data-report-form]').requestSubmit()");
  assert.equal(await evaluate('reportCalls'),0,'A changed section must be rejected before requesting a report');
  assert.match(await evaluate("document.querySelector('[data-report-error]').textContent"),/section.*changed/);
  await evaluate("document.querySelector('[data-cancel-report]').click()");
  await until("!document.querySelector('[data-report-dialog]').open && document.activeElement.matches('[data-section-select]')");
  await select(id);
  await evaluate("document.querySelector('[data-export]').click()");
  await size(1440,1000);
  await capture('confirmation-1440');
  await size(390,844);
  await capture('confirmation-390');
  await evaluate("document.querySelector('[data-report-form]').requestSubmit();document.querySelector('[data-report-form]').requestSubmit()");
  assert.equal(await evaluate('reportCalls'),1);
  assert.equal(await evaluate("document.querySelector('[data-section-select]').disabled"),true);
  assert.equal(await evaluate("document.querySelector('[data-confirm-report]').hasAttribute('data-loading')"),true);
  assert.equal(await evaluate("document.querySelector('[data-cancel-report]').disabled"),true);
  await evaluate("document.querySelector('[data-report-dialog]').requestClose()");
  assert.equal(await evaluate("document.querySelector('[data-report-dialog]').open"),true,'Pending preparation cannot be dismissed');
  await size(390,844);
  assert.equal(await evaluate('document.documentElement.scrollWidth<=innerWidth'),true);
  await capture('preparing-390');
  await evaluate('failReport()');
  await until("!document.querySelector('[data-confirm-report]').disabled");
  assert.equal(await evaluate("document.querySelector('[data-report-dialog]').open"),true);
  assert.match(await evaluate("document.querySelector('[data-report-error]').textContent"),/interrupted/);
  assert.equal(await evaluate("document.querySelector('[data-section-select]').disabled"),false);
  assert.equal(await evaluate('reportFiles.length'),0);
  // Hold a valid response while the selected section changes, as in another tab.
  await evaluate(`ReportCloud.prototype.getSectionReport=async function(id){
    const report=await originalReport.call(this,id);
    return new Promise(resolve=>window.finishChangedReport=()=>resolve(report));
  };document.querySelector('[data-confirm-report]').click()`);
  await until("typeof finishChangedReport==='function'");
  await select('all');
  await evaluate('finishChangedReport()');
  await until("!document.querySelector('[data-confirm-report]').hasAttribute('data-loading')");
  assert.equal(await evaluate('reportFiles.length'),0,'A stale section response must not download');
  assert.match(await evaluate("document.querySelector('[data-report-error]').textContent"),/section changed/);
  await evaluate("document.querySelector('[data-cancel-report]').click()");
  await until("!document.querySelector('[data-report-dialog]').open && document.activeElement.matches('[data-section-select]')");
  await select(id);
  await evaluate("ReportCloud.prototype.getSectionReport=originalReport;document.querySelector('[data-export]').click();document.querySelector('[data-confirm-report]').click()");
  await until("reportFiles.length===1 && !document.querySelector('[data-report-dialog]').open && document.activeElement.matches('[data-export]')");
  assert.match(await evaluate("document.querySelector('[data-report-status]').textContent"),/Excel download started/);
  const readDownload = async filename => {
    for(let attempt=0;attempt<40;attempt++) {
      try { return await readFile(join(directory,filename)); } catch { await new Promise(resolve=>setTimeout(resolve,50)); }
    }
    throw new Error(`Browser did not save ${filename}`);
  };
  const excelName=await evaluate('reportFiles[0]');
  assert.match(excelName,/\.xlsx$/);
  const workbook=workbookParts(await readDownload(excelName));
  assert.match(workbook.get('xl/workbook.xml'),/Scores and completion/);
  assert.match(workbook.get('xl/workbook.xml'),/Safety and waste/);
  assert.match(workbook.get('xl/workbook.xml'),/Procedural accuracy/);
  const raw=workbook.get('xl/worksheets/sheet4.xml');
  const sectionCells=[...raw.matchAll(/<c r="C(\d+)"[^>]*>(.*?)<\/c>/g)].filter(match=>Number(match[1])>6);
  assert.ok(sectionCells.length>0);
  assert.ok(sectionCells.every(match=>match[2].includes(`>${id}</t>`)));
  await capture('ready-390');
  await size(1440,1000);
  await capture('ready-1440');
  const excelTemplate=await evaluate("fetch(document.querySelector('.report-template').href).then(r=>r.arrayBuffer()).then(b=>Array.from(new Uint8Array(b)))");
  assert.match(workbookParts(excelTemplate).get('xl/worksheets/sheet1.xml'),/Blank report template/);
  for (const name of ['section-report-template.csv', 'section-report-example.csv']) {
    assert.equal(await evaluate(`fetch('/assets/reports/${name}').then(response=>response.status)`),404);
  }
  await evaluate('HTMLAnchorElement.prototype.click=originalLinkClick');
  await select(originalSection);
  console.log('Report browser checks passed: Excel confirmation, cancellation/focus, section validation before and after preparation, pending/duplicate prevention, failure/retry, actual workbook download, per-section rows, Excel template, retired CSV removal and mobile layout.');
}
