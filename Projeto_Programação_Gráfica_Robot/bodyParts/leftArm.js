function drawLeftArm(mParent, shoulderL) {
  // --- Shoulder pad (Shiny Metal) ---
  specularMaterial(150); 
  shininess(50);
  let mLSPad = multiplyMatrix(
    mParent,
    multiplyMatrix(
      getTranslate_Matrix(-0.68, -0.68, 0), 
      getScaleMatrix(0.22, 0.07, 0.24)
    )
  );
  texture(texBody);
  drawObject(vertexBox, uvBox, mLSPad);

  // --- Arm Mechanics (Matte Carbon Fiber) ---
  specularMaterial(40);
  shininess(5);
  let mLSJoint = multiplyMatrix(
    mParent,
    multiplyMatrix(
      getTranslate_Matrix(-0.64, -0.52, 0), 
      getScaleMatrix(0.14, 0.14, 0.14)
    )
  );
  texture(texJoint);
  drawObject(vertexBox, uvBox, mLSJoint);

  let mArmRoot = multiplyMatrix(
    mParent,
    multiplyMatrix(
      getTranslate_Matrix(-0.68, -0.18, 0),
      getRotateX_Matrix(shoulderL)
    )
  );

  let mLArm = multiplyMatrix(
    mArmRoot,
    // multiplyMatrix(getTranslate_Matrix(-0.68, -0.18, 0),
    getScaleMatrix(0.15, 0.58, 0.15)
  );
  texture(texJoint);
  drawObject(vertexBox, uvBox, mLArm);

  let mRElbowRoot = multiplyMatrix(
    mArmRoot,
    multiplyMatrix(
      getTranslate_Matrix(0, 0.62, 0),
      getRotateX_Matrix(angleElbow)
    )
  );

  let mLElbow = multiplyMatrix(
    mArmRoot,
    multiplyMatrix(
      getTranslate_Matrix(0, 0.62, 0), 
      getScaleMatrix(0.13, 0.13, 0.13)
    )
  );
  texture(texJoint);
  drawObject(vertexBox, uvBox, mLElbow);

  let mLFore = multiplyMatrix(
      mRElbowRoot,
      multiplyMatrix(getTranslate_Matrix(0, 0.28, 0), 
      getScaleMatrix(0.12, 0.48, 0.12)
    )
  );
  texture(texJoint);
  drawObject(vertexBox, uvBox, mLFore);

  // --- Palm  ---
  specularMaterial(180); 
  shininess(60);
  let mLHand = multiplyMatrix(
      mRElbowRoot,
      multiplyMatrix(getTranslate_Matrix(0, 0.84, 0), 
      getScaleMatrix(0.20, 0.12, 0.14)
    )
  );
  texture(texHand); // Using the gold/plastic texture here
  drawObject(vertexBox, uvBox, mLHand);

  // --- Fingers (Matte) ---
  specularMaterial(40);
  shininess(5);
  let fX = [-0.14, -0.03, 0.07, 0.16];
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