using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sprint3.Models;

public class Turma
{
    [Key]
    [Required]
    public int Id { get; set; }

    [Required(ErrorMessage = "O nome da turma é obrigatório")]
    [StringLength(30, ErrorMessage = "O nome da turma não pode exceder 30 caracteres")]
    public string Nome { get; set; }

    // Chaves Estrangeiras
    [Required(ErrorMessage = "O ID do professor é obrigatório")]
    public int ProfessorId { get; set; }

    [ForeignKey("ProfessorId")]
    public Professor Professor { get; set; }

    [Required(ErrorMessage = "O ID da disciplina é obrigatório")]
    public int DisciplinaId { get; set; }

    [ForeignKey("DisciplinaId")]
    public Disciplina Disciplina { get; set; }

    public ICollection<Matricula> Matriculas { get; set; } = new List<Matricula>();
}