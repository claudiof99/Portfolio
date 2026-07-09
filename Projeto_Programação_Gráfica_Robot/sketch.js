// sketch.js

let posX = 0;
let posY = 0;
let posZ = 0;
let angle = 0;
let scaleFactor = 100;

let camZ      = 900;
let camAngleX = 0;
let camAngleY = 0;

let angleShoulder = 0;
let angleElbow    = 0;
let angleHip      = 0;
let angleKnee     = 0;
let angleHeadX    = 0;
let angleHeadY    = 0;

// Upper-body lean / twist (torso, arms, head); legs stay on base pose
let angleTorsoX = 0;
let angleTorsoY = 0;

// Extra motion on the left leg only (right leg still follows angleHip / angleKnee from Y/H/U/N)
let angleHipLAdjust = 0;
let angleKneeLAdjust = 0;

// Spotlight shape (cone angle & edge softness); position still follows robot
let spotConeAngle = Math.PI / 3;
let spotConcentration = 50;


let walkCycle = 0;
let walkSpeed = 0.08;
let walkAmp = 0.6;
let isWalking = false;


// ─── Texture Variables ──────────────────────────────────────────────────
// Declaring variables for the textures used in the robot's material pipeline.
// These represent different surface types (shiny metal, matte carbon fiber, glowing LEDs).
let texBody,
  texEye,
  texLEDEye,
  texJoint,
  texHand,
  texAccent,
  texSciFi,
  texBodyPlate,
  texrobot;

function preload() {
  // --- 1. High-Gloss Armor (Shiny Metal Surfaces) ---
  // Used with high specular reflection for the main structural pieces.
  texBody = loadImage("textures/pastel-gray.jpg"); // Main armor: Head, Torso, Palms, Thighs, Feet

  // --- 2. Matte Mechanics (Non-Metallic Surfaces) ---
  // Used with low specular reflection to simulate dull, flexible carbon fiber.
  texJoint = loadImage("textures/blackCarbon.jpg"); // Mechanical joints: Shoulders, Elbows, Knees, Pelvis, Fingers

  // --- 3. Shiny Armor Plates (Secondary Details) ---
  // Reusing the carbon fiber image but applying shiny metallic lighting to it.
  texBodyPlate = loadImage("textures/blackCarbon.jpg"); // Hard armor plates: Chest plate, Knee caps, Shin plates, Outer core

  // --- 4. Emissive / Glowing Elements ---
  // Used with emissiveMaterial(0,0,0) and high shine to let the pure colors pop.
  texEye = loadImage("textures/redTextureLED.jpg"); // Glowing red LED ring for the eyes
  texLEDEye = loadImage("textures/LED_EYE.jpg"); // Blue/purple noise for the inner chest reactor core
  texAccent = loadImage("textures/fire.jpg"); // Orange fire texture for the antenna tip and reactor aura

  // --- 5. Specific Component Details ---
  texrobot = loadImage("textures/robobodytex.png"); // Metallic ribbed detail specifically for the neck

  // --- 6. Alternate Textures  ---
  texHand = loadImage("textures/PlasticSpace.jpg"); // used for wrist and palm details, providing a contrasting smooth plastic look against the metallic body

  preloadResetSound();
  preloadOpeningSound();
}

let vertexBox, uvBox;

function setup() {
  createCanvas(windowWidth, windowHeight, WEBGL);
  textureMode(NORMAL);
  const mesh = getVertexBox(cubeVerts, cubeIndexs, faceUVs);
  vertexBox = mesh.vertexBox;
  uvBox     = mesh.uvBox;
}

function draw() {
	
	doKeys()
  updateHeadFollowMouse();
  updateJumpAnim();
	
  background(18, 18, 28);

 // ── Lighting ──────────────────────────────────────────────────────────────
  // Lower base visibility so the textures don't wash out
  ambientLight(100, 100, 110); 

  // Just ONE soft directional light 
  directionalLight(120, 120, 130,  0.5, 0.5, -1.0); 

  //SPOTLIGHT (Strong, but balanced)
  spotLight(
    200,
    200,
    200,
    posX,
    posY + jumpYOffset - 400,
    posZ + 500,
    0,
    0.5,
    -1,
    spotConeAngle,
    spotConcentration,
  );

  // ── Matrices ──────────────────────────────────────────────────────────────
  let mTranslate  = getTranslate_Matrix(posX, posY + jumpYOffset, posZ);
  let mRotate     = getRotateY_Matrix(angle);
  let mScale      = getScaleMatrix(scaleFactor);
  let mModelRobot = multiplyMatrix(mTranslate, multiplyMatrix(mRotate, mScale));

  let mFocus = getTranslate_Matrix(-posX, -posY, -posZ);
  let mCamTransl  = getTranslate_Matrix(0, 0, -camZ);
  let mCamRotateX = getRotateX_Matrix(-camAngleX);
  let mCamRotateY = getRotateY_Matrix(-camAngleY);
  let mView       = multiplyMatrix(mCamTransl,
                      multiplyMatrix(mCamRotateX,
                        multiplyMatrix(mCamRotateY, mFocus)));

  let mMV_Robot = multiplyMatrix(mView, mModelRobot);

  let autoPelvisZ = Math.sin(walkCycle) * 0.08;
  let autoPelvisY = Math.abs(Math.sin(walkCycle)) * 0.08;

  let autoHipR = Math.sin(walkCycle) * walkAmp;
  let autoHipL = -Math.sin(walkCycle) * walkAmp;
  let autoKneeR = Math.max(0, Math.sin(walkCycle + Math.PI / 4)) * walkAmp * 0.7;
  let autoKneeL = -Math.max(0, Math.sin(walkCycle + Math.PI + Math.PI / 4)) * walkAmp * 0.7;
  
  let autoShoulderR = -Math.sin(walkCycle) * walkAmp * 0.5;
  let autoShoulderL = Math.sin(walkCycle) * walkAmp * 0.5;


  drawAxes(mView);
  drawRobot(mMV_Robot,
    isWalking ? autoPelvisZ : 0,
    isWalking ? autoPelvisY : 0,
    isWalking ? autoHipR : angleHip,
    isWalking ? autoHipL : -angleHip,
    isWalking ? autoKneeR : angleKnee,
    isWalking ? autoKneeL : -angleKnee,
    isWalking ? autoShoulderR : angleShoulder,
    isWalking ? autoShoulderL : -angleShoulder
  );
}

function windowResized() {
  resizeCanvas(windowWidth, windowHeight);
}