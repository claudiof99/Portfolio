/**
 * hub-tilt.js
 * Lightweight 3D tilt effect on hover.
 * Listens for `mousemove` on `[data-tilt]` elements,
 * applies perspective + rotateX/rotateY transforms.
 * Resets smoothly on `mouseleave`.
 *
 * OCP: Self-contained module. Zero coupling to existing code.
 */
(function hubTiltInit() {
  'use strict';

  var MAX_TILT = 4; // degrees
  var PERSPECTIVE = 800; // px

  document.addEventListener('mousemove', function (e) {
    var el = e.target.closest('[data-tilt]');
    if (!el) return;

    var rect = el.getBoundingClientRect();
    var centerX = rect.left + rect.width / 2;
    var centerY = rect.top + rect.height / 2;

    var percentX = (e.clientX - centerX) / (rect.width / 2);
    var percentY = (e.clientY - centerY) / (rect.height / 2);

    // Clamp values
    percentX = Math.max(-1, Math.min(1, percentX));
    percentY = Math.max(-1, Math.min(1, percentY));

    var rotateY = percentX * MAX_TILT;
    var rotateX = -percentY * MAX_TILT;

    el.style.transform =
      'perspective(' + PERSPECTIVE + 'px) rotateX(' + rotateX + 'deg) rotateY(' + rotateY + 'deg) scale(1.02)';
  });

  document.addEventListener('mouseleave', function (e) {
    var el = e.target.closest('[data-tilt]');
    if (!el) return;

    el.style.transform = '';
  }, true);

  // Also reset on mouseout from each tilt element directly
  document.addEventListener('mouseout', function (e) {
    var el = e.target.closest('[data-tilt]');
    if (!el) return;

    // Check if we're leaving the tilt element (not entering a child)
    var related = e.relatedTarget;
    if (related && el.contains(related)) return;

    el.style.transform = '';
  });
})();
