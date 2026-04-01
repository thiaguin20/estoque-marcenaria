using System.ComponentModel.DataAnnotations;

namespace Estoque.Models
{
    public class Produto
    {
        [Key]
        public int Id { get; set; }

        
        public string Nome { get; set; }

        
        public string Descricao { get; set; }

        
        public decimal Preco { get; set; }

        public string ImagemUrl { get; set; }

        public int Quantidade { get; set; }
    }
}