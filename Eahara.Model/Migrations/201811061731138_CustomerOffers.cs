namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CustomerOffers : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CustomerOffers",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        CustomerId = c.Long(nullable: false),
                        RefNo = c.String(maxLength: 150),
                        PromoOfferId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Customers", t => t.CustomerId, cascadeDelete: true)
                .ForeignKey("dbo.PromoOffers", t => t.PromoOfferId, cascadeDelete: true)
                .Index(t => t.CustomerId)
                .Index(t => t.PromoOfferId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CustomerOffers", "PromoOfferId", "dbo.PromoOffers");
            DropForeignKey("dbo.CustomerOffers", "CustomerId", "dbo.Customers");
            DropIndex("dbo.CustomerOffers", new[] { "PromoOfferId" });
            DropIndex("dbo.CustomerOffers", new[] { "CustomerId" });
            DropTable("dbo.CustomerOffers");
        }
    }
}
