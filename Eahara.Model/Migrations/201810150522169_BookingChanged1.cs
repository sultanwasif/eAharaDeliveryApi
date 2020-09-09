namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BookingChanged1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Bookings", "IsOrderLater", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Bookings", "IsOrderLater");
        }
    }
}
