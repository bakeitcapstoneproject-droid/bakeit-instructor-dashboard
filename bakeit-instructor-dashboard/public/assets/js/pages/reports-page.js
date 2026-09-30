import { PageController } from '../app/page-controller.js';
import { CloudDataService } from '../services/cloud-data-service.js';
import { sectionReportWorkbook, sectionWorkbookFilename, workbookMime } from '../domain/section-workbook.js';
import { beginBusy } from '../views/busy-action.js';
import { downloadFile } from '../views/file-download.js';

export class ReportsPage extends PageController {
  constructor({ shell, cloud = new CloudDataService(), download = downloadFile } = {}) {
    super({ shell });
    this.cloud = cloud;
    this.download = download;
  }
  setup() {
    this.button = document.querySelector('[data-export]');
    this.status = document.querySelector('[data-report-status]');
    this.selector = document.querySelector('[data-section-select]');
    this.dialog = document.querySelector('[data-report-dialog]');
    this.form = document.querySelector('[data-report-form]');
    this.confirm = document.querySelector('[data-confirm-report]');
    this.description = document.querySelector('[data-report-description]');
    this.error = document.querySelector('[data-report-error]');
    this.button.addEventListener('click', () => this.open());
    this.form.addEventListener('submit', event => { event.preventDefault(); this.prepare(); });
    document.querySelector('[data-cancel-report]').addEventListener('click', () => {
      if (!this.preparing) this.dialog.close();
    });
    this.dialog.addEventListener('cancel', event => { if (this.preparing) event.preventDefault(); });
    this.dialog.addEventListener('close', () => {
      if (this.dialog.open) return;
      this.reportSectionId = null;
      (this.button.disabled ? this.selector : this.button).focus();
    });
    this.selector.addEventListener('change', () => { this.clearStatus(); this.updateSelection(); });
    this.updateSelection();
  }
  refresh() { this.updateSelection(); }
  clearStatus() {
    clearTimeout(this.statusTimer);
    this.status.textContent = '';
  }
  selectedSection() {
    return this.shell.sections.sections.find(item => item.id === this.shell.sections.selected() && item.id !== 'all');
  }
  updateSelection() {
    this.button.disabled = Boolean(this.preparing || !this.selectedSection());
  }
  open() {
    if (this.preparing || this.dialog.open) return;
    const section = this.selectedSection();
    if (!section) {
      this.status.textContent = 'Choose a class section before preparing a report.';
      this.selector.focus();
      return;
    }
    this.reportSectionId = section.id;
    this.description.textContent = `Download an Excel (.xlsx) report for ${section.name}?`;
    this.clearStatus();
    this.error.textContent = '';
    this.dialog.showModal();
    this.confirm.focus();
  }
  async prepare() {
    if (this.preparing || !this.dialog.open) return;
    this.error.textContent = '';
    const sectionId = this.reportSectionId;
    if (!this.selectedSection() || sectionId !== this.shell.sections.selected()) {
      this.error.textContent = 'The selected section is no longer available or has changed. Cancel and choose a class section again.';
      this.error.focus();
      return;
    }
    this.preparing = true;
    this.clearStatus();
    const finish = beginBusy(this.confirm, 'Preparing Excel…', this.form);
    const selectorDisabled = this.selector.disabled;
    this.selector.disabled = true;
    this.updateSelection();
    let succeeded = false;
    try {
      const report = await this.cloud.getSectionReport(sectionId);
      // A section may be removed/changed in another tab during the request.
      if (!this.selectedSection() || sectionId !== this.shell.sections.selected()) throw new Error('The selected section changed. Cancel and choose a section again.');
      this.download(sectionReportWorkbook(report), sectionWorkbookFilename(report), workbookMime);
      this.status.textContent = report.rows.length
        ? `Excel download started for ${report.section.name}: ${report.rows.length} learner${report.rows.length === 1 ? '' : 's'}.`
        : `Excel template downloaded for ${report.section.name}. This section has no enrolled learners yet.`;
      this.statusTimer = setTimeout(() => this.clearStatus(), 2000);
      succeeded = true;
    } catch (error) {
      this.error.textContent = `${error.message} You can prepare the report again.`;
    } finally {
      finish();
      this.selector.disabled = selectorDisabled;
      this.preparing = false;
      this.updateSelection();
      if (succeeded) this.dialog.close();
      else this.error.focus();
    }
  }
}
