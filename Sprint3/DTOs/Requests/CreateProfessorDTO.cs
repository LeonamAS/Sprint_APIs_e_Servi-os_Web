using System.ComponentModel.DataAnnotations;

namespace Sprint3.DTOs.Requests;

public class CreateProfessorDTO
{
    [Required(ErrorMessage = "O nome do professor é obrigatório.")]
    [StringLength(100, ErrorMessage = "O nome não pode exceder 100 caracteres.")]
    public string Nome { get; set; }

    [Required(ErrorMessage = "O CPF é obrigatório.")]
    [RegularExpression(@"^\d{3}\.?\d{3}\.?\d{3}-?\d{2}$", ErrorMessage = "Formato de CPF inválido.")]
    [StringLength(14, ErrorMessage = "O CPF deve ter no máximo 14 caracteres.")]
    public string Cpf { get; set; }

    [Required(ErrorMessage = "A especialidade do professor é obrigatória.")]
    public string Especialidade { get; set; }
}
