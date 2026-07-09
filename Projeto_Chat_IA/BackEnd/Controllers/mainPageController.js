const User = require("../Models/user.js");

//Esta função renderiza a página principal
const formGetMainPage = async function(req,res)
{   
    if(!req.isAuthenticated())
        return res.redirect("Login");
    
    const user = await User.findById(req.user._id).populate("amigos")

    res.render("MainPage", {user : req.user , amigos : user.amigos});
};

const formPostMainPage = async function (req,res) 
{   
    if(req.body.form == "addFriend")
    {
        const user = await User.findById(req.user.id);
        
        const addedFriend = await User.findByUsername(req.body.friendName);

        if(addedFriend && !user.amigos.includes(addedFriend.id))
        {
            user.amigos.push(addedFriend.id);
            await user.save();

            const userAmigos = await User.findById(req.user._id).populate("amigos")

            res.render("MainPage" , {user : user , amigos : userAmigos.amigos});
        }
        else
        {   
            const userAmigos = await User.findById(req.user._id).populate("amigos")
            res.render("MainPage",{error : "There is no user with that username or user already added", user : user , amigos : userAmigos.amigos});   
        }
    }
    else
    {
        try
        {
            const payLoad = 
            {
                "model" : "gpt-4o-mini", //Modelo do chatgot
                "messages" : req.body.Usermessages, //Mensagem enviada pelo utilizador
                "temperature" : 0.7, //Precisão do chatgpt e aleatoriedade da resposta
                "stream" : "false",//Definido assim para a resposta não ser transmitida em partes
            }

            const response = await fetch("https://api.openai.com/v1/chat/completions" , {
                method : "POST",
                headers : {"Content-Type" : "application/json","Authorization": "lm-studio",},//O tipo de conteudo a ser enviado é JSON e a chave para usar o modelo chatgpt
                body : JSON.stringify(payLoad),//Os dados da mensagem do utilizador
            });

            console.log("Response Status:", response.status);

            if(response.ok)
            {   
                const data = await response.json();//Vai buscar a resposta do chatgpt
                const chatResponse = data.choices[0].message.content;//Coloca a resposta numa variável

                console.log(chatResponse);

                res.render("Mainpage",{messages : chatResponse , user: req.user});
            }
            else
            {
                res.render("MainPage",{error : "An error ocurred in the AI response" , messages:"" , user : req.user});
            }
        }
        catch(error)
        {
            res.render("MainPage",{error:"An error ocurred trying to send information to the AI",messages : "" , user : req.user});
        }
    }    
};

module.exports = {formGetMainPage,formPostMainPage};//Exporta as funções para as rota definida a