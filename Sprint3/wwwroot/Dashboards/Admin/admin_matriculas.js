const API_URL_MATRICULAS = '/api/matricula';
const API_URL_ALUNOS = '/api/aluno';
const API_URL_TURMAS = '/api/turma';
const token = localStorage.getItem("meuToken");

if (!token) {
    alert("Acesso negado. Faça login primeiro.");
    window.location.href = "index.html";
}

const modalMatricula = new bootstrap.Modal(document.getElementById('modalMatricula'));
const modalConfirmarExclusao = new bootstrap.Modal(document.getElementById('modalConfirmarExclusao'));
let idMatriculaParaExcluir = null;

// ==========================================
// CARREGAR DADOS (TABELA E SELECTS)
// ==========================================
async function carregarMatriculas() {
    const tbody = document.getElementById('tabelaMatriculas');
    try {
        const res = await fetch(API_URL_MATRICULAS, { headers: { 'Authorization': `Bearer ${token}` } });
        if (res.ok) {
            const matriculas = await res.json();
            tbody.innerHTML = matriculas.length === 0 ? '<tr><td colspan="5" class="text-center">Nenhuma matrícula encontrada.</td></tr>' : '';

            matriculas.forEach(m => {
                const notaFormatada = m.nota !== null ? parseFloat(m.nota).toFixed(1) : '<span class="text-muted">-</span>';
                const freqFormatada = m.frequencia !== null ? `${m.frequencia}%` : '<span class="text-muted">-</span>';

                tbody.innerHTML += `
                    <tr>
                        <td class="fw-bold">${m.nomeAluno}</td>
                        <td>${m.nomeTurma}</td>
                        <td class="text-center">${notaFormatada}</td>
                        <td class="text-center">${freqFormatada}</td>
                        <td class="text-center">
                            <button class="btn btn-sm btn-danger btn-action-custom" onclick="prepararExclusao(${m.id}, '${m.nomeAluno}', '${m.nomeTurma}')"" title="Cancelar Matrícula">
                                <i class="bi bi-x-circle"></i>
                            </button>
                        </td>
                    </tr>`;
            });
        }
    } catch (e) {
        tbody.innerHTML = '<tr><td colspan="5" class="text-danger text-center">Erro de conexão.</td></tr>';
    }
}

async function carregarSelects() {
    try {
        const resAlunos = await fetch(API_URL_ALUNOS, { headers: { 'Authorization': `Bearer ${token}` } });
        if (resAlunos.ok) {
            const alunos = await resAlunos.json();
            const selectAluno = document.getElementById('matriculaAlunoId');
            selectAluno.innerHTML = '<option value="">Selecione um aluno...</option>';
            alunos.forEach(a => selectAluno.innerHTML += `<option value="${a.id}">${a.nome}</option>`);
        }

        const resTurmas = await fetch(API_URL_TURMAS, { headers: { 'Authorization': `Bearer ${token}` } });
        if (resTurmas.ok) {
            const turmas = await resTurmas.json();
            const selectTurma = document.getElementById('matriculaTurmaId');
            selectTurma.innerHTML = '<option value="">Selecione uma turma...</option>';
            turmas.forEach(t => selectTurma.innerHTML += `<option value="${t.id}">${t.nome} (${t.nomeDisciplina})</option>`);
        }
    } catch (e) {
        console.error("Erro ao carregar opções para matrícula.", e);
    }
}

// ==========================================
// CADASTRAR MATRÍCULA
// ==========================================
function abrirModalMatricula() {
    document.getElementById('formMatricula').reset();
    document.getElementById('msgModalMatricula').innerHTML = '';
    modalMatricula.show();
}

document.getElementById('formMatricula').addEventListener('submit', async (e) => {
    e.preventDefault();

    const dados = {
        alunoId: parseInt(document.getElementById('matriculaAlunoId').value),
        turmaId: parseInt(document.getElementById('matriculaTurmaId').value)
    };

    const divMsg = document.getElementById('msgModalMatricula');
    divMsg.innerHTML = `<div class="alert alert-info">Processando...</div>`;

    try {
        const res = await fetch(API_URL_MATRICULAS, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json', 'Authorization': `Bearer ${token}` },
            body: JSON.stringify(dados)
        });

        if (res.ok) {
            divMsg.innerHTML = `<div class="alert alert-success">Matrícula realizada com sucesso!</div>`;
            carregarMatriculas();
            setTimeout(() => modalMatricula.hide(), 1500);
        } else {
            const erro = await res.json();
            divMsg.innerHTML = `<div class="alert alert-danger">${erro.mensagem || "Erro ao matricular."}</div>`;
        }
    } catch (error) {
        divMsg.innerHTML = `<div class="alert alert-danger">Erro de conexão.</div>`;
    }
});

// ==========================================
// EXCLUIR MATRÍCULA
// ==========================================
function prepararExclusao(id, nomeAluno, nomeTurma) {
    idMatriculaParaExcluir = id;
    document.getElementById('nomeAlunoExcluir').textContent = nomeAluno;
    document.getElementById('nomeTurmaExcluir').textContent = nomeTurma;
    modalConfirmarExclusao.show();
}

document.getElementById('btnConfirmarExclusaoFinal').addEventListener('click', async () => {
    if (!idMatriculaParaExcluir) return;

    try {
        const res = await fetch(`${API_URL_MATRICULAS}/${idMatriculaParaExcluir}`, {
            method: 'DELETE',
            headers: { 'Authorization': `Bearer ${token}` }
        });

        if (res.ok) {
            modalConfirmarExclusao.hide();
            carregarMatriculas();
        } else {
            const erro = await res.json();
            alert(erro.mensagem || "Erro ao cancelar matrícula. Verifique as permissões do seu usuário.");
        }
    } catch (error) {
        alert("Erro de conexão ao tentar excluir.");
    } finally {
        idMatriculaParaExcluir = null;
    }
});

carregarMatriculas();
carregarSelects();