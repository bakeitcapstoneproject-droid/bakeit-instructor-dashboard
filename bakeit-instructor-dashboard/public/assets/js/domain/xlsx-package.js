// Minimal Open Packaging Convention ZIP writer for the report's XML parts.
// Stored entries keep this export usable offline in both browsers and Node.
const encoder = new TextEncoder();
const crcTable = Uint32Array.from({ length: 256 }, (_, value) => {
  for (let bit = 0; bit < 8; bit++) value = (value >>> 1) ^ ((value & 1) ? 0xedb88320 : 0);
  return value >>> 0;
});
function crc32(bytes) {
  let crc = 0xffffffff;
  for (const byte of bytes) crc = (crc >>> 8) ^ crcTable[(crc ^ byte) & 255];
  return (crc ^ 0xffffffff) >>> 0;
}
function header(size) {
  const bytes = new Uint8Array(size);
  return { bytes, view: new DataView(bytes.buffer) };
}
export function xlsxPackage(parts) {
  const local = [], central = [];
  let offset = 0, directorySize = 0;
  for (const [path, xml] of parts) {
    const name = encoder.encode(path), data = encoder.encode(xml), crc = crc32(data);
    const entry = header(30);
    entry.view.setUint32(0, 0x04034b50, true);
    entry.view.setUint16(4, 20, true);
    entry.view.setUint16(6, 0x0800, true); // UTF-8 file names.
    entry.view.setUint16(12, 33, true); // 1980-01-01, deterministic ZIP date.
    entry.view.setUint32(14, crc, true);
    entry.view.setUint32(18, data.length, true);
    entry.view.setUint32(22, data.length, true);
    entry.view.setUint16(26, name.length, true);
    local.push(entry.bytes, name, data);
    const index = header(46);
    index.view.setUint32(0, 0x02014b50, true);
    index.view.setUint16(4, 20, true);
    index.view.setUint16(6, 20, true);
    index.view.setUint16(8, 0x0800, true);
    index.view.setUint16(14, 33, true);
    index.view.setUint32(16, crc, true);
    index.view.setUint32(20, data.length, true);
    index.view.setUint32(24, data.length, true);
    index.view.setUint16(28, name.length, true);
    index.view.setUint32(42, offset, true);
    central.push(index.bytes, name);
    offset += entry.bytes.length + name.length + data.length;
    directorySize += index.bytes.length + name.length;
  }
  if (offset + directorySize > 0xffffffff || parts.length > 65535) throw new Error('This report is too large for an Excel download.');
  const end = header(22);
  end.view.setUint32(0, 0x06054b50, true);
  end.view.setUint16(8, parts.length, true);
  end.view.setUint16(10, parts.length, true);
  end.view.setUint32(12, directorySize, true);
  end.view.setUint32(16, offset, true);
  const result = new Uint8Array(offset + directorySize + end.bytes.length);
  let position = 0;
  for (const part of [...local, ...central, end.bytes]) { result.set(part, position); position += part.length; }
  return result;
}
