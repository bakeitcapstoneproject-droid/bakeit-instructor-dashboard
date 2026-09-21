import { cp, mkdir, writeFile } from 'node:fs/promises';
import { fileURLToPath } from 'node:url';

const project = new URL('../', import.meta.url);
const output = new URL('dist/', project);
await mkdir(output, { recursive: true });
await cp(new URL('public/', project), output, { recursive: true });
await writeFile(new URL('assets/js/runtime-config.js', output), "export const dataMode = 'static';\n");
await cp(new URL('public/login.html', project), new URL('index.html', output));
console.log(`Static demo built at ${fileURLToPath(output)}. No API, database, or environment variables required.`);
