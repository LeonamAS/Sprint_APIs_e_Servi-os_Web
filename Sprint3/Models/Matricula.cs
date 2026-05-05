using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sprint3.Models;

public class Matricula
{
    [Key]
    [Required]
    public int Id { get; set; }

    [Required(ErrorMessage = "O ID do aluno é obrigatório")]
    public int AlunoId { get; set; }
    
    [ForeignKey("AlunoId")]
    public Aluno Aluno { get; set; }

    [Required]
    public int TurmaId { get; set; }

    [ForeignKey("TurmaId")]
    public Turma Turma { get; set; }

    [Range(0, 10, ErrorMessage = "A nota deve estar entre 0 e 10")]
    [Column(TypeName = "decimal(4,2)")]
    public decimal? Nota { get; set; }

    [Range(0, 100, ErrorMessage = "A frequencia deve estar entre 0 e 100")]
    [Column(TypeName = "decimal(5,2)")]
    public decimal? Frequencia { get; set; }
}