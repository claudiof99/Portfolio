const express = require("express");//Importa express uma framework para Node.js
const { formGetMainPage , formPostMainPage } = require("../Controllers/mainPageController.js");//Importa as funções recebidas do controlador

const router = express.Router();//Criação do router para definir e organizar as rotas

router.get("/",formGetMainPage);//Esta rota é responsável por lidar quando o utilizador acessa a página fazendo o express chamar a função formGet do controlador
router.post("/",formPostMainPage);

module.exports =  router;//Exporta o router para ser usado no server.js