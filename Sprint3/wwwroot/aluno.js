const token = localStorage.getItem("meuToken");

if (!token) {
    alert("Acesso negado. Faça login primeiro.");
    window.location.href = "index.html";
}

document.getElementById('btnSair').addEventListener('click', () => {
    localStorage.removeItem("meuToken");
    window.location.href = "index.html"; 
});

// 3. BUSCAR OS DADOS NA API
async function carregarMatriculas() {
    try {
        // Faz a requisição enviando o Token no cabeçalho
        const response = await fetch('/api/matricula', {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            }
        });

        if (response.ok) {
            const matriculas = await response.json();
            preencherTabela(matriculas);
        } else if (response.status === 401 || response.status === 403) {
            alert("Sua sessão expirou ou você não tem permissão.");
            localStorage.removeItem("meuToken");
            window.location.href = "index.html";
        }
    } catch (error) {
        console.error("Erro ao carregar dados:", error);
    }
}

// 4. PREENCHER A TABELA NO HTML
function preencherTabela(matriculas) {
    const tbody = document.getElementById('tabelaMatriculas');
    tbody.innerHTML = ''; // Limpa a mensagem de "Carregando..."

    if (matriculas.length === 0) {
        tbody.innerHTML = '<tr><td colspan="3" class="text-center">Nenhuma matrícula encontrada.</td></tr>';
        return;
    }

    matriculas.forEach(m => {
        const tr = document.createElement('tr');
        tr.innerHTML = `
            <td>${m.nomeTurma}</td>
            <td>${m.nota}</td>
            <td>${m.frequencia}%</td>
        `;
        tbody.appendChild(tr);
    });
}

// Executa a busca assim que a página carrega
carregarMatriculas();