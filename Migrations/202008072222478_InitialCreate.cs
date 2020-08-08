namespace BuyingHistory.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Albums",
                c => new
                    {
                        AlbumId = c.Int(nullable: false, identity: true),
                        ItemId = c.Int(nullable: false),
                        Band = c.String(),
                        Title = c.String(),
                        Format = c.String(),
                    })
                .PrimaryKey(t => t.AlbumId)
                .ForeignKey("dbo.Items", t => t.ItemId, cascadeDelete: true)
                .Index(t => t.ItemId);
            
            CreateTable(
                "dbo.Items",
                c => new
                    {
                        ItemId = c.Int(nullable: false, identity: true),
                        SaleId = c.Int(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ItemId)
                .ForeignKey("dbo.Sales", t => t.SaleId, cascadeDelete: true)
                .Index(t => t.SaleId);
            
            CreateTable(
                "dbo.Sales",
                c => new
                    {
                        SaleId = c.Int(nullable: false, identity: true),
                        Store = c.String(nullable: false, maxLength: 100),
                        Seller = c.String(maxLength: 100),
                        Total = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.SaleId)
                .Index(t => new { t.Store, t.Seller, t.Total, t.Date }, unique: true, name: "IX_UniqueFields");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Items", "SaleId", "dbo.Sales");
            DropForeignKey("dbo.Albums", "ItemId", "dbo.Items");
            DropIndex("dbo.Sales", "IX_UniqueFields");
            DropIndex("dbo.Items", new[] { "SaleId" });
            DropIndex("dbo.Albums", new[] { "ItemId" });
            DropTable("dbo.Sales");
            DropTable("dbo.Items");
            DropTable("dbo.Albums");
        }
    }
}
