using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sprint3.Data;
using Sprint3.DTOs.Requests;
using Sprint3.DTOs.Response;
using Sprint3.Models;

namespace Sprint3.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TurmaController : ControllerBase
{
    private readonly EmhsDbContext _context;

    public TurmaController(EmhsDbContext context)
    {
        _context = context;
    }

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

    [HttpPatch("{id}")]
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