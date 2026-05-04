using System.ComponentModel.DataAnnotations;

namespace Sprint3.DTOs.Requests;

public class UpdateMatriculaDTO
{
    [Range(0, 10, ErrorMessage = "A nota deve estar entre 0 e 10.")]
    public decimal? Nota { get; set; }

    [Range(0, 100, ErrorMessage = "A frequencia deve estar entre 0 e 100.")]
    public decimal? Frequencia { get; set; }
}
