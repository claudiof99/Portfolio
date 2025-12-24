const express = require('express');
const router = express.Router();

const complaintController = require('../controllers/complaintcontroller');
const { isAuthenticated } = require('../middlewares/auth');

router.get('/complaints', isAuthenticated, complaintController.list);
router.get('/complaints/new', isAuthenticated, complaintController.newForm);
router.post('/complaints', isAuthenticated, complaintController.create);
router.get('/complaints/:id', isAuthenticated, complaintController.view);
router.get('/complaints/:id/edit', isAuthenticated, complaintController.editForm);
router.put('/complaints/:id', isAuthenticated, complaintController.update);
router.delete('/complaints/:id', isAuthenticated, complaintController.delete);

module.exports = router;
