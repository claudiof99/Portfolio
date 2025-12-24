const fs = require('fs');
const path = require('path');

// Base folder for all JSON files
const DATA_DIR = __dirname; // __dirname points to models folder

function read(file) {
  const filepath = path.join(DATA_DIR, file);
  if (!fs.existsSync(filepath)) {
    console.log('File not found:', filepath);
    return [];
  }
  const raw = fs.readFileSync(filepath, 'utf8');
  try {
    return JSON.parse(raw);
  } catch (e) {
    console.log('JSON parse error in', file, e);
    return [];
  }
}

function write(file, data) {
  const filepath = path.join(DATA_DIR, file);
  fs.writeFileSync(filepath, JSON.stringify(data, null, 2), 'utf8');
}

function getNextId(collection) {
  if (!collection || collection.length === 0) return 1;
  return Math.max(...collection.map(i => i.id || 0)) + 1;
}

module.exports = { read, write, getNextId };
