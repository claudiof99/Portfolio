function drawRightLeg(mParent, hipR, kneeR) {
  // --- Hip joint (Matte) ---
  specularMaterial(40); 
  shininess(5);
  let mRHip = multiplyMatrix(mParent,
    multiplyMatrix(getTranslate_Matrix(0.24, 1.3, 0), getScaleMatrix(0.16, 0.16, 0.16)));
  texture(texJoint);
  drawObject(vertexBox, uvBox, mRHip);

  let mThinghRoot = multiplyMatrix(mParent,
    multiplyMatrix(getTranslate_Matrix(0.24, 1.82, -0.20),
      getRotateX_Matrix(hipR)));

  // --- Thigh (Shiny Metal) ---
  specularMaterial(150); 
  shininess(50);
  let mRThigh = multiplyMatrix(mThinghRoot,
    multiplyMatrix(getTranslate_Matrix(0, 0, 0.25), getScaleMatrix(0.17, 0.76, 0.17)));
  texture(texBody);
  drawObject(vertexBox, uvBox, mRThigh);

  // --- Thigh armor plate (Shiny Metal) ---
  specularMaterial(150); 
  shininess(50);
  let mRThighPlate = multiplyMatrix(mThinghRoot,
    multiplyMatrix(getTranslate_Matrix(0, 0, 0.29), getScaleMatrix(0.13, 0.5, 0.04)));
  texture(texBodyPlate);
  drawObject(vertexBox, uvBox, mRThighPlate);

  // --- Knee cap (Shiny Metal) ---
  specularMaterial(150); 
  shininess(50);
  let mRKneeCap = multiplyMatrix(mThinghRoot,
    multiplyMatrix(getTranslate_Matrix(0, 0.83, 0.36), getScaleMatrix(0.16, 0.12, 0.05)));
  texture(texBodyPlate);
  drawObject(vertexBox, uvBox, mRKneeCap);

  let mKneeRoot = multiplyMatrix(mThinghRoot,
    multiplyMatrix(getTranslate_Matrix(0, 0.83, 0),
      getRotateX_Matrix(kneeR)));

  // --- Knee joint (Matte) ---
  specularMaterial(40); 
  shininess(5);
  let mRKnee = multiplyMatrix(mThinghRoot,
    multiplyMatrix(getTranslate_Matrix(0, 0.83, 0.20), getScaleMatrix(0.15, 0.15, 0.15)));
  texture(texJoint);
  drawObject(vertexBox, uvBox, mRKnee);

  // --- Shin (Matte) ---
  specularMaterial(40); 
  shininess(5);
  let mRShin = multiplyMatrix(mKneeRoot,
    multiplyMatrix(getTranslate_Matrix(0, 0.55, 0.20), getScaleMatrix(0.14, 0.72, 0.14)));
  texture(texJoint);
  drawObject(vertexBox, uvBox, mRShin);

  // --- Shin armor plate (Shiny Metal) ---
  specularMaterial(150); 
  shininess(50);
  let mRShinPlate = multiplyMatrix(mKneeRoot,
    multiplyMatrix(getTranslate_Matrix(0, 0.55, 0.26), getScaleMatrix(0.10, 0.55, 0.04)));
  texture(texBodyPlate);
  drawObject(vertexBox, uvBox, mRShinPlate);

  // --- Ankle block / Foot (Shiny Metal) ---
  specularMaterial(150); 
  shininess(50);
  let mRAnkle = multiplyMatrix(mKneeRoot,
    multiplyMatrix(getTranslate_Matrix(0, 1.3, 0.20), getScaleMatrix(0.16, 0.10, 0.16)));
  texture(texBody);
  drawObject(vertexBox, uvBox, mRAnkle);
}