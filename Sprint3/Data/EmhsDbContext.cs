using Microsoft.EntityFrameworkCore;
using Sprint3.Models;

namespace Sprint3.Data;

public class EmhsDbContext : DbContext
{
    public EmhsDbContext(DbContextOptions<EmhsDbContext> options) : base(options) { }

    public DbSet<Aluno> Alunos { get; set; }
    public DbSet<Professor> Professores { get; set; }
    public DbSet<Disciplina> Disciplinas { get; set; }
    public DbSet<Turma> Turmas { get; set; }
    public DbSet<MatriculaDisciplina> MatriculasDisciplinas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<MatriculaDisciplina>(entity =>
        {
            entity.Property(m => m.Nota)
                .HasPrecision(4, 2);

            entity.Property(m => m.Frequencia)
                .HasPrecision(5, 2);
        });

        // ==========================================
        // Configurações de Relacionamentos: Turma
        // ==========================================
        modelBuilder.Entity<Turma>()
            .HasOne(t => t.Professor)
            .WithMany(p => p.Turmas)
            .HasForeignKey(t => t.ProfessorId)
            .OnDelete(DeleteBehavior.Restrict); // Evita deletar professor com turmas ativas

        modelBuilder.Entity<Turma>()
            .HasOne(t => t.Disciplina)
            .WithMany(d => d.Turmas)
            .HasForeignKey(t => t.DisciplinaId)
            .OnDelete(DeleteBehavior.Restrict); // Evita deletar disciplina vinculada a turmas

        // ==========================================
        // Configurações de Relacionamentos: Matrícula
        // ==========================================
        modelBuilder.Entity<MatriculaDisciplina>()
            .HasOne(m => m.Aluno)
            .WithMany(a => a.Matriculas)
            .HasForeignKey(m => m.AlunoId)
            .OnDelete(DeleteBehavior.Cascade); // Se o aluno for deletado, suas matrículas/notas também são

        modelBuilder.Entity<MatriculaDisciplina>()
            .HasOne(m => m.Turma)
            .WithMany(t => t.Matriculas)
            .HasForeignKey(m => m.TurmaId)
            .OnDelete(DeleteBehavior.Cascade); // Se a turma for deletada, limpa os registros dela
    }
}