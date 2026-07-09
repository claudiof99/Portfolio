const express = require("express");//Importa express uma framework para Node.js
const { formGetLogin , formPostLogin } = require("../Controllers/loginController.js");//Importa as funções recebidas do controlador
const passport = require("passport");

const router = express.Router();//Criação do router para definir e organizar as rotas

router.get("/",formGetLogin);//Esta rota é responsável por lidar quando o utilizador acessa a página fazendo o express chamar a função formGet do controlador
router.post("/",passport.authenticate("local" , { failureRedirect : "/Login"}) , function(req,res){res.redirect("MainPage");});

module.exports = router;//Exporta o router para ser usado no server.js