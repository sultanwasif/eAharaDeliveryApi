namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ShopCategoryId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Shops", "ShopCategoryId", c => c.Long(nullable: false));
            CreateIndex("dbo.Shops", "ShopCategoryId");
            AddForeignKey("dbo.Shops", "ShopCategoryId", "dbo.ShopCategories", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Shops", "ShopCategoryId", "dbo.ShopCategories");
            DropIndex("dbo.Shops", new[] { "ShopCategoryId" });
            DropColumn("dbo.Shops", "ShopCategoryId");
        }
    }
}
