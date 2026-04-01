using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Estoque.Models
{
    public class Movimentacao
    {
        public int Id { get; set; }

        public int ProdutoId { get; set; }

        public string NomeProduto { get; set; }

        public string Tipo { get; set; } // Entrada ou Saida

        public int Quantidade { get; set; }

        public DateTime Data { get; set; }
    }
}