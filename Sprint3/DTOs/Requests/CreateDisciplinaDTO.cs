using System.ComponentModel.DataAnnotations;

namespace Sprint3.DTOs.Requests;

public class CreateDisciplinaDTO
{
    [Required(ErrorMessage = "O nome da disciplina é obrigatório.")]
    [MaxLength(100, ErrorMessage = "O nome não pode exceder 100 caracteres.")]
    public string Nome { get; set; }

    [Range(10, 500, ErrorMessage = "A carga horária deve ser entre 10 e 500 horas.")]
    public int CargaHoraria { get; set; }
}
