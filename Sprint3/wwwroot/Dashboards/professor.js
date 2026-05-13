// ==========================================
// CONFIGURAÇÕES E SEGURANÇA
// ==========================================
const token = localStorage.getItem("meuToken");
const API_URL_MATRICULAS = '/api/matricula';

if (!token) {
    alert("Acesso negado. Faça login primeiro.");
    window.location.href = "index.html";
}

document.getElementById('btnSair').addEventListener('click', () => {
    localStorage.removeItem("meuToken");
    window.location.href = "../../index.html";
});

function exibirAlerta(mensagem, tipo) {
    const div = document.getElementById('alertaGeral');
    div.innerHTML = `<div class="alert alert-${tipo} alert-dismissible fade show" role="alert">
                        ${mensagem}
                        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
                     </div>`;
    setTimeout(() => div.innerHTML = '', 4000);
}

// ==========================================
// LÓGICA DE CARREGAMENTO E RENDERIZAÇÃO
// ==========================================
async function carregarMinhasTurmas() {
    const accordion = document.getElementById('accordionTurmas');

    try {
        const response = await fetch(`${API_URL_MATRICULAS}/minhas`, {
            headers: { 'Authorization': `Bearer ${token}` }
        });

        if (!response.ok) throw new Error("Falha ao buscar dados.");

        const matriculas = await response.json();

        if (matriculas.length === 0) {
            accordion.innerHTML = `<div class="alert alert-info text-center">Nenhum aluno matriculado em suas turmas no momento.</div>`;
            return;
        }

        const turmasAgrupadas = agruparPorTurma(matriculas);
        renderizarAccordion(turmasAgrupadas, accordion);

    } catch (error) {
        accordion.innerHTML = `<div class="alert alert-danger text-center">Erro de conexão ao carregar o diário.</div>`;
    }
}

function agruparPorTurma(matriculas) {
    return matriculas.reduce((acc, matricula) => {
        const idTurma = matricula.turmaId;
        if (!acc[idTurma]) {
            acc[idTurma] = {
                nomeTurma: matricula.nomeTurma,
                alunos: []
            };
        }
        acc[idTurma].alunos.push(matricula);
        return acc;
    }, {});
}

function renderizarAccordion(turmas, container) {
    container.innerHTML = '';

    let index = 0;
    for (const [turmaId, turmaData] of Object.entries(turmas)) {
        const isExpanded = index === 0 ? "true" : "false";
        const collapseClass = index === 0 ? "show" : "";
        const headerId = `heading${turmaId}`;
        const collapseId = `collapse${turmaId}`;

        let trsAlunos = '';
        turmaData.alunos.forEach(aluno => {
            const notaVal = aluno.nota !== null ? aluno.nota : '';
            const freqVal = aluno.frequencia !== null ? aluno.frequencia : '';

            trsAlunos += `
                <tr>
                    <td class="align-middle fw-bold">${aluno.nomeAluno}</td>
                    <td class="align-middle text-center">
                        <input type="number" id="nota_${aluno.id}" class="form-control input-nota-freq mx-auto" step="0.1" min="0" max="10" value="${notaVal}" placeholder="0.0">
                    </td>
                    <td class="align-middle text-center">
                        <div class="input-group input-group-sm mx-auto" style="max-width: 110px;">
                            <input type="number" id="freq_${aluno.id}" class="form-control text-center" min="0" max="100" value="${freqVal}" placeholder="0">
                            <span class="input-group-text">%</span>
                        </div>
                    </td>
                    <td class="align-middle text-center">
                        <button class="btn btn-sm btn-success" onclick="salvarDesempenho(${aluno.id})" id="btn_${aluno.id}">
                            <i class="bi bi-check-circle me-1"></i> Salvar
                        </button>
                    </td>
                </tr>
            `;
        });

        container.innerHTML += `
            <div class="accordion-item">
                <h2 class="accordion-header" id="${headerId}">
                    <button class="accordion-button ${index !== 0 ? 'collapsed' : ''}" type="button" data-bs-toggle="collapse" data-bs-target="#${collapseId}" aria-expanded="${isExpanded}" aria-controls="${collapseId}">
                        <i class="bi bi-bookmark-fill me-2"></i> ${turmaData.nomeTurma}
                    </button>
                </h2>
                <div id="${collapseId}" class="accordion-collapse collapse ${collapseClass}" aria-labelledby="${headerId}" data-bs-parent="#accordionTurmas">
                    <div class="accordion-body p-0 table-responsive">
                        <table class="table table-hover table-striped mb-0">
                            <thead class="table-light">
                                <tr>
                                    <th>Nome do Aluno</th>
                                    <th class="text-center">Nota (0-10)</th>
                                    <th class="text-center">Frequência (%)</th>
                                    <th class="text-center">Ações</th>
                                </tr>
                            </thead>
                            <tbody>
                                ${trsAlunos}
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
        `;
        index++;
    }
}

