// 1. VERIFICAÇÃO DE SEGURANÇA
const token = localStorage.getItem("meuToken");
if (!token) {
    alert("Acesso negado. Faça login primeiro.");
    window.location.href = "index.html";
}

// 2. FUNÇÃO DE LOGOUT
document.getElementById('btnSair').addEventListener('click', () => {
    localStorage.removeItem("meuToken");
    window.location.href = "../../index.html";
});

// 3. CARREGAR DADOS DO PROFESSOR (Esqueleto preparado)
async function carregarMinhasTurmas() {
    const divConteudo = document.getElementById('conteudoTurmas');

    // Aqui faremos a chamada para a sua API futuramente, por exemplo:
    // const response = await fetch('/api/professor/minhas-turmas', { ... })

    // Por enquanto, deixamos uma mensagem para sabermos que a tela funcionou
    divConteudo.innerHTML = `
        <div class="alert alert-info">
            Área pronta para listar os alunos e lançar notas! Precisamos criar a rota na API para buscar as turmas deste professor.
        </div>
    `;
}

// Inicializa a tela
carregarMinhasTurmas();