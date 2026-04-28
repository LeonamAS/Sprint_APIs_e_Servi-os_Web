using System.ComponentModel.DataAnnotations;

namespace Sprint3.Models;

public class Professor
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "O nome do professor é obrigatório")]
    [StringLength(100, ErrorMessage = "O nome não pode exceder 100 caracteres")]
    public string Nome { get; set; }

    [Required(ErrorMessage = "A especialidade do professor é obrigatória")]
    public string Especialidade { get; set; }

    // Relacionamento: Um Professor tem muitas Turmas
    public ICollection<Turma> Turmas { get; set; }
}