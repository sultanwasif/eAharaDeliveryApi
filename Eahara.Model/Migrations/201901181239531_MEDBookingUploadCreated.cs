namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MEDBookingUploadCreated : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MEDBookingDetails",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Quantity = c.Int(nullable: false),
                        Price = c.Single(nullable: false),
                        TotalPrice = c.Single(nullable: false),
                        DiscountPrice = c.Single(nullable: false),
                        DelCharge = c.Single(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        Remarks = c.String(),
                        MEDBookingId = c.Long(nullable: false),
                        MEDItemId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MEDItems", t => t.MEDItemId, cascadeDelete: true)
                .ForeignKey("dbo.MEDBookings", t => t.MEDBookingId, cascadeDelete: true)
                .Index(t => t.MEDBookingId)
                .Index(t => t.MEDItemId);
            
            CreateTable(
                "dbo.MEDBookings",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        IsActive = c.Boolean(nullable: false),
                        IsPaid = c.Boolean(nullable: false),
                        Description = c.String(maxLength: 400),
                        Remarks = c.String(maxLength: 400),
                        Time = c.String(maxLength: 50),
                        Date = c.DateTime(nullable: false),
                        OrderDate = c.DateTime(nullable: false),
                        Count = c.Int(nullable: false),
                        Total = c.Single(nullable: false),
                        PromoOfferPrice = c.Single(nullable: false),
                        TotalDeliveryCharge = c.Single(nullable: false),
                        SubTotal = c.Single(nullable: false),
                        WalletCash = c.Single(nullable: false),
                        Name = c.String(maxLength: 150),
                        MobileNo = c.String(maxLength: 150),
                        EmailId = c.String(maxLength: 150),
                        Address = c.String(maxLength: 250),
                        CancelRemarks = c.String(maxLength: 250),
                        RefNo = c.String(maxLength: 100),
                        Lat = c.String(maxLength: 200),
                        Lng = c.String(maxLength: 200),
                        CustomerId = c.Long(),
                        MEDStatusId = c.Long(),
                        PromoOfferId = c.Long(),
                        LocationId = c.Long(),
                        IsOrderLater = c.Boolean(nullable: false),
                        EmployeeId = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Customers", t => t.CustomerId)
                .ForeignKey("dbo.Employees", t => t.EmployeeId)
                .ForeignKey("dbo.Locations", t => t.LocationId)
                .ForeignKey("dbo.MEDStatus", t => t.MEDStatusId)
                .ForeignKey("dbo.PromoOffers", t => t.PromoOfferId)
                .Index(t => t.CustomerId)
                .Index(t => t.MEDStatusId)
                .Index(t => t.PromoOfferId)
                .Index(t => t.LocationId)
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "dbo.MEDUploads",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Path = c.String(maxLength: 250),
                        CustomerId = c.Long(),
                        Date = c.DateTime(nullable: false),
                        Remarks = c.String(maxLength: 500),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Customers", t => t.CustomerId)
                .Index(t => t.CustomerId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MEDUploads", "CustomerId", "dbo.Customers");
            DropForeignKey("dbo.MEDBookings", "PromoOfferId", "dbo.PromoOffers");
            DropForeignKey("dbo.MEDBookings", "MEDStatusId", "dbo.MEDStatus");
            DropForeignKey("dbo.MEDBookingDetails", "MEDBookingId", "dbo.MEDBookings");
            DropForeignKey("dbo.MEDBookings", "LocationId", "dbo.Locations");
            DropForeignKey("dbo.MEDBookings", "EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.MEDBookings", "CustomerId", "dbo.Customers");
            DropForeignKey("dbo.MEDBookingDetails", "MEDItemId", "dbo.MEDItems");
            DropIndex("dbo.MEDUploads", new[] { "CustomerId" });
            DropIndex("dbo.MEDBookings", new[] { "EmployeeId" });
            DropIndex("dbo.MEDBookings", new[] { "LocationId" });
            DropIndex("dbo.MEDBookings", new[] { "PromoOfferId" });
            DropIndex("dbo.MEDBookings", new[] { "MEDStatusId" });
            DropIndex("dbo.MEDBookings", new[] { "CustomerId" });
            DropIndex("dbo.MEDBookingDetails", new[] { "MEDItemId" });
            DropIndex("dbo.MEDBookingDetails", new[] { "MEDBookingId" });
            DropTable("dbo.MEDUploads");
            DropTable("dbo.MEDBookings");
            DropTable("dbo.MEDBookingDetails");
        }
    }
}
