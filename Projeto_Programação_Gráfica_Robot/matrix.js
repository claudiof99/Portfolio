function getIdentity_Matrix() {
  return [
    [1,0,0,0],
    [0,1,0,0],
    [0,0,1,0],
    [0,0,0,1],
  ];
}

function multiplyMatrix(m1, m2) {
  const rows1 = m1.length;
  const cols1 = m1[0].length;
  const cols2 = m2[0].length;
  const res = [];
  for (let i = 0; i < rows1; i++) {
    res[i] = [];
    for (let j = 0; j < cols2; j++) {
      let sum = 0;
      for (let k = 0; k < cols1; k++) {
        sum += m1[i][k] * m2[k][j];
      }
      res[i][j] = sum;  
    }
  }
  return res;
}

function multiplyMatrixVertex(m, v) {
  return [
    m[0][0]*v[0] + m[0][1]*v[1] + m[0][2]*v[2] + m[0][3]*v[3],
    m[1][0]*v[0] + m[1][1]*v[1] + m[1][2]*v[2] + m[1][3]*v[3],
    m[2][0]*v[0] + m[2][1]*v[1] + m[2][2]*v[2] + m[2][3]*v[3],
    1
  ];
}
function getRotateX_Matrix(angle) {
  let ca = Math.cos(angle);
  let sa = Math.sin(angle);
  return [
    [1,0,0,0],
    [0,ca,-sa,0],
    [0,sa,ca,0],
    [0,0,0,1]
  ];
}

function getRotateY_Matrix(angle) {
  let ca = Math.cos(angle);
  let sa = Math.sin(angle);
  return [
    [ca,0,sa,0],
    [0,1,0,0],
    [-sa,0,ca,0],
    [0,0,0,1]
  ];
}

function getRotateZ_Matrix(angle) {
  let ca = Math.cos(angle);
  let sa = Math.sin(angle);
  return [
    [ca,-sa,0,0],
    [sa,ca,0,0],
    [0,0,1,0],
    [0,0,0,1]
  ];
}

function getTranslate_Matrix(xt, yt, zt) {
  return [
    [1,0,0,xt],
    [0,1,0,yt],
    [0,0,1,zt],
    [0,0,0,1]
  ];
}

function getScaleMatrix(sx, sy, sz) {
  if (sy === undefined) { sy = sx; sz = sx; }
  return [
    [sx,0,0,0],
    [0,sy,0,0],
    [0,0,sz,0],
    [0,0,0,1]
  ];
}