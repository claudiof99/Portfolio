const path = require('path');
const db = require('../models/db');

exports.index = (req, res) => {
  const complaints = db.read('complaints.json') || [];
  const clauses = db.read('clauses.json') || [];
  const notifications = db.read('notifications.json') || [];

  
console.log('Clauses file path (db.js):', path.join(__dirname, '../models/clauses.json'));
  const userComplaints = complaints.filter(c => c.userId === req.session.user.id);
  const userNotifications = notifications.filter(n => n.userId === req.session.user.id && !n.read);

  res.render('dashboard', {
    complaints: userComplaints,
    clauses: clauses,
    notifications: userNotifications
  });
};
