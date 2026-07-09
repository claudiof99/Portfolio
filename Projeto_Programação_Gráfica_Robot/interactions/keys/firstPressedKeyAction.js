// First keypress: reactor pulse + opening sound.
// headFollowingCursor.js keyPressed calls onFirstAnyKeyPressed().

function onFirstAnyKeyPressed() {
  unlockReactorPulse();
  PlayOpeningSoundOnce();
}
