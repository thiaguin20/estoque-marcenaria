using System.ComponentModel.DataAnnotations;

namespace Estoque.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Informe o e-mail.")]
        [EmailAddress(ErrorMessage = "Informe um e-mail valido.")]
        public string Usuario { get; set; }

        [Required(ErrorMessage = "Informe a senha.")]
        public string Senha { get; set; }
    }
}
