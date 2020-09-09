namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReviewEdit : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reviews", "Name", c => c.String(maxLength: 100));
            AddColumn("dbo.Reviews", "MobileNo", c => c.String(maxLength: 100));
            AddColumn("dbo.Reviews", "EmailId", c => c.String(maxLength: 100));
            AlterColumn("dbo.Reviews", "Rating", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Reviews", "Rating", c => c.String(maxLength: 200));
            DropColumn("dbo.Reviews", "EmailId");
            DropColumn("dbo.Reviews", "MobileNo");
            DropColumn("dbo.Reviews", "Name");
        }
    }
}
