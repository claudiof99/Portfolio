"use strict";

const socket = io();

let timer;
let timeLeft;
let isWriting = false;
let menuAberto = null;
let room = null;

socket.emit("connected",document.querySelector('meta[name="user-id"]').content);
socket.on("receberMensagemChatAmigo", message =>
{   
    console.log("Recebido");
    displayMessage(message);
});

document.addEventListener("DOMContentLoaded", function () {
    const butaoPerfil = document.getElementById("perfil");

    const menu = document.createElement("div");
    menu.style.position = "absolute";
    menu.style.background = "white";
    menu.style.border = "2px solid rgb(23, 157, 247)";
    menu.style.borderRadius = "5px";
    menu.style.padding = "10px 20px";
    menu.style.display = "none";

    const profile = document.createElement("div");
    profile.textContent = "Profile";
    profile.onclick = () => { window.location.href = "/Profile"; };

    const logout = document.createElement("div");
    logout.textContent = "Logout";
    logout.onclick = () => { window.location.href = "/Login"; };

    menu.appendChild(profile);
    menu.appendChild(logout);

    [...menu.children].forEach(item => {
        item.style.fontSize = "20px";
        item.style.cursor = "pointer";

        item.onmouseover = () => {
            item.style.transform = "scale(1.1)";
            item.style.transition = "transform 0.2s";
        };
        item.onmouseout = () => {
            item.style.transform = "scale(1)";
        };
    });

    document.body.appendChild(menu);

    // Função para atualizar a posição do menu
    function atualizarPosicaoMenu() {
        if (!menuAberto) return;

        const rect = butaoPerfil.getBoundingClientRect();
        menu.style.top = `${rect.bottom + window.scrollY}px`;
        menu.style.left = `${rect.left + window.scrollX - 50}px`;
    }

    butaoPerfil.addEventListener("click", function (event) {
        const rect = butaoPerfil.getBoundingClientRect();

        if (menuAberto && menuAberto !== menu) {
            menuAberto.style.display = "none";
        }

        menu.style.top = `${rect.bottom + window.scrollY}px`;
        menu.style.left = `${rect.left + window.scrollX - 50}px`;
        menu.style.display = (menu.style.display === "none") ? "block" : "none";

        menuAberto = (menu.style.display === "block") ? menu : null;

        event.stopPropagation();
    });

    window.addEventListener("resize", function () {
        atualizarPosicaoMenu(); // Recalcula a posição do menu ao redimensionar o ecrã
    });

    document.addEventListener("scroll", function () {
        atualizarPosicaoMenu(); // Atualiza a posição ao fazer scroll
    });

    document.addEventListener("click", function (event) {
        if (menu.style.display === "block" && !menu.contains(event.target) && event.target !== butaoPerfil)
            menu.style.display = "none";
    });

    menu.addEventListener("mouseleave", function () {
        menu.style.display = "none";
        menuAberto = null;
    });
});

