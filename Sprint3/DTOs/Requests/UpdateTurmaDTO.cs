using System.ComponentModel.DataAnnotations;

namespace Sprint3.DTOs.Requests;

public class UpdateTurmaDTO
{
    [StringLength(20, ErrorMessage = "O nome da turma não pode exceder 20 caracteres.")]
    public string? Nome { get; set; }
    public int? ProfessorId { get; set; }
    public int? DisciplinaId { get; set; }
}
