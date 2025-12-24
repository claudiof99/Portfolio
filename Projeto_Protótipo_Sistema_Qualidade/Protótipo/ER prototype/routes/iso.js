const express = require('express');
const router = express.Router();
const isoController = require('../controllers/isoController');
const { isAuthenticated, requireRole } = require('../middlewares/auth');
const multer = require('multer');
const path = require('path');


const storage = multer.diskStorage({
    destination: (req, file, cb) => {
        
        cb(null, 'public/uploads'); 
    },
    filename: (req, file, cb) => {
        
        cb(null, Date.now() + path.extname(file.originalname));
    }
});
const upload = multer({ storage: storage });


// Main ISO Hub
router.get('/', isAuthenticated, isoController.index);

// Clause Pages
router.get('/context', isAuthenticated, isoController.context);       // Clause 4
router.get('/leadership', isAuthenticated, isoController.leadership); // Clause 5
router.get('/planning', isAuthenticated, isoController.planning);     // Clause 6
router.get('/support', isAuthenticated, isoController.support);       // Clause 7

// action routes

// Clause 4: Context Actions
router.post('/context/add', isAuthenticated, requireRole('quality_manager'), isoController.addStakeholder); 
router.post('/context/delete/:id', isAuthenticated, requireRole('quality_manager'), isoController.deleteStakeholder);

// Clause 5: Leadership Actions
router.post('/leadership/update', isAuthenticated, requireRole('quality_manager'), isoController.updatePolicy);

// Clause 6: Planning Actions
router.post('/planning/add', isAuthenticated, requireRole('quality_manager'), isoController.addRisk); 

// Clause 7: Support Actions (The Missing Part)
router.post('/support/upload', isAuthenticated, upload.single('document'), isoController.uploadDocument);
router.post('/support/delete/:id', isAuthenticated, requireRole('quality_manager'), isoController.deleteDocument);

// General Clause Update
router.post('/update-clause', isAuthenticated, requireRole('quality_manager'), isoController.updateClause);

module.exports = router;