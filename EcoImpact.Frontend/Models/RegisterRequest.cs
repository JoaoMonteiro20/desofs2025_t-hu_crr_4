using System.ComponentModel.DataAnnotations;

namespace EcoImpact.Frontend.Models
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Username é obrigatório")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password é obrigatória")]
        [MinLength(8, ErrorMessage = "Password deve ter no mínimo 8 caracteres")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).+$",
            ErrorMessage = "A password deve conter maiúscula, minúscula, número e símbolo.")]
        public string Password { get; set; } = string.Empty;
    }
}
