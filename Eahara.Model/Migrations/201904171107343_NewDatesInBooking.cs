namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewDatesInBooking : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Bookings", "PickUpDate", c => c.DateTime());
            AddColumn("dbo.Bookings", "AssignedDate", c => c.DateTime());
            AddColumn("dbo.Bookings", "StatusDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Bookings", "StatusDate");
            DropColumn("dbo.Bookings", "AssignedDate");
            DropColumn("dbo.Bookings", "PickUpDate");
        }
    }
}
