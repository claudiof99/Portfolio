// Chest reactor pulse: glowing aura + outer/inner cores (uses p5 frameCount).
// Pulse animates only after unlockReactorPulse() — called from keys/anyKeyPressed.js on first key.

let reactorPulseUnlocked = false;

function unlockReactorPulse() {
  reactorPulseUnlocked = true;
}

function getReactorPulseState() {
  let pulse;
  let pulseSoft;
  if (reactorPulseUnlocked) {
    const glowPhase = frameCount * 0.07;
    pulse = 0.5 + 0.5 * Math.sin(glowPhase);
    pulseSoft = 0.5 + 0.5 * Math.sin(glowPhase + 0.9);
  } else {
    pulse = 0.5;
    pulseSoft = 0.5;
  }
  const glowScale = 0.165 + 0.038 * pulse;
  const glowZ = 0.256 + 0.018 * pulseSoft;
  const glowThick = 0.034 + 0.014 * pulse;

  const auraRefXY = 0.165;
  const auraRefThick = 0.034;
  const coreXYScale = glowScale / auraRefXY;
  const coreThickScale = Math.max(0.92, glowThick / auraRefThick);

  const em = 18 + 42 * pulse;
  const zOuter = glowZ + 0.026;
  const zInner = glowZ + 0.046;
  const outerThick = 0.03 * coreThickScale;
  const innerThick = 0.03 * coreThickScale;

  return {
    pulse,
    pulseSoft,
    glowScale,
    glowZ,
    glowThick,
    coreXYScale,
    coreThickScale,
    em,
    zOuter,
    zInner,
    outerThick,
    innerThick,
  };
}
