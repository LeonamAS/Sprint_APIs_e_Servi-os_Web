namespace Sprint3.Models;

public class Aluno
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public string Matricula { get; set; }

    // Relacionamento: Um Aluno tem muitas Notas
    public ICollection<Nota> Notas { get; set; }
}
