using System.ComponentModel.DataAnnotations;

namespace Sprint3.DTOs.Requests;

public class UpdateProfessorDTO
{
    [StringLength(100, ErrorMessage = "O nome não pode exceder 100 caracteres.")]
    public string? Nome { get; set; }

    [RegularExpression(@"^\d{3}\.?\d{3}\.?\d{3}-?\d{2}$", ErrorMessage = "Formato de CPF inválido.")]
    [StringLength(14, ErrorMessage = "O CPF deve ter no máximo 14 caracteres.")]
    public string? Cpf { get; set; }

    public string? Especialidade { get; set; }
}
