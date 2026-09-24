export class SectionSelector {
  constructor(root, sections) { this.root = root; this.sections = sections; }
  mount() {
    if (!this.root) return;
    this.refresh();
    this.root.addEventListener('change', () => {
      const sectionId = this.sections.select(this.root.value);
      this.updateLabels();
      document.dispatchEvent(new CustomEvent('bakeit:section-change', {
        detail: { sectionId, sectionName: this.sections.name(sectionId) }
      }));
    });
  }
  refresh() {
    if (!this.root) return;
    const signature = JSON.stringify(this.sections.sections.map(({ id, name }) => [id, name]));
    if (signature !== this.signature) {
      this.root.replaceChildren(...this.sections.sections.map(section => new Option(section.name, section.id)));
      this.signature = signature;
    }
    this.root.value = this.sections.selected();
    this.updateLabels();
  }
  updateLabels() {
    const name = this.sections.name();
    document.querySelectorAll('[data-section-label]').forEach(element => { element.textContent = name; });
  }
}
