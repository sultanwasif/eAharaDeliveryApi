namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemCategoryCancel : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ItemCategories", "ItemCategoryId", "dbo.ItemCategories");
            DropIndex("dbo.ItemCategories", new[] { "ItemCategoryId" });
            AddColumn("dbo.Items", "ItemCategoryId", c => c.Long(nullable: false));
            CreateIndex("dbo.Items", "ItemCategoryId");
            AddForeignKey("dbo.Items", "ItemCategoryId", "dbo.ItemCategories", "Id", cascadeDelete: true);
            DropColumn("dbo.ItemCategories", "ItemCategoryId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ItemCategories", "ItemCategoryId", c => c.Long(nullable: false));
            DropForeignKey("dbo.Items", "ItemCategoryId", "dbo.ItemCategories");
            DropIndex("dbo.Items", new[] { "ItemCategoryId" });
            DropColumn("dbo.Items", "ItemCategoryId");
            CreateIndex("dbo.ItemCategories", "ItemCategoryId");
            AddForeignKey("dbo.ItemCategories", "ItemCategoryId", "dbo.ItemCategories", "Id");
        }
    }
}
