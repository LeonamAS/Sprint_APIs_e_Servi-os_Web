const API_URL = '/api/aluno';
const token = localStorage.getItem("meuToken");

if (!token) {
    alert("Acesso negado. Faça login primeiro.");
    window.location.href = "index.html";
}

const modalAluno = new bootstrap.Modal(document.getElementById('modalAluno'));

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

function mascaraNumeros(campo) {
    campo.value = campo.value.replace(/\D/g, "");
}

function formatarDataParaExibicao(dataIso) {
    const data = new Date(dataIso);
    return data.toLocaleDateString('pt-BR', { timeZone: 'UTC' });
}

async function carregarAlunos() {
    const tbody = document.getElementById('tabelaAlunos');
    tbody.innerHTML = '<tr><td colspan="5" class="text-center text-muted">Carregando alunos...</td></tr>';

    try {
        const response = await fetch(API_URL, {
            method: 'GET',
            headers: { 'Authorization': `Bearer ${token}` }
        });

        if (response.ok) {
            const alunos = await response.json();

            if (alunos.length === 0) {
                tbody.innerHTML = '<tr><td colspan="5" class="text-center text-muted">Nenhum aluno cadastrado.</td></tr>';
                return;
            }

            tbody.innerHTML = '';
            alunos.forEach(aluno => {
                tbody.innerHTML += `
                    <tr>
                        <td class="fw-bold text-center">${aluno.matricula}</td>
                        <td>
                            <span style="cursor: pointer;" onclick="abrirBoletim('${aluno.nome}')" title="Clique para ver o boletim">
                                ${aluno.nome}
                            </span>
                        </td>
                        <td>${aluno.cpf}</td>
                        <td>${formatarDataParaExibicao(aluno.dataNascimento)}</td>
                        <td class="text-center d-flex justify-content-center">
                            <button class="btn btn-sm btn-primary me-2 btn-action-custom" onclick="abrirModalEdicao(${aluno.id})">
                                <i class="bi bi-pencil fs-6"></i>
                            </button>
                            <button class="btn btn-sm btn-danger btn-action-custom" onclick="excluirAluno(${aluno.id}, '${aluno.nome}')">
                                <i class="bi bi-trash fs-6"></i>
                            </button>
                        </td>
                    </tr>
                `;
            });
        } else {
            tbody.innerHTML = '<tr><td colspan="5" class="text-center text-danger">Erro ao carregar alunos. Status: ' + response.status + '</td></tr>';
        }
    } catch (error) {
        tbody.innerHTML = '<tr><td colspan="5" class="text-center text-danger">Erro de conexão com a API. Verifique o console.</td></tr>';
        console.error('Erro na API:', error);
    }
}

function abrirModalCadastro() {
    document.getElementById('formAluno').reset();
    document.getElementById('alunoId').value = '';
    document.getElementById('msgModalAluno').innerHTML = '';
    document.getElementById('modalAlunoTitulo').textContent = 'Cadastrar Novo Aluno';
    modalAluno.show();
}

async function abrirModalEdicao(id) {
    document.getElementById('msgModalAluno').innerHTML = '<div class="text-info mt-2">Buscando dados...</div>';
    modalAluno.show();

    try {
        const response = await fetch(`${API_URL}/${id}`, {
            method: 'GET',
            headers: { 'Authorization': `Bearer ${token}` }
        });

        if (response.ok) {
            const aluno = await response.json();
            document.getElementById('alunoId').value = aluno.id;
            document.getElementById('alunoNome').value = aluno.nome;
            document.getElementById('alunoCpf').value = aluno.cpf;
            document.getElementById('alunoMatricula').value = aluno.matricula;
            document.getElementById('alunoDataNasc').value = aluno.dataNascimento.split('T')[0];
            document.getElementById('modalAlunoTitulo').textContent = 'Editar Aluno';
            document.getElementById('msgModalAluno').innerHTML = '';
        } else {
            exibirMensagem('msgModalAluno', 'Erro ao buscar dados do aluno.', 'danger');
        }
    } catch (error) {
        exibirMensagem('msgModalAluno', 'Erro de conexão.', 'danger');
    }
}

