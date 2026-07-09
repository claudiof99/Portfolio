/**
 * hub-countdown.js
 * Flip-clock countdown timer.
 * Reads `data-countdown-target` (ISO 8601 UTC date string) from a container,
 * renders countdown digits, and switches to a "LIVE NOW" badge at zero.
 *
 * OCP: Self-contained module. Zero coupling to existing code.
 */
(function hubCountdownInit() {
  'use strict';

  var containers = document.querySelectorAll('[data-countdown-target]');
  if (!containers.length) return;

  containers.forEach(function (container) {
    var targetDate = new Date(container.getAttribute('data-countdown-target'));
    if (isNaN(targetDate.getTime())) return;

    var endDateStr = container.getAttribute('data-countdown-end');
    var endDate = endDateStr ? new Date(endDateStr) : null;

    // Build DOM
    var countdownEl = document.createElement('div');
    countdownEl.className = 'hub-countdown';
    countdownEl.setAttribute('aria-live', 'polite');
    countdownEl.setAttribute('aria-label', 'Countdown timer');

    var units = ['days', 'hours', 'mins', 'secs'];
    var unitLabels = {
      days: container.getAttribute('data-countdown-days') || 'days',
      hours: container.getAttribute('data-countdown-hours') || 'hours',
      mins: container.getAttribute('data-countdown-mins') || 'mins',
      secs: container.getAttribute('data-countdown-secs') || 'secs'
    };
    var liveLabel = container.getAttribute('data-countdown-live') || 'LIVE NOW';
    var endedLabel = container.getAttribute('data-countdown-ended') || 'FESTIVAL ENDED';
    var valueEls = {};

    units.forEach(function (unit, i) {
      if (i > 0) {
        var sep = document.createElement('span');
        sep.className = 'hub-countdown-separator';
        sep.textContent = ':';
        sep.setAttribute('aria-hidden', 'true');
        countdownEl.appendChild(sep);
      }

      var unitWrap = document.createElement('div');
      unitWrap.className = 'hub-countdown-unit hub-countdown-unit--' + unit;

      var valueEl = document.createElement('span');
      valueEl.className = 'hub-countdown-value';
      valueEl.textContent = '00';

      var labelEl = document.createElement('span');
      labelEl.className = 'hub-countdown-label';
      labelEl.textContent = unitLabels[unit];

      unitWrap.appendChild(valueEl);
      unitWrap.appendChild(labelEl);
      countdownEl.appendChild(unitWrap);
      valueEls[unit] = valueEl;
    });

    container.appendChild(countdownEl);

    // Prefix label (hidden when live)
    var prefixLabel = container.getAttribute('data-countdown-prefix');
    var prefixEl = null;
    if (prefixLabel) {
      prefixEl = document.createElement('p');
      prefixEl.className = 'hub-countdown-prefix';
      prefixEl.style.fontSize = '0.85rem';
      prefixEl.style.color = 'var(--hub-muted, #a3a3a3)';
      prefixEl.style.marginBottom = '0.5rem';
      prefixEl.style.textTransform = 'uppercase';
      prefixEl.style.letterSpacing = '0.05em';
      prefixEl.textContent = prefixLabel;
      // Insert before the countdown digits
      container.insertBefore(prefixEl, countdownEl);
    }

    // Live badge (hidden initially)
    var liveEl = document.createElement('div');
    liveEl.className = 'hub-countdown-live';
    liveEl.style.display = 'none';

    var liveDot = document.createElement('span');
    liveDot.className = 'hub-countdown-live-dot';
    liveEl.appendChild(liveDot);

    var liveTextEl = document.createElement('span');
    liveTextEl.textContent = liveLabel;
    liveEl.appendChild(liveTextEl);
    container.appendChild(liveEl);

    // Ended badge
    var endedEl = document.createElement('div');
    endedEl.className = 'hub-countdown-live';
    endedEl.style.display = 'none';
    endedEl.style.background = '#6b7280';
    endedEl.style.animation = 'none';

    var endedTextEl = document.createElement('span');
    endedTextEl.textContent = endedLabel;
    endedEl.appendChild(endedTextEl);
    container.appendChild(endedEl);

    var prevValues = {};
    var intervalId = null;

    function update() {
      var now = new Date();

      // Check if festival has ended
      if (endDate && now >= endDate) {
        countdownEl.style.display = 'none';
        liveEl.style.display = 'none';
        endedEl.style.display = 'inline-flex';
        if (intervalId) clearInterval(intervalId);
        return;
      }

      var diff = targetDate.getTime() - now.getTime();

      // Festival is live
      if (diff <= 0) {
        countdownEl.style.display = 'none';
        if (prefixEl) prefixEl.style.display = 'none';
        liveEl.style.display = 'inline-flex';
        endedEl.style.display = 'none';
        if (intervalId) clearInterval(intervalId);

        // If there's an end date, keep checking
        if (endDate) {
          intervalId = setInterval(update, 60000);
        }
        return;
      }

      var totalSecs = Math.floor(diff / 1000);
      var days  = Math.floor(totalSecs / 86400);
      var hours = Math.floor((totalSecs % 86400) / 3600);
      var mins  = Math.floor((totalSecs % 3600) / 60);
      var secs  = totalSecs % 60;

      var values = {
        days:  String(days).padStart(2, '0'),
        hours: String(hours).padStart(2, '0'),
        mins:  String(mins).padStart(2, '0'),
        secs:  String(secs).padStart(2, '0')
      };

      // Update with flip animation
      units.forEach(function (unit) {
        if (values[unit] !== prevValues[unit]) {
          var el = valueEls[unit];
          el.textContent = values[unit];
          el.classList.add('hub-countdown-flip');
          setTimeout(function () {
            el.classList.remove('hub-countdown-flip');
          }, 150);
        }
      });

      prevValues = values;

      // Urgent state: under 24 hours
      var isUrgent = diff < 86400000;
      countdownEl.classList.toggle('hub-countdown-urgent', isUrgent);
    }

    update();
    intervalId = setInterval(update, 1000);
  });
})();
