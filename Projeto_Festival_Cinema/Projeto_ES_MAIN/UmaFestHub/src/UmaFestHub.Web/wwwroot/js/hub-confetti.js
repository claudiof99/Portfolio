/**
 * hub-confetti.js
 * Pure canvas confetti celebration.
 * Auto-fires when `[data-confetti="true"]` is present on the page.
 * Also exposes `window.hubConfetti.fire()` for programmatic triggers.
 * Zero dependencies.
 */
(function hubConfettiInit() {
  'use strict';

  var colors = [
    '#e50914', '#ff4d56', '#fbbf24', '#34d399',
    '#60a5fa', '#a78bfa', '#f472b6', '#ffffff'
  ];

  function createCanvas() {
    var canvas = document.createElement('canvas');
    canvas.className = 'hub-confetti-canvas';
    document.body.appendChild(canvas);

    var ctx = canvas.getContext('2d');

    function resize() {
      canvas.width = window.innerWidth;
      canvas.height = window.innerHeight;
    }

    resize();
    window.addEventListener('resize', resize);

    return { canvas: canvas, ctx: ctx, resize: resize };
  }

  function createParticle(canvas, options) {
    var originX = options.originX != null ? options.originX : canvas.width * 0.5;
    var originY = options.originY != null ? options.originY : canvas.height * 0.35;
    var angle = options.angle != null ? options.angle : -Math.PI / 2 + (Math.random() - 0.5) * 0.8;
    var speed = options.speed != null ? options.speed : 6 + Math.random() * 8;

    return {
      x: originX + (Math.random() - 0.5) * 40,
      y: originY + (Math.random() - 0.5) * 20,
      w: 4 + Math.random() * 6,
      h: 6 + Math.random() * 10,
      vx: Math.cos(angle) * speed + (Math.random() - 0.5) * 3,
      vy: Math.sin(angle) * speed + (Math.random() - 0.5) * 2,
      rotation: Math.random() * 360,
      rotationSpeed: (Math.random() - 0.5) * 14,
      color: colors[Math.floor(Math.random() * colors.length)],
      opacity: 1,
      gravity: 0.12 + Math.random() * 0.06,
      wobble: Math.random() * 10,
      wobbleSpeed: 0.03 + Math.random() * 0.05
    };
  }

  function buildParticles(canvas, count) {
    var particles = [];
    var width = canvas.width;
    var height = canvas.height;

    // Center burst
    for (var i = 0; i < count; i++) {
      particles.push(createParticle(canvas, {
        originX: width * 0.5,
        originY: height * 0.3
      }));
    }

    // Side cannons
    for (var j = 0; j < Math.floor(count * 0.25); j++) {
      particles.push(createParticle(canvas, {
        originX: 0,
        originY: height * 0.55,
        angle: -Math.PI / 4 + (Math.random() - 0.5) * 0.4,
        speed: 10 + Math.random() * 6
      }));
      particles.push(createParticle(canvas, {
        originX: width,
        originY: height * 0.55,
        angle: (-3 * Math.PI) / 4 + (Math.random() - 0.5) * 0.4,
        speed: 10 + Math.random() * 6
      }));
    }

    return particles;
  }

  function fire(options) {
    options = options || {};
    var count = options.count || 80;
    var duration = options.duration || 4000;
    var stage = createCanvas();
    var particles = buildParticles(stage.canvas, count);
    var startTime = Date.now();

    function animate() {
      var elapsed = Date.now() - startTime;
      var progress = elapsed / duration;

      if (progress >= 1) {
        stage.canvas.remove();
        return;
      }

      stage.ctx.clearRect(0, 0, stage.canvas.width, stage.canvas.height);

      var globalAlpha = progress > 0.65 ? 1 - (progress - 0.65) / 0.35 : 1;

      particles.forEach(function (p) {
        p.vy += p.gravity;
        p.x += p.vx;
        p.y += p.vy;
        p.rotation += p.rotationSpeed;
        p.x += Math.sin(p.wobble) * 0.6;
        p.wobble += p.wobbleSpeed;
        p.vx *= 0.995;

        stage.ctx.save();
        stage.ctx.globalAlpha = globalAlpha * p.opacity;
        stage.ctx.translate(p.x, p.y);
        stage.ctx.rotate((p.rotation * Math.PI) / 180);
        stage.ctx.fillStyle = p.color;
        stage.ctx.fillRect(-p.w / 2, -p.h / 2, p.w, p.h);
        stage.ctx.restore();
      });

      requestAnimationFrame(animate);
    }

    requestAnimationFrame(animate);
  }

  window.hubConfetti = { fire: fire };

  if (document.querySelector('[data-confetti="true"]')) {
    fire();
  }
})();
