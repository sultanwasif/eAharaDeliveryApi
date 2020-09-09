namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BookingChanged2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Bookings", "OrderDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Bookings", "OrderDate");
        }
    }
}
