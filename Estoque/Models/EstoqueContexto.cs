using System.Collections.Generic;
using System.Data.Entity;

namespace Estoque.Models
{
    public class EstoqueContext : DbContext
    {
        public DbSet<Produto> Produtos { get; set; }

        public DbSet<Movimentacao> Movimentacoes { get; set; }

        public DbSet<Usuario> Usuarios { get; set; }

    }
}
