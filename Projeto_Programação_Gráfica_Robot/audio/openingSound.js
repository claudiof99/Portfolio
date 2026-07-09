// Robot talk on first interaction; call preloadOpeningSound() from sketch preload.
// PlayOpeningSoundOnce() is invoked from interactions/keys/firstPressedKeyAction.js.

let robotOpeningSound = null;
let openingSoundPlayed = false;

function preloadOpeningSound() {
  robotOpeningSound = loadSound("audio/diff_style-robot-talk-344757.mp3");
}

function PlayOpeningSoundOnce() {
  if (openingSoundPlayed || !robotOpeningSound) return;
  openingSoundPlayed = true;
  const ctx =
    typeof getAudioContext === "function" ? getAudioContext() : null;
  const play = () => {
    robotOpeningSound.setVolume(0.45);
    robotOpeningSound.play();
  };
  if (ctx && ctx.state === "suspended") ctx.resume().then(play);
  else play();
}
