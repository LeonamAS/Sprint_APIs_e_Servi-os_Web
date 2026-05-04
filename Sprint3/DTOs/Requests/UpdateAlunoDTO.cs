using System.ComponentModel.DataAnnotations;

namespace Sprint3.DTOs.Requests;

public class UpdateAlunoDTO
{
    [StringLength(100, ErrorMessage = "O nome do aluno não pode ultrapassar 100 caracteres.")]
    public string? Nome { get; set; }

    [RegularExpression(@"^\d{3}\.?\d{3}\.?\d{3}-?\d{2}$", ErrorMessage = "Formato de CPF inválido.")]
    public string? Cpf { get; set; }

    public DateTime? DataNascimento { get; set; }

    [StringLength(6, MinimumLength = 6, ErrorMessage = "A matrícula deve ter exatamente 6 caracteres.")]
    [RegularExpression(@"^\d{6}$", ErrorMessage = "A matrícula deve conter exatamente 6 números.")]
    public string? Matricula { get; set; }
}
