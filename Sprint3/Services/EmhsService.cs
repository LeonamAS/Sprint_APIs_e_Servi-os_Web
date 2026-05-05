using Microsoft.EntityFrameworkCore;
using Sprint3.Data;
using Sprint3.DTOs.Response;

namespace Sprint3.Services;

public class EmhsService
{
    private readonly EmhsDbContext _context;

    public EmhsService(EmhsDbContext context)
    {
        _context = context;
    }

    public async Task<BoletimDTO> CalcularMediaEStatusAsync(int alunoId, int disciplinaId)
    {
        var aluno = await _context.Alunos.FindAsync(alunoId);
        var disciplina = await _context.Disciplinas.FindAsync(disciplinaId);

        if (aluno == null || disciplina == null)
            throw new Exception("Aluno ou Disciplina não encontrados.");

        // No novo modelo, navegamos pela tabela associativa (Matriculas)
        // e incluímos a Turma para conseguir filtrar pela Disciplina desejada.
        var matricula = await _context.Matriculas
        .Include(m => m.Turma) // "Viajamos" até a Turma...
        .Where(m => m.AlunoId == alunoId && m.Turma.DisciplinaId == disciplinaId) 
        .OrderByDescending(m => m.Id)
        .FirstOrDefaultAsync();

        if (matricula == null)
        {
            return new BoletimDTO
            {
                Aluno = aluno.Nome,
                Disciplina = disciplina.Nome,
                Turma = "N/A",
                Media = 0,
                Frequencia = 0,
                Status = "Não Matriculado"
            };
        }

        decimal media = matricula.Nota ?? 0;
        decimal frequencia = matricula.Frequencia ?? 0;
        string status;

        if (matricula.Nota.HasValue && matricula.Frequencia.HasValue)
        {
            status = (media >= 7.0m && frequencia >= 75.0m) ? "Aprovado" : "Reprovado";
        }
        else
        {
            status = "Aguardando Lançamento";
        }
        return new BoletimDTO
        {
            Aluno = aluno.Nome,
            Disciplina = disciplina.Nome,
            Turma = matricula.Turma.Nome,
            Media = Math.Round(media, 2),
            Frequencia = Math.Round(frequencia, 2),
            Status = status
        };
    }
}