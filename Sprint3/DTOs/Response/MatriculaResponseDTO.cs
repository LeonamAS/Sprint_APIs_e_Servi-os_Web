namespace Sprint3.DTOs.Response;

public class MatriculaResponseDTO
{
    public int Id { get; set; }
    public int AlunoId { get; set; }
    public string NomeAluno { get; set; }
    public int TurmaId { get; set; }
    public string NomeTurma { get; set; }
    public decimal? Nota { get; set; }
    public decimal? Frequencia { get; set; }
}
