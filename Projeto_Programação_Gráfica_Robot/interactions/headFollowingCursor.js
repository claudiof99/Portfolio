// Head follows mouse on canvas while headFollowMouse is true (toggle with M).
// Arrow keys control head only when follow mode is off (see InteractionKey doKeys).

let headFollowMouse = false;

function updateHeadFollowMouse() {
  if (!headFollowMouse) return;
  const w = width > 0 ? width : 1;
  const h = height > 0 ? height : 1;
  const mx = Math.max(0, Math.min(w, mouseX));
  const my = Math.max(0, Math.min(h, mouseY));
  const nx = (mx - w / 2) / (w / 2);
  const ny = (my - h / 2) / (h / 2);
  const lim = Math.PI / 2;
  angleHeadY = Math.max(-lim, Math.min(lim, nx * lim));
  angleHeadX = Math.max(-lim, Math.min(lim, -ny * lim));
}

function resetHeadFollowMouse() {
  headFollowMouse = false;
}

function keyPressed() {
  onFirstAnyKeyPressed();
  if (key === "m" || key === "M") {
    headFollowMouse = !headFollowMouse;
  }
}
