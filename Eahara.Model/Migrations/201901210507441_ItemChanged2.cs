namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemChanged2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MEDItems", "MEDCategoryId", c => c.Long());
            CreateIndex("dbo.MEDItems", "MEDCategoryId");
            AddForeignKey("dbo.MEDItems", "MEDCategoryId", "dbo.MEDCategories", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MEDItems", "MEDCategoryId", "dbo.MEDCategories");
            DropIndex("dbo.MEDItems", new[] { "MEDCategoryId" });
            DropColumn("dbo.MEDItems", "MEDCategoryId");
        }
    }
}
