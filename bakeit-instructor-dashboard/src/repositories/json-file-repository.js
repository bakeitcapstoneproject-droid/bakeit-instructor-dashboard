import { mkdir, readFile, rename, writeFile } from 'node:fs/promises';
import { dirname } from 'node:path';

// One repository instance owns the write queue for a data file.
export class JsonFileRepository {
  constructor(file) { this.file = file; this.pending = Promise.resolve(); }
  async read() {
    try { return JSON.parse(await readFile(this.file, 'utf8')); }
    catch (error) {
      if (error.code === 'ENOENT') return { sections: [], enrollments: [] };
      throw error;
    }
  }
  mutate(change) {
    const operation = this.pending.then(async () => {
      const data = await this.read();
      const result = change(data);
      await mkdir(dirname(this.file), { recursive: true });
      await writeFile(`${this.file}.tmp`, JSON.stringify(data, null, 2), 'utf8');
      await rename(`${this.file}.tmp`, this.file);
      return result;
    });
    this.pending = operation.catch(() => {});
    return operation;
  }
  async snapshot() { await this.pending; return this.read(); }
}
