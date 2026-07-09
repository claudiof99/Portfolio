/**
 * hub-scroll-reveal.js
 * IntersectionObserver-based scroll-triggered animations.
 * Watches elements with `.hub-reveal` class and adds `.hub-visible`
 * when they enter the viewport. Supports staggered children.
 *
 * OCP: Self-contained module. Zero coupling to existing code.
 */
(function hubScrollRevealInit() {
  "use strict";

  if (!("IntersectionObserver" in window)) {
    // Fallback: show everything immediately on unsupported browsers
    document.querySelectorAll(".hub-reveal").forEach(function (el) {
      el.classList.add("hub-visible");
    });
    return;
  }

  if (document.body.classList.contains("hub-instant-page")) {
    document.querySelectorAll(".hub-reveal").forEach(function (el) {
      el.classList.add("hub-visible");
    });
    return;
  }

  var observer = new IntersectionObserver(
    function (entries) {
      entries.forEach(function (entry) {
        if (entry.isIntersecting) {
          entry.target.classList.add("hub-visible");
          observer.unobserve(entry.target);
        }
      });
    },
    {
      threshold: 0.08,
      rootMargin: "0px 0px -40px 0px",
    },
  );

  document.querySelectorAll(".hub-reveal").forEach(function (el) {
    observer.observe(el);
  });
})();
