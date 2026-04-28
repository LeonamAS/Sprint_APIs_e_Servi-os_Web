using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sprint3.Models;

public class Nota
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "O valor da nota é obrigatório")]
    [Range(0, 10, ErrorMessage = "A nota deve estar entre 0 e 10")]
    [Column(TypeName = "decimal(5,2)")]
    public decimal Valor { get; set; }

    // Chaves Estrangeiras
    [Required(ErrorMessage = "O ID do aluno é obrigatório")]
    public int AlunoId { get; set; }
    
    [ForeignKey("AlunoId")]
    public Aluno Aluno { get; set; }

    [Required(ErrorMessage = "O ID da disciplina é obrigatório")]
    public int DisciplinaId { get; set; }

    [ForeignKey("DisciplinaId")]
    public Disciplina Disciplina { get; set; }
}