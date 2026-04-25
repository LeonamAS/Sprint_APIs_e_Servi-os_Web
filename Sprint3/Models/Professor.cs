namespace Sprint3.Models;

public class Professor
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public string Especialidade { get; set; }

    // Relacionamento: Um Professor tem muitas Turmas
    public ICollection<Turma> Turmas { get; set; }
}
