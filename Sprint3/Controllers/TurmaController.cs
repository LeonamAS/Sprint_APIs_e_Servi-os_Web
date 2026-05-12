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
[Authorize(Roles = "administrador")]
public class TurmaController : ControllerBase
{
    private readonly EmhsDbContext _context;

    public TurmaController(EmhsDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Lista todos as turmas cadastradas.
    /// </summary>
    /// <remarks>
    /// Acesso permitido apenas para: **Administradores**.
    /// </remarks>
    /// <response code="200">Uma lista de turmas simplificada.</response>
    [HttpGet]
    public async Task<IActionResult> GetTurmas()
    {
        var turmas = await _context.Turmas
            .Include(t => t.Professor)
            .Include(t => t.Disciplina)
            .AsNoTracking()
            .Select(t => new TurmaResponseDTO
            {
                Id = t.Id,
                Nome = t.Nome,
                ProfessorId = t.ProfessorId,
                NomeProfessor = t.Professor.Nome,
                DisciplinaId = t.DisciplinaId,
                NomeDisciplina = t.Disciplina.Nome
            })
            .ToListAsync();

        return Ok(turmas);
    }

    /// <summary>
    /// Obtém os detalhes de uma turma específico pelo ID.
    /// </summary>
    /// <remarks>
    /// Acesso permitido apenas para: **Administradores**.
    /// </remarks>
    /// <param name="id">ID numérico da turma.</param>
    /// <response code="200">Retorna a turma encontrada.</response>
    /// <response code="404">Se a turma não for encontrada.</response>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetTurmaById(int id)
    {
        var turma = await _context.Turmas
            .Include(t => t.Professor)
            .Include(t => t.Disciplina)
            .AsNoTracking()
            .Where(t => t.Id == id)
            .Select(t => new TurmaResponseDTO
            {
                Id = t.Id,
                Nome = t.Nome,
                ProfessorId = t.ProfessorId,
                NomeProfessor = t.Professor.Nome,
                DisciplinaId = t.DisciplinaId,
                NomeDisciplina = t.Disciplina.Nome
            })
            .FirstOrDefaultAsync();

        if (turma == null)
            return NotFound(new { Mensagem = "Turma não encontrada." });

        return Ok(turma);
    }

    /// <summary>
    /// Cadastra uma nova turma.
    /// </summary>
    /// <remarks>
    /// Exemplo de requisição:
    /// 
    ///     POST /api/Turma
    ///     {
    ///        "nome": "Turma F - Sistemas Op. (Noite)",
    ///        "professorId": 6,
    ///        "disciplinaId": 6
    ///     }
    /// </remarks>
    [HttpPost]
    public async Task<IActionResult> PostTurma([FromBody] CreateTurmaDTO dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var professorExiste = await _context.Professores.AnyAsync(p => p.Id == dto.ProfessorId);
        var disciplinaExiste = await _context.Disciplinas.AnyAsync(d => d.Id == dto.DisciplinaId);

        if (!professorExiste || !disciplinaExiste)
            return BadRequest(new { Mensagem = "Professor ou Disciplina inválidos (ID inexistente)." });

        var novaTurma = new Turma
        {
            Nome = dto.Nome,
            ProfessorId = dto.ProfessorId,
            DisciplinaId = dto.DisciplinaId
        };

        _context.Turmas.Add(novaTurma);
        await _context.SaveChangesAsync();

        return await GetTurmaById(novaTurma.Id);
    }

    /// <summary>
    /// Atualiza dados parciais de uma turma existente.
    /// </summary>
    /// <remarks>
    /// Acesso permitido apenas para: **Administradores**.
    /// Apenas os campos enviados serão atualizados.
    /// </remarks>
    /// <param name="id">ID numérico da turma a ser atualizada.</param>
    /// <param name="dto">Objeto contendo os dados parciais para atualização.</param>
    /// <response code="204">Turma atualizada com sucesso.</response>
    /// <response code="400">Dados inválidos.</response>
    /// <response code="401">Usuário não autenticado.</response>
    /// <response code="403">Usuário não tem permissão para esta ação.</response>
    /// <response code="404">Turma não encontrada pelo ID informado.</response>
    [HttpPatch("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PatchTurma(int id, [FromBody] UpdateTurmaDTO dto)
    {
        var turma = await _context.Turmas.FindAsync(id);
        if (turma == null) return NotFound(new { Mensagem = "Turma não encontrada." });

        if (!string.IsNullOrWhiteSpace(dto.Nome))
            turma.Nome = dto.Nome;

        if (dto.ProfessorId.HasValue)
        {
            var existe = await _context.Professores.AnyAsync(p => p.Id == dto.ProfessorId);
            if (!existe) return BadRequest(new { Mensagem = "ID de Professor inválido." });
            turma.ProfessorId = dto.ProfessorId.Value;
        }

        if (dto.DisciplinaId.HasValue)
        {
            var existe = await _context.Disciplinas.AnyAsync(d => d.Id == dto.DisciplinaId);
            if (!existe) return BadRequest(new { Mensagem = "ID de Disciplina inválido." });
            turma.DisciplinaId = dto.DisciplinaId.Value;
        }

        await _context.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>
    /// Remove uma turma do sistema.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTurma(int id)
    {
        var turma = await _context.Turmas.FindAsync(id);
        if (turma == null) return NotFound(new { Mensagem = "Turma não encontrada." });

        _context.Turmas.Remove(turma);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}