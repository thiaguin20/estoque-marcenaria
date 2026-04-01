namespace Estoque.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Movimentacao : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Movimentacaos",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProdutoId = c.Int(nullable: false),
                        NomeProduto = c.String(),
                        Tipo = c.String(),
                        Quantidade = c.Int(nullable: false),
                        Data = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Movimentacaos");
        }
    }
}
