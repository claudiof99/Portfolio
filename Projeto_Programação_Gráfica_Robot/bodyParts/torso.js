function drawTorso(mParent) {
  emissiveMaterial(0, 0, 0); 

  // --- TORSO ---
  let mTorso = multiplyMatrix(mParent,
    multiplyMatrix(
      getTranslate_Matrix(0, 0, 0), 
      getScaleMatrix(0.5, 1.0, 0.22)
    )
  );
  specularMaterial(255); 
  shininess(80);
  texture(texBody);
  drawObject(vertexBox, uvBox, mTorso);

  // --- Chest plate ---
  let mChestPlate = multiplyMatrix(
    mParent,
    multiplyMatrix(
      getTranslate_Matrix(0, -0.1, 0.24), 
      getScaleMatrix(0.36, 0.55, 0.04)
    )
  );
  specularMaterial(80);
  shininess(15);
  texture(texBodyPlate);
  drawObject(vertexBox, uvBox, mChestPlate);

  // --- Glowing aura + reactor cores (pulse from interactions/pulsingAura.js) ---
  const p = getReactorPulseState();
  let mGlow = multiplyMatrix(
    mParent,
    multiplyMatrix(
      getTranslate_Matrix(0, -0.08, p.glowZ), 
      getScaleMatrix(p.glowScale, p.glowScale, p.glowThick)
    )
  );
  emissiveMaterial(p.em * 1.15, p.em * 0.5, p.em * 0.12);
  specularMaterial(200 + 55 * p.pulse);     // Reflect the spotlight
  texture(texAccent); 
  drawObject(vertexBox, uvBox, mGlow);

  
  // --- Outer core ---
  let mOuter = multiplyMatrix(
    mParent,
    multiplyMatrix(
      getTranslate_Matrix(0, -0.08, p.zOuter),
      getScaleMatrix(
        0.13 * p.coreXYScale,
        0.13 * p.coreXYScale,
        p.outerThick,
      ),
    ),
  );
  emissiveMaterial(p.em * 1.15, p.em * 0.5, p.em * 0.12);
  specularMaterial(200 + 55 * p.pulse);
  shininess(55 + 45 * p.pulseSoft);
  texture(texBodyPlate); 
  drawObject(vertexBox, uvBox, mOuter);

  let mReactor = multiplyMatrix(
    mParent,
    multiplyMatrix(
      getTranslate_Matrix(0, -0.08, p.zInner),
      getScaleMatrix(
        0.09 * p.coreXYScale,
        0.09 * p.coreXYScale,
        p.innerThick,
      ),
    ),
  );
  emissiveMaterial(0, 0, 0); // Zero added color!
  // specularMaterial(255);
  texture(texLEDEye); // Your LED_EYE.jpg
  drawObject(vertexBox, uvBox, mReactor);
}