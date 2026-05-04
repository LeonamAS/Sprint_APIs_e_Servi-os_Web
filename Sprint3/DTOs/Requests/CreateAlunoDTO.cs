using System.ComponentModel.DataAnnotations;

namespace Sprint3.DTOs.Requests;

public class CreateAlunoDTO
{
    [Required(ErrorMessage = "O nome do aluno é obrigatório")]
    [StringLength(100, ErrorMessage = "O nome do aluno não pode ultrapassar 100 caracteres")]
    public string Nome { get; set; }

    [Required(ErrorMessage = "O CPF é obrigatório")]
    [RegularExpression(@"^\d{3}\.?\d{3}\.?\d{3}-?\d{2}$", ErrorMessage = "Formato de CPF inválido.")]
    [StringLength(14)]
    public string Cpf { get; set; }

    [Required(ErrorMessage = "A data de nascimento é obrigatória")]
    public DateTime DataNascimento { get; set; }

    [Required(ErrorMessage = "A matrícula do aluno é obrigatória.")]
    [StringLength(6, MinimumLength = 6, ErrorMessage = "A matrícula deve ter exatamente 6 caracteres.")]
    [RegularExpression(@"^\d{6}$", ErrorMessage = "A matrícula deve conter exatamente 6 números.")]
    public string Matricula { get; set; }
}
