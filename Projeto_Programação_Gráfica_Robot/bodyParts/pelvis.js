function drawPelvis(mParent, pelvisZ = 0, pelvisY = 0) {
  // PELVIS: Matte Carbon Fiber
  specularMaterial(40); 
  shininess(5);

  let mPelvisRoot = multiplyMatrix(
    mParent,
    multiplyMatrix(
      getTranslate_Matrix(0, 1.12, 0),
      multiplyMatrix(
        getTranslate_Matrix(0, pelvisY, 0),
        getRotateX_Matrix(pelvisZ)
      )
    )
  );
  
  let mPelvis = multiplyMatrix(
    mPelvisRoot,
    getScaleMatrix(0.4, 0.16, 0.2)
  );
  texture(texJoint);
  drawObject(vertexBox, uvBox, mPelvis);
}