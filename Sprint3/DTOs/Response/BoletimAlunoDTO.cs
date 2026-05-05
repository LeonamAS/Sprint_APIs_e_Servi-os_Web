namespace Sprint3.DTOs.Response;

public class BoletimAlunoDTO
{
    public int AlunoId { get; set; }
    public string NomeAluno { get; set; }
    public string Matricula { get; set; }
    public IEnumerable<MatriculaDetalheDTO> Disciplinas { get; set; } = new List<MatriculaDetalheDTO>();
}
