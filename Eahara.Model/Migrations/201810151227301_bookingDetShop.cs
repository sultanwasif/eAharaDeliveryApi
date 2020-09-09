namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class bookingDetShop : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Bookings", "ShopId", "dbo.Shops");
            DropIndex("dbo.Bookings", new[] { "ShopId" });
            AddColumn("dbo.BookingDetails", "ShopId", c => c.Long(nullable: false));
            CreateIndex("dbo.BookingDetails", "ShopId");
            AddForeignKey("dbo.BookingDetails", "ShopId", "dbo.Shops", "Id", cascadeDelete: true);
            DropColumn("dbo.Bookings", "ShopId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Bookings", "ShopId", c => c.Long(nullable: false));
            DropForeignKey("dbo.BookingDetails", "ShopId", "dbo.Shops");
            DropIndex("dbo.BookingDetails", new[] { "ShopId" });
            DropColumn("dbo.BookingDetails", "ShopId");
            CreateIndex("dbo.Bookings", "ShopId");
            AddForeignKey("dbo.Bookings", "ShopId", "dbo.Shops", "Id", cascadeDelete: true);
        }
    }
}
