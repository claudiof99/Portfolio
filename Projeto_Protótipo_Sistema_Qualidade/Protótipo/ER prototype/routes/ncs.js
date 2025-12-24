const express = require('express');
const router = express.Router();
const ncController = require('../controllers/ncController');
const multer = require('multer');
const path = require('path');
const fs = require('fs');
const { isAuthenticated } = require('../middlewares/auth'); 

// --- Multer Config ---
const storage = multer.diskStorage({
  destination: function (req, file, cb) {
    const uploadPath = 'public/uploads/';
    if (!fs.existsSync(uploadPath)){
        fs.mkdirSync(uploadPath, { recursive: true });
    }
    cb(null, uploadPath);
  },
  filename: function (req, file, cb) {
    cb(null, Date.now() + path.extname(file.originalname));
  }
});
const upload = multer({ storage: storage });


router.get('/', isAuthenticated, ncController.listarNcs);


router.post('/', isAuthenticated, upload.single('evidencia'), ncController.novaNc);


router.post('/delete/:id', isAuthenticated, ncController.deleteNc);

router.get('/history', isAuthenticated, ncController.history);

module.exports = router;