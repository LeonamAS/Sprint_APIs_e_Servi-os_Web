using System.ComponentModel.DataAnnotations;

namespace Sprint3.Models;

public class Disciplina
{
    [Key]
    [Required]
    public int Id { get; set; }

    [Required(ErrorMessage = "O nome da disciplina é obrigatório")]
    [StringLength(100, ErrorMessage = "O nome da disciplina não pode exceder 100 caracteres")]
    public string Nome { get; set; }

    [Required(ErrorMessage = "A carga horária é obrigatória")]
    [Range(1, 500, ErrorMessage = "A carga horária deve estar entre 1 e 500 horas")]
    public int CargaHoraria { get; set; }

    public ICollection<Turma> Turmas { get; set; } = new List<Turma>();
}