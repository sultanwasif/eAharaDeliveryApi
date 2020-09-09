namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class statusinBooking2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Bookings", "StatusId", c => c.Long());
            CreateIndex("dbo.Bookings", "StatusId");
            AddForeignKey("dbo.Bookings", "StatusId", "dbo.Status", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Bookings", "StatusId", "dbo.Status");
            DropIndex("dbo.Bookings", new[] { "StatusId" });
            DropColumn("dbo.Bookings", "StatusId");
        }
    }
}
