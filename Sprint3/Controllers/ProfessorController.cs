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
public class ProfessorController : ControllerBase
{
    private readonly EmhsDbContext _context;

    public ProfessorController(EmhsDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Lista todos os professores cadastrados.
    /// </summary>
    /// <remarks>
    /// Acesso permitido apenas para: **Administradores**.
    /// </remarks>
    /// <response code="200">Uma lista de professores simplificada.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProfessorResponseDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "administrador")]
    public async Task<IActionResult> GetProfessores()
    {
        try
        {
            var professores = await _context.Professores
                .AsNoTracking()
                .Select(p => new ProfessorResponseDTO
                {
                    Id = p.Id,
                    Nome = p.Nome,
                    Cpf = p.Cpf,
                    Especialidade = p.Especialidade
                })
                .ToListAsync();

            return Ok(professores);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Ocorreu um erro interno ao processar a requisição.");
        }
    }

    /// <summary>
    /// Obtém os detalhes de um professor específico pelo ID.
    /// </summary>
    /// <remarks>
    /// Acesso permitido apenas para: **Administradores**.
    /// </remarks>
    /// <param name="id">ID numérico do professor.</param>
    /// <response code="200">Retorna o professor encontrado.</response>
    /// <response code="404">Se o professor não for encontrado.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ProfessorResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "administrador")]
    public async Task<IActionResult> GetProfessorById(int id)
    {
        var professor = await _context.Professores
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new ProfessorResponseDTO
            {
                Id = p.Id,
                Nome = p.Nome,
                Cpf = p.Cpf,
                Especialidade = p.Especialidade
            })
            .FirstOrDefaultAsync();

        if (professor == null)
            return NotFound(new { Mensagem = "Professor não encontrado." });

        return Ok(professor);
    }

    /// <summary>
    /// Cadastra um novo professor.
    /// </summary>
    /// <remarks>
    /// Exemplo de requisição:
    /// 
    ///     POST /api/Professor
    ///     {
    ///        "nome": "Marcos Vinícius",
    ///        "cpf": "666.777.888-99",
    ///        "especialidade": "Sistemas Operacionais"
    ///     }
    /// </remarks>
    /// <response code="201">Aluno criado com sucesso.</response>
    /// <response code="400">Dados inválidos ou CPF/Matrícula já existente.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Roles = "administrador")]
    public async Task<IActionResult> PostProfessor([FromBody] CreateProfessorDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var cpfExisteProfessor = await _context.Professores.AnyAsync(p => p.Cpf == dto.Cpf);
        var cpfExisteAluno = await _context.Alunos.AnyAsync(a => a.Cpf == dto.Cpf);

        if (cpfExisteProfessor || cpfExisteAluno)
            return BadRequest(new { Mensagem = "Já existe um usuário (Professor ou Aluno) cadastrado com este CPF." });

        var novoProfessor = new Professor
        {
            Nome = dto.Nome,
            Cpf = dto.Cpf,
            Especialidade = dto.Especialidade
        };

        _context.Professores.Add(novoProfessor);
        await _context.SaveChangesAsync();

        var responseDto = new ProfessorResponseDTO
        {
            Id = novoProfessor.Id,
            Nome = novoProfessor.Nome,
            Cpf = novoProfessor.Cpf,
            Especialidade = novoProfessor.Especialidade
        };

        return CreatedAtAction(nameof(GetProfessorById), new { id = novoProfessor.Id }, responseDto);
    }

    /// <summary>
    /// Atualiza dados parciais de um professor existente.
    /// </summary>
    /// <remarks>
    /// Acesso permitido apenas para: **Administradores**.
    /// Apenas os campos enviados serão atualizados.
    /// </remarks>
    /// <param name="id">ID numérico do professor a ser atualizado.</param>
    /// <param name="dto">Objeto contendo os dados parciais para atualização.</param>
    /// <response code="204">Professor atualizado com sucesso.</response>
    /// <response code="400">Dados inválidos (CPF duplicado).</response>
    /// <response code="404">Aluno não encontrado pelo ID informado.</response>
    [HttpPatch("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Roles = "administrador")]
    public async Task<IActionResult> PatchProfessor(int id, [FromBody] UpdateProfessorDTO dto)
    {
        var professor = await _context.Professores.FindAsync(id);

        if (professor == null)
            return NotFound(new { Mensagem = "Professor não encontrado." });

        if (!string.IsNullOrWhiteSpace(dto.Nome))
            professor.Nome = dto.Nome;

        if (!string.IsNullOrWhiteSpace(dto.Especialidade))
            professor.Especialidade = dto.Especialidade;

        if (!string.IsNullOrWhiteSpace(dto.Cpf))
        {
            var cpfExisteProfessor = await _context.Professores.AnyAsync(p => p.Cpf == dto.Cpf && p.Id != id);
            var cpfExisteAluno = await _context.Alunos.AnyAsync(a => a.Cpf == dto.Cpf);

            if (cpfExisteProfessor || cpfExisteAluno)
                return BadRequest(new { Mensagem = "Já existe outro usuário (Professor ou Aluno) cadastrado com este CPF." });

            professor.Cpf = dto.Cpf;
        }

        await _context.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Remove um professor do sistema.
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "administrador")]
    public async Task<IActionResult> DeleteProfessor(int id)
    {
        var professor = await _context.Professores.FindAsync(id);

        if (professor == null)
            return NotFound(new { Mensagem = "Professor não encontrado." });

        _context.Professores.Remove(professor);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Retorna o perfil do professor logado usando o CPF do token.
    /// </summary>
    [HttpGet("meu-perfil")]
    [Authorize(Roles = "professor")]
    public async Task<IActionResult> GetMeuPerfil()
    {
        var cpfLogado = User.Identity?.Name;

        if (string.IsNullOrEmpty(cpfLogado))
            return Unauthorized(new { Mensagem = "Não foi possível identificar o usuário logado." });

        var perfilProfessor = await _context.Professores
            .AsNoTracking()
            .Where(p => p.Cpf == cpfLogado)
            .Select(p => new
            {
                Id = p.Id,
                Nome = p.Nome,
                Especialidade = p.Especialidade
            })
            .FirstOrDefaultAsync();

        if (perfilProfessor == null)
            return NotFound(new { Mensagem = "Professor não encontrado na base de dados." });

        return Ok(perfilProfessor);
    }
}