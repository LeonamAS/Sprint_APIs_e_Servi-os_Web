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

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProfessorResponseDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ProfessorResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PostProfessor([FromBody] CreateProfessorDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var cpfExiste = await _context.Professores.AnyAsync(p => p.Cpf == dto.Cpf);
        if (cpfExiste)
            return BadRequest(new { Mensagem = "Já existe um professor cadastrado com este CPF." });

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

    [HttpPatch("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
            var cpfExiste = await _context.Professores.AnyAsync(p => p.Cpf == dto.Cpf && p.Id != id);
            if (cpfExiste)
                return BadRequest(new { Mensagem = "Já existe outro professor cadastrado com este CPF." });

            professor.Cpf = dto.Cpf;
        }

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProfessor(int id)
    {
        var professor = await _context.Professores.FindAsync(id);

        if (professor == null)
            return NotFound(new { Mensagem = "Professor não encontrado." });

        _context.Professores.Remove(professor);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}