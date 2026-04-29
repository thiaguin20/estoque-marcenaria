using System.ComponentModel.DataAnnotations;

namespace Estoque.Models.ViewModels
{
    public class CadastroUsuarioViewModel
    {
        [Required(ErrorMessage = "Informe o nome.")]
        [StringLength(100, ErrorMessage = "O nome deve ter no maximo 100 caracteres.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Informe o e-mail.")]
        [EmailAddress(ErrorMessage = "Informe um e-mail valido.")]
        [StringLength(150, ErrorMessage = "O e-mail deve ter no maximo 150 caracteres.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Informe a senha.")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "A senha deve ter entre 4 e 50 caracteres.")]
        public string Senha { get; set; }
    }
}
