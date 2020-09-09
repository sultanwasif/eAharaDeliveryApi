namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TagCreated : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Tags",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        IsActive = c.Boolean(nullable: false),
                        Description = c.String(maxLength: 400),
                        ShopId = c.Long(),
                        ItemId = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Items", t => t.ItemId)
                .ForeignKey("dbo.Shops", t => t.ShopId)
                .Index(t => t.ShopId)
                .Index(t => t.ItemId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tags", "ShopId", "dbo.Shops");
            DropForeignKey("dbo.Tags", "ItemId", "dbo.Items");
            DropIndex("dbo.Tags", new[] { "ItemId" });
            DropIndex("dbo.Tags", new[] { "ShopId" });
            DropTable("dbo.Tags");
        }
    }
}