document.getElementById('formAluno').addEventListener('submit', async function (e) {
    e.preventDefault();

    const id = document.getElementById('alunoId').value;
    const dados = {
        nome: document.getElementById('alunoNome').value,
        cpf: document.getElementById('alunoCpf').value,
        dataNascimento: document.getElementById('alunoDataNasc').value,
        matricula: document.getElementById('alunoMatricula').value
    };

    const metodo = id ? 'PATCH' : 'POST';
    const urlFinal = id ? `${API_URL}/${id}` : API_URL;

    exibirMensagem('msgModalAluno', 'Processando...', 'info');

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
            exibirMensagem('msgModalAluno', 'Operação realizada com sucesso!', 'success');
            carregarAlunos();

            setTimeout(() => {
                modalAluno.hide();
            }, 1500);
        } else {
            const erro = await response.json();
            exibirMensagem('msgModalAluno', erro.mensagem || "Erro ao processar a requisição. Verifique os dados.", 'danger');
        }
    } catch (error) {
        exibirMensagem('msgModalAluno', 'Erro de conexão com o servidor.', 'danger');
    }
});

let idAlunoParaExcluir = null;
const modalConfirmarExclusao = new bootstrap.Modal(document.getElementById('modalConfirmarExclusao'));

async function excluirAluno(id, nome) {
    idAlunoParaExcluir = id;
    document.getElementById('nomeAlunoExcluir').textContent = nome;
    modalConfirmarExclusao.show();
}

document.getElementById('btnConfirmarExclusaoFinal').addEventListener('click', async function () {
    if (!idAlunoParaExcluir) return;

    try {
        const response = await fetch(`${API_URL}/${idAlunoParaExcluir}`, {
            method: 'DELETE',
            headers: { 'Authorization': `Bearer ${token}` }
        });

        if (response.ok) {
            modalConfirmarExclusao.hide();
            carregarAlunos();
        } else {
            const erro = await response.json();
            alert(erro.mensagem || 'Erro ao excluir aluno.');
        }
    } catch (error) {
        alert('Erro de conexão ao tentar excluir.');
    } finally {
        idAlunoParaExcluir = null;
    }
});

const modalBoletim = new bootstrap.Modal(document.getElementById('modalBoletim'));

async function abrirBoletim(nomeAluno) {
    document.getElementById('boletimNomeAluno').textContent = 'Buscando informações...';
    document.getElementById('boletimMatriculaAluno').textContent = '---';
    const tbody = document.getElementById('tabelaBoletimCorpo');
    tbody.innerHTML = '<tr><td colspan="4" class="text-center">Carregando boletim...</td></tr>';

    modalBoletim.show();

    try {
        const response = await fetch(`${API_URL}/boletim/busca-nome?nome=${encodeURIComponent(nomeAluno)}`, {
            method: 'GET',
            headers: { 'Authorization': `Bearer ${token}` }
        });

        if (response.ok) {
            const boletim = await response.json();

            document.getElementById('boletimNomeAluno').textContent = boletim.nomeAluno;
            document.getElementById('boletimMatriculaAluno').textContent = boletim.matricula;

            tbody.innerHTML = '';

            if (!boletim.disciplinas || boletim.disciplinas.length === 0) {
                tbody.innerHTML = '<tr><td colspan="4" class="text-center text-muted">Nenhuma nota ou disciplina cadastrada para este aluno.</td></tr>';
                return;
            }

            boletim.disciplinas.forEach(disc => {
                const corNota = disc.nota >= 7 ? 'text-success fw-bold' : 'text-danger fw-bold';

                tbody.innerHTML += `
                    <tr>
                        <td>${disc.nomeTurma}</td>
                        <td>${disc.nomeDisciplina}</td>
                        <td class="text-center ${corNota}">${disc.nota !== null ? disc.nota.toFixed(1) : '-'}</td>
                        <td class="text-center">${disc.frequencia !== null ? disc.frequencia + '%' : '-'}</td>
                    </tr>
                `;
            });

        } else if (response.status === 404) {
            document.getElementById('boletimNomeAluno').textContent = nomeAluno;
            tbody.innerHTML = '<tr><td colspan="4" class="text-center text-danger">Boletim não encontrado.</td></tr>';
        } else {
            document.getElementById('boletimNomeAluno').textContent = "Erro";
            tbody.innerHTML = '<tr><td colspan="4" class="text-center text-danger">Erro ao buscar o boletim.</td></tr>';
        }
    } catch (error) {
        document.getElementById('boletimNomeAluno').textContent = "Erro de conexão";
        tbody.innerHTML = '<tr><td colspan="4" class="text-center text-danger">Falha ao conectar com o servidor.</td></tr>';
    }
}

carregarAlunos();