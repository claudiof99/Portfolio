    const express = require("express");//Importa express uma framework para Node.js
    const { formGetRegister , formPostRegister } = require("../Controllers/registerController.js");//Importa as funções recebidas do controlador

    const router = express.Router();//Criação do router para definir e organizar as rotas

    router.get("/",formGetRegister);//Esta rota é responsável por lidar quando o utilizador acessa a página fazendo o express chamar a função formGet do controlador
    router.post("/",formPostRegister);//Esta rota é responsável por lidar quando o utilizador envia algo na página fazendo o express chamar a função formGet do controlador

    module.exports =  router;//Exporta o router para ser usado no server.js