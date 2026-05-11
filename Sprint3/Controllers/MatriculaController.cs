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
public class MatriculaController : ControllerBase
{
    private readonly EmhsDbContext _context;

    public MatriculaController(EmhsDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Lista todos as matriculas cadastradas.
    /// </summary>
    /// <remarks>
    /// Acesso permitido apenas para: **Administradores** e **Professsores**.
    /// </remarks>
    /// <response code="200">Uma lista de todas as matriculas.</response>
    [HttpGet]
    [Authorize(Roles = "administrador,professor")]
    public async Task<IActionResult> GetMatriculas()
    {
        var matriculas = await _context.Matriculas
            .Include(m => m.Aluno)
            .Include(m => m.Turma)
            .AsNoTracking()
            .Select(m => new MatriculaResponseDTO
            {
                Id = m.Id,
                AlunoId = m.AlunoId,
                NomeAluno = m.Aluno.Nome,
                TurmaId = m.TurmaId,
                NomeTurma = m.Turma.Nome,
                Nota = m.Nota,
                Frequencia = m.Frequencia
            })
            .ToListAsync();

        return Ok(matriculas);
    }

    /// <summary>
    /// Obtém os detalhes de uma matricula específica pelo ID.
    /// </summary>
    /// <param name="id">ID numérico da matricula.</param>
    /// <response code="200">Retorna a matricula encontrada.</response>
    /// <response code="404">Se a matricula não for encontrada.</response>
    [HttpGet("{id}")]
    [Authorize(Roles = "administrador,professor,aluno")]
    public async Task<IActionResult> GetMatriculaById(int id)
    {
        var matricula = await _context.Matriculas
            .Include(m => m.Aluno)
            .Include(m => m.Turma)
            .AsNoTracking()
            .Where(m => m.Id == id)
            .Select(m => new MatriculaResponseDTO
            {
                Id = m.Id,
                AlunoId = m.AlunoId,
                NomeAluno = m.Aluno.Nome,
                TurmaId = m.TurmaId,
                NomeTurma = m.Turma.Nome,
                Nota = m.Nota,
                Frequencia = m.Frequencia
            })
            .FirstOrDefaultAsync();

        if (matricula == null)
            return NotFound(new { Mensagem = "Matrícula não encontrada." });

        return Ok(matricula);
    }

    /// <summary>
    /// Cadastra uma nova matricula.
    /// </summary>
    /// <remarks>
    /// Exemplo de requisição:
    /// 
    ///     POST /api/Matricula
    ///     {
    ///        "alunoId": 4,
    ///        "turmaId": 5,
    ///        "nota": 10,
    ///        "frequencia": 90
    ///     }
    /// </remarks>
    [HttpPost]
    [Authorize(Roles = "administrador,professor")]
    public async Task<IActionResult> PostMatricula([FromBody] CreateMatriculaDTO dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var alunoExiste = await _context.Alunos.AnyAsync(a => a.Id == dto.AlunoId);
        var turmaExiste = await _context.Turmas.AnyAsync(t => t.Id == dto.TurmaId);

        if (!alunoExiste || !turmaExiste)
            return BadRequest(new { Mensagem = "Aluno ou Turma inválidos (ID inexistente)." });

        var matriculaDuplicada = await _context.Matriculas
            .AnyAsync(m => m.AlunoId == dto.AlunoId && m.TurmaId == dto.TurmaId);

        if (matriculaDuplicada)
            return BadRequest(new { Mensagem = "Este aluno já está matriculado nesta turma." });

        var novaMatricula = new Matricula
        {
            AlunoId = dto.AlunoId,
            TurmaId = dto.TurmaId,
            Nota = dto.Nota,
            Frequencia = dto.Frequencia
        };

        _context.Matriculas.Add(novaMatricula);
        await _context.SaveChangesAsync();

        return await GetMatriculaById(novaMatricula.Id);
    }

    /// <summary>
    /// Atualiza dados parciais de uma matricula existente.
    /// </summary>
    /// <remarks>
    /// Acesso permitido apenas para: **Administradores** e **Professores**.
    /// Apenas os campos enviados serão atualizados.
    /// </remarks>
    /// <param name="id">ID numérico da matricula a ser atualizada.</param>
    /// <param name="dto">Objeto contendo os dados parciais para atualização.</param>
    [HttpPatch("{id}")]
    [Authorize(Roles = "administradores,professor")]
    public async Task<IActionResult> PatchMatricula(int id, [FromBody] UpdateMatriculaDTO dto)
    {
        var matricula = await _context.Matriculas.FindAsync(id);
        if (matricula == null) return NotFound(new { Mensagem = "Matrícula não encontrada." });

        if (dto.Nota.HasValue)
            matricula.Nota = dto.Nota.Value;

        if (dto.Frequencia.HasValue)
            matricula.Frequencia = dto.Frequencia.Value;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>
    /// Remove um aluno do sistema.
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "professor")]
    public async Task<IActionResult> DeleteMatricula(int id)
    {
        var matricula = await _context.Matriculas.FindAsync(id);
        if (matricula == null) return NotFound(new { Mensagem = "Matrícula não encontrada." });

        _context.Matriculas.Remove(matricula);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}