import { escapeHtml } from './utils/html.js';
import { getRecipeProgress } from './recipes.js';

export class SessionView {
  constructor(root) { this.root = root; }
  render(items) {
    const snapshot = JSON.stringify(items);
    if (snapshot === this.snapshot) return;
    const expanded = new Set([...this.root.querySelectorAll('details[open]')].map(item => item.dataset.session));
    const focused = this.root.ownerDocument.activeElement?.closest('details')?.dataset.session;
    this.root.innerHTML = items.map(session => {
      const key = JSON.stringify([session.sectionId, session.id ?? session.learnerId ?? session.student]);
      const mappedProgress = getRecipeProgress(session.recipeId ?? session.recipe, session.stepId, session.completed === true);
      const hasStepCount = Number.isInteger(session.current) && Number.isInteger(session.total)
        && session.total > 0 && session.current > 0 && session.current <= session.total;
      const completed = session.completed === true && session.current === session.total;
      const completedSteps = completed ? session.total : session.current - 1;
      const progress = mappedProgress ?? (hasStepCount ? {
        current: session.current, total: session.total, completed, completedSteps,
        percent: Math.round(completedSteps / session.total * 100),
        step: { title: session.step || 'Recipe in progress' }
      } : null);
      const recipe = progress?.recipe;
      const heading = progress
        ? progress.completed ? recipe?.completionLabel ?? 'Recipe complete' : `Step ${progress.current} of ${progress.total} · ${progress.step.title}`
        : 'Waiting for recipe progress';
      return `<article class="card live-card">
        <div class="row-between"><div><h3>${escapeHtml(session.student)}</h3><p class="sub">${escapeHtml(recipe?.displayName ?? session.recipe)} · <span class="section-tag">${escapeHtml(session.section)}</span></p></div><span class="badge session-badge">Live session</span></div>
        <p class="step">${escapeHtml(heading)}</p>
        ${progress ? `<div class="progress" role="progressbar" aria-label="Recipe steps completed" aria-valuemin="0" aria-valuemax="${progress.total}" aria-valuenow="${progress.completedSteps}"><i style="width:${progress.percent}%"></i></div>
        <p class="session-progress-caption"><span>${progress.completedSteps} of ${progress.total} steps completed</span><strong>${progress.percent}%</strong></p>` : ''}
        ${recipe ? `<p class="step-instruction">${escapeHtml(progress.completed ? recipe.scope : progress.step.instruction)}</p>` : ''}
        <p class="sub">Session time ${escapeHtml(session.time ?? '—')}</p>
        ${recipe ? `<details class="recipe-details" data-session="${escapeHtml(key)}" ${expanded.has(key) ? 'open' : ''}>
          <summary>View recipe steps (${progress.total})</summary>
          <p class="recipe-scope">${escapeHtml(recipe.scope)}</p>
          ${recipe.limitation ? `<p class="recipe-note">${escapeHtml(recipe.limitation)}</p>` : ''}
          <ol class="recipe-steps">${recipe.steps.map((step, index) => {
            const current = !progress.completed && index === progress.current - 1;
            const done = index < progress.completedSteps;
            return `<li ${current ? 'aria-current="step"' : ''}><div class="recipe-step-title"><strong>${escapeHtml(step.title)}</strong><span>${current ? 'Current' : done ? 'Done' : 'Upcoming'}</span></div><p>${escapeHtml(step.instruction)}</p></li>`;
          }).join('')}</ol>
        </details>` : ''}
        <ul class="event-list">${(session.events ?? []).map(event => `<li><span>${escapeHtml(event)}</span><span class="session-badge badge">Recorded</span></li>`).join('')}</ul>
      </article>`;
    }).join('') || '<div class="card empty">No active VR sessions in this section.</div>';
    this.snapshot = snapshot;
    if (focused) [...this.root.querySelectorAll('details')].find(item => item.dataset.session === focused)?.querySelector('summary').focus();
  }
}
