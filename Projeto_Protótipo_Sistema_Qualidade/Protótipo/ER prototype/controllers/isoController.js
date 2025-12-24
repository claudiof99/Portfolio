const db = require('../models/db');

//MAIN ISO HUB
exports.index = (req, res) => {
    const clauses = db.read('clauses.json') || [];
    clauses.sort((a, b) => a.id - b.id);
    res.render('iso/index', { clauses, currentUser: req.session.user });
};

exports.updateClause = (req, res) => {
    const { id, definition } = req.body;
    let clauses = db.read('clauses.json') || [];
    const index = clauses.findIndex(c => c.id == id);
    if (index !== -1) {
        clauses[index].definition = definition;
        db.write('clauses.json', clauses);
    }
    res.redirect('/iso');
};

//CLAUSE 4: CONTEXT
exports.context = (req, res) => {
    const stakeholders = db.read('stakeholders.json') || [];
    res.render('iso/context', { 
        title: '4. Context', 
        stakeholders, 
        currentUser: req.session.user 
    });
};

exports.addStakeholder = (req, res) => {
    const { name, type, needs, expectations } = req.body; 
    const list = db.read('stakeholders.json') || [];
    list.push({ 
        id: Date.now(), 
        name, type, needs, expectations, 
        active: true 
    });
    db.write('stakeholders.json', list);
    res.redirect('/iso/context');
};

exports.deleteStakeholder = (req, res) => {
    let list = db.read('stakeholders.json') || [];
    list = list.filter(s => s.id != req.params.id);
    db.write('stakeholders.json', list);
    res.redirect('/iso/context');
};

//  CLAUSE 5: LEADERSHIP 
exports.leadership = (req, res) => {
    const policy = db.read('policy.json') || { content: "No policy defined yet.", lastUpdated: new Date() };
    res.render('iso/leadership', { 
        title: '5. Leadership', 
        policy, 
        currentUser: req.session.user 
    });
};

exports.updatePolicy = (req, res) => {
    const policyData = {
        content: req.body.content,
        lastUpdated: new Date(),
        updatedBy: req.session.user.username
    };
    db.write('policy.json', policyData);
    res.redirect('/iso/leadership');
};

// --- CLAUSE 6: PLANNING ---
exports.planning = (req, res) => {
    const risks = db.read('risks.json') || [];
    res.render('iso/planning', { 
        title: '6. Planning (Risks)', 
        risks, 
        currentUser: req.session.user 
    });
};

exports.addRisk = (req, res) => {
    const { description, prob, impact } = req.body;
    const p = parseInt(prob), i = parseInt(impact);
    
    if (p < 1 || p > 5 || i < 1 || i > 5) {
        return res.render('error', { 
            message: "Validation Error: Values must be 1-5.",
            solution: "Go back and correct the values."
        });
    }

    const risks = db.read('risks.json') || [];
    risks.push({ 
        id: Date.now(), 
        description, prob: p, impact: i, 
        score: p * i 
    });
    db.write('risks.json', risks);
    res.redirect('/iso/planning');
};

// CLAUSE 7: SUPPORT
exports.support = (req, res) => {
    
    const resources = db.read('tables.json') || []; 
    const documents = db.read('documents.json') || [];

    res.render('iso/support', { 
        title: '7. Support', 
        resources,
        documents, 
        currentUser: req.session.user 
    });
};


exports.uploadDocument = (req, res) => {
   
    const filePath = req.file ? '/uploads/' + req.file.filename : '#';

    const docs = db.read('documents.json') || [];
    
    docs.push({
        id: Date.now(),
        name: req.body.docName,
        type: req.body.docType,   
        owner: req.body.docOwner, 
        path: filePath,
        uploadedBy: req.session.user.username,
        date: new Date().toISOString()
    });

    db.write('documents.json', docs);
    res.redirect('/iso/support');
};

// Delete Document Handler
exports.deleteDocument = (req, res) => {
    let docs = db.read('documents.json') || [];
    docs = docs.filter(d => d.id != req.params.id);
    db.write('documents.json', docs);
    res.redirect('/iso/support');
};