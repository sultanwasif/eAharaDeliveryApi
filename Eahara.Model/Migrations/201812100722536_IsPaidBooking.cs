namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IsPaidBooking : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Bookings", "IsPaid", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Bookings", "IsPaid");
        }
    }
}
