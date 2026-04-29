using System.ComponentModel.DataAnnotations;

namespace Sprint3.DTOs.Requests;

public class ProfessorRequestDTO
{
    [Required(ErrorMessage = "O nome do professor é obrigatório")]
    [StringLength(100, ErrorMessage = "O nome não pode exceder 100 caracteres")]
    public string Nome { get; set; }

    [Required(ErrorMessage = "A especialidade do professor é obrigatória")]
    public string Especialidade { get; set; }
}
