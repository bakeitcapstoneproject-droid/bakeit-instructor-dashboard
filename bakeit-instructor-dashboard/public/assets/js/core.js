// Public compatibility facade. Implementations live in focused modules.
export { StorageService } from './services/storage-service.js';
export { AuthService } from './services/auth-service.js';
export { SectionService } from './services/section-service.js';
export { ApiClient, apiClient, apiRequest } from './services/api-client.js';
export { CloudDataService } from './services/cloud-data-service.js';
export { scoreRemark, DashboardMetrics } from './domain/performance.js';
export { escapeHtml } from './utils/html.js';
export { TableView } from './views/table-view.js';
export { SectionSelector } from './views/section-selector.js';
export { AppShell } from './app/app-shell.js';
