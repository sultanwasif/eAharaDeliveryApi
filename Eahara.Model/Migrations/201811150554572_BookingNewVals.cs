namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BookingNewVals : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Bookings", "TotalDeliveryCharge", c => c.Single(nullable: false));
            AddColumn("dbo.Bookings", "SubTotal", c => c.Single(nullable: false));
            AddColumn("dbo.Bookings", "ShopId", c => c.Long());
            CreateIndex("dbo.Bookings", "ShopId");
            AddForeignKey("dbo.Bookings", "ShopId", "dbo.Shops", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Bookings", "ShopId", "dbo.Shops");
            DropIndex("dbo.Bookings", new[] { "ShopId" });
            DropColumn("dbo.Bookings", "ShopId");
            DropColumn("dbo.Bookings", "SubTotal");
            DropColumn("dbo.Bookings", "TotalDeliveryCharge");
        }
    }
}
