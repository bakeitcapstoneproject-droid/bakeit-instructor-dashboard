import { JsonFileRepository } from './repositories/json-file-repository.js';
import { ClassroomService } from './services/classroom-service.js';

export { ClassroomService };
export { RequestError } from './http/request-error.js';
export { readJson } from './http/json-body-reader.js';

// Preserve the existing file-based constructor for CLI tools and external callers.
export class ClassStore extends ClassroomService {
  constructor(file, codeGenerator) { super(new JsonFileRepository(file), codeGenerator); }
}
