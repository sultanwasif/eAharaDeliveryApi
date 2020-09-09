namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BookingChanged : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Bookings", "ShopId", c => c.Long(nullable: false));
            AddColumn("dbo.Bookings", "ItemId", c => c.Long());
            CreateIndex("dbo.Bookings", "ShopId");
            CreateIndex("dbo.Bookings", "ItemId");
            AddForeignKey("dbo.Bookings", "ItemId", "dbo.Items", "Id");
            AddForeignKey("dbo.Bookings", "ShopId", "dbo.Shops", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Bookings", "ShopId", "dbo.Shops");
            DropForeignKey("dbo.Bookings", "ItemId", "dbo.Items");
            DropIndex("dbo.Bookings", new[] { "ItemId" });
            DropIndex("dbo.Bookings", new[] { "ShopId" });
            DropColumn("dbo.Bookings", "ItemId");
            DropColumn("dbo.Bookings", "ShopId");
        }
    }
}
