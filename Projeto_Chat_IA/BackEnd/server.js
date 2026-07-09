const express = require("express");//Importa express uma framework para Node.js
const path = require("path");//Converte a Url em caminho de arquivos
const mongoose = require("mongoose");//Importa mongoose para usar MongoDb
const methodOverride = require("method-override");//Permite usar outros métodos para além do get e do post em formularios html
const passport = require("passport");//Middleware para autenticação
const localStategy = require("passport-local");// Estratégia de autenticação local (usuário e senha)
const session = require("express-session");// Middleware para gerenciamento de sessões
const user = require("./Models/user.js");// Importa o modelo de utilizador
const http = require("http");// Módulo nativo do Node.js para criar servidores HTTP
const { Server } = require("socket.io");// Importa a classe Server do Socket.IO para comunicação em tempo real

const loginRoutes = require("./Routes/loginRoutes.js");//Importação da rota que vai ser responsável por lidar com a página de login
const registerRoutes = require("./Routes/registerRoutes.js");//Importação da rota que vai ser responsável por lidar com a página de registo
const mainPageRoutes = require("./Routes/mainPageRoutes.js");//Importação da rota que vai ser responsável por lidar com a página principal

const app = express();//Cria uma aplicação express
const server = http.createServer(app);// Cria um servidor HTTP com o Express
const io = new Server(server);// Inicializa o Socket.IO no servidor
const userSocketMap = {};//Mapeamento onde o id do utilizador da mongoDb é a chave e o socketID é o valor utilizado no chat 

const PORT = 3050;//Porta onde o servidor irá rodar

app.set("view engine", "ejs");//Diz ao express para usar ejs
app.set("views", path.join(__dirname, "../FrontEnd/views"));//Define a pasta onde as views estão armazenadas

app.use(express.static(path.join(__dirname, "../FrontEnd/public")));//Mete o caminho por default das coisas para a pasta public
app.use(express.urlencoded({ extended: true }));//Permite processar dados mais complexos (arrays) vindo de formulários
app.use(methodOverride("_method"));//Permite utilizar o método override na aplicação
app.use(session({
    secret: "chave-secreta-super-segura",
    resave: false,   
    saveUninitialized: false
}));
app.use(passport.initialize());//Inicializa o passport
app.use(passport.session());//Usado para guardar a sessão do utilizador em todas as solicitações que o site fará usando os dados da sessão

passport.use(new localStategy(user.authenticate()));//Authenticate é adicionado automaticamente pelo plugin
passport.serializeUser(user.serializeUser());//Permite guardar um utilizador na sessão
passport.deserializeUser(user.deserializeUser());//Permite retirar um utilizador na sessão

app.use("/Login", loginRoutes);//Diz à aplicação que quando a requisição for feita pelo url da página de login, vai ser redirecionada para o router do login
app.use("/Register", registerRoutes);//Diz à aplicação que quando a requisição for feita pelo url da página de registo, vai ser redirecionada para o router do registo
app.use("/MainPage", mainPageRoutes);//Diz à aplicação que quando a requisição for feita pelo url da página principal, vai ser redirecionada para o router da página principal

server.listen(PORT, (error) => //Inicializa o servidor na porta definida na constante
{
    if (error)
        console.error(error);
    else
        console.log(`Server running on PORT ${PORT}`);
});

app.get("/", (req, res) => 
{
    res.render("Login" , {error : null , username : ""});
});

mongoose
    .connect
    (
        "mongodb+srv://BrainStormingProjectDBW:by3em64VXCIQQbVH@cluster0.aejhrzv.mongodb.net/ProjetoDBW?retryWrites=true&w=majority&appName=Cluster0",
        {useUnifiedTopology : true , useNewUrlParser : true}
    )
    .then( () => {console.log("Connected");})
    .catch((err) => {console.log(err);});

io.on("connection" , socket =>
{
    socket.on("connected" , userId => {
            userSocketMap[userId] = socket.id;
    });
        
    socket.on("disconnect", () => {
        for(let userId in userSocketMap)
        {
            if(userSocketMap[userId] === socket.id)
            {   
                delete userSocketMap[userId];
                break;
            }
        }
    });

    socket.on("chatAmigo" , (userId,message) => {
        io.to(userSocketMap[userId]).emit("receberMensagemChatAmigo",message);
    });
});