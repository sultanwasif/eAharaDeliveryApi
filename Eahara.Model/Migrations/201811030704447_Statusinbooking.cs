namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Statusinbooking : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BookingDetails", "Status", c => c.String(maxLength: 60));
        }
        
        public override void Down()
        {
            DropColumn("dbo.BookingDetails", "Status");
        }
    }
}
