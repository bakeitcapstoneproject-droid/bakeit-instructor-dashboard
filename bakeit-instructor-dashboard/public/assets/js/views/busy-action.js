// Keep each action's original labels and disabled state, including on failure.
export function beginBusy(button, label, scope = button) {
  const controls = [...new Set([button, ...scope.querySelectorAll('button, input, select')])];
  const disabled = controls.map(control => control.disabled);
  const nodes = [...button.childNodes];
  const originalBusy = scope.getAttribute('aria-busy');
  button.textContent = label;
  button.setAttribute('data-loading', '');
  scope.setAttribute('aria-busy', 'true');
  controls.forEach(control => { control.disabled = true; });
  let finished = false;
  return () => {
    if (finished) return;
    finished = true;
    button.replaceChildren(...nodes);
    button.removeAttribute('data-loading');
    if (originalBusy === null) scope.removeAttribute('aria-busy');
    else scope.setAttribute('aria-busy', originalBusy);
    controls.forEach((control, index) => { control.disabled = disabled[index]; });
  };
}
