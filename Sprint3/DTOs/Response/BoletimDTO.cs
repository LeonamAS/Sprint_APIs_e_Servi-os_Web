namespace Sprint3.DTOs.Response;

public class BoletimDTO
{
    public string Aluno { get; set; }
    public string Disciplina { get; set; }
    public string Turma { get; set; }
    public decimal Media { get; set; }
    public decimal? Frequencia { get; set; }
    public string Status { get; set; }
}
