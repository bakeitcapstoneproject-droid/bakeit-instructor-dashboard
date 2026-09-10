import test from 'node:test';
import assert from 'node:assert/strict';
import { AuthService, StorageService } from '../public/assets/js/core.js';

function createAuth() {
  const values = new Map();
  return new AuthService(new StorageService({
    getItem: key => values.get(key) ?? null,
    setItem: (key, value) => values.set(key, value),
    removeItem: key => values.delete(key)
  }));
}

test('invalid login attempts do not create a session', () => {
  const auth = createAuth();
  for (const email of ['', 'invalid', 'a@@mcl.edu.ph', 'other@mcl.edu.ph']) {
    assert.throws(() => auth.login(email, 'demo123'), { field: 'email' });
    assert.equal(auth.current(), null);
  }
  for (const password of ['', 'incorrect', 'Demo123', 'demo123 ']) {
    assert.throws(() => auth.login('instructor@mcl.edu.ph', password), { field: 'password' });
    assert.equal(auth.current(), null);
  }
});

test('demo login normalizes email and accepts only the current password', () => {
  const auth = createAuth();
  assert.equal(auth.login(' Instructor@MCL.edu.ph ', 'demo123').email, 'instructor@mcl.edu.ph');
  auth.logout();
  assert.throws(() => auth.requestPasswordReset('other@mcl.edu.ph'), /Invalid email/);
  auth.requestPasswordReset('Instructor@MCL.edu.ph');
  auth.resetPassword('123456', 'changed123', 'changed123');
  assert.throws(() => auth.login('instructor@mcl.edu.ph', 'demo123'), /Invalid Password/);
  assert.equal(auth.current(), null);
  assert.equal(auth.login('instructor@mcl.edu.ph', 'changed123').role, 'Instructor');
});

test('three wrong passwords trigger a persistent 30-second timeout', () => {
  const auth = createAuth();
  let now = 1000;
  auth.now = () => now;
  for (let attempt = 0; attempt < 3; attempt++) {
    assert.throws(() => auth.login('instructor@mcl.edu.ph', 'wrong'), { message: 'Invalid Password' });
    assert.equal(auth.loginCooldown(), attempt === 2 ? 30 : 0);
  }
  const reloaded = new AuthService(auth.store, () => now);
  assert.throws(() => reloaded.login('instructor@mcl.edu.ph', 'demo123'), { retryAfter: 30 });
  assert.equal(reloaded.current(), null);
  now += 29000;
  assert.equal(reloaded.loginCooldown(), 1);
  now += 1000;
  assert.equal(reloaded.loginCooldown(), 0);
  assert.equal(reloaded.login('instructor@mcl.edu.ph', 'demo123').role, 'Instructor');
});

test('successful login clears previous failed attempts', () => {
  const auth = createAuth();
  for (let attempt = 0; attempt < 2; attempt++) {
    assert.throws(() => auth.login('instructor@mcl.edu.ph', 'wrong'), /Invalid Password/);
  }
  auth.login('instructor@mcl.edu.ph', 'demo123');
  auth.logout();
  assert.throws(() => auth.login('instructor@mcl.edu.ph', 'wrong'), /Invalid Password/);
  assert.equal(auth.loginCooldown(), 0);
  assert.throws(() => auth.login('invalid', 'demo123'), { message: 'Invalid email' });
});
