import { AuthService } from './core.js';

class LoginPage {
  constructor() {
    this.auth = new AuthService();
    this.loginView = document.querySelector('[data-auth-view="login"]');
    this.recoveryView = document.querySelector('[data-auth-view="recovery"]');
    this.requestStep = document.querySelector('[data-reset-step="request"]');
    this.confirmStep = document.querySelector('[data-reset-step="confirm"]');
    this.successStep = document.querySelector('[data-reset-step="success"]');
    this.loginForm = document.querySelector('#login-form');
    this.requestForm = document.querySelector('#request-reset-form');
    this.confirmForm = document.querySelector('#confirm-reset-form');
  }

  init() {
    if (this.auth.current()) location.href = '/dashboard.html';
    this.loginForm.addEventListener('submit', event => this.signIn(event));
    this.requestForm.addEventListener('submit', event => this.requestReset(event));
    this.confirmForm.addEventListener('submit', event => this.confirmReset(event));
    document.querySelector('[data-forgot]').addEventListener('click', () => this.showRecovery());
    document.querySelector('[data-back-login]').addEventListener('click', () => this.showLogin());
    document.querySelector('[data-back-request]').addEventListener('click', () => this.showResetStep('request'));
    document.querySelector('[data-return-login]').addEventListener('click', () => this.finishRecovery());
  }

  signIn(event) {
    event.preventDefault();
    const error = document.querySelector('[data-login-error]');
    error.textContent = '';
    try {
      this.auth.login(this.loginForm.email.value, this.loginForm.password.value);
      location.href = '/dashboard.html';
    } catch (exception) { error.textContent = exception.message; }
  }

  showRecovery() {
    this.loginView.hidden = true;
    this.recoveryView.hidden = false;
    this.requestForm.resetEmail.value = this.loginForm.email.value;
    this.showResetStep('request');
    this.requestForm.resetEmail.focus();
  }

  showLogin() {
    this.recoveryView.hidden = true;
    this.loginView.hidden = false;
    this.loginForm.email.focus();
  }

  showResetStep(step) {
    this.requestStep.hidden = step !== 'request';
    this.confirmStep.hidden = step !== 'confirm';
    this.successStep.hidden = step !== 'success';
  }

  requestReset(event) {
    event.preventDefault();
    const error = document.querySelector('[data-request-error]');
    error.textContent = '';
    try {
      const email = this.auth.requestPasswordReset(this.requestForm.resetEmail.value.trim());
      document.querySelector('[data-reset-email]').textContent = email;
      this.showResetStep('confirm');
      this.confirmForm.resetCode.focus();
    } catch (exception) { error.textContent = exception.message; }
  }

  confirmReset(event) {
    event.preventDefault();
    const error = document.querySelector('[data-confirm-error]');
    error.textContent = '';
    try {
      this.auth.resetPassword(
        this.confirmForm.resetCode.value.trim(),
        this.confirmForm.newPassword.value,
        this.confirmForm.confirmPassword.value
      );
      this.showResetStep('success');
    } catch (exception) { error.textContent = exception.message; }
  }

  finishRecovery() {
    const email = document.querySelector('[data-reset-email]').textContent;
    this.loginForm.email.value = email;
    this.loginForm.password.value = '';
    this.requestForm.reset();
    this.confirmForm.reset();
    this.showLogin();
    this.loginForm.password.focus();
  }
}

new LoginPage().init();
