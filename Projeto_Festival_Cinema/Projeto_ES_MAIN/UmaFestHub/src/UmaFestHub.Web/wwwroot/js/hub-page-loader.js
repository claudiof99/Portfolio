/**
 * hub-page-loader.js
 * Brief skeleton shimmer on initial page load, then fades main content in.
 * OCP: Self-contained module. Zero coupling to existing code.
 */
(function hubPageLoaderInit() {
  'use strict';

  var skeleton = document.getElementById('hub-page-skeleton');
  var shell = document.querySelector('.app-shell');
  if (!skeleton || !shell) return;

  if (document.body.classList.contains('hub-instant-page')) {
    skeleton.classList.add('hub-page-skeleton-hidden');
    shell.classList.add('hub-content-ready');
    if (skeleton.parentNode) skeleton.parentNode.removeChild(skeleton);
    return;
  }

  function reveal() {
    skeleton.classList.add('hub-page-skeleton-hidden');
    shell.classList.add('hub-content-ready');
    setTimeout(function () {
      if (skeleton.parentNode) skeleton.parentNode.removeChild(skeleton);
    }, 500);
  }

  if (document.readyState === 'complete') {
    setTimeout(reveal, 180);
  } else {
    window.addEventListener('load', function () {
      setTimeout(reveal, 180);
    });
    // Fallback if load is slow
    setTimeout(reveal, 2200);
  }
})();
