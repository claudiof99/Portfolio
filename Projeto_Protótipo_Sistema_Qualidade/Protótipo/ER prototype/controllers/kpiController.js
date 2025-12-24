const db = require('../models/db');


// 1. List all KPIs (Grouped by Process)
exports.index = (req, res) => {
    let kpis = db.read('kpis.json') || [];
    
    // 1. Filter by Search (if exists)
    const searchQuery = req.query.search;
    if (searchQuery) {
        const lowerCaseQuery = searchQuery.toLowerCase();
        kpis = kpis.filter(k => 
            k.name.toLowerCase().includes(lowerCaseQuery) || 
            (k.process && k.process.toLowerCase().includes(lowerCaseQuery))
        );
    }


    const kpisByProcess = {};
    
    kpis.forEach(kpi => {
     
        const processName = kpi.process || 'General Processes';
        
        if (!kpisByProcess[processName]) {
            kpisByProcess[processName] = [];
        }
        kpisByProcess[processName].push(kpi);
    });

    res.render('kpis/index', { 
        title: 'KPI Management',
        kpisByProcess: kpisByProcess, 
        currentUser: req.session.user,
        searchQuery: searchQuery
    });
};


exports.show = (req, res) => {
    const kpis = db.read('kpis.json') || [];
    const kpi = kpis.find(k => k.id == req.params.id);
    
    if (!kpi) return res.redirect('/kpis');

    res.render('kpis/show', { 
        title: kpi.name,
        kpi: kpi,
        currentUser: req.session.user
    });
};

exports.edit = (req, res) => {
    const kpis = db.read('kpis.json') || [];
    const kpi = kpis.find(k => k.id == req.params.id);
    if (!kpi) return res.redirect('/kpis');
    res.render('kpis/edit', { title: 'Edit KPI', kpi, currentUser: req.session.user });
};

exports.update = (req, res) => {
    const { name, target, unit, frequency, status } = req.body;
    const kpis = db.read('kpis.json') || [];
    const index = kpis.findIndex(k => k.id == req.params.id);

    if (index !== -1) {
        kpis[index].name = name;
        kpis[index].target = target;
        kpis[index].unit = unit;
        kpis[index].frequency = frequency;
        
        if (status === 'Active' || status === 'Inactive') {
            kpis[index].status = status;
        }

        db.write('kpis.json', kpis);
    }
    res.redirect('/kpis/' + req.params.id);
};


exports.add = (req, res) => {
    const { name, process, target, unit, frequency } = req.body; 
    const targetVal = parseFloat(target);
    const userRole = req.session.user.role;
    
    if (targetVal < 0) {
        return res.render('error', { message: "Target cannot be negative.", solution: "Enter a positive number." });
    }
    if (unit === '%' && targetVal > 100) {
        return res.render('error', { message: "Percentage cannot exceed 100.", solution: "Enter value between 0-100." });
    }

    let initialStatus = (userRole === 'quality_manager' || userRole === 'admin') ? 'Active' : 'Pending';
    const creatorName = req.session.user.username || req.session.user.role || 'User';

    const kpis = db.read('kpis.json') || [];
    kpis.push({
        id: Date.now(),
        name, 
        process, 
        target, 
        unit, 
        frequency,
        status: initialStatus,
        createdBy: creatorName,
        dataPoints: []
    });
    
    db.write('kpis.json', kpis);
    res.redirect('/kpis');
};

// 6. Add Data Point
exports.addData = (req, res) => {
    const { value, date } = req.body;
    const val = parseFloat(value);
    const kpis = db.read('kpis.json') || [];
    const kpi = kpis.find(k => k.id == req.params.id);

    if (kpi) {
        if (val < 0) return res.render('error', { message: "Negative value.", solution: "Enter positive number." });
        if (kpi.unit === '%' && val > 100) return res.render('error', { message: "Over 100%.", solution: "Enter 0-100." });

        const recorderName = req.session.user.username || req.session.user.role || 'User';

        kpi.dataPoints.push({
            date,
            value: val,
            recordedBy: recorderName,
            active: true
        });
        db.write('kpis.json', kpis);
    }
    res.redirect('/kpis/' + req.params.id);
};

// 7. Archive Data Point
exports.removeData = (req, res) => {
    const { kpiId, date } = req.params;
    const kpis = db.read('kpis.json') || [];
    const kpi = kpis.find(k => k.id == kpiId);
    
    if (kpi) {
        const point = kpi.dataPoints.find(d => d.date === date);
        if (point) {
            point.active = false;
            point.archivedBy = req.session.user.username || 'User';
            point.archivedAt = new Date().toISOString();
        }
        db.write('kpis.json', kpis);
    }
    res.redirect('/kpis/' + kpiId);
};

// 8. Restore Data Point
exports.restoreData = (req, res) => {
    const { kpiId, date } = req.params;
    const kpis = db.read('kpis.json') || [];
    const kpi = kpis.find(k => k.id == kpiId);
    
    if (kpi) {
        const point = kpi.dataPoints.find(d => d.date === date);
        if (point) {
            point.active = true;
        }
        db.write('kpis.json', kpis);
    }
    res.redirect('/kpis/' + kpiId);
};

// 9. Update Data Point
exports.updateDataPoint = (req, res) => {
    const kpiId = req.params.id;
    const oldDate = req.params.oldDate;
    const { newDate, newValue } = req.body;
    const kpis = db.read('kpis.json');
    const kpi = kpis.find(k => k.id == kpiId);

    if (kpi) {
        const point = kpi.dataPoints.find(p => p.date === oldDate);
        if (point) {
            point.previousValue = point.value; 
            point.date = newDate;
            point.value = parseFloat(newValue);
            let editor = req.session.user.username || req.session.user.role ||   'Admin';
            point.lastEditedBy = editor.charAt(0).toUpperCase() + editor.slice(1);
            point.lastEditedAt = new Date().toISOString();
            db.write('kpis.json', kpis);
        }
    }
    res.redirect(`/kpis/${kpiId}`);
};

// 10. Delete Data Point
exports.deleteDataPoint = (req, res) => {
    if(req.session.user.role !== 'admin') return res.status(403).send("Unauthorized");
    const kpiId = req.params.id;
    const date = req.params.date;
    const kpis = db.read('kpis.json');
    const kpi = kpis.find(k => k.id == kpiId);

    if (kpi) {
        kpi.dataPoints = kpi.dataPoints.filter(p => p.date !== date);
        db.write('kpis.json', kpis);
    }
    res.redirect(`/kpis/${kpiId}`);
};

// 11. Approvals
exports.approvals = (req, res) => {
    const kpis = db.read('kpis.json') || [];
    const pendingItems = kpis.filter(k => k.status === 'Pending');
    res.render('approvals', { pending: pendingItems, currentUser: req.session.user });
};

exports.approveKpi = (req, res) => {
    let kpis = db.read('kpis.json') || [];
    const index = kpis.findIndex(k => k.id == req.params.id);
    if (index !== -1) {
        kpis[index].status = 'Active';
        db.write('kpis.json', kpis);
    }
    res.redirect('/kpis/approvals');
};