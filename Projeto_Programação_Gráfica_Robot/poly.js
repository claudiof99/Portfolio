// 8 vertices of the canonical cube
const cubeVerts = [
  [-1, -1,  1], [ 1, -1,  1], [ 1,  1,  1], [-1,  1,  1],
  [-1, -1, -1], [ 1, -1, -1], [ 1,  1, -1], [-1,  1, -1],
];

// 12 triangles (2 per face)
const cubeIndexs = [
  0, 1, 2,   0, 2, 3,   // front
  5, 4, 7,   5, 7, 6,   // back
  3, 2, 6,   3, 6, 7,   // top
  4, 5, 1,   4, 1, 0,   // bottom
  1, 5, 6,   1, 6, 2,   // right
  4, 0, 3,   4, 3, 7    // left
];

// Per-face UVs: 6 faces × 6 vertices × 2 components
const faceUVs = [
  [0,1, 1,1, 1,0,  0,1, 1,0, 0,0], // front
  [0,1, 1,1, 1,0,  0,1, 1,0, 0,0], // back
  [0,1, 1,1, 1,0,  0,1, 1,0, 0,0], // top
  [0,1, 1,1, 1,0,  0,1, 1,0, 0,0], // bottom
  [0,1, 1,1, 1,0,  0,1, 1,0, 0,0], // right
  [0,1, 1,1, 1,0,  0,1, 1,0, 0,0], // left
];

// Flattens cube geometry and UVs for drawObject
function getVertexBox(vertices, cubeIndexs, faceUVs) {
  const vertexBox = [];
  const uvBox = [];
  for (let f = 0; f < 6; f++) {
    for (let v = 0; v < 6; v++) {
      let idx = cubeIndexs[f*6 + v];
      let vert = vertices[idx];
      let uv = [faceUVs[f][v*2], faceUVs[f][v*2+1]];
      vertexBox.push(vert[0], vert[1], vert[2]);
      uvBox.push(uv[0], uv[1]);
    }
  }
  return {vertexBox, uvBox};
}