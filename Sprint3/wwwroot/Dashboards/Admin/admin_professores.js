const API_URL = '/api/professor';
const token = localStorage.getItem("meuToken");

if (!token) {
    alert("Acesso negado. Faça login primeiro.");
    window.location.href = "index.html";
}

const modalProfessor = new bootstrap.Modal(document.getElementById('modalProfessor'));
const modalConfirmarExclusao = new bootstrap.Modal(document.getElementById('modalConfirmarExclusao'));
let idProfParaExcluir = null;

function exibirMensagem(idElemento, mensagem, tipoCor) {
    const div = document.getElementById(idElemento);
    div.innerHTML = `
        <div class="alert alert-${tipoCor} alert-dismissible fade show" role="alert">
            ${mensagem}
            <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
        </div>
    `;
}

function mascaraCpf(campo) {
    let valor = campo.value.replace(/\D/g, "");
    if (valor.length > 11) valor = valor.slice(0, 11);
    valor = valor.replace(/(\d{3})(\d)/, "$1.$2");
    valor = valor.replace(/(\d{3})(\d)/, "$1.$2");
    valor = valor.replace(/(\d{3})(\d{1,2})$/, "$1-$2");
    campo.value = valor;
}

async function carregarProfessores() {
    const tbody = document.getElementById('tabelaProfessores');
    tbody.innerHTML = '<tr><td colspan="4" class="text-center text-muted">Carregando professores...</td></tr>';

    try {
        const response = await fetch(API_URL, {
            method: 'GET',
            headers: { 'Authorization': `Bearer ${token}` }
        });

        if (response.ok) {
            const professores = await response.json();

            if (professores.length === 0) {
                tbody.innerHTML = '<tr><td colspan="4" class="text-center text-muted">Nenhum professor cadastrado.</td></tr>';
                return;
            }

            tbody.innerHTML = '';
            professores.forEach(prof => {
                tbody.innerHTML += `
                    <tr>
                        <td>${prof.nome}</td>
                        <td>${prof.cpf}</td>
                        <td>${prof.especialidade}</td>
                        <td class="text-center d-flex justify-content-center">
                            <button class="btn btn-sm btn-primary me-2 btn-action-custom" onclick="abrirModalEdicao(${prof.id})">
                                <i class="bi bi-pencil fs-6"></i>
                            </button>
                            <button class="btn btn-sm btn-danger btn-action-custom" onclick="prepararExclusao(${prof.id}, '${prof.nome}')">
                                <i class="bi bi-trash fs-6"></i>
                            </button>
                        </td>
                    </tr>
                `;
            });
        } else {
            tbody.innerHTML = '<tr><td colspan="4" class="text-center text-danger">Erro ao carregar professores.</td></tr>';
        }
    } catch (error) {
        tbody.innerHTML = '<tr><td colspan="4" class="text-center text-danger">Erro de conexão com a API.</td></tr>';
    }
}

function abrirModalCadastro() {
    document.getElementById('formProfessor').reset();
    document.getElementById('profId').value = '';
    document.getElementById('msgModalProfessor').innerHTML = '';
    document.getElementById('modalProfessorTitulo').textContent = 'Cadastrar Novo Professor';
    modalProfessor.show();
}

async function abrirModalEdicao(id) {
    document.getElementById('msgModalProfessor').innerHTML = '<div class="text-info mt-2">Buscando dados...</div>';
    modalProfessor.show();

    try {
        const response = await fetch(`${API_URL}/${id}`, {
            method: 'GET',
            headers: { 'Authorization': `Bearer ${token}` }
        });

        if (response.ok) {
            const prof = await response.json();
            document.getElementById('profId').value = prof.id;
            document.getElementById('profNome').value = prof.nome;
            document.getElementById('profCpf').value = prof.cpf;
            document.getElementById('profEspecialidade').value = prof.especialidade;

            document.getElementById('modalProfessorTitulo').textContent = 'Editar Professor';
            document.getElementById('msgModalProfessor').innerHTML = '';
        } else {
            exibirMensagem('msgModalProfessor', 'Erro ao buscar dados.', 'danger');
        }
    } catch (error) {
        exibirMensagem('msgModalProfessor', 'Erro de conexão.', 'danger');
    }
}

document.getElementById('formProfessor').addEventListener('submit', async function (e) {
    e.preventDefault();

    const id = document.getElementById('profId').value;
    const dados = {
        nome: document.getElementById('profNome').value,
        cpf: document.getElementById('profCpf').value,
        especialidade: document.getElementById('profEspecialidade').value
    };

    const metodo = id ? 'PATCH' : 'POST';
    const urlFinal = id ? `${API_URL}/${id}` : API_URL;

    exibirMensagem('msgModalProfessor', 'Processando...', 'info');

    try {
        const response = await fetch(urlFinal, {
            method: metodo,
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            },
            body: JSON.stringify(dados)
        });

        if (response.ok) {
            exibirMensagem('msgModalProfessor', 'Operação realizada com sucesso!', 'success');
            carregarProfessores();
            setTimeout(() => modalProfessor.hide(), 1500);
        } else {
            const erro = await response.json();
            exibirMensagem('msgModalProfessor', erro.mensagem || "Erro ao processar a requisição.", 'danger');
        }
    } catch (error) {
        exibirMensagem('msgModalProfessor', 'Erro de conexão.', 'danger');
    }
});

function prepararExclusao(id, nome) {
    idProfParaExcluir = id;
    document.getElementById('nomeProfExcluir').textContent = nome;
    modalConfirmarExclusao.show();
}

document.getElementById('btnConfirmarExclusaoFinal').addEventListener('click', async function () {
    if (!idProfParaExcluir) return;

    try {
        const response = await fetch(`${API_URL}/${idProfParaExcluir}`, {
            method: 'DELETE',
            headers: { 'Authorization': `Bearer ${token}` }
        });

        if (response.ok) {
            modalConfirmarExclusao.hide();
            carregarProfessores();
        } else {
            const erro = await response.json();
            alert(erro.mensagem || 'Erro ao excluir professor.');
        }
    } catch (error) {
        alert('Erro de conexão ao tentar excluir.');
    } finally {
        idProfParaExcluir = null;
    }
});

carregarProfessores();