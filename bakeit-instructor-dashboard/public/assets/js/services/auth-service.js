import { StorageService } from './storage-service.js';

export class AuthService {
  constructor(store = new StorageService(), now = () => Date.now()) { this.store = store; this.now = now; }
  loginCooldown() {
    const attempts = this.loginAttempts();
    if (attempts.until && attempts.until <= this.now()) {
      try { this.store.remove('bakeit_login_attempts'); } catch {}
      return 0;
    }
    return Math.max(0, Math.ceil((attempts.until - this.now()) / 1000));
  }
  loginAttempts() {
    const attempts = this.store.get('bakeit_login_attempts');
    return attempts && Number.isInteger(attempts.failures) && attempts.failures >= 0 && Number.isFinite(attempts.until)
      ? attempts : { failures: 0, until: 0 };
  }
  validateEmail(email) {
    const normalized = email.trim().toLowerCase();
    if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(normalized)) {
      throw Object.assign(new Error('Invalid email'), { field: 'email' });
    }
    if (normalized !== 'instructor@mcl.edu.ph') {
      throw Object.assign(new Error('Invalid email'), { field: 'email' });
    }
    return normalized;
  }
  login(email, password) {
    const retryAfter = this.loginCooldown();
    if (retryAfter) throw Object.assign(new Error('Invalid Password'), { field: 'password', retryAfter });
    email = this.validateEmail(email);
    if (!password) throw Object.assign(new Error('Invalid Password'), { field: 'password' });
    const changedCredentials = this.store.get('bakeit_demo_credentials');
    const expectedPassword = changedCredentials?.email === email ? changedCredentials.password : 'demo123';
    if (password !== expectedPassword) {
      const attempts = this.loginAttempts();
      attempts.failures += 1;
      if (attempts.failures >= 3) attempts.until = this.now() + 30000;
      this.store.set('bakeit_login_attempts', attempts);
      throw Object.assign(new Error('Invalid Password'), { field: 'password', retryAfter: this.loginCooldown() });
    }
    const user = { name: 'Authorized Instructor', email, role: 'Instructor' };
    this.store.set('bakeit_user', user);
    this.store.remove('bakeit_login_attempts');
    return user;
  }
  requestPasswordReset(email) {
    email = this.validateEmail(email);
    this.store.set('bakeit_reset_email', email);
    return email;
  }
  resetPassword(code, password, confirmation) {
    const email = this.store.get('bakeit_reset_email');
    if (!email) throw new Error('Request a verification code first.');
    if (!/^\d{6}$/.test(code)) throw new Error('Enter the 6-digit verification code.');
    if (password.length < 8) throw new Error('Password must contain at least 8 characters.');
    if (password !== confirmation) throw new Error('The passwords do not match.');
    this.store.set('bakeit_demo_credentials', { email, password });
    this.store.remove('bakeit_reset_email');
    this.store.remove('bakeit_login_attempts');
    return true;
  }
  logout() { this.store.remove('bakeit_user'); }
  current() {
    const user = this.store.get('bakeit_user');
    return user?.email === 'instructor@mcl.edu.ph' && user.role === 'Instructor' ? user : null;
  }
  requireAuth() { if (!this.current()) location.href = '/login.html'; }
}
