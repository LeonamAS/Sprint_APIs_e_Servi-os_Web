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
    public DbSet<Nota> Notas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Nota>(entity =>
        {
            entity.Property(n => n.Valor)
                .HasPrecision(5, 2)
                .IsRequired();
        });

        // Configurações adicionais de Relacionamentos (Opcional, mas recomendado)
        modelBuilder.Entity<Turma>()
            .HasOne(t => t.Professor)
            .WithMany(p => p.Turmas)
            .HasForeignKey(t => t.ProfessorId)
            .OnDelete(DeleteBehavior.Restrict); // Evita deletar professor com turmas ativas

        modelBuilder.Entity<Nota>()
            .HasOne(n => n.Aluno)
            .WithMany(a => a.Notas)
            .HasForeignKey(n => n.AlunoId);
    }
}