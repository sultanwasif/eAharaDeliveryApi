namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BookindDetailsCreated : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BokkingDetails",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Quentity = c.Int(nullable: false),
                        Price = c.Single(nullable: false),
                        TotalPrice = c.Single(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        BookingId = c.Long(nullable: false),
                        ItemId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Bookings", t => t.BookingId, cascadeDelete: true)
                .ForeignKey("dbo.Items", t => t.ItemId, cascadeDelete: true)
                .Index(t => t.BookingId)
                .Index(t => t.ItemId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BokkingDetails", "ItemId", "dbo.Items");
            DropForeignKey("dbo.BokkingDetails", "BookingId", "dbo.Bookings");
            DropIndex("dbo.BokkingDetails", new[] { "ItemId" });
            DropIndex("dbo.BokkingDetails", new[] { "BookingId" });
            DropTable("dbo.BokkingDetails");
        }
    }
}
