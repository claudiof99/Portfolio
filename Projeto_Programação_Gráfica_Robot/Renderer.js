
// Draws a triangulated object and computes face Normals for perfect lighting!

function drawObject(_vertex, _uv, matrix) {
  noStroke();   // stroke obscures texture edges
  beginShape(TRIANGLES);
  
  // Process 3 vertices (1 full triangle) at a time
  for (let i = 0, j = 0; i < _vertex.length; i += 9, j += 6) {

    // 1. Transform the 3 vertices
    let v1 = multiplyMatrixVertex(matrix, [_vertex[i],   _vertex[i+1], _vertex[i+2], 1]);
    let v2 = multiplyMatrixVertex(matrix, [_vertex[i+3], _vertex[i+4], _vertex[i+5], 1]);
    let v3 = multiplyMatrixVertex(matrix, [_vertex[i+6], _vertex[i+7], _vertex[i+8], 1]);

    // 2. Compute vectors U and V
    let U = [v2[0] - v1[0], v2[1] - v1[1], v2[2] - v1[2]];
    let V = [v3[0] - v1[0], v3[1] - v1[1], v3[2] - v1[2]];

    // 3. Cross Product to find the Normal
    let nx = U[1]*V[2] - U[2]*V[1];
    let ny = U[2]*V[0] - U[0]*V[2];
    let nz = U[0]*V[1] - U[1]*V[0];

    // 4. Normalize the vector
    let len = Math.sqrt(nx*nx + ny*ny + nz*nz);
    if (len > 0) {
      nx /= len; ny /= len; nz /= len;
    }

    // 5. Apply the normal
    normal(nx, ny, nz);

    // 6. Draw the vertices
    vertex(v1[0], v1[1], v1[2], _uv[j],   _uv[j+1]);
    vertex(v2[0], v2[1], v2[2], _uv[j+2], _uv[j+3]);
    vertex(v3[0], v3[1], v3[2], _uv[j+4], _uv[j+5]);
  }
  endShape();
}

// Draws XYZ world axes for debugging.
function drawAxes(matrix) {
  push();
  strokeWeight(1);
  let axisLines = [
    [-2000, 0, 0,  2000, 0, 0,  255, 0,   0  ], // X - Red
    [0, -2000, 0,  0, 2000, 0,  0,   0,   255], // Y - Blue
    [0, 0, -2000,  0, 0, 2000,  0,   255, 0  ], // Z - Green
  ];

  for (let p of axisLines) {
    let x1 = matrix[0][0]*p[0] + matrix[0][1]*p[1] + matrix[0][2]*p[2] + matrix[0][3];
    let y1 = matrix[1][0]*p[0] + matrix[1][1]*p[1] + matrix[1][2]*p[2] + matrix[1][3];
    let z1 = matrix[2][0]*p[0] + matrix[2][1]*p[1] + matrix[2][2]*p[2] + matrix[2][3];

    let x2 = matrix[0][0]*p[3] + matrix[0][1]*p[4] + matrix[0][2]*p[5] + matrix[0][3];
    let y2 = matrix[1][0]*p[3] + matrix[1][1]*p[4] + matrix[1][2]*p[5] + matrix[1][3];
    let z2 = matrix[2][0]*p[3] + matrix[2][1]*p[4] + matrix[2][2]*p[5] + matrix[2][3];

    stroke(p[6], p[7], p[8]);
    line(x1, y1, z1, x2, y2, z2);
  }
  pop();
}