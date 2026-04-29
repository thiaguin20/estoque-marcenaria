using System.ComponentModel.DataAnnotations;

namespace Estoque.Models
{
    public class Produto
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Informe o nome do produto.")]
        [StringLength(100, ErrorMessage = "O nome deve ter no maximo 100 caracteres.")]
        public string Nome { get; set; }

        [StringLength(300, ErrorMessage = "A descricao deve ter no maximo 300 caracteres.")]
        public string Descricao { get; set; }

        [Required(ErrorMessage = "Informe o preco.")]
        [Range(0.01, 999999.99, ErrorMessage = "O preco deve ser maior que zero.")]
        public decimal Preco { get; set; }

        [StringLength(500, ErrorMessage = "A URL da imagem deve ter no maximo 500 caracteres.")]
        public string ImagemUrl { get; set; }

        [Required(ErrorMessage = "Informe a quantidade.")]
        [Range(0, 999999, ErrorMessage = "A quantidade nao pode ser negativa.")]
        public int Quantidade { get; set; }
    }
}
