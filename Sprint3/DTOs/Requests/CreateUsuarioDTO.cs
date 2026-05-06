using System.ComponentModel.DataAnnotations;

namespace Sprint3.DTOs.Requests;

public class CreateUsuarioDTO
{
    public class RegistroDTO
    {
        [Required(ErrorMessage = "O login é obrigatório.")]
        [RegularExpression(@"^[a-zA-Z]+$",
            ErrorMessage = "O login deve conter apenas letras (sem números, espaços ou caracteres especiais).")]
        public string Login { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [MinLength(8, ErrorMessage = "A senha deve ter no mínimo 8 caracteres.")]
        [RegularExpression(@"^(?=.*[a-zA-Z])(?=.*\d)(?=.*[\W_]).+$",
            ErrorMessage = "A senha deve conter pelo menos uma letra, um número e um caractere especial.")]
        public string Senha { get; set; }

        [Required(ErrorMessage = "A regra é obrigatória.")]
        public string Regra { get; set; }
    }
}
