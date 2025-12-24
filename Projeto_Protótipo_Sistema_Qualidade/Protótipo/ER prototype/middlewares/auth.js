exports.isAuthenticated = (req, res, next) => {
    if (req.session && req.session.user) {
        return next();
    }
    return res.redirect('/login');
};

exports.requireRole = (allowedRoles) => {
    return (req, res, next) => {
       
        if (!req.session || !req.session.user) {
            return res.redirect('/login');
        }

        const userRole = req.session.user.role;

        if (userRole === 'admin') {
            return next();
        }

       
        if (Array.isArray(allowedRoles)) {
            if (allowedRoles.includes(userRole)) {
                return next(); 
            }
        } 
      
        else {
            if (userRole === allowedRoles) {
                return next(); 
            }
        }

        // Access Denied
        return res.status(403).send(`
            <h1>403 Forbidden</h1>
            <p>Access Denied. You are logged in as <strong>${userRole}</strong>, 
            but this page requires: <strong>${allowedRoles}</strong>.</p>
            <a href="/dashboard">Return to Dashboard</a>
        `);
    };
};