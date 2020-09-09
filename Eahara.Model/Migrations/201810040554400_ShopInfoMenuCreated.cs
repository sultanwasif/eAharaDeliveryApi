namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ShopInfoMenuCreated : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ShopInfoes",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Description = c.String(maxLength: 400),
                        ShopId = c.Long(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Shops", t => t.ShopId, cascadeDelete: true)
                .Index(t => t.ShopId);
            
            CreateTable(
                "dbo.ShopMenus",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Image = c.String(maxLength: 200),
                        Tittle = c.String(maxLength: 150),
                        ShopId = c.Long(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Shops", t => t.ShopId, cascadeDelete: true)
                .Index(t => t.ShopId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ShopMenus", "ShopId", "dbo.Shops");
            DropForeignKey("dbo.ShopInfoes", "ShopId", "dbo.Shops");
            DropIndex("dbo.ShopMenus", new[] { "ShopId" });
            DropIndex("dbo.ShopInfoes", new[] { "ShopId" });
            DropTable("dbo.ShopMenus");
            DropTable("dbo.ShopInfoes");
        }
    }
}
