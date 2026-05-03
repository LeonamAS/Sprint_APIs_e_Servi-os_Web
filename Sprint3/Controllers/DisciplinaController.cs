using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sprint3.Data;
using Sprint3.DTOs.Requests;
using Sprint3.DTOs.Response;
using Sprint3.Models;

namespace Sprint3.Controllers;

[ApiController]
[Route("api/[controller]")]
// [Authorize] // Descomente caso o JWT já esteja configurado no Program.cs
public class DisciplinaController : ControllerBase
{
    private readonly EmhsDbContext _context;

    public DisciplinaController(EmhsDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<DisciplinaResponseDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetDisciplinas()
    {
        try
        {
            var disciplinas = await _context.Disciplinas
                .Select(d => new DisciplinaResponseDTO
                {
                    Id = d.Id,
                    Nome = d.Nome,
                    CargaHoraria = d.CargaHoraria,
                })
                .AsNoTracking()
                .ToListAsync();

            return Ok(disciplinas);
        }
        catch (Exception ex)
        {
            // Aqui o ideal seria logar o erro: _logger.LogError(ex, "Erro ao buscar disciplinas.");
            return StatusCode(500, "Ocorreu um erro interno ao processar a requisição.");
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(DisciplinaResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDisciplinaById(int id)
    {
        var disciplina = await _context.Disciplinas
            .AsNoTracking() 
            .Where(d => d.Id == id) 
            .Select(d => new DisciplinaResponseDTO
            {
                Id = d.Id,
                Nome = d.Nome,
                CargaHoraria = d.CargaHoraria
            })
            .FirstOrDefaultAsync();

        if (disciplina == null)
            return NotFound(new { Mensagem = "Disciplina não encontrada." });

        return Ok(disciplina);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PostDisciplina([FromBody] CreateDisciplinaDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var novaDisciplina = new Disciplina
        {
            Nome = dto.Nome,
            CargaHoraria = dto.CargaHoraria
        };

        _context.Disciplinas.Add(novaDisciplina);
        await _context.SaveChangesAsync();

        var responseDto = new DisciplinaResponseDTO
        {
            Id = novaDisciplina.Id,
            Nome = novaDisciplina.Nome,
            CargaHoraria = novaDisciplina.CargaHoraria
        };

        return CreatedAtAction(nameof(GetDisciplinaById), new { id = novaDisciplina.Id }, responseDto);
    }

    [HttpPatch("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PatchDisciplina(int id, [FromBody] UpdateDisciplinaDTO dto)
    {
        var disciplina = await _context.Disciplinas.FindAsync(id);

        if (disciplina == null)
            return NotFound(new { Mensagem = "Disciplina não encontrada." });

        if (!string.IsNullOrWhiteSpace(dto.Nome))
        {
            disciplina.Nome = dto.Nome;
        }

        if (dto.CargaHoraria.HasValue)
        {
            disciplina.CargaHoraria = dto.CargaHoraria.Value;
        }

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteDisciplina(int id)
    {
        var disciplina = await _context.Disciplinas.FindAsync(id);

        if (disciplina == null)
            return NotFound(new { Mensagem = "Disciplina não encontrada." });

        _context.Disciplinas.Remove(disciplina);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
