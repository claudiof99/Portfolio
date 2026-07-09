"use strict";

document.querySelector('.imagem_utilizador').addEventListener('click', function() {
    window.location.href = "alteracao_imagem.html"; 
});

document.addEventListener("DOMContentLoaded", function() {
    const menuButton = document.querySelector('.definicoes');
    const menu = document.createElement('div');

    menu.style.position = 'absolute';
    menu.style.background = 'white';
    menu.style.border = '1px solid rgb(16, 71, 188)';
    menu.style.borderRadius = '5px';
    menu.style.padding = '5px';
    menu.style.display = 'none'; 
    menu.style.zIndex = '1000';

    const aboutUs = document.createElement('div');
    aboutUs.textContent = 'About Us';
    aboutUs.style.padding = '8px';
    aboutUs.style.cursor = 'pointer';
    aboutUs.onclick = function() {
        window.location.href = 'about_us.html'; 
    };

    const logOut = document.createElement('div');
    logOut.textContent = 'Log Out';
    logOut.style.padding = '8px';
    logOut.style.cursor = 'pointer';
    logOut.onclick = function() {
        window.location.href = 'login.html'; 
    };

    [aboutUs, logOut].forEach(item => {
        item.onmouseover = () => item.style.backgroundColor = '#FFD1DC';
        item.onmouseout = () => item.style.backgroundColor = 'white';
    });

    menu.appendChild(aboutUs);
    menu.appendChild(logOut);
    document.body.appendChild(menu);

    menuButton.addEventListener('click', function(event) {
        const rect = menuButton.getBoundingClientRect();
        menu.style.top = `${rect.bottom + window.scrollY}px`;
        menu.style.left = `${rect.left + window.scrollX}px`;
        menu.style.display = menu.style.display === 'none' ? 'block' : 'none';
        event.stopPropagation();
    });

    document.addEventListener('click', function(event) {
        if (!menu.contains(event.target) && event.target !== menuButton) {
            menu.style.display = 'none';
        }
    });
});

document.getElementById('fileInput').addEventListener('change', function(event) {
    const file = event.target.files[0];
    if (file) {
        const reader = new FileReader();
        reader.onload = function(e) {
            const img = document.getElementById('imgPreview');
            img.src = e.target.result;
            img.style.display = 'block';
        };
        reader.readAsDataURL(file);
    }
});

function uploadImage() {
    window.location.href = "pagina_principal.html"; 
}

document.querySelector('.back-btn').addEventListener('click', function() {
    window.location.href = 'pagina_principal.html'; 
});
