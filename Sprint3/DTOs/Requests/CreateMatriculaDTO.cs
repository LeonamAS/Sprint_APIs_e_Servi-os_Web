using System.ComponentModel.DataAnnotations;

namespace Sprint3.DTOs.Requests;

public class CreateMatriculaDTO
{
    [Required(ErrorMessage = "O ID do aluno é obrigatório.")]
    public int AlunoId { get; set; }

    [Required(ErrorMessage = "O ID da turma é obrigatório.")]
    public int TurmaId { get; set; }

    [Range(0, 10, ErrorMessage = "A nota deve estar entre 0 e 10.")]
    public decimal? Nota { get; set; }

    [Range(0, 100, ErrorMessage = "A frequencia deve estar entre 0 e 100.")]
    public decimal? Frequencia { get; set; }
}
