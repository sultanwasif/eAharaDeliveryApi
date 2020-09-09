namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Itemchange : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Items", "ShopId", c => c.Long(nullable: false));
            CreateIndex("dbo.Items", "ShopId");
            AddForeignKey("dbo.Items", "ShopId", "dbo.Shops", "Id", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Items", "ShopId", "dbo.Shops");
            DropIndex("dbo.Items", new[] { "ShopId" });
            DropColumn("dbo.Items", "ShopId");
        }
    }
}
