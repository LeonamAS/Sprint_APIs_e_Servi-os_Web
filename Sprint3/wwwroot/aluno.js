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

// 3. BUSCAR BOLETIM ESPECÍFICO
document.getElementById('formBoletim').addEventListener('submit', async function (e) {
    e.preventDefault(); // Evita recarregar a página

    const matriculaId = document.getElementById('inputMatricula').value;
    const cardResultado = document.getElementById('cardResultado');
    const divConteudo = document.getElementById('conteudoBoletim');

    // Mostra estado de carregamento
    cardResultado.classList.remove('d-none');
    divConteudo.innerHTML = '<p class="text-center text-muted">Buscando informações...</p>';

    try {
        // Bate no seu endpoint GET /api/matricula/{id}
        const response = await fetch(`/api/matricula/${matriculaId}`, {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            }
        });

        if (response.ok) {
            const boletim = await response.json();

            // Monta o visual do boletim
            divConteudo.innerHTML = `
                <ul class="list-group list-group-flush">
                    <li class="list-group-item"><strong>Aluno:</strong> ${boletim.nomeAluno}</li>
                    <li class="list-group-item"><strong>Turma:</strong> ${boletim.nomeTurma}</li>
                    <li class="list-group-item"><strong>Nota:</strong> <span class="badge ${boletim.nota >= 6 ? 'bg-success' : 'bg-danger'}">${boletim.nota}</span></li>
                    <li class="list-group-item"><strong>Frequência:</strong> ${boletim.frequencia}%</li>
                </ul>
            `;
        } else if (response.status === 404) {
            divConteudo.innerHTML = '<div class="alert alert-warning">Matrícula não encontrada. Verifique o número digitado.</div>';
        } else if (response.status === 401 || response.status === 403) {
            alert("Sua sessão expirou ou você não tem permissão.");
            localStorage.removeItem("meuToken");
            window.location.href = "index.html";
        } else {
            divConteudo.innerHTML = '<div class="alert alert-danger">Erro ao buscar os dados.</div>';
        }
    } catch (error) {
        console.error("Erro na API:", error);
        divConteudo.innerHTML = '<div class="alert alert-danger">Erro de conexão com o servidor.</div>';
    }
});