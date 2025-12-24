const db = require('../models/db');

exports.list = (req, res) => {
  const complaints = db.read('complaints.json');
  const userComplaints = req.session.user.role === 'admin' 
    ? complaints 
    : complaints.filter(c => c.userId === req.session.user.id);
  
  res.render('complaints/list', { complaints: userComplaints });
};

exports.newForm = (req, res) => {
  const clauses = db.read('clauses.json');
  res.render('complaints/new', { clauses });
};

exports.create = (req, res) => {
  const complaints = db.read('complaints.json');
  const { title, description, clauseId, priority } = req.body;
  
  const newComplaint = {
    id: db.getNextId(complaints),
    title,
    description,
    clauseId: parseInt(clauseId),
    priority: priority || 'medium',
    status: 'pending',
    userId: req.session.user.id,
    createdBy: req.session.user.name,
    createdAt: new Date().toISOString(),
    updatedAt: new Date().toISOString()
  };
  
  complaints.push(newComplaint);
  db.write('complaints.json', complaints);
  
  res.redirect('/complaints');
};

exports.view = (req, res) => {
  const complaints = db.read('complaints.json');
  const clauses = db.read('clauses.json');
  const complaint = complaints.find(c => c.id === parseInt(req.params.id));
  
  if (!complaint) return res.status(404).send('Not found');
  
  if (req.session.user.role !== 'admin' && complaint.userId !== req.session.user.id) {
    return res.status(403).send('Forbidden');
  }
  
  const clause = clauses.find(cl => cl.id === complaint.clauseId);
  res.render('complaints/view', { complaint, clause });
};

exports.editForm = (req, res) => {
  const complaints = db.read('complaints.json');
  const clauses = db.read('clauses.json');
  const complaint = complaints.find(c => c.id === parseInt(req.params.id));
  
  if (!complaint) return res.status(404).send('Not found');
  
  if (req.session.user.role !== 'admin' && complaint.userId !== req.session.user.id) {
    return res.status(403).send('Forbidden');
  }
  
  res.render('complaints/edit', { complaint, clauses });
};

exports.update = (req, res) => {
  const complaints = db.read('complaints.json');
  const index = complaints.findIndex(c => c.id === parseInt(req.params.id));
  
  if (index === -1) return res.status(404).send('Not found');
  
  const { title, description, clauseId, priority, status } = req.body;
  
  complaints[index] = {
    ...complaints[index],
    title,
    description,
    clauseId: parseInt(clauseId),
    priority,
    status: status || complaints[index].status,
    updatedAt: new Date().toISOString()
  };
  
  db.write('complaints.json', complaints);
  res.redirect('/complaints/' + req.params.id);
};

exports.delete = (req, res) => {
  const complaints = db.read('complaints.json');
  const filtered = complaints.filter(c => c.id !== parseInt(req.params.id));
  db.write('complaints.json', filtered);
  res.redirect('/complaints');
};