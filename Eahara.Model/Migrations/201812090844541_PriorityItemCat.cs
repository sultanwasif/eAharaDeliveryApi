namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PriorityItemCat : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemCategories", "Priority", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ItemCategories", "Priority");
        }
    }
}
