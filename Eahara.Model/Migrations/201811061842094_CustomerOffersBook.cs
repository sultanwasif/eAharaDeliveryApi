namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CustomerOffersBook : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Bookings", "PromoOfferId", c => c.Long());
            CreateIndex("dbo.Bookings", "PromoOfferId");
            AddForeignKey("dbo.Bookings", "PromoOfferId", "dbo.PromoOffers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Bookings", "PromoOfferId", "dbo.PromoOffers");
            DropIndex("dbo.Bookings", new[] { "PromoOfferId" });
            DropColumn("dbo.Bookings", "PromoOfferId");
        }
    }
}
