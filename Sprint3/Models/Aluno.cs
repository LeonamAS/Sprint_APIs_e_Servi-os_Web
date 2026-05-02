using System.ComponentModel.DataAnnotations;

namespace Sprint3.Models;

public class Aluno
{
    [Key]
    [Required]
    public int Id { get; set; }

    [Required(ErrorMessage = "O nome do aluno é obrigatório")]
    [StringLength(100, ErrorMessage = "O nome do aluno não pode ultrapassar 100 caracteres")]
    public string Nome { get; set; }

    [Required(ErrorMessage = "O CPF é obrigatório")]
    [StringLength(14)] 
    public string Cpf { get; set; } //Checar por erros

    [Required(ErrorMessage = "A data de nascimento é obrigatória")]
    public DateTime DataNascimento { get; set; } //Checar o formato da data

    [Required(ErrorMessage = "A matrícula do aluno é obrigatória")]
    [RegularExpression(@"^[0-9]*$", ErrorMessage = "A matrícula deve conter apenas números")]
    public string Matricula { get; set; }

    // Relacionamento: Um Aluno tem muitas Notas
    public ICollection<MatriculaDisciplina> Matriculas { get; set; } = new List<MatriculaDisciplina>();
}