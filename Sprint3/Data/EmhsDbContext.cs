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
    public DbSet<Matricula> Matriculas { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Matricula>(entity =>
        {
            entity.Property(m => m.Nota)
                .HasPrecision(4, 2);

            entity.Property(m => m.Frequencia)
                .HasPrecision(5, 2);
        });

        modelBuilder.Entity<Turma>()
            .HasOne(t => t.Professor)
            .WithMany(p => p.Turmas)
            .HasForeignKey(t => t.ProfessorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Turma>()
            .HasOne(t => t.Disciplina)
            .WithMany(d => d.Turmas)
            .HasForeignKey(t => t.DisciplinaId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Matricula>()
            .HasOne(m => m.Aluno)
            .WithMany(a => a.Matriculas)
            .HasForeignKey(m => m.AlunoId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Matricula>()
            .HasOne(m => m.Turma)
            .WithMany(t => t.Matriculas)
            .HasForeignKey(m => m.TurmaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}