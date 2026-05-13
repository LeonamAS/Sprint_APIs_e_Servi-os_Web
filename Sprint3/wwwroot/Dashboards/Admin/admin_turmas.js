// ==========================================
// CONFIGURAÇÕES INICIAIS
// ==========================================
const API_URL_TURMAS = '/api/turma';
const API_URL_DISCIPLINAS = '/api/disciplina';
const API_URL_PROFESSORES = '/api/professor';
const token = localStorage.getItem("meuToken");

if (!token) {
    alert("Acesso negado. Faça login primeiro.");
    window.location.href = "index.html";
}

const modalTurma = new bootstrap.Modal(document.getElementById('modalTurma'));
const modalDisciplina = new bootstrap.Modal(document.getElementById('modalDisciplina'));
const modalConfirmarExclusao = new bootstrap.Modal(document.getElementById('modalConfirmarExclusao'));

// Controle para saber o que estamos excluindo
let itemExclusao = { id: null, tipo: null };

function exibirMensagem(idElemento, mensagem, tipoCor) {
    const div = document.getElementById(idElemento);
    div.innerHTML = `<div class="alert alert-${tipoCor} alert-dismissible fade show">${mensagem}<button type="button" class="btn-close" data-bs-dismiss="alert"></button></div>`;
}

// ==========================================
// GESTÃO DE DISCIPLINAS
// ==========================================
async function carregarDisciplinas() {
    const tbody = document.getElementById('tabelaDisciplinas');
    try {
        const res = await fetch(API_URL_DISCIPLINAS, { headers: { 'Authorization': `Bearer ${token}` } });
        if (res.ok) {
            const disciplinas = await res.json();
            tbody.innerHTML = disciplinas.length === 0 ? '<tr><td colspan="3" class="text-center">Nenhuma disciplina cadastrada.</td></tr>' : '';
            disciplinas.forEach(d => {
                tbody.innerHTML += `
                    <tr>
                        <td>${d.nome}</td>
                        <td class="text-center">${d.cargaHoraria} h</td>
                        <td class="text-center">
                            <button class="btn btn-sm btn-primary me-2 btn-action-custom" onclick="abrirModalDisciplina(${d.id})"><i class="bi bi-pencil"></i></button>
                            <button class="btn btn-sm btn-danger btn-action-custom" onclick="prepararExclusao(${d.id}, '${d.nome}', 'disciplina')"><i class="bi bi-trash"></i></button>
                        </td>
                    </tr>`;
            });
            preencherSelectDisciplinas(disciplinas);
        }
    } catch (e) { tbody.innerHTML = '<tr><td colspan="3" class="text-danger text-center">Erro de conexão.</td></tr>'; }
}

async function abrirModalDisciplina(id = null) {
    document.getElementById('formDisciplina').reset();
    document.getElementById('disciplinaId').value = id || '';
    document.getElementById('msgModalDisciplina').innerHTML = '';
    document.getElementById('modalDisciplinaTitulo').textContent = id ? 'Editar Disciplina' : 'Cadastrar Disciplina';

    if (id) {
        const res = await fetch(`${API_URL_DISCIPLINAS}/${id}`, { headers: { 'Authorization': `Bearer ${token}` } });
        if (res.ok) {
            const d = await res.json();
            document.getElementById('disciplinaNome').value = d.nome;
            document.getElementById('disciplinaCarga').value = d.cargaHoraria;
        }
    }
    modalDisciplina.show();
}

document.getElementById('formDisciplina').addEventListener('submit', async (e) => {
    e.preventDefault();
    const id = document.getElementById('disciplinaId').value;
    const dados = {
        nome: document.getElementById('disciplinaNome').value,
        cargaHoraria: parseInt(document.getElementById('disciplinaCarga').value)
    };

    const res = await fetch(id ? `${API_URL_DISCIPLINAS}/${id}` : API_URL_DISCIPLINAS, {
        method: id ? 'PATCH' : 'POST',
        headers: { 'Content-Type': 'application/json', 'Authorization': `Bearer ${token}` },
        body: JSON.stringify(dados)
    });

    if (res.ok) {
        modalDisciplina.hide();
        carregarDisciplinas();
    } else {
        const erro = await res.json();
        exibirMensagem('msgModalDisciplina', erro.mensagem || "Erro ao salvar.", 'danger');
    }
});

