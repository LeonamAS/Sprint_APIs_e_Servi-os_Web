// 1. VERIFICAÇÃO DE SEGURANÇA
const token = localStorage.getItem("meuToken");

if (!token) {
    alert("Acesso negado. Faça login primeiro.");
    window.location.href = "index.html";
}

// 2. FUNÇÃO DE LOGOUT
document.getElementById('btnSair').addEventListener('click', () => {
    localStorage.removeItem("meuToken");
    window.location.href = "index.html";
});

// 3. BUSCAR BOLETIM POR NOME
document.getElementById('formBoletim').addEventListener('submit', async function (e) {
    e.preventDefault();

    // Agora pegamos o valor do input de texto
    const nomeAluno = document.getElementById('inputNomeAluno').value;
    const cardResultado = document.getElementById('cardResultado');
    const divConteudo = document.getElementById('conteudoBoletim');

    cardResultado.classList.remove('d-none');
    divConteudo.innerHTML = '<p class="text-center text-muted">Gerando boletim completo...</p>';

    try {
        // encodeURIComponent garante que espaços e acentos não quebrem a URL
        const urlBusca = `/api/aluno/boletim/busca-nome?nome=${encodeURIComponent(nomeAluno)}`;

        const response = await fetch(urlBusca, {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            }
        });

        if (response.ok) {
            const boletim = await response.json();

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
                    const corNota = d.nota >= 6 ? 'text-success fw-bold' : 'text-danger fw-bold';

                    htmlFinal += `
                        <tr>
                            <td>${d.nomeDisciplina}</td>
                            <td>${d.nomeTurma}</td>
                            <td class="${corNota}">${d.nota}</td>
                            <td>${d.frequencia}%</td>
                        </tr>
                    `;
                });

                htmlFinal += `
                        </tbody>
                    </table>
                </div>`;
            } else {
                htmlFinal += `<div class="alert alert-info mt-3">Nenhuma nota registrada para este aluno ainda.</div>`;
            }

            divConteudo.innerHTML = htmlFinal;

        } else if (response.status === 404) {
            divConteudo.innerHTML = '<div class="alert alert-warning">Aluno não encontrado. Verifique se o nome está correto.</div>';
        } else if (response.status === 401 || response.status === 403) {
            alert("Sua sessão expirou ou você não tem permissão.");
            localStorage.removeItem("meuToken");
            window.location.href = "index.html";
        } else {
            divConteudo.innerHTML = '<div class="alert alert-danger">Erro ao buscar os dados do boletim.</div>';
        }
    } catch (error) {
        console.error("Erro na API:", error);
        divConteudo.innerHTML = '<div class="alert alert-danger">Erro de conexão com o servidor.</div>';
    }
});