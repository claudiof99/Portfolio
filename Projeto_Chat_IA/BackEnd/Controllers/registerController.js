const User = require("../Models/user.js");

// Esta função renderiza a página do registo
const formGetRegister = function(req, res) {
    res.render("Register", { error: null });  // Passa 'error: null' para garantir que o erro seja sempre definido
};

const formPostRegister = async function(req, res) {
    const { username, email, password, confirmPassword } = req.body;

    // Verifica se as senhas são iguais
    if (password !== confirmPassword) {
        return res.render("Register", { error: "A senha nao é igual a anterior!", username, email });
    }

    try 
    {    
        const user = new User({email,username});//Cria um novo utilizador
        await User.register(user,password);//Guarda na base de dados
        res.redirect("/Login");//Redireciona para a página de login
    } 
    catch (err) 
    {
        console.log("Error registering user", err);
        return res.render("Register", { error: "An error occurred", username, email });  // Passa username e email para manter os campos preenchidos
    }
};

module.exports = { formGetRegister, formPostRegister };  // Exporta as funções para as rotas definidas