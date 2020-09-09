namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BookEdit123 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Bookings", "LocationId", "dbo.Locations");
            DropIndex("dbo.Bookings", new[] { "LocationId" });
            AddColumn("dbo.Bookings", "Remarks", c => c.String(maxLength: 400));
            AddColumn("dbo.Bookings", "Name", c => c.String(maxLength: 150));
            AddColumn("dbo.Bookings", "EmailId", c => c.String(maxLength: 150));
            AddColumn("dbo.Bookings", "Lat", c => c.String(maxLength: 200));
            AddColumn("dbo.Bookings", "Lng", c => c.String(maxLength: 200));
            AlterColumn("dbo.Bookings", "Address", c => c.String(maxLength: 250));
            AlterColumn("dbo.Bookings", "LocationId", c => c.Long());
            CreateIndex("dbo.Bookings", "LocationId");
            AddForeignKey("dbo.Bookings", "LocationId", "dbo.Locations", "Id");
            DropColumn("dbo.Bookings", "Place");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Bookings", "Place", c => c.String(maxLength: 100));
            DropForeignKey("dbo.Bookings", "LocationId", "dbo.Locations");
            DropIndex("dbo.Bookings", new[] { "LocationId" });
            AlterColumn("dbo.Bookings", "LocationId", c => c.Long(nullable: false));
            AlterColumn("dbo.Bookings", "Address", c => c.String(maxLength: 150));
            DropColumn("dbo.Bookings", "Lng");
            DropColumn("dbo.Bookings", "Lat");
            DropColumn("dbo.Bookings", "EmailId");
            DropColumn("dbo.Bookings", "Name");
            DropColumn("dbo.Bookings", "Remarks");
            CreateIndex("dbo.Bookings", "LocationId");
            AddForeignKey("dbo.Bookings", "LocationId", "dbo.Locations", "Id", cascadeDelete: true);
        }
    }
}
