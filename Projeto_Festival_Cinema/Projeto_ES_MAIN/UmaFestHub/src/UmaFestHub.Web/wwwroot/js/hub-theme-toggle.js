/**
 * hub-theme-toggle.js
 * Manages dark/light theme toggle.
 * Reads/writes `localStorage('hub-theme')`.
 * Sets `data-theme` attribute on `<html>`.
 * Listens for click on `.hub-theme-toggle` button.
 *
 * OCP: Self-contained module. Zero coupling to existing code.
 */
(function hubThemeToggleInit() {
  'use strict';

  var STORAGE_KEY = 'hub-theme';
  var html = document.documentElement;

  // Restore saved theme (or default to dark)
  var saved = localStorage.getItem(STORAGE_KEY);
  if (saved === 'light' || saved === 'dark') {
    html.setAttribute('data-theme', saved);
  }

  // Listen for toggle clicks
  document.addEventListener('click', function (e) {
    var btn = e.target.closest('.hub-theme-toggle');
    if (!btn) return;

    var current = html.getAttribute('data-theme') || 'dark';
    var next = current === 'dark' ? 'light' : 'dark';

    html.setAttribute('data-theme', next);
    localStorage.setItem(STORAGE_KEY, next);

    // Add a brief spin animation on the button
    btn.style.transform = 'rotate(360deg)';
    setTimeout(function () {
      btn.style.transform = '';
    }, 350);
  });
})();
