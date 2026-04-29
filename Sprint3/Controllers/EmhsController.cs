using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Sprint3.Data;
using Sprint3.Models;
using Sprint3.Services;

namespace Sprint3.Controllers;

[ApiController]
[Route("api/[controller]")]
// [Authorize] // Descomente caso o JWT já esteja configurado no Program.cs
public class EmhsController : ControllerBase
{
    private readonly EmhsService _service;
    private readonly EmhsDbContext _context;

    public EmhsController(EmhsService service, EmhsDbContext context)
    {
        _service = service;
        _context = context;
    }

    [HttpGet("aluno/{alunoId}/disciplina/{disciplinaId}/boletim")]
    public async Task<IActionResult> GetBoletim(int alunoId, int disciplinaId)
    {
        try
        {
            var resultado = await _service.CalcularMediaEStatusAsync(alunoId, disciplinaId);
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpGet("alunos")]
    public async Task<IActionResult> GetAlunos()
    {
        var alunos = await _context.Alunos.Select(a => new { a.Id, a.Nome, a.Matricula }).ToListAsync();
        return Ok(alunos);
    }

    [HttpPost("notas")]
    public async Task<IActionResult> LancarNota([FromBody] Nota nota)
    {
        _context.Notas.Add(nota);
        await _context.SaveChangesAsync();
        return StatusCode(201, new { mensagem = "Nota lançada com sucesso!" });
    }
}
