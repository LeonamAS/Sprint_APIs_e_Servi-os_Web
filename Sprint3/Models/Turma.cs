namespace Sprint3.Models;

public class Turma
{
    public int Id { get; set; }
    public string CodigoDaTurma { get; set; }

    // Chaves Estrangeiras
    public int ProfessorId { get; set; }
    public Professor Professor { get; set; }

    public int DisciplinaId { get; set; }
    public Disciplina Disciplina { get; set; }
}
