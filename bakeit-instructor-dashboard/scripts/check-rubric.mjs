import assert from 'node:assert/strict';
export async function checkRubric({ app, staticMode, evaluate, until, navigate, size, screenshot, cdp }) {
  if (!staticMode && !process.argv.includes('--running-server')) {
    const section = await app.classes.createSection({name:'VR progress'});
    await app.classes.joinSection({classCode:section.classCode,learnerId:'vr-1',learnerName:'Ana Santos'});
    await app.classes.mutate(data => { data.enrollments[0].assessment = {ratings:{decorum:1,kitchen_organization:2,safety_sanitation:3,baking_skills:4,product_appraisal:5}}; });
  }
  await size(1440,1000);
  await until("document.readyState==='complete' && !!document.querySelector('#login-form')");
  await evaluate("document.querySelector('#email').value='instructor@mcl.edu.ph';document.querySelector('#password').value='demo123';document.querySelector('#login-form').requestSubmit()");
  await until("location.pathname==='/dashboard.html' && !!document.querySelector('[data-section-select] option')");
  if (staticMode && process.argv.includes('--legacy-scores')) {
    await until("document.querySelectorAll('tbody tr').length>1");
    await evaluate(`(() => {
      const key='bakeit_static_workspace_v1';
      const data=JSON.parse(localStorage.getItem(key));
      for (const student of data.students) {
        delete student.assessment; delete student.demo; student.score=88;
      }
      localStorage.setItem(key,JSON.stringify(data));
    })()`);
    await cdp('Page.reload');
    await until("document.readyState==='complete' && document.querySelectorAll('tbody tr').length>1");
    assert.equal(await evaluate("[...document.querySelectorAll('tbody tr')].every(row=>/\\d+ \\/ 25/.test(row.cells[4].textContent))"),true);
    assert.equal(await evaluate("JSON.parse(localStorage.getItem('bakeit_static_workspace_v1')).students.every(s=>s.assessment?.ratings && s.demo===true)"),true);
  }
  const tag=staticMode?'static':'server';
  for (const page of ['dashboard','students']) {
    await navigate(page);
    if (page==='students') {
      await evaluate("document.querySelector('[data-view]').click()");
      await until("!document.querySelector('#learner-records').hidden && document.querySelectorAll('tbody td').length>=6");
    }
    assert.equal(await evaluate("document.querySelectorAll('thead th').length"),page==='students'?7:6);
    assert.equal(await evaluate("document.querySelectorAll('[data-grade], [data-rubric-editor], [name=decorum]').length"),0);
    assert.doesNotMatch(await evaluate("document.querySelector('thead').textContent"), /Decorum|Grading/);
    assert.match(await evaluate("document.querySelector('tbody').textContent"), /\d+ \/ 25/);
    assert.equal(await evaluate(`Array.from(document.querySelectorAll('tbody tr')).every(row => {
      const total=Number.parseInt(row.cells[4].textContent,10);
      return row.cells[5].textContent.trim()===(Number.isNaN(total)?'Awaiting assessment':total>=15?'Passed':'Failed');
    })`),true);
    if(page==='students') {
      for(const status of ['Passed','Failed','Awaiting assessment']) {
        await evaluate(`document.querySelector('#status').value='${status}';document.querySelector('#status').dispatchEvent(new Event('input'))`);
        assert.equal(await evaluate(`Array.from(document.querySelectorAll('tbody tr')).every(row=>row.cells.length===1 || row.cells[5].textContent.trim()==='${status}')`),true);
      }
      await evaluate("document.querySelector('#status').value='';document.querySelector('#status').dispatchEvent(new Event('input'))");
      assert.doesNotMatch(await evaluate("document.querySelector('tbody').textContent"), /DEMO-|\bDemo\b/);
      if (staticMode || process.argv.includes('--running-server')) {
        assert.equal(await evaluate("[...document.querySelectorAll('.learner-id')].every(element=>/^\\d{10}$/.test(element.textContent))"),true);
        assert.equal(await evaluate("new Set([...document.querySelectorAll('.learner-id')].map(element=>element.textContent)).size===document.querySelectorAll('.learner-id').length"),true);
        const learnerId = await evaluate("document.querySelector('.learner-id').textContent");
        await evaluate(`document.querySelector('#search').value=${JSON.stringify(learnerId)};document.querySelector('#search').dispatchEvent(new Event('input'))`);
        assert.equal(await evaluate("document.querySelectorAll('#learner-records tbody tr').length"),1);
        assert.equal(await evaluate("document.querySelector('.learner-id').textContent"),learnerId);
        await evaluate("document.querySelector('#search').value='';document.querySelector('#search').dispatchEvent(new Event('input'))");
      }
      await evaluate("document.querySelector('[data-history]').click()");
      await until("document.querySelector('[data-history-dialog]').open && !document.querySelector('[data-history-content]').hasAttribute('aria-busy')");
      assert.equal(await evaluate("document.querySelector('.history-error').hidden"),true);
      if (staticMode || process.argv.includes('--running-server')) {
        assert.equal(await evaluate("document.querySelectorAll('.history-table tbody tr').length === Number(document.querySelector('#learner-records tbody tr').cells[3].textContent)"),true);
        assert.match(await evaluate("document.querySelector('.history-table time').textContent"), /2026.*10:00/);
        for (const width of [1440,390]) {
          await size(width,width===390?844:1000);
          assert.equal(await evaluate("document.querySelector('[data-history-dialog]').getBoundingClientRect().right<=innerWidth"),true);
          await screenshot('learner-history-'+tag+'-'+width);
        }
      } else assert.match(await evaluate("document.querySelector('[data-history-content]').textContent"),/No sessions/);
      await evaluate("document.querySelector('[data-close-history]').dispatchEvent(new KeyboardEvent('keydown',{key:'Escape',bubbles:true,cancelable:true}))");
      await until("!document.querySelector('[data-history-dialog]').open");
      await until("document.activeElement.hasAttribute('data-history')");
      const empty = await evaluate("[...document.querySelectorAll('#learner-records tbody tr')].find(row=>row.cells[3]?.textContent.trim()==='0')?.querySelector('[data-history]')?.dataset.history ?? null");
      if (empty) {
        await evaluate(`document.querySelector('[data-history="${empty}"]').click()`);
        await until("!document.querySelector('[data-history-content]').hasAttribute('aria-busy')");
        assert.match(await evaluate("document.querySelector('[data-history-content]').textContent"),/No sessions recorded yet/);
        await evaluate("document.querySelector('[data-close-history]').click()");
        await until("!document.querySelector('[data-history-dialog]').open");
      }
      await evaluate("(async()=>{const {CloudDataService}=await import('/assets/js/services/cloud-data-service.js');window.savedHistory=CloudDataService.prototype.getLearnerHistory;CloudDataService.prototype.getLearnerHistory=async()=>{throw new Error('History temporarily unavailable')};document.querySelector('[data-history]').click()})()");
      await until("!document.querySelector('.history-error').hidden");
      assert.match(await evaluate("document.querySelector('[data-history-error]').textContent"),/temporarily unavailable/);
      await evaluate("(async()=>{const {CloudDataService}=await import('/assets/js/services/cloud-data-service.js');CloudDataService.prototype.getLearnerHistory=window.savedHistory;document.querySelector('[data-retry-history]').click()})()");
      await until("!document.querySelector('[data-history-content]').hasAttribute('aria-busy') && document.querySelector('.history-error').hidden");
      await evaluate("document.querySelector('[data-close-history]').click()");
      await until("!document.querySelector('[data-history-dialog]').open");
    }
    for (const width of [1440,390]) {
      await size(width,width===390?844:1000);
      assert.equal(await evaluate('document.documentElement.scrollWidth<=innerWidth'),true);
      await screenshot('monitoring-'+page+'-'+tag+'-'+width);
    }
  }
  const before=await evaluate("document.querySelector('tbody').textContent");
  await cdp('Page.reload');
  await until("!!document.querySelector('tbody td') && !document.querySelector('#learner-records').hidden");
  assert.equal(await evaluate("document.querySelector('tbody').textContent"),before);
  const report=await evaluate("(async()=>{const {CloudDataService}=await import('/assets/js/services/cloud-data-service.js');const id=new URLSearchParams(location.hash.slice(1)).get('section');return new CloudDataService().getSectionReport(id)})()");
  assert.ok(report.rows.some(row=>row.total_score!==null));
  for(const key of ['decorum','kitchen_organization','safety_sanitation','baking_skills','product_appraisal']) {
    assert.ok(report.rows.every(row=>Object.hasOwn(row,key+'_rating')));
  }
  await navigate('reports');
  for(const width of [1440,390]) {
    await size(width,width===390?844:1000);
    assert.equal(await evaluate('document.documentElement.scrollWidth<=innerWidth'),true);
    await screenshot('monitoring-reports-'+tag+'-'+width);
  }
  console.log('Read-only totals, preserved criterion reports, reload and desktop/mobile layouts passed: '+tag);
}
