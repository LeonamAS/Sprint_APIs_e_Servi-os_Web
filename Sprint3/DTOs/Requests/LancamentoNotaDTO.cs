using System.ComponentModel.DataAnnotations;

namespace Sprint3.DTOs.Requests;

public class LancamentoNotaDTO
{
    [Required(ErrorMessage = "O valor da nota é obrigatório.")]
    [Range(0, 10, ErrorMessage = "A nota deve ser um valor entre 0 e 10.")]
    public decimal Nota { get; set; }

    [Required(ErrorMessage = "O valor da frequência é obrigatório.")]
    [Range(0, 100, ErrorMessage = "A frequência deve ser um percentual entre 0 e 100.")]
    public decimal Frequencia { get; set; }
}
