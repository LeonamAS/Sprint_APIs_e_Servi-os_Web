const API_URL = "/api/auth";

function exibirMensagem(elementoId, mensagem, tipoCor) {
    const divMensagem = document.getElementById(elementoId);
    divMensagem.innerHTML = `
        <div class="alert alert-${tipoCor} alert-dismissible fade show" role="alert">
            ${mensagem}
            <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
        </div>
    `;
}

function limparMensagens() {
    document.getElementById('msgLogin').innerHTML = '';
    document.getElementById('msgRegistro').innerHTML = '';
}

function aplicarMascaraCpf(campo) {
    let valor = campo.value.replace(/\D/g, "");
    if (valor.length > 11) valor = valor.slice(0, 11);

    valor = valor.replace(/(\d{3})(\d)/, "$1.$2");
    valor = valor.replace(/(\d{3})(\d)/, "$1.$2");
    valor = valor.replace(/(\d{3})(\d{1,2})$/, "$1-$2");

    campo.value = valor;
}
function mascaraLogin(campo) {
    let valor = campo.value;

    if (/[a-zA-Z]/.test(valor)) {
        return;
    }

    valor = valor.replace(/\D/g, "");
    if (valor.length > 11) valor = valor.slice(0, 11);

    valor = valor.replace(/(\d{3})(\d)/, "$1.$2");
    valor = valor.replace(/(\d{3})(\d)/, "$1.$2");
    valor = valor.replace(/(\d{3})(\d{1,2})$/, "$1-$2");

    campo.value = valor;
}

document.getElementById('formRegistro').addEventListener('submit', async function (e) {
    e.preventDefault();

    const dados = {
        cpf: document.getElementById('regCpf').value,
        senha: document.getElementById('regSenha').value
    };

    exibirMensagem('msgRegistro', 'Processando seu cadastro...', 'info');

    try {
        const response = await fetch(`${API_URL}/registrar`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(dados)
        });

        const result = await response.json();

        if (response.ok) {
            document.getElementById('formRegistro').reset();

            limparMensagens();

            document.getElementById('cardRegistro').classList.add('d-none');
            document.getElementById('cardLogin').classList.remove('d-none');

            exibirMensagem('msgLogin', result.mensagem + ' Agora é só entrar.', 'success')
        } else {
            exibirMensagem('msgRegistro', result.mensagem || "A senha deve conter pelo menos uma letra, um número e um caractere especial.", 'danger');
        }
    } catch (error) {
        exibirMensagem('msgRegistro', 'Erro de conexão com o servidor.', 'danger');
    }
});

document.getElementById('formLogin').addEventListener('submit', async function (e) {
    e.preventDefault();

    const dados = {
        cpf: document.getElementById('loginCpf').value,
        senha: document.getElementById('loginSenha').value
    };

    exibirMensagem('msgLogin', 'Autenticando...', 'info');

    try {
        const response = await fetch(`${API_URL}/login`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(dados)
        });

        const result = await response.json();

        if (response.ok) {
            exibirMensagem('msgLogin', 'Login aprovado! Redirecionando...', 'success');
            localStorage.setItem("meuToken", result.token);

            setTimeout(() => {
                if (result.tipoUsuario === "aluno") {
                    window.location.href = "dashboard_aluno.html";
                } else if (result.tipoUsuario === "professor") {
                    window.location.href = "dashboard_professor.html";
                } else {
                    window.location.href = "dashboard_admin.html";
                }
            }, 1000);

        } else {
            exibirMensagem('msgLogin', result.mensagem || "CPF ou senha incorretos.", 'danger');
        }
    } catch (error) {
        exibirMensagem('msgLogin', 'Erro de conexão com o servidor.', 'danger');
    }
});

document.getElementById('linkIrParaRegistro').addEventListener('click', function (e) {
    e.preventDefault();
    limparMensagens();
    document.getElementById('cardLogin').classList.add('d-none');
    document.getElementById('cardRegistro').classList.remove('d-none');
});

document.getElementById('linkIrParaLogin').addEventListener('click', function (e) {
    e.preventDefault();
    limparMensagens();
    document.getElementById('cardRegistro').classList.add('d-none');
    document.getElementById('cardLogin').classList.remove('d-none');
});

function toggleSenha(inputId, btnElement) {
    const input = document.getElementById(inputId);
    if (input.type === 'password') {
        input.type = 'text';
        btnElement.textContent = 'Ocultar';
    } else {
        input.type = 'password';
        btnElement.textContent = 'Mostrar';
    }
}