const db = require('../models/db');


exports.loginPage = (req, res) => {
  res.render('auth/login', { error: null });
};

// Handle login
exports.login = (req, res) => {
  const { email, password } = req.body;
  const users = db.read('users.json');
  const user = users.find(u => u.email === email); // find by email

  // check if user exists and password matches
  if (!user || user.password !== password) {
    return res.render('auth/login', { error: 'Invalid credentials' });
  }

  // set session
  req.session.user = {
    id: user.id,
    email: user.email,
    role: user.role,
    name: user.name
  };

  res.redirect('/dashboard');
};

// Render register page
exports.registerPage = (req, res) => {
  res.render('auth/register', { error: null });
};

// Handle registration
exports.register = (req, res) => {
  const { email, password, name } = req.body;
  const users = db.read('users.json');

  if (users.find(u => u.email === email)) {
    return res.render('auth/register', { error: 'Email already exists' });
  }

  const newUser = {
    id: db.getNextId(users),
    email,
    password, 
    name,
    role: 'user',
    createdAt: new Date().toISOString()
  };

  users.push(newUser);
  db.write('users.json', users);

  res.redirect('/login');
};


exports.logout = (req, res) => {
  req.session.destroy(err => {
    if (err) console.log(err);
    res.redirect('/login');
  });
};

