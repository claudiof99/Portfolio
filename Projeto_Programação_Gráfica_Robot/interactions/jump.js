// Jump (V): crouch → launch → land; sketch applies jumpYOffset to translate & spotlight

let jumpT = 0;
let jumpYOffset = 0;
let jumpKneeExtra = 0;
let jumpHipExtra = 0;
let jumpShinMul = 1;

function updateJumpAnim() {
  jumpKneeExtra = 0;
  jumpHipExtra = 0;
  jumpShinMul = 1;
  jumpYOffset = 0;
  if (jumpT <= 0) return;

  jumpT += 1 / 78;
  if (jumpT >= 1) {
    jumpT = 0;
    return;
  }

  const t = jumpT;
  const smooth = (x) => x * x * (3 - 2 * x);

  let crouch = 0;
  if (t < 0.26) crouch = smooth(t / 0.26);
  else if (t < 0.42) crouch = 1 - smooth((t - 0.26) / 0.16);

  const t0 = 0.22;
  const t1 = 0.78;
  let air = 0;
  if (t > t0 && t < t1) {
    air = Math.sin(((t - t0) / (t1 - t0)) * Math.PI);
  }

  jumpKneeExtra = -1.05 * crouch;
  jumpHipExtra = 0.34 * crouch;
  jumpShinMul = 1 - 0.48 * crouch;

  const sink = 34 * crouch;
  const peakLift = 168 * air;
  jumpYOffset = sink - peakLift;
}
