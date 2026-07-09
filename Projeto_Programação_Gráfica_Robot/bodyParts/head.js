let eyeGap = 0.40;

function drawHead(mParent) {
  emissiveMaterial(0, 0, 0);

  // --- NECK & HEAD ---
  specularMaterial(150); 
  shininess(50);
  
  let mNeck = multiplyMatrix(mParent,
    multiplyMatrix(getTranslate_Matrix(0, -1.15, 0), getScaleMatrix(0.09, 0.14, 0.09)));
  texture(texrobot); 
  drawObject(vertexBox, uvBox, mNeck);

  let mHead = multiplyMatrix(mParent,
    multiplyMatrix(getTranslate_Matrix(0, -1.4, 0),
      multiplyMatrix(getRotateX_Matrix(angleHeadX),
        multiplyMatrix(getRotateY_Matrix(angleHeadY), getScaleMatrix(0.4, 0.4, 0.4)))));
  texture(texBody);
  drawObject(vertexBox, uvBox, mHead);

  // --- EYES (Emissive OFF for true texture) ---
  emissiveMaterial(0, 0, 0); 
  specularMaterial(255);     
  shininess(100);
  
  let mEyeL = multiplyMatrix(mHead,
    multiplyMatrix(getTranslate_Matrix(-eyeGap, -0.1, 1.0), getScaleMatrix(0.25, 0.18, 0.01)));
  texture(texEye); 
  drawObject(vertexBox, uvBox, mEyeL);

  let mEyeR = multiplyMatrix(mHead,
    multiplyMatrix(getTranslate_Matrix(eyeGap, -0.1, 1.0), getScaleMatrix(0.25, 0.18, 0.01)));
  texture(texEye);
  drawObject(vertexBox, uvBox, mEyeR);

  // DIGITAL MOUTH ---
  // Positioned below the eyes, using the red LED texture
  let mMouth = multiplyMatrix(mHead,
    multiplyMatrix(getTranslate_Matrix(0, 0.5, 1.0), getScaleMatrix(0.4, 0.05, 0.01)));
  texture(texEye); 
  drawObject(vertexBox, uvBox, mMouth);

  // --- DETAILS ---
  specularMaterial(40);      
  shininess(5);

  let mChin = multiplyMatrix(mHead,
    multiplyMatrix(getTranslate_Matrix(0, 0.35, 0.58), getScaleMatrix(0.42, 0.12, 0.05)));
  texture(texJoint);
  drawObject(vertexBox, uvBox, mChin);

  let mEarL = multiplyMatrix(mHead,
    multiplyMatrix(getTranslate_Matrix(-1.15, 0.05, 0), getScaleMatrix(0.18, 0.45, 0.38)));
  texture(texJoint);
  drawObject(vertexBox, uvBox, mEarL);

  let mEarR = multiplyMatrix(mHead,
    multiplyMatrix(getTranslate_Matrix(1.15, 0.05, 0), getScaleMatrix(0.18, 0.45, 0.38)));
  texture(texJoint);
  drawObject(vertexBox, uvBox, mEarR);

  let mAntBase = multiplyMatrix(mHead,
    multiplyMatrix(getTranslate_Matrix(0.28, -0.82, 0), getScaleMatrix(0.05, 0.28, 0.05)));
  texture(texJoint);
  drawObject(vertexBox, uvBox, mAntBase);

  let mAntTop = multiplyMatrix(mHead,
    multiplyMatrix(getTranslate_Matrix(0.31, -1.18, 0),
      multiplyMatrix(getRotateZ_Matrix(0.12), getScaleMatrix(0.038, 0.24, 0.038))));
  texture(texLEDEye);
  drawObject(vertexBox, uvBox, mAntTop);

  // --- ANTENNA TIP ---
  specularMaterial(255);
  let mAntTip = multiplyMatrix(mHead,
    multiplyMatrix(getTranslate_Matrix(0.34, -1.46, 0), getScaleMatrix(0.08, 0.08, 0.08)));
  texture(texAccent); 
  drawObject(vertexBox, uvBox, mAntTip);
}