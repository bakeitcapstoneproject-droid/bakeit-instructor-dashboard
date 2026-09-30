import { validateSectionReport, sectionReportFilename } from './section-report.js';
import { rubricCriteria } from './performance.js';
import { learnerDisplayId } from './learner-identity.js';
import { xlsxPackage } from './xlsx-package.js';

export const workbookMime = 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet';
const spreadsheetNs = 'http://schemas.openxmlformats.org/spreadsheetml/2006/main';
const relationshipNs = 'http://schemas.openxmlformats.org/officeDocument/2006/relationships';
const declaration = '<?xml version="1.0" encoding="UTF-8" standalone="yes"?>';
const column = (key, label, width, kind = 'text') => ({ key, label, width, kind });
const identity = [column('learner_name', 'Learner name', 27), column('learner_id', 'Learner ID', 20),
  column('recipe', 'Latest recipe', 22), column('assessed_at', 'Assessed at (PHT)', 26, 'date')];
const sheets = [
  { name: 'Class performance', color: 'FFAE38', style: 6, columns: [...identity,
    ...rubricCriteria.map(({ key, label }) => column(key + '_rating', (key === 'decorum' ? 'Decorum (waste)' : label) + ' / 5', 24, 'number')),
    column('total_score', 'Total / 25', 14, 'number'), column('session_count', 'Sessions', 12, 'number'),
    column('score_remark', 'Status', 22)] },
  { name: 'Session history', color: 'FFAE38', style: 6, history: true, columns: [
    column('learner_name', 'Learner name', 27), column('learner_id', 'Learner ID', 20),
    column('recipe', 'Recipe', 22), column('session_status', 'Session status', 20),
    column('started_at', 'Started (PHT)', 26, 'date'), column('ended_at', 'Ended (PHT)', 26, 'date'),
    column('total_score', 'Total / 25', 14, 'number'), column('score_remark', 'Result', 22)
  ] }
];
function xml(value) {
  return String(value).replace(/[^\u0009\u000A\u000D\u0020-\uD7FF\uE000-\uFFFD\u{10000}-\u{10FFFF}]/gu, '')
    .replaceAll('&', '&amp;').replaceAll('<', '&lt;').replaceAll('>', '&gt;').replaceAll('"', '&quot;');
}
function letters(index) {
  let name = '';
  for (index++; index; index = Math.floor((index - 1) / 26)) name = String.fromCharCode(65 + (index - 1) % 26) + name;
  return name;
}
function cell(reference, value, style) {
  if (value == null) return `<c r="${reference}" s="${style}"/>`;
  if (typeof value === 'number') return `<c r="${reference}" s="${style}"><v>${value}</v></c>`;
  if (String(value).length > 32767) throw new Error('A report field exceeds Excel’s cell limit. Shorten that value and try again.');
  // Explicit string cells preserve IDs/leading zeroes and cannot execute formulas.
  const text = xml(String(value).replace(/_x[0-9a-f]{4}_/gi, match => '_x005F_' + match.slice(1)));
  return `<c r="${reference}" s="${style}" t="inlineStr"><is><t xml:space="preserve">${text}</t></is></c>`;
}
function styles() {
  const font = (size, bold = false) => `<font><sz val="${size}"/><color rgb="FF000000"/><name val="Calibri"/><family val="2"/>${bold ? '<b/>' : ''}</font>`;
  const fill = color => `<fill><patternFill patternType="solid"><fgColor rgb="FF${color}"/><bgColor indexed="64"/></patternFill></fill>`;
  const xf = (fontId, fillId, borderId = 0, numFmtId = 0, alignment = 'left') => `<xf numFmtId="${numFmtId}" fontId="${fontId}" fillId="${fillId}" borderId="${borderId}" xfId="0" applyFont="1" applyFill="1" applyBorder="1" applyNumberFormat="1" applyAlignment="1"><alignment horizontal="${alignment}" vertical="center" wrapText="1" indent="1"/></xf>`;
  const formats = [xf(0, 0), xf(1, 2), xf(2, 3), xf(3, 4), xf(3, 0),
    xf(4, 2), xf(2, 3), xf(2, 5), xf(4, 6),
    xf(0, 0, 1), xf(0, 4, 1), xf(0, 0, 1, 0, 'right'), xf(0, 4, 1, 0, 'right'),
    xf(0, 0, 1, 165, 'right'), xf(0, 4, 1, 165, 'right'),
    xf(0, 0, 1, 0, 'center'), xf(0, 4, 1, 0, 'center'),
    xf(0, 0, 1, 166), xf(0, 4, 1, 166)];
  return declaration + `<styleSheet xmlns="${spreadsheetNs}">
    <numFmts count="2"><numFmt numFmtId="165" formatCode="0.00%"/><numFmt numFmtId="166" formatCode="mmm d, yyyy h:mm AM/PM"/></numFmts>
    <fonts count="5">${font(11)}${font(18, true)}${font(11, true)}${font(10)}${font(11, true)}</fonts>
    <fills count="7"><fill><patternFill patternType="none"/></fill><fill><patternFill patternType="gray125"/></fill>${fill('FFF1D4')}${fill('FFAE38')}${fill('FFF7E4')}${fill('EAF2EB')}${fill('FFF1D4')}</fills>
    <borders count="2"><border><left/><right/><top/><bottom/><diagonal/></border><border><left/><right/><top/><bottom style="hair"><color rgb="FFE3DCCB"/></bottom><diagonal/></border></borders>
    <cellStyleXfs count="1"><xf numFmtId="0" fontId="0" fillId="0" borderId="0"/></cellStyleXfs>
    <cellXfs count="${formats.length}">${formats.join('')}</cellXfs>
    <cellStyles count="1"><cellStyle name="Normal" xfId="0" builtinId="0"/></cellStyles>
  </styleSheet>`;
}
function worksheet(report, sheet, template, historyRows) {
  const end = letters(sheet.columns.length - 1), rows = [];
  const heading = (row, value, style, height) => rows.push(`<row r="${row}" ht="${height}" customHeight="1">${sheet.columns.map((_, index) => cell(`${letters(index)}${row}`, index ? null : value, style)).join('')}</row>`);
  heading(1, `BakeIT | ${sheet.name}`, 1, 38);
  heading(2, template ? 'Class: ____________________' : `${report.section.name}  |  ${report.rows.length} learner${report.rows.length === 1 ? '' : 's'}`, 2, 28);
  heading(3, template ? 'Blank report template' : `Generated: ${new Date(report.generatedAt).toISOString().replace('T', ' ').replace('.000Z', ' UTC')}`, 3, 24);
  heading(4, sheet.name === 'Class performance' ? '5 Excellent · 4 Good · 3 Satisfactory · 2 Needs improvement · 1 Poor' : '', 4, sheet.name === 'Class performance' ? 24 : 8);
  rows.push('<row r="5" ht="10" customHeight="1"/>');
  rows.push(`<row r="6" ht="36" customHeight="1">${sheet.columns.map((col, index) => cell(`${letters(index)}6`, col.label, index < 4 ? 5 : sheet.style)).join('')}</row>`);
  const data = template ? Array.from({ length: 12 }, () => ({})) : sheet.history ? historyRows : report.rows;
  data.forEach((record, index) => {
    const row = index + 7;
    let height = 28;
    const values = sheet.columns.map((col, colIndex) => {
      let value = record[col.key];
      if (col.key === 'learner_id' && value != null) value = learnerDisplayId({ id: value, demo: record.data_source === 'demo' });
      if (col.key === 'score_remark' && !template) value ??= 'Awaiting assessment';
      const numeric = typeof value === 'number';
      const percent = col.kind === 'percent';
      if (percent && numeric) value /= 100;
      if (col.kind === 'date' && value) value = (Date.parse(value) + 8 * 60 * 60 * 1000) / 86400000 + 25569;
      const lines = String(value ?? '').split('\n').reduce((count, line) => count + Math.max(1, Math.ceil(line.length / (col.width - 3))), 0);
      height = Math.min(409, Math.max(height, lines * 15 + 10));
      const centered = col.kind === 'number' || col.key === 'score_remark';
      // Status text is always black on an unfilled cell, including striped rows.
      const style = col.key === 'score_remark' ? 15
        : (col.kind === 'date' ? 17 : centered ? 15 : percent ? 13 : numeric || col.kind === 'number' ? 11 : 9) + index % 2;
      return cell(`${letters(colIndex)}${row}`, value, style);
    });
    rows.push(`<row r="${row}" ht="${height}" customHeight="1">${values.join('')}</row>`);
  });
  const lastRow = Math.max(6, data.length + 6);
  return declaration + `<worksheet xmlns="${spreadsheetNs}">
    <sheetPr><tabColor rgb="FF${sheet.color}"/><pageSetUpPr fitToPage="1"/></sheetPr>
    <dimension ref="A1:${end}${lastRow}"/>
    <sheetViews><sheetView showGridLines="0" zoomScale="85" workbookViewId="0"><pane xSplit="2" ySplit="6" topLeftCell="C7" activePane="bottomRight" state="frozen"/><selection pane="bottomRight" activeCell="C7" sqref="C7"/></sheetView></sheetViews>
    <sheetFormatPr defaultRowHeight="28"/>
    <cols>${sheet.columns.map((col, index) => `<col min="${index + 1}" max="${index + 1}" width="${col.width}" customWidth="1"/>`).join('')}</cols>
    <sheetData>${rows.join('')}</sheetData>
    <autoFilter ref="A6:${end}${lastRow}"/>
    <mergeCells count="4">${[1, 2, 3, 4].map(row => `<mergeCell ref="A${row}:${end}${row}"/>`).join('')}</mergeCells>
    <printOptions horizontalCentered="1"/>
    <pageMargins left="0.25" right="0.25" top="0.4" bottom="0.4" header="0.2" footer="0.2"/>
    <pageSetup paperSize="9" orientation="landscape" fitToWidth="1" fitToHeight="0"/>
    <headerFooter><oddFooter>${xml('&LBakeIT | ' + sheet.name + '&RPage &P of &N')}</oddFooter></headerFooter>
  </worksheet>`;
}

