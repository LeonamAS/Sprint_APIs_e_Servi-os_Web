using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sprint3.Data;
using Sprint3.DTOs.Requests;
using Sprint3.DTOs.Response;
using Sprint3.Models;

namespace Sprint3.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "administrador,aluno,professor")]
public class AlunoController : ControllerBase
{
    private readonly EmhsDbContext _context;

    public AlunoController(EmhsDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Lista todos os alunos cadastrados.
    /// </summary>
    /// <response code="200">Uma lista de alunos simplificada.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AlunoResponseDTO>), StatusCodes.Status200OK)]
    [Authorize(Roles = "administrador,professor")]
    public async Task<IActionResult> GetAlunos()
    {
        var alunos = await _context.Alunos
            .AsNoTracking()
            .Select(a => new AlunoResponseDTO
            {
                Id = a.Id,
                Nome = a.Nome,
                Cpf = a.Cpf,
                DataNascimento = a.DataNascimento,
                Matricula = a.Matricula
            })
            .ToListAsync();

        return Ok(alunos);
    }

    /// <summary>
    /// Obtém os detalhes de um aluno específico pelo ID.
    /// </summary>
    /// <param name="id">ID numérico do aluno.</param>
    /// <response code="200">Retorna o aluno encontrado.</response>
    /// <response code="404">Se o aluno não for encontrado.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(AlunoResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "administrador,professor")]
    public async Task<IActionResult> GetAlunoById(int id)
    {
        var aluno = await _context.Alunos
            .AsNoTracking()
            .Where(a => a.Id == id)
            .Select(a => new AlunoResponseDTO
            {
                Id = a.Id,
                Nome = a.Nome,
                Cpf = a.Cpf,
                DataNascimento = a.DataNascimento,
                Matricula = a.Matricula
            })
            .FirstOrDefaultAsync();

        if (aluno == null)
            return NotFound(new { Mensagem = "Aluno não encontrado." });

        return Ok(aluno);
    }

    /// <summary>
    /// Cadastra um novo aluno.
    /// </summary>
    /// <remarks>
    /// Exemplo de requisição:
    /// 
    ///     POST /api/Aluno
    ///     {
    ///        "nome": "João Silva",
    ///        "cpf": "12345678901",
    ///        "dataNascimento": "2005-01-01",
    ///        "matricula": "A2024001"
    ///     }
    /// </remarks>
    /// <response code="201">Aluno criado com sucesso.</response>
    /// <response code="400">Dados inválidos ou CPF/Matrícula já existente.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Roles = "administrador")]
    public async Task<IActionResult> PostAluno([FromBody] CreateAlunoDTO dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        if (dto.DataNascimento.Date > DateTime.Now.Date)
            return BadRequest(new { Mensagem = "A data de nascimento não pode estar no futuro." });

        var cpfExisteAluno = await _context.Alunos.AnyAsync(a => a.Cpf == dto.Cpf);
        var cpfExisteProfessor = await _context.Professores.AnyAsync(p => p.Cpf == dto.Cpf);

        if (cpfExisteAluno || cpfExisteProfessor)
            return BadRequest(new { Mensagem = "Já existe um usuário (Aluno ou Professor) com este CPF." });

        var matriculaExiste = await _context.Alunos.AnyAsync(a => a.Matricula == dto.Matricula);
        if (matriculaExiste) return BadRequest(new { Mensagem = "Já existe um aluno com esta matrícula." });

        var novoAluno = new Aluno
        {
            Nome = dto.Nome,
            Cpf = dto.Cpf,
            DataNascimento = dto.DataNascimento,
            Matricula = dto.Matricula
        };

        _context.Alunos.Add(novoAluno);
        await _context.SaveChangesAsync();

        var responseDto = new AlunoResponseDTO
        {
            Id = novoAluno.Id,
            Nome = novoAluno.Nome,
            Cpf = novoAluno.Cpf,
            DataNascimento = novoAluno.DataNascimento,
            Matricula = novoAluno.Matricula
        };

        return CreatedAtAction(nameof(GetAlunoById), new { id = novoAluno.Id }, responseDto);
    }

    /// <summary>
    /// Atualiza dados parciais de um aluno.
    /// </summary>
    [HttpPatch("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Roles = "administrador")]
    public async Task<IActionResult> PatchAluno(int id, [FromBody] UpdateAlunoDTO dto)
    {
        var aluno = await _context.Alunos.FindAsync(id);
        if (aluno == null) return NotFound(new { Mensagem = "Aluno não encontrado." });

        if (!string.IsNullOrWhiteSpace(dto.Nome))
            aluno.Nome = dto.Nome;

        if (dto.DataNascimento.HasValue)
        {
            if (dto.DataNascimento.Value.Date > DateTime.Now.Date)
                return BadRequest(new { Mensagem = "A data de nascimento não pode estar no futuro." });

            aluno.DataNascimento = dto.DataNascimento.Value;
        }

        if (!string.IsNullOrWhiteSpace(dto.Cpf))
        {
            var cpfExisteAluno = await _context.Alunos.AnyAsync(a => a.Cpf == dto.Cpf && a.Id != id);
            var cpfExisteProfessor = await _context.Professores.AnyAsync(p => p.Cpf == dto.Cpf);

            if (cpfExisteAluno || cpfExisteProfessor)
                return BadRequest(new { Mensagem = "Já existe outro usuário (Aluno ou Professor) com este CPF." });

            aluno.Cpf = dto.Cpf;
        }

        if (!string.IsNullOrWhiteSpace(dto.Matricula))
        {
            var matriculaExiste = await _context.Alunos.AnyAsync(a => a.Matricula == dto.Matricula && a.Id != id);
            if (matriculaExiste) return BadRequest(new { Mensagem = "Já existe outro aluno com esta matrícula." });
            aluno.Matricula = dto.Matricula;
        }

        await _context.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>
    /// Remove um aluno do sistema.
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "administrador")]
    public async Task<IActionResult> DeleteAluno(int id)
    {
        var aluno = await _context.Alunos.FindAsync(id);
        if (aluno == null) return NotFound(new { Mensagem = "Aluno não encontrado." });

        _context.Alunos.Remove(aluno);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>
    /// Busca o boletim escolar de um aluno através do nome.
    /// </summary>
    /// <param name="nome">Nome ou parte do nome do aluno.</param>
    [HttpGet("boletim/busca-nome")]
    [ProducesResponseType(typeof(BoletimAlunoDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBoletimAlunoPorNome([FromQuery] string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            return BadRequest(new { Mensagem = "O nome não pode estar vazio." });

        var boletim = await _context.Alunos
            .AsNoTracking()
            .Where(a => a.Nome.ToLower().Contains(nome.ToLower()))
            .Select(a => new BoletimAlunoDTO
            {
                AlunoId = a.Id,
                NomeAluno = a.Nome,
                Matricula = a.Matricula,
                Disciplinas = a.Matriculas.Select(m => new MatriculaDetalheDTO
                {
                    NomeTurma = m.Turma.Nome,
                    NomeDisciplina = m.Turma.Disciplina.Nome,
                    Nota = m.Nota,
                    Frequencia = m.Frequencia
                }).ToList()
            })
            .FirstOrDefaultAsync();

        if (boletim == null)
            return NotFound(new { Mensagem = "Aluno não encontrado." });

        return Ok(boletim);
    }
}