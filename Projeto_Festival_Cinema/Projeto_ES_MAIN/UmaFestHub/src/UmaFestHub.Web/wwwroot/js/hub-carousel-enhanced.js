/**
 * hub-carousel-enhanced.js
 * Enhances `.hub-favorites-viewport` carousels with:
 * - CSS scroll-snap (native horizontal scroll)
 * - Mouse drag-to-scroll with momentum
 * - Edge fade gradient masking
 *
 * OCP: Self-contained module. Extends existing carousel behavior.
 */
(function hubCarouselEnhancedInit() {
  'use strict';

  var viewports = document.querySelectorAll('.hub-favorites-viewport');
  if (!viewports.length) return;

  viewports.forEach(function (viewport) {
    viewport.classList.add('hub-carousel-enhanced');

    var isDragging = false;
    var startX = 0;
    var scrollLeft = 0;
    var velocity = 0;
    var lastX = 0;
    var lastTime = 0;
    var momentumId = null;

    function getMaxScroll() {
      return Math.max(0, viewport.scrollWidth - viewport.clientWidth);
    }

    function updateEdges() {
      var max = getMaxScroll();
      var x = viewport.scrollLeft;
      var atStart = x <= 2;
      var atEnd = max <= 2 || x >= max - 2;

      viewport.classList.toggle('hub-carousel-at-start', atStart);
      viewport.classList.toggle('hub-carousel-at-end', atEnd);
    }

    updateEdges();
    viewport.addEventListener('scroll', updateEdges, { passive: true });

    viewport.addEventListener('mousedown', function (e) {
      if (e.target.closest('a, button, form')) return;

      isDragging = true;
      startX = e.clientX;
      scrollLeft = viewport.scrollLeft;
      velocity = 0;
      lastX = e.clientX;
      lastTime = Date.now();

      if (momentumId) cancelAnimationFrame(momentumId);

      viewport.style.cursor = 'grabbing';
      viewport.style.userSelect = 'none';
      e.preventDefault();
    });

    document.addEventListener('mousemove', function (e) {
      if (!isDragging) return;

      var dx = e.clientX - startX;
      var now = Date.now();
      var dt = now - lastTime;

      if (dt > 0) {
        velocity = (e.clientX - lastX) / dt;
      }

      lastX = e.clientX;
      lastTime = now;

      viewport.scrollLeft = scrollLeft - dx;
      updateEdges();
    });

    document.addEventListener('mouseup', function () {
      if (!isDragging) return;
      isDragging = false;

      viewport.style.cursor = '';
      viewport.style.userSelect = '';

      var friction = 0.95;

      function momentumStep() {
        if (Math.abs(velocity) < 0.01) return;

        velocity *= friction;
        viewport.scrollLeft -= velocity * 16;
        updateEdges();

        momentumId = requestAnimationFrame(momentumStep);
      }

      if (Math.abs(velocity) > 0.1) {
        momentumStep();
      }
    });

    if ('MutationObserver' in window) {
      var track = viewport.querySelector('.hub-favorites-track');
      if (track) {
        var mutObs = new MutationObserver(updateEdges);
        mutObs.observe(track, { childList: true, subtree: true });
      }
    }

    window.addEventListener('resize', updateEdges);
  });
})();
