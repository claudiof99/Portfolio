const formGetLogin = function(req, res , next) 
{
    const username = req.body.username;//Recebe o username 

    if(req.isAuthenticated ())
    {
        req.logout(function(err)
        {   
            if(err)
                return next(err);
        });
    }

    res.render("Login", { username, error: null }); // Garante que error seja sempre definido
};

module.exports = { formGetLogin };