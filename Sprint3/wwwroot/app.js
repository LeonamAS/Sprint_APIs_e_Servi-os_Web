const API_URL = "/api/auth";

// Lógica de Registro
document.getElementById('formRegistro').addEventListener('submit', async function (e) {
    e.preventDefault();

    const dados = {
        login: document.getElementById('regUsuario').value,
        senha: document.getElementById('regSenha').value,
        tipoUsuario: document.getElementById('regTipo').value
    };

    try {
        const response = await fetch(`${API_URL}/registrar`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(dados)
        });

        const result = await response.json();

        if (response.ok) {
            alert("Sucesso: " + result.mensagem);
            document.getElementById('formRegistro').reset();
        } else {
            alert("Erro: " + (result.mensagem || JSON.stringify(result)));
        }
    } catch (error) {
        alert("Erro ao conectar com a API.");
    }
});

// Lógica de Login
document.getElementById('formLogin').addEventListener('submit', async function (e) {
    e.preventDefault();

    const dados = {
        usuario: document.getElementById('loginUsuario').value,
        senha: document.getElementById('loginSenha').value
    };

    try {
        const response = await fetch(`${API_URL}/login`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(dados)
        });

        const result = await response.json();

        if (response.ok) {
            localStorage.setItem("meuToken", result.token);

            if (result.tipoUsuario === "aluno") {
                window.location.href = "dashboard_aluno.html";
            } else if (result.tipoUsuario === "professor") {
                // window.location.href = dashboard_professor.html
                alert("Bem-vindo Professor! Tela em construção.");
            } else {
                //window.location.href = dashboard_admin.html
                alert("Bem-vindo Admin! Tela em construção.");
            }
        } else {
                alert("Erro: " + result.mensagem);
        }
    } catch (error) {
        alert("Erro ao conectar com a API.");
    }
});