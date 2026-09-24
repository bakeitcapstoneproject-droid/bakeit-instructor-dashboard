import { cp, mkdir, writeFile } from 'node:fs/promises';
import { fileURLToPath } from 'node:url';
import { resolve } from 'node:path';

export class StaticSiteBuilder {
  constructor({ project = new URL('../', import.meta.url), output = new URL('dist/', project) } = {}) {
    this.project = project;
    this.output = output;
  }
  async build() {
    await mkdir(this.output, { recursive: true });
    await cp(new URL('public/', this.project), this.output, { recursive: true });
    await writeFile(new URL('assets/js/runtime-config.js', this.output), "export const dataMode = 'static';\n");
    await cp(new URL('public/login.html', this.project), new URL('index.html', this.output));
    return fileURLToPath(this.output);
  }
}

if (process.argv[1] && resolve(process.argv[1]) === fileURLToPath(import.meta.url)) {
  const output = await new StaticSiteBuilder().build();
  console.log(`Static demo built at ${output}. No API, database, or environment variables required.`);
}
