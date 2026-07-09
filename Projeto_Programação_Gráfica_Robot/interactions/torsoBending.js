// Soft waist between pelvis (mParent) and torso (mUpper): stacked partial hinges blend the bend

function drawWaistFiller(mParent, waistPivotY) {
  const layers = [
    { frac: 0.22, y: waistPivotY - 0.14, sy: 0.1 },
    { frac: 0.5, y: waistPivotY - 0.02, sy: 0.11 },
    { frac: 0.78, y: waistPivotY + 0.1, sy: 0.1 },
  ];
  specularMaterial(40);
  shininess(8);
  texture(texJoint);
  for (let i = 0; i < layers.length; i++) {
    const { frac, y, sy } = layers[i];
    let mH = multiplyMatrix(
      getTranslate_Matrix(0, waistPivotY, 0),
      multiplyMatrix(
        getRotateY_Matrix(angleTorsoY * frac),
        multiplyMatrix(
          getRotateX_Matrix(angleTorsoX * frac),
          getTranslate_Matrix(0, -waistPivotY, 0),
        ),
      ),
    );
    let mSeg = multiplyMatrix(
      mParent,
      multiplyMatrix(
        mH,
        multiplyMatrix(getTranslate_Matrix(0, y, 0), getScaleMatrix(0.38, sy, 0.21)),
      ),
    );
    drawObject(vertexBox, uvBox, mSeg);
  }
}
