function drawLeftLeg(mParent, hipL, kneeL){
  // --- Hip joint (Matte) ---
  specularMaterial(40); 
  shininess(5);
  let mLHip = multiplyMatrix(mParent,
    multiplyMatrix(getTranslate_Matrix(-0.24, 1.3, 0), getScaleMatrix(0.16, 0.16, 0.16)));
  texture(texJoint);
  drawObject(vertexBox, uvBox, mLHip);

  let mThinghRoot = multiplyMatrix(mParent,
    multiplyMatrix(getTranslate_Matrix(-0.24, 1.82, -0.20),
      getRotateX_Matrix(hipL)));

  // --- Thigh (Shiny Metal) ---
  specularMaterial(150); 
  shininess(50);
  let mLThigh = multiplyMatrix(mThinghRoot,
    multiplyMatrix(getTranslate_Matrix(0, 0, 0.25), getScaleMatrix(0.17, 0.76, 0.17)));
  texture(texBody);
  drawObject(vertexBox, uvBox, mLThigh);

   // --- Thigh armor plate (Shiny Metal) ---
  specularMaterial(150); 
  shininess(50);
  let mLThighPlate = multiplyMatrix(mThinghRoot,
    multiplyMatrix(getTranslate_Matrix(0, 0, 0.29), getScaleMatrix(0.13, 0.5, 0.04)));
  texture(texBodyPlate);
  drawObject(vertexBox, uvBox, mLThighPlate);
  
  
  // --- Knee cap (Shiny Metal) ---
  specularMaterial(150); 
  shininess(50);
  let mLKneeCap = multiplyMatrix(mThinghRoot,
    multiplyMatrix(getTranslate_Matrix(0, 0.83, 0.36), getScaleMatrix(0.16, 0.12, 0.05)));
  texture(texBodyPlate);
  drawObject(vertexBox, uvBox, mLKneeCap);

  let mKneeRoot = multiplyMatrix(mThinghRoot,
    multiplyMatrix(getTranslate_Matrix(0, 0.83, 0),
      getRotateX_Matrix(kneeL)));

  // --- Knee joint (Matte) ---
  specularMaterial(40); 
  shininess(5);
  let mLKnee = multiplyMatrix(mThinghRoot,
    multiplyMatrix(getTranslate_Matrix(0, 0.83, 0.20), getScaleMatrix(0.15, 0.15, 0.15)));
  texture(texJoint);
  drawObject(vertexBox, uvBox, mLKnee);

  // --- Shin (Matte) ---
  specularMaterial(40); 
  shininess(5);
  let mLShin = multiplyMatrix(mKneeRoot,
    multiplyMatrix(getTranslate_Matrix(0, 0.55, 0.20), getScaleMatrix(0.14, 0.72, 0.14)));
  texture(texJoint);
  drawObject(vertexBox, uvBox, mLShin);

  // --- Shin armor plate (Shiny Metal) ---
  specularMaterial(150); 
  shininess(50);
  let mLShinPlate = multiplyMatrix(mKneeRoot,
    multiplyMatrix(getTranslate_Matrix(0, 0.55, 0.26), getScaleMatrix(0.10, 0.55, 0.04)));
  texture(texBodyPlate);
  drawObject(vertexBox, uvBox, mLShinPlate);

  // --- Ankle block / Foot (Shiny Metal) ---
  specularMaterial(150); 
  shininess(50);
  let mLAnkle = multiplyMatrix(mKneeRoot,
    multiplyMatrix(getTranslate_Matrix(0, 1.3, 0.20), getScaleMatrix(0.16, 0.10, 0.16)));
  texture(texBody);
  drawObject(vertexBox, uvBox, mLAnkle);
}