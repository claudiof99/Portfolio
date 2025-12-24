const express = require('express');
const router = express.Router();

const notificationController = require('../controllers/notificationcontroller');
const { isAuthenticated } = require('../middlewares/auth');

router.get('/', isAuthenticated, notificationController.index);
router.post('/:id/read', isAuthenticated, notificationController.markRead);

module.exports = router;
