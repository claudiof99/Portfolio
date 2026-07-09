/**
 * hub-star-rating.js
 * Interactive animated star rating widget.
 * Progressively enhances any `<select>` with `data-star-rating` attribute.
 * Hides the <select>, creates a visual star row, syncs value back to the
 * hidden select. Falls back gracefully if JS is disabled.
 *
 * OCP: Self-contained module. Zero coupling to existing code.
 */
(function hubStarRatingInit() {
  'use strict';

  var selects = document.querySelectorAll('select[data-star-rating]');
  if (!selects.length) return;

  var MOODS = ['', '😕', '😐', '🙂', '😊', '🤩'];

  selects.forEach(function (select) {
    var maxStars = 5;
    var currentValue = parseInt(select.value, 10) || 0;

    // Hide original select
    select.style.display = 'none';

    // Create wrapper
    var wrapper = document.createElement('div');
    wrapper.style.display = 'flex';
    wrapper.style.alignItems = 'center';
    wrapper.style.gap = '0.5rem';

    // Create star container (uses RTL trick for sibling hover)
    var starContainer = document.createElement('div');
    starContainer.className = 'hub-star-rating';

    // Mood emoji
    var moodEl = document.createElement('span');
    moodEl.className = 'hub-star-rating-mood';
    moodEl.textContent = MOODS[currentValue] || '';

    var ariaTemplate = select.getAttribute('data-rate-aria') || ('Rate {0} out of ' + maxStars);

    // Create stars (in reverse order for RTL sibling trick)
    var starEls = [];
    for (var i = maxStars; i >= 1; i--) {
      var star = document.createElement('span');
      star.className = 'hub-star-rating-star';
      star.textContent = '★';
      star.setAttribute('data-value', i);
      star.setAttribute('role', 'button');
      star.setAttribute('tabindex', '0');
      star.setAttribute('aria-label', ariaTemplate.replace('{0}', i));

      if (i <= currentValue) {
        star.classList.add('hub-star-active');
      }

      // Click handler
      (function (val) {
        star.addEventListener('click', function () {
          currentValue = val;
          select.value = val;

          // Trigger change event for any listeners
          var event = new Event('change', { bubbles: true });
          select.dispatchEvent(event);

          // Update active states
          updateStars();

          // Bounce animation
          star.classList.remove('hub-star-bounce');
          // Force reflow
          void star.offsetWidth;
          star.classList.add('hub-star-bounce');

          // Update mood
          moodEl.textContent = MOODS[val] || '';
          moodEl.style.transform = 'scale(1.3)';
          setTimeout(function () {
            moodEl.style.transform = '';
          }, 200);
        });

        // Keyboard support
        star.addEventListener('keydown', function (e) {
          if (e.key === 'Enter' || e.key === ' ') {
            e.preventDefault();
            star.click();
          }
        });
      })(i);

      starContainer.appendChild(star);
      starEls.unshift(star); // Store in correct order (1-5)
    }

    function updateStars() {
      starEls.forEach(function (s, idx) {
        var val = idx + 1;
        if (val <= currentValue) {
          s.classList.add('hub-star-active');
        } else {
          s.classList.remove('hub-star-active');
        }
      });
    }

    wrapper.appendChild(starContainer);
    wrapper.appendChild(moodEl);

    // Insert after the select
    select.parentNode.insertBefore(wrapper, select.nextSibling);
  });
})();