export function sectionReportWorkbook(report, { template = false } = {}) {
  validateSectionReport(report);
  const histories = new Map(report.histories.map(history => [history.learnerId, history]));
  const historyRows = report.rows.flatMap(row => [...histories.get(row.learner_id).entries]
    .sort((a, b) => (Date.parse(b.startedAt) || 0) - (Date.parse(a.startedAt) || 0) || a.id.localeCompare(b.id))
    .map(entry => ({ learner_name: row.learner_name, learner_id: row.learner_id, data_source: row.data_source,
      recipe: entry.recipe, session_status: entry.status, started_at: entry.startedAt, ended_at: entry.endedAt,
      total_score: entry.score, score_remark: entry.result })));
  if (Math.max(report.rows.length, historyRows.length) > 1048570) throw new Error('This report exceeds Excel’s row limit. Choose a smaller class section.');
  const parts = [
    ['[Content_Types].xml', declaration + `<Types xmlns="http://schemas.openxmlformats.org/package/2006/content-types"><Default Extension="rels" ContentType="application/vnd.openxmlformats-package.relationships+xml"/><Default Extension="xml" ContentType="application/xml"/><Override PartName="/xl/workbook.xml" ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml"/><Override PartName="/xl/styles.xml" ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.styles+xml"/>${sheets.map((_, index) => `<Override PartName="/xl/worksheets/sheet${index + 1}.xml" ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml"/>`).join('')}</Types>`],
    ['_rels/.rels', declaration + `<Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships"><Relationship Id="rId1" Type="${relationshipNs}/officeDocument" Target="xl/workbook.xml"/></Relationships>`],
    ['xl/workbook.xml', declaration + `<workbook xmlns="${spreadsheetNs}" xmlns:r="${relationshipNs}"><bookViews><workbookView activeTab="0"/></bookViews><sheets>${sheets.map((sheet, index) => `<sheet name="${sheet.name}" sheetId="${index + 1}" r:id="rId${index + 1}"/>`).join('')}</sheets><definedNames>${sheets.map((sheet, index) => `<definedName name="_xlnm.Print_Titles" localSheetId="${index}">'${sheet.name}'!$1:$6</definedName>`).join('')}</definedNames></workbook>`],
    ['xl/_rels/workbook.xml.rels', declaration + `<Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships">${sheets.map((_, index) => `<Relationship Id="rId${index + 1}" Type="${relationshipNs}/worksheet" Target="worksheets/sheet${index + 1}.xml"/>`).join('')}<Relationship Id="rId${sheets.length + 1}" Type="${relationshipNs}/styles" Target="styles.xml"/></Relationships>`],
    ['xl/styles.xml', styles()],
    ...sheets.map((sheet, index) => [`xl/worksheets/sheet${index + 1}.xml`, worksheet(report, sheet, template, historyRows)])
  ];
  return xlsxPackage(parts);
}
export function sectionWorkbookFilename(report) { return sectionReportFilename(report); }
