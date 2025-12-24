const express = require('express');
const session = require('express-session');
const bodyParser = require('body-parser');
const path = require('path');
const expressLayouts = require('express-ejs-layouts');

const app = express();

// view engine
app.set('view engine', 'ejs');
app.set('views', path.join(__dirname, 'views'));
app.use(expressLayouts);
app.set('layout', 'layout');

// middleware
app.use(bodyParser.urlencoded({ extended: false }));
app.use(session({
  secret: 'change_this_secret',
  resave: false,
  saveUninitialized: false
}));

// make variables available in all views
app.use((req, res, next) => {
  res.locals.currentUser = req.session.user || null;
  res.locals.title = "ISO Prototype";
  res.locals.message = null;
  next();
});

// static files
app.use('/public', express.static(path.join(__dirname, 'public')));

// routes
const authRoutes = require('./routes/auth');
const dashboardRoutes = require('./routes/dashboard');

const kpiRoutes = require('./routes/kpis');

const tableRoutes = require('./routes/tables');
const graphRoutes = require('./routes/graphs'); 
const ncRoutes = require('./routes/ncs');


app.use('/kpis', kpiRoutes);

app.use('/tables', tableRoutes);
app.use('/graphs', graphRoutes); 


const isoRoutes = require('./routes/iso');      


app.use('/iso', isoRoutes);        
app.use('/', authRoutes);             
app.use('/dashboard', dashboardRoutes); 
app.use('/ncs', ncRoutes);


// start server
const PORT = process.env.PORT || 3000;
app.listen(PORT, () => console.log(`App started at http://localhost:${PORT}`));