// ==========================================
// GESTÃO DE TURMAS
// ==========================================
async function carregarTurmas() {
    const tbody = document.getElementById('tabelaTurmas');
    try {
        const res = await fetch(API_URL_TURMAS, { headers: { 'Authorization': `Bearer ${token}` } });
        if (res.ok) {
            const turmas = await res.json();
            tbody.innerHTML = turmas.length === 0 ? '<tr><td colspan="4" class="text-center">Nenhuma turma cadastrada.</td></tr>' : '';
            turmas.forEach(t => {
                tbody.innerHTML += `
                    <tr>
                        <td class="fw-bold">${t.nome}</td>
                        <td>${t.nomeDisciplina}</td>
                        <td>${t.nomeProfessor}</td>
                        <td class="text-center">
                            <button class="btn btn-sm btn-primary me-2 btn-action-custom" onclick="abrirModalTurma(${t.id})"><i class="bi bi-pencil"></i></button>
                            <button class="btn btn-sm btn-danger btn-action-custom" onclick="prepararExclusao(${t.id}, '${t.nome}', 'turma')"><i class="bi bi-trash"></i></button>
                        </td>
                    </tr>`;
            });
        }
    } catch (e) { tbody.innerHTML = '<tr><td colspan="4" class="text-danger text-center">Erro de conexão.</td></tr>'; }
}

function preencherSelectDisciplinas(disciplinas) {
    const select = document.getElementById('turmaDisciplinaId');
    select.innerHTML = '<option value="">Selecione uma disciplina...</option>';
    disciplinas.forEach(d => select.innerHTML += `<option value="${d.id}">${d.nome}</option>`);
}

async function carregarProfessoresParaSelect() {
    const select = document.getElementById('turmaProfessorId');
    try {
        const res = await fetch(API_URL_PROFESSORES, { headers: { 'Authorization': `Bearer ${token}` } });
        if (res.ok) {
            const professores = await res.json();
            select.innerHTML = '<option value="">Selecione um professor...</option>';
            professores.forEach(p => select.innerHTML += `<option value="${p.id}">${p.nome} (${p.especialidade})</option>`);
        }
    } catch (e) { console.error("Erro ao buscar professores", e); }
}

async function abrirModalTurma(id = null) {
    document.getElementById('formTurma').reset();
    document.getElementById('turmaId').value = id || '';
    document.getElementById('msgModalTurma').innerHTML = '';
    document.getElementById('modalTurmaTitulo').textContent = id ? 'Editar Turma' : 'Cadastrar Turma';

    if (id) {
        const res = await fetch(`${API_URL_TURMAS}/${id}`, { headers: { 'Authorization': `Bearer ${token}` } });
        if (res.ok) {
            const t = await res.json();
            document.getElementById('turmaNome').value = t.nome;
            document.getElementById('turmaDisciplinaId').value = t.disciplinaId;
            document.getElementById('turmaProfessorId').value = t.professorId;
        }
    }
    modalTurma.show();
}

document.getElementById('formTurma').addEventListener('submit', async (e) => {
    e.preventDefault();
    const id = document.getElementById('turmaId').value;
    const dados = {
        nome: document.getElementById('turmaNome').value,
        disciplinaId: parseInt(document.getElementById('turmaDisciplinaId').value),
        professorId: parseInt(document.getElementById('turmaProfessorId').value)
    };

    const res = await fetch(id ? `${API_URL_TURMAS}/${id}` : API_URL_TURMAS, {
        method: id ? 'PATCH' : 'POST',
        headers: { 'Content-Type': 'application/json', 'Authorization': `Bearer ${token}` },
        body: JSON.stringify(dados)
    });

    if (res.ok) {
        modalTurma.hide();
        carregarTurmas();
    } else {
        const erro = await res.json();
        exibirMensagem('msgModalTurma', erro.mensagem || "Erro ao salvar.", 'danger');
    }
});

// ==========================================
// EXCLUSÃO GENÉRICA (TURMAS E DISCIPLINAS)
// ==========================================
function prepararExclusao(id, nome, tipo) {
    itemExclusao = { id, tipo };
    document.getElementById('nomeItemExcluir').textContent = nome;
    modalConfirmarExclusao.show();
}

document.getElementById('btnConfirmarExclusaoFinal').addEventListener('click', async () => {
    if (!itemExclusao.id) return;

    const url = itemExclusao.tipo === 'turma' ? `${API_URL_TURMAS}/${itemExclusao.id}` : `${API_URL_DISCIPLINAS}/${itemExclusao.id}`;

    try {
        const res = await fetch(url, { method: 'DELETE', headers: { 'Authorization': `Bearer ${token}` } });
        if (res.ok) {
            modalConfirmarExclusao.hide();
            itemExclusao.tipo === 'turma' ? carregarTurmas() : carregarDisciplinas();
        } else {
            const erro = await res.json();
            alert(erro.mensagem || `Erro ao excluir ${itemExclusao.tipo}.`);
        }
    } catch (error) {
        alert('Erro de conexão ao tentar excluir.');
    } finally {
        itemExclusao = { id: null, tipo: null };
    }
});

// ==========================================
// INICIALIZAÇÃO DA TELA
// ==========================================
carregarDisciplinas();
carregarTurmas();
carregarProfessoresParaSelect();