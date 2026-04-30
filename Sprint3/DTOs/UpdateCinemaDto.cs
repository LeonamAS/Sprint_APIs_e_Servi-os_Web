using System.ComponentModel.DataAnnotations;

namespace Sprint3.DTOs;

public class UpdateCinemaDto
{
    [Required(ErrorMessage = "O campo de nome é obrigatório.")]
    public string Nome { get; set; }
}
