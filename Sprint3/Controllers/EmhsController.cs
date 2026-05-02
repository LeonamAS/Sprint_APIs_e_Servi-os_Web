using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sprint3.Data;
using Sprint3.DTOs.Requests;
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

    [HttpPost("matricular")]
    public async Task<IActionResult> MatricularAluno([FromBody] MatriculaDisciplina matricula)
    {
        var existe = await _context.MatriculasDisciplinas
            .AnyAsync(m => m.AlunoId == matricula.AlunoId && m.TurmaId == matricula.TurmaId);

        if (existe)
            return BadRequest(new { mensagem = "Aluno já matriculado nesta turma." });

        matricula.Nota = null;
        matricula.Frequencia = null;

        _context.MatriculasDisciplinas.Add(matricula);
        await _context.SaveChangesAsync();

        return StatusCode(201, new
        {
            mensagem = "Aluno matriculado com sucesso!",
            matriculaId = matricula.Id
        });
    }

    [HttpPatch("{matriculaId}/lancar-nota")]
    public async Task<IActionResult> LancarNota(int matriculaId, [FromBody] LancamentoNotaDTO dados)
    {
        var matriculaNoBanco = await _context.MatriculasDisciplinas.FindAsync(matriculaId);

        if (matriculaNoBanco == null)
            return NotFound(new { mensagem = "Registro de matrícula não encontrado." });

        matriculaNoBanco.Nota = dados.Nota;
        matriculaNoBanco.Frequencia = dados.Frequencia;

        await _context.SaveChangesAsync();

        return Ok(new { mensagem = "Nota e frequência lançadas com sucesso!" });
    }
}
