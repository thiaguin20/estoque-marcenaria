using System;
using System.ComponentModel.DataAnnotations;

namespace Estoque.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nome { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; }

        [Required]
        [StringLength(64)]
        public string SenhaHash { get; set; }

        public bool PodeAcessarAdmin { get; set; }

        public DateTime DataCadastro { get; set; }
    }
}
