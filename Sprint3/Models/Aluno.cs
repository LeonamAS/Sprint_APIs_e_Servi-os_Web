using System.ComponentModel.DataAnnotations;

namespace Sprint3.Models;

public class Aluno
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "O nome do aluno é obrigatório")]
    [StringLength(100, ErrorMessage = "O nome do aluno não pode ultrapassar 100 caracteres")]
    public string Nome { get; set; }

    [Required(ErrorMessage = "A matrícula do aluno é obrigatória")]
    [RegularExpression(@"^[0-9]*$", ErrorMessage = "A matrícula deve conter apenas números")]
    public string Matricula { get; set; }

    // Relacionamento: Um Aluno tem muitas Notas
    public ICollection<Nota> Notas { get; set; }
}