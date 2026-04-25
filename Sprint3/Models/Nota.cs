namespace Sprint3.Models;

public class Nota
{
    public int Id { get; set; }
    public decimal Valor { get; set; }

    // Chaves Estrangeiras
    public int AlunoId { get; set; }
    public Aluno Aluno { get; set; }

    public int DisciplinaId { get; set; }
    public Disciplina Disciplina { get; set; }
}
