// Ten-digit presentation for sample records; stored VR identities remain intact.
const tenDigits = value => value.slice(-10).padStart(10, '0');

export function learnerDisplayId(student) {
  if (student.demo !== true) return student.id;
  const localSample = /^DEMO-([0-9A-F]{8})-(\d+)$/i.exec(student.id);
  if (localSample) return tenDigits(`${Number.parseInt(localSample[1], 16)}${localSample[2].padStart(3, '0')}`);
  const browserSample = /^S-(\d+)$/.exec(student.id);
  return browserSample ? tenDigits(browserSample[1]) : student.id;
}
