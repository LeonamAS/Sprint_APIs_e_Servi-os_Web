using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sprint3.Models;

public class Turma
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "O código da turma é obrigatório")]
    [StringLength(20, ErrorMessage = "O código da turma não pode exceder 20 caracteres")]
    public string CodigoDaTurma { get; set; }

    // Chaves Estrangeiras
    [Required(ErrorMessage = "O ID do professor é obrigatório")]
    public int ProfessorId { get; set; }

    [ForeignKey("ProfessorId")]
    public Professor Professor { get; set; }

    [Required(ErrorMessage = "O ID da disciplina é obrigatório")]
    public int DisciplinaId { get; set; }

    [ForeignKey("DisciplinaId")]
    public Disciplina Disciplina { get; set; }
}