document.addEventListener("DOMContentLoaded", function () {
    const butaoOpcoes = document.getElementById("opcoes");
    const menu = document.createElement("div");

    menu.style.position = "absolute";
    menu.style.background = "white";
    menu.style.border = "2px solid rgb(23, 157, 247)";
    menu.style.borderRadius = "5px";
    menu.style.display = "none";
    menu.style.padding = "10px 20px";
    menu.style.marginLeft = "-20px";

    // Função para criar botões com efeito de transformação (aumentar e diminuir ao passar o mouse)
    function criarBotao(nome, acao) {
        const botao = document.createElement("div");
        botao.textContent = nome;
        botao.style.cursor = "pointer";
        
        botao.onmouseover = () => {
            botao.style.transform = "scale(1.1)";
            botao.style.transition = "transform 0.2s";
        };
        botao.onmouseout = () => {
            botao.style.transform = "scale(1)";
        };

        botao.onclick = acao;
        return botao;
    }

    // Função genérica para adicionar botão "Fechar"
    function adicionarBotaoFecharUniversal(elemento, tipo = "removerClasse", id = null) {
        if (!elemento.querySelector('.fechar-btn')) {
            const fecharBtn = document.createElement("button");
            fecharBtn.textContent = "Voltar atras";
            fecharBtn.classList.add("fechar-btn");

            // Estilo do botão
            fecharBtn.style.padding = "6px 12px";
            fecharBtn.style.border = "none";
            fecharBtn.style.backgroundColor = "#2196F3";
            fecharBtn.style.color = "white";
            fecharBtn.style.borderRadius = "4px";
            fecharBtn.style.marginTop = "10px";
            fecharBtn.style.cursor = "pointer";
            fecharBtn.onmouseover = () => fecharBtn.style.opacity = "0.8";
            fecharBtn.onmouseout = () => fecharBtn.style.opacity = "1";

            fecharBtn.onclick = () => {
                if (tipo === "removerClasse") {
                    elemento.classList.remove("fullscreen");
                    removerBotaoFechar(elemento);
                } else if (tipo === "removerElemento" && id) {
                    const alvo = document.getElementById(id);
                    if (alvo) alvo.remove();
                }
            };

            elemento.appendChild(fecharBtn);
        }
    }

    // Função para remover o botão de fechar
    function removerBotaoFechar(elemento) {
        const fecharBtn = elemento.querySelector('.fechar-btn');
        if (fecharBtn) fecharBtn.remove();
    }

    // Botões principais
    const historicoBtn = criarBotao("Histórico", () => {
        const historico = document.getElementById("direita");
        if (historico.classList.contains("fullscreen")) {
            historico.classList.remove("fullscreen");
            removerBotaoFechar(historico);
        } else {
            historico.classList.add("fullscreen");
            adicionarBotaoFecharUniversal(historico, "removerClasse");
        }
    });

    const socialBtn = criarBotao("Social", () => {
        const social = document.getElementById("esquerda");
        if (social.classList.contains("fullscreen")) {
            social.classList.remove("fullscreen");
            removerBotaoFechar(social);
        } else {
            social.classList.add("fullscreen");
            adicionarBotaoFecharUniversal(social, "removerClasse");
        }
    });

    // Política de Cookies
    function criarPoliticaCookies(menu) {
        const politicaBtn = criarBotao("Política de Cookies", () => {
            let politica = document.getElementById("politicaCookies");
            if (!politica) {
                politica = document.createElement("div");
                politica.id = "politicaCookies";
                politica.classList.add("fullscreen");
                politica.innerHTML = `
                    <h1>Política de Cookies</h1>
                    <p>Aqui vai o conteúdo da política de cookies...</p>
                `;
                document.body.appendChild(politica);
                adicionarBotaoFecharUniversal(politica, "removerElemento", "politicaCookies");
            }
        });
        menu.appendChild(politicaBtn);
    }

    // Adicionando os botões ao menu
    menu.appendChild(historicoBtn);
    menu.appendChild(socialBtn);
    criarPoliticaCookies(menu);
    document.body.appendChild(menu);

    // Atualiza o menu baseado no estado atual dos elementos
    function atualizarMenu() {
        const direita = window.getComputedStyle(document.getElementById("direita") || {}).display;
        const esquerda = window.getComputedStyle(document.getElementById("esquerda") || {}).display;

        menu.innerHTML = ""; // Limpa o menu

        if (direita === "none") {
            menu.appendChild(historicoBtn);
        }

        if (esquerda === "none") {
            menu.appendChild(socialBtn);
        }

        criarPoliticaCookies(menu);
    }

    // Verifica e fecha fullscreen quando necessário
    function verificarFullscreenAutoFechar() {
        const direita = document.getElementById("direita");
        const esquerda = document.getElementById("esquerda");

        const direitaDisplay = window.getComputedStyle(direita).display;
        const esquerdaDisplay = window.getComputedStyle(esquerda).display;

        if (direitaDisplay !== "none" && direita.classList.contains("fullscreen")) {
            direita.classList.remove("fullscreen");
            removerBotaoFechar(direita);
        }

        if (esquerdaDisplay !== "none" && esquerda.classList.contains("fullscreen")) {
            esquerda.classList.remove("fullscreen");
            removerBotaoFechar(esquerda);
        }
    }

    // Inicializa o menu e ouve mudanças de tamanho
    atualizarMenu();
    window.addEventListener("resize", function () {
        atualizarMenu();
        verificarFullscreenAutoFechar();
    });

    // Controle de exibição do menu
    let menuAberto = null;

    butaoOpcoes.addEventListener("click", function (event) {
        const rect = butaoOpcoes.getBoundingClientRect();

        if (menuAberto && menuAberto !== menu) {
            menuAberto.style.display = "none";
        }

        menu.style.top = `${rect.bottom + window.scrollY}px`;
        menu.style.left = `${rect.left + window.scrollX - 50}px`;
        menu.style.display = (menu.style.display === "none") ? "block" : "none";

        menuAberto = (menu.style.display === "block") ? menu : null;
        
        event.stopPropagation();
    });

    document.addEventListener("click", function (event) {
        if (menu.style.display === "block" && !menu.contains(event.target) && event.target !== butaoOpcoes) {
            menu.style.display = "none";
        }
    });

    menu.addEventListener("mouseleave", function() {
        menu.style.display = "none";
    });
});