// ==========================================
// LÓGICA DE ATUALIZAÇÃO (PATCH)
// ==========================================
async function salvarDesempenho(idMatricula) {
    const notaInput = document.getElementById(`nota_${idMatricula}`).value;
    const freqInput = document.getElementById(`freq_${idMatricula}`).value;
    const btn = document.getElementById(`btn_${idMatricula}`);

    const payload = {
        nota: notaInput === "" ? null : parseFloat(notaInput),
        frequencia: freqInput === "" ? null : parseFloat(freqInput)
    };

    if (payload.nota !== null && (payload.nota < 0 || payload.nota > 10)) {
        exibirAlerta("A nota deve estar entre 0 e 10.", "warning"); return;
    }
    if (payload.frequencia !== null && (payload.frequencia < 0 || payload.frequencia > 100)) {
        exibirAlerta("A frequência deve estar entre 0 e 100.", "warning"); return;
    }

    btn.innerHTML = `<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>`;
    btn.disabled = true;

    try {
        const response = await fetch(`${API_URL_MATRICULAS}/${idMatricula}`, {
            method: 'PATCH',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            },
            body: JSON.stringify(payload)
        });

        if (response.ok) {
            btn.classList.replace('btn-success', 'btn-primary');
            btn.innerHTML = `<i class="bi bi-check2-all me-1"></i> Salvo`;
            setTimeout(() => {
                btn.classList.replace('btn-primary', 'btn-success');
                btn.innerHTML = `<i class="bi bi-check-circle me-1"></i> Salvar`;
            }, 2000);
        } else {
            const erro = await response.json();
            exibirAlerta(erro.mensagem || "Erro ao salvar os dados.", "danger");
            btn.innerHTML = `<i class="bi bi-check-circle me-1"></i> Salvar`;
        }
    } catch (error) {
        exibirAlerta("Erro de conexão ao tentar salvar.", "danger");
        btn.innerHTML = `<i class="bi bi-check-circle me-1"></i> Salvar`;
    } finally {
        btn.disabled = false;
    }
}
// ==========================================
// FUNÇÃO PARA BUSCAR O PERFIL DO PROFESSOR
// ==========================================
async function carregarPerfilProfessor() {
    try {
        const response = await fetch('/api/professor/meu-perfil', {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            }
        });

        if (response.ok) {
            const perfil = await response.json();
            const primeiroNome = perfil.nome.split(' ')[0];
            document.getElementById('nomeUsuario').innerHTML = `Olá, <strong>${primeiroNome}</strong>!`;
        } else if (response.status === 401 || response.status === 403) {
            alert("Sua sessão expirou ou você não tem permissão.");
            localStorage.removeItem("meuToken");
            window.location.href = "../../index.html";
        }
    } catch (error) {
        console.error("Erro ao buscar perfil do professor:", error);
    }
}

carregarPerfilProfessor();
carregarMinhasTurmas();