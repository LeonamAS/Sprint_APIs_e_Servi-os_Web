const token = localStorage.getItem("meuToken");
if (!token) {
    alert("Acesso negado. Faça login primeiro.");
    window.location.href = "index.html";
}

document.getElementById('btnSair').addEventListener('click', () => {
    localStorage.removeItem("meuToken");
    window.location.href = "index.html";
});

async function carregarMeuBoletim() {
    const divConteudo = document.getElementById('conteudoBoletim');

    try {
        const response = await fetch('/api/aluno/meu-boletim', {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            }
        });

        if (response.ok) {
            const boletim = await response.json();

            const primeiroNome = boletim.nomeAluno.split(' ')[0];

            document.getElementById('nomeUsuario').textContent = `Bem-vindo(a), ${primeiroNome}!`;

            let htmlFinal = `
                <div class="mb-4">
                    <h6><strong>Nome do Aluno:</strong> ${boletim.nomeAluno}</h6>
                    <h6><strong>Código de Matrícula:</strong> ${boletim.matricula}</h6>
                </div>
                <h6 class="border-bottom pb-2">Desempenho nas Disciplinas</h6>
            `;

            if (boletim.disciplinas && boletim.disciplinas.length > 0) {
                htmlFinal += `
                <div class="table-responsive">
                    <table class="table table-sm table-striped mt-3">
                        <thead class="table-light">
                            <tr>
                                <th>Disciplina</th>
                                <th>Turma</th>
                                <th>Nota</th>
                                <th>Frequência</th>
                            </tr>
                        </thead>
                        <tbody>
                `;

                boletim.disciplinas.forEach(d => {
                    const temNota = d.nota !== null && d.nota !== undefined;
                    const temFreq = d.frequencia !== null && d.frequencia !== undefined;

                    const textoNota = temNota ? d.nota : '<span class="text-muted fst-italic">Sem dados no momento</span>';
                    const textoFreq = temFreq ? `${d.frequencia}%` : '<span class="text-muted fst-italic">Sem dados no momento</span>';

                    const corNota = temNota ? (d.nota >= 7 ? 'text-success fw-bold' : 'text-danger fw-bold') : '';

                    htmlFinal += `
                        <tr>
                            <td>${d.nomeDisciplina}</td>
                            <td>${d.nomeTurma}</td>
                            <td class="${corNota}">${textoNota}</td>
                            <td>${textoFreq}</td>
                        </tr>
                    `;
                });

                htmlFinal += `
                        </tbody>
                    </table>
                </div>`;
            } else {
                htmlFinal += `<div class="alert alert-info mt-3">Nenhuma nota registrada para você ainda.</div>`;
            }

            divConteudo.innerHTML = htmlFinal;

        } else if (response.status === 404) {
            divConteudo.innerHTML = '<div class="alert alert-warning mt-3">Seu CPF não foi encontrado na base de alunos. Contate a secretaria.</div>';
        } else if (response.status === 401 || response.status === 403) {
            alert("Sua sessão expirou ou você não tem permissão.");
            localStorage.removeItem("meuToken");
            window.location.href = "index.html";
        } else {
            divConteudo.innerHTML = '<div class="alert alert-danger mt-3">Erro ao buscar os dados do seu boletim.</div>';
        }
    } catch (error) {
        console.error("Erro na API:", error);
        divConteudo.innerHTML = '<div class="alert alert-danger mt-3">Erro de conexão com o servidor.</div>';
    }
}

carregarMeuBoletim();