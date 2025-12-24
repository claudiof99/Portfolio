const db = require('../models/db');

exports.dashboard = (req, res) => {
  const users = db.read('users.json');
  const complaints = db.read('complaints.json');
  const clauses = db.read('clauses.json');
  
  res.render('admin/dashboard', {
    totalUsers: users.length,
    totalComplaints: complaints.length,
    totalClauses: clauses.length,
    pendingComplaints: complaints.filter(c => c.status === 'pending').length
  });
};

exports.userList = (req, res) => {
  const users = db.read('users.json');
  res.render('admin/users', { users });
};

exports.updateUser = (req, res) => {
  const users = db.read('users.json');
  const index = users.findIndex(u => u.id === parseInt(req.params.id));
  
  if (index === -1) return res.status(404).send('Not found');
  
  const { role, name, email } = req.body;
  users[index] = { ...users[index], role, name, email };
  
  db.write('users.json', users);
  res.redirect('/admin/users');
};

exports.deleteUser = (req, res) => {
  const users = db.read('users.json');
  const filtered = users.filter(u => u.id !== parseInt(req.params.id));
  db.write('users.json', filtered);
  res.redirect('/admin/users');
};