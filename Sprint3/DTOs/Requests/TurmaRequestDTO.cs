using System.ComponentModel.DataAnnotations;

namespace Sprint3.DTOs.Requests;

public class TurmaRequestDTO
{
    [Required(ErrorMessage = "O código da turma é obrigatório")]
    [StringLength(20, ErrorMessage = "O código da turma não pode exceder 20 caracteres")]
    public string CodigoDaTurma { get; set; }

    // Chaves Estrangeiras
    [Required(ErrorMessage = "O ID do professor é obrigatório")]
    public int ProfessorId { get; set; }

    [Required(ErrorMessage = "O ID da disciplina é obrigatório")]
    public int DisciplinaId { get; set; }
}
