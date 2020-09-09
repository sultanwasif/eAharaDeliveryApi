namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemCategoryModified : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemCategories", "ItemCategoryId", c => c.Long(nullable: false));
            CreateIndex("dbo.ItemCategories", "ItemCategoryId");
            AddForeignKey("dbo.ItemCategories", "ItemCategoryId", "dbo.ItemCategories", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ItemCategories", "ItemCategoryId", "dbo.ItemCategories");
            DropIndex("dbo.ItemCategories", new[] { "ItemCategoryId" });
            DropColumn("dbo.ItemCategories", "ItemCategoryId");
        }
    }
}
