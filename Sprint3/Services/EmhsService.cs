using Microsoft.EntityFrameworkCore;
using Sprint3.Data;
using Sprint3.DTOs;

namespace Sprint3.Services
{
    public class EmhsService
    {
        private readonly EmhsDbContext _context;

        public EmhsService(EmhsDbContext context)
        {
            _context = context;
        }

        public async Task<BoletimDto> CalcularMediaEStatusAsync(int alunoId, int disciplinaId)
        {
            var aluno = await _context.Alunos.FindAsync(alunoId);
            var disciplina = await _context.Disciplinas.FindAsync(disciplinaId);

            if (aluno == null || disciplina == null)
                throw new Exception("Aluno ou Disciplina não encontrados.");

            var notas = await _context.Notas
                .Where(n => n.AlunoId == alunoId && n.DisciplinaId == disciplinaId)
                .ToListAsync();

            if (!notas.Any())
                return new BoletimDto { Aluno = aluno.Nome, Disciplina = disciplina.Nome, Media = 0, Status = "Sem Notas Lançadas" };

            decimal media = notas.Average(n => n.Valor);

            string status = media >= 5.0m ? "Aprovado" : "Reprovado";

            return new BoletimDto
            {
                Aluno = aluno.Nome,
                Disciplina = disciplina.Nome,
                Media = Math.Round(media, 2),
                Status = status
            };
        }
    }
}