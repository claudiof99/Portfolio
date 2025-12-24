const express = require('express');
const router = express.Router();
const kpiController = require('../controllers/kpiController');
const { isAuthenticated, requireRole } = require('../middlewares/auth');

// Safety Check
if (!kpiController.approvals) {
    console.error("❌ CRITICAL ERROR: 'approvals' function is missing in kpiController.js!");
}

// 1. APPROVALS
router.get('/approvals', isAuthenticated, kpiController.approvals);
router.post('/approvals/approve/:id', isAuthenticated, kpiController.approveKpi);

// 2. VIEWING
router.get('/', isAuthenticated, kpiController.index);

// 3. EDITING / ADDING DATA
router.post('/data/:id', isAuthenticated, kpiController.addData);

// 4. ADMIN ACTIONS
router.post('/add', isAuthenticated, kpiController.add);
router.post('/data/:kpiId/remove/:date', isAuthenticated, kpiController.removeData);

//new routes
router.post('/data/:kpiId/restore/:date', isAuthenticated, kpiController.restoreData); 
router.post('/data/:id/edit/:oldDate', isAuthenticated, kpiController.updateDataPoint);
router.post('/data/:id/delete/:date', isAuthenticated, requireRole('admin'), kpiController.deleteDataPoint); 

// 5. VIEW SINGLE KPI
router.get('/:id', isAuthenticated, kpiController.show);

router.post('/update/:id', isAuthenticated, requireRole(['quality_manager', 'admin']), kpiController.update);

module.exports = router;