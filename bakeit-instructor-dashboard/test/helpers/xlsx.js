import assert from 'node:assert/strict';
import { inflateRawSync } from 'node:zlib';

// Inspect the ZIP central directory (separate from the export implementation).
export function workbookParts(input) {
  const bytes = Buffer.from(input), end = bytes.length - 22;
  assert.equal(bytes.readUInt32LE(end), 0x06054b50);
  let position = bytes.readUInt32LE(end + 16);
  const parts = new Map();
  for (let index = 0; index < bytes.readUInt16LE(end + 10); index++) {
    assert.equal(bytes.readUInt32LE(position), 0x02014b50);
    const length = bytes.readUInt32LE(position + 20), nameLength = bytes.readUInt16LE(position + 28);
    const local = bytes.readUInt32LE(position + 42);
    assert.equal(bytes.readUInt32LE(local), 0x04034b50);
    const start = local + 30 + bytes.readUInt16LE(local + 26) + bytes.readUInt16LE(local + 28);
    const data = bytes.subarray(start, start + length);
    const name = bytes.toString('utf8', position + 46, position + 46 + nameLength);
    parts.set(name, (bytes.readUInt16LE(position + 10) === 8 ? inflateRawSync(data) : data).toString('utf8'));
    position += 46 + nameLength + bytes.readUInt16LE(position + 30) + bytes.readUInt16LE(position + 32);
  }
  return parts;
}
