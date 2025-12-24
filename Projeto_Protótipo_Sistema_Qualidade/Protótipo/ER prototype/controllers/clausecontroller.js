const db = require('../models/db');

exports.list = (req, res) => {
  const clauses = db.read('clauses.json');
  res.render('clauses/list', { clauses });
};

exports.newForm = (req, res) => {
  res.render('clauses/new');
};

exports.create = (req, res) => {
  const clauses = db.read('clauses.json');
  const { code, title, description, category } = req.body;
  
  const newClause = {
    id: db.getNextId(clauses),
    code,
    title,
    description,
    category,
    createdAt: new Date().toISOString()
  };
  
  clauses.push(newClause);
  db.write('clauses.json', clauses);
  res.redirect('/clauses');
};

exports.view = (req, res) => {
  const clauses = db.read('clauses.json');
  const clause = clauses.find(c => c.id === parseInt(req.params.id));
  
  if (!clause) return res.status(404).send('Not found');
  
  res.render('clauses/view', { clause });
};

exports.update = (req, res) => {
  const clauses = db.read('clauses.json');
  const index = clauses.findIndex(c => c.id === parseInt(req.params.id));
  
  if (index === -1) return res.status(404).send('Not found');
  
  const { code, title, description, category } = req.body;
  clauses[index] = { ...clauses[index], code, title, description, category };
  
  db.write('clauses.json', clauses);
  res.redirect('/clauses/' + req.params.id);
};

exports.delete = (req, res) => {
  const clauses = db.read('clauses.json');
  const filtered = clauses.filter(c => c.id !== parseInt(req.params.id));
  db.write('clauses.json', filtered);
  res.redirect('/clauses');
};