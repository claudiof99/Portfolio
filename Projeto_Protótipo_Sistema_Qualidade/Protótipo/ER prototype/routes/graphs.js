const express = require('express');
const router = express.Router();
const { isAuthenticated } = require('../middlewares/auth');
const db = require('../models/db'); 
router.get('/', isAuthenticated, (req, res) => {
    
    const kpis = db.read('kpis.json') || [];
    
   
    res.render('graphs/index', { 
        kpis: kpis,
        currentUser: req.session.user 
    });
});

module.exports = router;