document.addEventListener("DOMContentLoaded", function()
{ 
    //codigo para tempo iterativo
    const options = document.querySelectorAll(".main-box"); // Pegamos os botões Short, Medium, Long
    const timeDisplay = document.getElementById("time-display"); // Pegamos a div que mostra o tempo
    options[0].classList.add('active');//Seleciona a box Short por default
        
    options.forEach(option => {
        option.addEventListener("click", () => {
            if (timeDisplay) {
                timeDisplay.textContent = option.getAttribute("data-time"); // Atualiza corretamente o tempo na tela
                console.log(timeDisplay.textContent);
            }
        });
    });

    // Adicionando evento de clique para cada caixa
    options.forEach(option => {
        option.addEventListener('click', () => {
            // Remover a classe 'active' de todas as caixas
            options.forEach(b => b.classList.remove('active'));
            
            // Adicionar a classe 'active' à caixa clicada
            option.classList.add('active');
        });
    });
});

document.getElementById("chatContainer").addEventListener("input", function() {   
    const timeDisplay = document.getElementById("time-display");
    
    if(!isWriting)
    {
        isWriting = true;
        timeLeft = parseFloat(timeDisplay.textContent);

        timer = setInterval(() => 
        {
            timeLeft -= 0.1;

            if (timeLeft <= 0) {
                timeLeft = 0;
                clearInterval(timer);
                isWriting = false;
            }

            timeDisplay.textContent = timeLeft.toFixed(2);
        }, 100);
    }
});

document.querySelectorAll(".amigos").forEach(amigo => {
    amigo.addEventListener("click", function (event) 
    {   
        const chatWindow = document.getElementsByClassName("amigos-chat")[0];//Pega a janela do chat
        const chat = document.getElementsByClassName("chat-body")[0];//Pega a janela do chat
        chat.innerHTML = "";
        
        chatWindow.style.left = event.clientX + "px";//Coloca a posição horizontal da janela baseada no lugar do click
        chatWindow.style.top =  event.clientY + "px";//Coloca a posição vertical da janela baseada no lugar do click
        chatWindow.style.display = chatWindow.style.display === "block" ? "none" : "block";//Mostra a janela do chat

        // Definir o amigo com quem tamos a conversar no chat
        const friendChatName = document.getElementById("amigoNome");
        friendChatName.textContent = this.querySelector("span:first-child");
        
        const chatHeader = chatWindow.querySelector(".chat-header");

        //Variáveis para a movimentação da popup window
        let isDragging = false;
        let offsetX, offsetY;

        if (!chatHeader.dataset.eventsAdded) 
        {
            chatHeader.dataset.eventsAdded = "true";

            chatHeader.addEventListener("mousedown", (event) => //Quando o utilizador clica e segura a janela popup
            {
                isDragging = true;//Indica que a janela está a ser movida
                offsetX = event.clientX - chatWindow.getBoundingClientRect().left;
                offsetY = event.clientY - chatWindow.getBoundingClientRect().top;
                chatHeader.style.cursor = "grabbing";//Coloca o cursor personalizado quando passamos o rato em cima da janela

                document.addEventListener("mousemove", onMouseMove);//Quando o utilizador arrasta a janela chama a função move
                document.addEventListener("mouseup", onMouseUp);//Quando o utilizador para de clicar na janela
            });

            function onMouseMove(event) 
            {
                if (isDragging) 
                {
                    const bodyRect = document.body.getBoundingClientRect();//Recebe as dimensões do body
                    const chatRect = chatWindow.getBoundingClientRect();//Recebe as dimensões da janela do pop-up

                    // Limites para o chat não sair da tela
                    let newX = event.clientX - offsetX;
                    let newY = event.clientY - offsetY;

                    // Garantir que o chat não saia da janela
                    if (newX < bodyRect.left) newX = bodyRect.left;
                    if (newY < bodyRect.top) newY = bodyRect.top;
                    if (newX + chatRect.width > bodyRect.right) newX = bodyRect.right - chatRect.width;
                    if (newY + chatRect.height > bodyRect.bottom) newY = bodyRect.bottom - chatRect.height;

                    //Move a janela para a sua nova posição
                    chatWindow.style.left = newX + "px";
                    chatWindow.style.top = newY + "px";
                }
            }

            function onMouseUp() 
            {
                isDragging = false;//Quando o utilizador para de clicar na popup window envia um sinal que parou de mover-se
                chatHeader.style.cursor = "grab";//Volta ao estilo de mão aberta quando solta o click

                //Remove estes processos para evitar processamento desnecessário
                document.removeEventListener("mousemove", onMouseMove);
                document.removeEventListener("mouseup", onMouseUp);
            }
        }
    });
});

