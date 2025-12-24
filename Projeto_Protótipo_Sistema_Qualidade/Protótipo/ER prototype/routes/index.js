const express = require('express');
const router = express.Router();

// Auth & dashboard routes
router.use('/', require('./auth'));
router.use('/', require('./dashboard'));

// Notifications, complaints, admin
router.use('/notifications', require('./notifications'));
router.use('/complaints', require('./complaints'));
router.use('/admin', require('./admin'));

// NEW route groups
router.use('/kpis', require('./kpis'));
router.use('/tables', require('./tables'));
router.use('/graphs', require('./graphs'));
router.use('/iso', require('./iso'));

module.exports = router;
