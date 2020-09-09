namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ShopItemImagesCreated : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ItemImages",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Image = c.String(maxLength: 250),
                        Name = c.String(maxLength: 200),
                        ItemId = c.Long(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Items", t => t.ItemId, cascadeDelete: true)
                .Index(t => t.ItemId);
            
            CreateTable(
                "dbo.ShopImages",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Image = c.String(maxLength: 250),
                        Name = c.String(maxLength: 200),
                        ShopId = c.Long(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Shops", t => t.ShopId, cascadeDelete: true)
                .Index(t => t.ShopId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ShopImages", "ShopId", "dbo.Shops");
            DropForeignKey("dbo.ItemImages", "ItemId", "dbo.Items");
            DropIndex("dbo.ShopImages", new[] { "ShopId" });
            DropIndex("dbo.ItemImages", new[] { "ItemId" });
            DropTable("dbo.ShopImages");
            DropTable("dbo.ItemImages");
        }
    }
}
