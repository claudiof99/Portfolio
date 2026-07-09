function drawRightArm(mParent, shoulderR) {
  // --- Shoulder pad (Shiny Metal) ---
  specularMaterial(150);
  shininess(50);
  let mRSPad = multiplyMatrix(
    mParent,
    multiplyMatrix(
      getTranslate_Matrix(0.68, -0.68, 0), 
      getScaleMatrix(0.22, 0.07, 0.24)
    )
  );
  texture(texBody);
  drawObject(vertexBox, uvBox, mRSPad);

  // --- Arm Mechanics ---
  specularMaterial(40);
  shininess(5);
  let mRSJoint = multiplyMatrix(
    mParent,
    multiplyMatrix(
      getTranslate_Matrix(0.64, -0.52, 0), 
      getScaleMatrix(0.14, 0.14, 0.14)
    )
  );
  texture(texJoint);
  drawObject(vertexBox, uvBox, mRSJoint);

  let mArmRoot = multiplyMatrix(
    mParent,
    multiplyMatrix(
      getTranslate_Matrix(0.68, -0.18, 0),
      getRotateX_Matrix(shoulderR)
    )
  );

  let mRArm = multiplyMatrix(
    mArmRoot, getScaleMatrix(0.15, 0.58, 0.15)
  );
  texture(texJoint);
  drawObject(vertexBox, uvBox, mRArm);

  let mRElbowRoot = multiplyMatrix(
    mArmRoot,
    multiplyMatrix(
      getTranslate_Matrix(0, 0.62, 0),
      getRotateX_Matrix(angleElbow)
    )
  );

  let mRElbow = multiplyMatrix(
    mArmRoot,
    multiplyMatrix(
      getTranslate_Matrix(0, 0.62, 0), 
      getScaleMatrix(0.13, 0.13, 0.13)
    )
  );
  texture(texJoint);
  drawObject(vertexBox, uvBox, mRElbow);

  let mRFore = multiplyMatrix(
    mRElbowRoot,
    multiplyMatrix(
      getTranslate_Matrix(0, 0.28, 0), 
      getScaleMatrix(0.12, 0.48, 0.12)
    )
  );
  texture(texJoint);
  drawObject(vertexBox, uvBox, mRFore);
  
  // --- Palm  ---
  specularMaterial(180);
  shininess(60);
  let mRHand = multiplyMatrix(
      mRElbowRoot,
      multiplyMatrix(getTranslate_Matrix(0, 0.84, 0), 
      getScaleMatrix(0.20, 0.12, 0.14)
    )
  );
  texture(texHand); 
  drawObject(vertexBox, uvBox, mRHand);

  // --- Fingers ---
  specularMaterial(40);
  shininess(5);
  let fX = [0.14, -0.03, 0.07, 0.16];
  for (let i = 0; i < 4; i++) {
    let mF = multiplyMatrix(mRElbowRoot,
      multiplyMatrix(
        getTranslate_Matrix(fX[i], 1.02, 0), 
        getScaleMatrix(0.06, 0.08, 0.06)
      )
    );
    texture(texJoint);
    drawObject(vertexBox, uvBox, mF);
  }
}