namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewBookFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BookingDetails", "DiscountPrice", c => c.Single(nullable: false));
            AddColumn("dbo.BookingDetails", "DelCharge", c => c.Single(nullable: false));
            AddColumn("dbo.Bookings", "PromoOfferPrice", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Bookings", "PromoOfferPrice");
            DropColumn("dbo.BookingDetails", "DelCharge");
            DropColumn("dbo.BookingDetails", "DiscountPrice");
        }
    }
}
