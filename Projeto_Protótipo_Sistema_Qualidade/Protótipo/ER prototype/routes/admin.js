const express = require('express');
const router = express.Router();

const adminController = require('../controllers/admincontroller');
const { isAuthenticated, requireRole } = require('../middlewares/auth');

router.get('/users', isAuthenticated, requireRole('admin'), adminController.userList);
router.post('/users/:id/role', isAuthenticated, requireRole('admin'), adminController.updateUser);


module.exports = router;
