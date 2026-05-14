Desafio da Sprint de Desenvolvimento de APIs e Serviços Web.
Aluno: Leonam Araujo Sales
RA: 020.747731 
Turma: Back-End Pelourinho - manhã.

link do repositório: https://github.com/LeonamAS/Sprint_APIs_e_Servi-os_Web
Inclui também um script do mySQL para inserção de dados para facilitar nos testes da aplicação.

Documentação da API - Sistema de Gestão Escolar (Faculdade Hilberto Silva)

Introdução:

  Minha API é de um sistema de gestão escolar de uma faculdade fictícia.

  Importante: 
  * É nescesário alterar a ConnectionString no appsettings.json para poder realizar os testes.
("EmhsConnection": "server=localhost;database=HilbertoSilva;user=root;password={SUA_SENHA_AQUI}"
  * Também é nescessário criar o usuário de admin diretamente no banco de dados.
  

  Minha API é focada no gerenciamento de uma faculdade, sendo assim criei uma interface que possibilita ao administrador o total controle dos alunos e professores cadastrados no banco de dados, além das turmas e disciplinas cadastradas. O sistema também oferece portais dedicados para o corpo docente (lançamento de notas e frequências) e discente (consulta de boletins).

1. Stack Tecnológico
  A API foi desenvolvida utilizando tecnologias modernas e focadas em alta performance:

  * Framework: ASP.NET Core Web API (C#)

  * ORM (Object-Relational Mapper): Entity Framework Core

  * Banco de Dados: Relacional (MySQL)

  * Segurança: Autenticação via JSON Web Tokens (JWT)

  * Documentação: Swagger (OpenAPI)

2. Autenticação e Segurança
  A segurança da aplicação é garantida através de Tokens JWT. A arquitetura foi desenhada com os seguintes princípios:

  * Identificação por CPF: O sistema utiliza o CPF do usuário como chave de acesso (ClaimTypes.Name), que é injetado no token no momento do login e lido pelos Controllers nativamente através de User.Identity?.Name.

  * Controle de Acesso Baseado em Papéis: Os endpoints são protegidos por anotações de autorização restritas ([Authorize(Roles = "...")]), divididas em três perfis principais:

    * administrador
    * professor
    * aluno

3. Módulos e Fluxos de Negócio
   
  Módulo de Administração: Responsável pela gestão global da faculdade.

  * Gestão de Matrículas: O administrador pode matricular alunos em turmas (POST /api/matricula) e cancelar matrículas ativas (DELETE /api/matricula/{id}). Nesta etapa, apenas o vínculo é criado; o lançamento de notas não é responsabilidade da secretaria.

Módulo do Professor: Focado no diário de classe eletrônico.

  * Privacidade de Turmas: Através do endpoint GET /api/matricula/minhas, a API intercepta o CPF do professor logado no Token, busca seu ID no banco de dados e retorna estritamente os alunos matriculados nas disciplinas que ele leciona.

  * Lançamento de Desempenho: Utiliza o método PATCH /api/matricula/{id} para atualização parcial, permitindo que o professor salve a nota (0 a 10) e a frequência (0 a 100%) dos alunos individualmente de forma rápida e assíncrona.

Módulo do Aluno: Focado na consulta de desempenho acadêmico.

  * Boletim Eletrônico: O endpoint GET /api/aluno/meu-boletim utiliza o token do aluno para buscar seu histórico de notas e frequências em todas as turmas em que está matriculado, sem a necessidade de passar parâmetros expostos na URL.

4. Padrões de Projeto e Boas Práticas Implementadas
  A arquitetura de código respeita as diretrizes de código limpo e otimização de banco de dados:

  * Uso de DTOs (Data Transfer Objects): Entidades do banco de dados nunca são expostas diretamente nas requisições ou respostas. O tráfego de dados ocorre estritamente através de classes especializadas (ex: CreateMatriculaDTO, BoletimAlunoDTO, MatriculaResponseDTO), prevenindo over-posting e protegendo dados sensíveis.

  * Projeções Diretas no EF Core: Utilização do método .Select() integrado ao Entity Framework para traduzir entidades em DTOs ainda na etapa de consulta. Isso faz com que o banco de dados retorne apenas as colunas necessárias, reduzindo drasticamente o consumo de rede e memória.

  * Consultas Não-Rastreadas (NoTracking): Endpoints exclusivamente de leitura (GET) utilizam .AsNoTracking(), desligando o monitoramento de estado do ORM e entregando uma resposta muito mais rápida.

  * Validações Nativas: Os DTOs de entrada utilizam atributos de anotação de dados (como [Required] e [Range]) combinados com ModelState.IsValid para garantir a integridade dos dados antes de qualquer interação com o banco.

 * Documentação: Os Controllers possuem documentação XML e anotações de resposta ([ProducesResponseType]) que geram um Swagger intuitivo e completo para os desenvolvedores Front-end.