document.addEventListener("DOMContentLoaded", function() {
    const addFriendsButton = document.getElementById("addFriend");

    const menu = document.createElement("form");
    menu.action = "/MainPage";
    menu.method = "POST";
    menu.name = "form";
    menu.style.position = "absolute";
    menu.style.background = "white";
    menu.style.border = "2px solid rgb(23, 157, 247)";
    menu.style.borderRadius = "5px";
    menu.style.display = "none";
    menu.style.padding = "10px 20px"; // Extra space inside the box

    // Create the input element dynamically
    const input = document.createElement("input");
    input.type = "text"; // Set the type of the input
    input.name = "friendName"; // This should be the name you want to send to the server

    const inputHidden = document.createElement("input");
    inputHidden.type = "hidden"; // Set the type of the input
    inputHidden.name = "form";
    inputHidden.value = "addFriend";

    menu.appendChild(input);
    menu.appendChild(inputHidden);

    document.body.appendChild(menu); // Append the menu to the body

    addFriendsButton.addEventListener("click", function(event) {
        // Get the mouse position when clicking the button
        const mouseX = event.clientX;
        const mouseY = event.clientY;

        // Set the menu's position to the mouse position
        menu.style.left = `${mouseX + 10}px`; // Add some offset to the right
        menu.style.top = `${mouseY + 10}px`;  // Add some offset below the cursor

        // Toggle visibility of the menu
        menu.style.display = (menu.style.display === "block") ? "none" : "block";
    });

    // Directly submit the form when the user presses "Enter" or submits it
    menu.addEventListener("submit", function(event) {
        const friendName = input.value.trim(); // Get the entered friend's username

        if (friendName === "") {
            alert("Please enter a valid friend's username");
            event.preventDefault();  // Prevent submission if the input is empty
            return;
        }
    });
});

document.getElementById("chatMensagem").addEventListener("keypress",function(event)
{
    if(event.key == "Enter")
    {
        const chatMensagemInput = document.getElementById("chatMensagem");
        const message = chatMensagemInput.value;
        displayMessage(message);
        chatMensagemInput.value = ""
        socket.emit("chatAmigo", room ,message);
    }
});

function displayMessage(message) {
    const chatBody = document.getElementsByClassName("chat-body")[0];

    // Criação de um contêiner para a mensagem com o horário
    const messageContainer = document.createElement("div");
    messageContainer.classList.add("message-container"); // Criação de uma classe para o contêiner da mensagem

    // Criação do elemento de horário
    const messageTime = document.createElement("span");
    const currentTime = new Date(); // Obtém a data e hora atual
    const formattedTime = currentTime.toLocaleTimeString([], {hour: '2-digit', minute: '2-digit'}); // Formata como HH:mm
    messageTime.textContent = formattedTime; // Exibe o horário
    messageTime.classList.add("message-time"); // Classe para o horário

    // Criação da mensagem
    const mensagem = document.createElement("p");
    mensagem.textContent = message;
    document.getElementsByClassName("chat-body")[0].insertBefore(mensagem,document.getElementById("chatMensagem").lastElementChild);
}

document.querySelectorAll(".amigos").forEach(amigo => 
{
    amigo.addEventListener("click", function (event) 
    {
        room = this.getAttribute("name");
        console.log(room);
    });
});