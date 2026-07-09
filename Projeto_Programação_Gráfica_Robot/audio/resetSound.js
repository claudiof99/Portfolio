// Reset (HOME) — power-off clip; call preloadResetSound() from sketch preload.

let robotResetSound = null;

function preloadResetSound() {
  robotResetSound = loadSound(
    "audio/freesound_community-robot-power-off-97246.mp3",
  );
}

function playResetSound() {
  if (robotResetSound) robotResetSound.play();
}
