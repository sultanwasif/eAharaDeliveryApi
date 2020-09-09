namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewDatesInMEDBooking : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MEDBookings", "PickUpDate", c => c.DateTime());
            AddColumn("dbo.MEDBookings", "AssignedDate", c => c.DateTime());
            AddColumn("dbo.MEDBookings", "StatusDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.MEDBookings", "StatusDate");
            DropColumn("dbo.MEDBookings", "AssignedDate");
            DropColumn("dbo.MEDBookings", "PickUpDate");
        }
    }
}
