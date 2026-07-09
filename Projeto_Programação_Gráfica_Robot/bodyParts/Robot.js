// Model-space Y where torso meets pelvis (~torso top y=+1, pelvis center 1.12); hinge for lean/twist
const WAIST_PIVOT_Y = 1.0;

function drawRobot(mParent, pelvisZ = 0, pelvisY = 0, hipR, hipL, kneeR, kneeL, shoulderR, shoulderL) {

  // Rotate upper body around waist so chest/head move but the band at the pelvis stays aligned
  let mWaistHinge = multiplyMatrix(
    getTranslate_Matrix(0, WAIST_PIVOT_Y, 0),
    multiplyMatrix(
      getRotateY_Matrix(angleTorsoY),
      multiplyMatrix(
        getRotateX_Matrix(angleTorsoX),
        getTranslate_Matrix(0, -WAIST_PIVOT_Y, 0),
      ),
    ),
  );
  let mUpper = multiplyMatrix(mParent, mWaistHinge);

  drawPelvis(mParent, pelvisZ, pelvisY);
  drawWaistFiller(mParent, WAIST_PIVOT_Y);
  drawTorso(mUpper);
  drawHead(mUpper);
  drawRightArm(mUpper, shoulderR);
  drawLeftArm(mUpper, shoulderL);
  drawRightLeg(mParent, hipR, kneeR);
  drawLeftLeg(mParent, hipL, kneeL);
}


