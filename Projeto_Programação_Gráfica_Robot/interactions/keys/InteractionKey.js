// Keyboard controls for robot and camera.
//
// Movement      W/S/A/D       — move robot forward/back/left/right
// Vertical      SPACE / SHIFT — move robot up / down
// Rotation      Q / E         — rotate robot around Y axis
// Scale         Z / X         — shrink / grow robot
// Head          Arrow keys    — tilt head up/down, turn left/right (see headFollowingCursor.js for M + mouse)
// Torso twist   , / .         — turn upper body left/right (Y); legs unchanged
// Torso lean    9 / 0         — lean upper body back / forward (X)
// Shoulder      R / F         — raise / lower shoulder
// Elbow         T / G         — bend / straighten elbow
// Hip           Y / H         — swing hip forward / back (both legs base; left also uses 5/6)
// Knee          U / N         — bend / straighten knee (both legs base; left also uses 7/8)
// Left leg +    5 / 6         — extra left hip back / forward (additive)
// Left knee +   7 / 8         — extra left knee bend / straighten (additive)
// Spotlight     [ / ]         — narrower / wider cone (see lighting on robot)
// Spot edge     - / =         — softer / sharper spotlight falloff (concentration)
// Camera orbit  I/K/J/L       — pitch and yaw camera
// Camera zoom   O / P         — zoom camera in / out
// Reset pose    HOME          — hold: default pose, camera, scale, spotlight
// Jump          V             — hold: crouch, jump, land (smooth)


function KID(mk) {
  return keyIsDown(mk);
}

