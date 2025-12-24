const db = require('../models/db');

exports.index = (req, res) => {
 
    const tables = db.read('tables.json') || [];
    
    res.render('tables/index', { 
        title: 'Data Tables',
        tables: tables,
        currentUser: req.session.user
    });
};

exports.add = (req, res) => {
   
    const { name, description, type, status } = req.body;
    const tables = db.read('tables.json') || [];
    
    tables.push({
        id: Date.now(),
        name,
        description,
        type: type || "General",   
        status: status || "Active" 
    });
    
    db.write('tables.json', tables);
    res.redirect('/tables');
};