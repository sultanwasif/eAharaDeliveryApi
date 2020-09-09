namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemChanged1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Items", "InActive", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Items", "InActive");
        }
    }
}
