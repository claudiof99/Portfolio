const express = require('express');
const router = express.Router();
const { isAuthenticated } = require('../middlewares/auth');
const tablesController = require('../controllers/tableController');

router.get('/', isAuthenticated, tablesController.index);
router.post('/add', isAuthenticated, tablesController.add);

module.exports = router;