function doKeys() {

  isWalking = false;

  const step  = 10;
  const rot   = 0.05;
  const spotStep = 0.04;
  const concStep = 3;

  // ── Robot position (FIXED NATURAL MOVEMENT) ─────────────────────────────
  if (KID(65)) {  // A (Left)
    angle += rot;
    // posX -= Math.cos(angle) * step;   
    // posZ += Math.sin(angle) * step;
  }
  if (KID(68)) {  // D (Right)
    angle -= rot;
    // posX += Math.cos(angle) * step;
    // posZ -= Math.sin(angle) * step;
  }
  if (KID(87)) {  // W (Forward into screen)
    posX += Math.sin(angle) * step;   
    posZ += Math.cos(angle) * step; 
    isWalking = true;
  }
  if (KID(83)){   // S (Backward out of screen)
    posX -= Math.sin(angle) * step; 
    posZ -= Math.cos(angle) * step;
    isWalking = true;
  }
  if (KID(32)) posY -= step; // SPACE (Fly Up)
  if (KID(16)) posY += step; // SHIFT (Fly Down)

  // ── Robot Y rotation ────────────────────────────────────────────────────
  if (KID(81)) angle -= rot; // Q
  if (KID(69)) angle += rot; // E

  // ── Scale ────────────────────────────────────────────────────────────────
  if (KID(90)) scaleFactor *= 0.9;     // Z
  if (KID(88)) scaleFactor *= (10/9);  // X

  // ── Head (arrow keys only when not following mouse) ───────────────────────────────────────────────────
  if (!headFollowMouse) {
    if (KID(UP_ARROW))    angleHeadX -= rot;
    if (KID(DOWN_ARROW))  angleHeadX += rot;
    if (KID(LEFT_ARROW))  angleHeadY -= rot;
    if (KID(RIGHT_ARROW)) angleHeadY += rot;
  }
  
  // Clamp head rotation so it doesn't snap its neck
  angleHeadX = Math.max(-Math.PI/2, Math.min(Math.PI/2, angleHeadX));
  angleHeadY = Math.max(-Math.PI/2, Math.min(Math.PI/2, angleHeadY));

  // ── Torso (upper body only: torso, arms, head) ────────────────────────────
  if (KID(188)) angleTorsoY -= rot; // comma
  if (KID(190)) angleTorsoY += rot; // period
  if (KID(57)) angleTorsoX -= rot; // 9 — lean back
  if (KID(48)) angleTorsoX += rot; // 0 — lean forward
  angleTorsoY = Math.max(-Math.PI / 2.2, Math.min(Math.PI / 2.2, angleTorsoY));
  angleTorsoX = Math.max(-Math.PI / 3, Math.min(Math.PI / 3, angleTorsoX));


  // ── Arm articulation ────────────────────────────────────────────────────
  if (KID(82)) angleShoulder -= rot; // R
  if (KID(70)) angleShoulder += rot; // F
  if (KID(84)) angleElbow    -= rot; // T
  if (KID(71)) angleElbow    += rot; // G

  // Clamp arm joints so they don't spin 360 degrees
  angleShoulder = Math.max(-Math.PI, Math.min(Math.PI, angleShoulder));
  angleElbow    = Math.max(-Math.PI/1.2, Math.min(Math.PI/1.2, angleElbow));

  // ── Leg articulation ────────────────────────────────────────────────────
  if (KID(89)) angleHip  -= rot; // Y
  if (KID(72)) angleHip  += rot; // H
  if (KID(85)) angleKnee -= rot; // U
  if (KID(78)) angleKnee += rot; // N

  // Clamp leg joints so they behave like real mechanics
  angleHip  = Math.max(-Math.PI/1.5, Math.min(Math.PI/1.5, angleHip));
  angleKnee = Math.max(-Math.PI/1.2, Math.min(Math.PI/1.2, angleKnee));


  // ── Left leg fine control (additive on top of Y/H/U/N) ───────────────────
  if (KID(53)) angleHipLAdjust -= rot; // 5
  if (KID(54)) angleHipLAdjust += rot; // 6
  if (KID(55)) angleKneeLAdjust -= rot; // 7
  if (KID(56)) angleKneeLAdjust += rot; // 8
  angleHipLAdjust = Math.max(
    -Math.PI / 1.5,
    Math.min(Math.PI / 1.5, angleHipLAdjust),
  );
  angleKneeLAdjust = Math.max(
    -Math.PI / 1.2,
    Math.min(Math.PI / 1.2, angleKneeLAdjust),
  );

  // ── Spotlight (cone + edge falloff) ─────────────────────────────────────
  if (KID(219)) spotConeAngle = Math.max(0.12, spotConeAngle - spotStep); // [
  if (KID(221)) spotConeAngle = Math.min(Math.PI / 2, spotConeAngle + spotStep); // ]
  if (KID(189)) spotConcentration = Math.max(1, spotConcentration - concStep); // - softer edge
  if (KID(187)) spotConcentration = Math.min(200, spotConcentration + concStep); // = sharper

  // ── Camera orbit ────────────────────────────────────────────────────────
  if (KID(73)) camAngleX -= rot; // I
  if (KID(75)) camAngleX += rot; // K
  if (KID(74)) camAngleY -= rot; // J
  if (KID(76)) camAngleY += rot; // L

  // ── Camera zoom ─────────────────────────────────────────────────────────
  if (KID(79)) camZ -= step * 2; // O
  if (KID(80)) camZ += step * 2; // P
  if (KID(86)) jumpT === 0 ? (jumpT = 0.0001) : null; // Jump (V)

  if (KID(36)) {
    // HOME BUTTON
    posX = posY = posZ = 0;
    angle = 0;
    scaleFactor = 100;
    camZ = 900;
    camAngleX = camAngleY = 0;
    angleShoulder = angleElbow = 0;
    angleHip = angleKnee = 0;
    angleHeadX = angleHeadY = 0;
    angleTorsoX = angleTorsoY = 0;
    angleHipLAdjust = angleKneeLAdjust = 0;
    spotConeAngle = Math.PI / 3;
    spotConcentration = 50;
    jumpT = 0;
    jumpYOffset = 0;
    jumpKneeExtra = 0;
    jumpHipExtra = 0;
    jumpShinMul = 1;
    resetHeadFollowMouse(); // following mouse is off
    playResetSound();
  }




  if (isWalking) walkCycle += walkSpeed;

}