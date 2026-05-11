using System.ComponentModel.DataAnnotations;

namespace Sprint3.DTOs.Requests;

public class TurmaRequestDTO
{
    [Required(ErrorMessage = "O nome da turma é obrigatório")]
    [StringLength(20, ErrorMessage = "O nome da turma não pode exceder 20 caracteres")]
    public string Nome { get; set; }

    [Required(ErrorMessage = "O ID do professor é obrigatório")]
    public int ProfessorId { get; set; }

    [Required(ErrorMessage = "O ID da disciplina é obrigatório")]
    public int DisciplinaId { get; set; }
}
