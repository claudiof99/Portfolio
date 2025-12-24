const db = require('../models/db');

exports.index = (req, res) => {
  const notifications = db.read('notifications.json');
  const userNotifications = notifications.filter(n => n.userId === req.session.user.id);
  res.render('notifications/list', { notifications: userNotifications });
};

exports.markRead = (req, res) => {
  const notifications = db.read('notifications.json');
  const index = notifications.findIndex(n => n.id === parseInt(req.params.id));
  
  if (index !== -1) {
    notifications[index].read = true;
    db.write('notifications.json', notifications);
  }
  
  res.redirect('/notifications');
};
