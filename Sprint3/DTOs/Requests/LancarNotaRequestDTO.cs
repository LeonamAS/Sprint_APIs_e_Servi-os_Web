using Sprint3.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sprint3.DTOs.Requests;

public class LancarNotaRequestDTO
{
    [Required(ErrorMessage = "O valor da nota é obrigatório")]
    [Range(0, 10, ErrorMessage = "A nota deve estar entre 0 e 10")]
    [Column(TypeName = "decimal(5,2)")]
    public decimal Valor { get; set; }

    [Required(ErrorMessage = "O ID do aluno é obrigatório")]
    public int AlunoId { get; set; }

    [Required(ErrorMessage = "O ID da disciplina é obrigatório")]
    public int DisciplinaId { get; set; }
}
