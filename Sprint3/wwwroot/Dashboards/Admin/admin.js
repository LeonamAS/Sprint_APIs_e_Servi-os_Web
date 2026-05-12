const token = localStorage.getItem("meuToken");
const API_URL = "/api/auth";

if (!token) {
    alert("Acesso negado. Faça login primeiro.");
    window.location.href = "index.html";
}

document.getElementById('btnSair').addEventListener('click', () => {
    localStorage.removeItem("meuToken");
    window.location.href = "../../index.html";
});

function exibirMensagemModal(mensagem, tipoCor) {
    const divMensagem = document.getElementById('msgModalAdmin');
    divMensagem.innerHTML = `
        <div class="alert alert-${tipoCor} alert-dismissible fade show" role="alert">
            ${mensagem}
            <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
        </div>
    `;
}

document.getElementById('formNovoAdmin').addEventListener('submit', async function (e) {
    e.preventDefault();

    const dados = {
        cpf: document.getElementById('novoAdminLogin').value,
        senha: document.getElementById('novoAdminSenha').value
    };

    exibirMensagemModal('Processando...', 'info');

    try {
        const response = await fetch(`${API_URL}/registrar-admin`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            },
            body: JSON.stringify(dados)
        });

        const result = await response.json();

        if (response.ok) {
            exibirMensagemModal(result.mensagem, 'success');

            document.getElementById('formNovoAdmin').reset();

            setTimeout(() => {
                const modalElement = document.getElementById('modalAdmin');
                const modalInstance = bootstrap.Modal.getInstance(modalElement);

                if (modalInstance) {
                    modalInstance.hide();
                }

                document.getElementById('msgModalAdmin').innerHTML = '';
            }, 2000);

        } else {
            exibirMensagemModal(result.mensagem || "Erro ao cadastrar.", 'danger');
        }
    } catch (error) {
        exibirMensagemModal('Erro de conexão com o servidor.', 'danger');
    }
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