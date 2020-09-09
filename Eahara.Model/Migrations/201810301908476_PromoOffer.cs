namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PromoOffer : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PromoOffers",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Value = c.Single(nullable: false),
                        IsPercentage = c.Boolean(nullable: false),
                        Code = c.String(maxLength: 150),
                        Image = c.String(maxLength: 250),
                        Tittle = c.String(maxLength: 200),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Offers", "ShopId", c => c.Long());
            AddColumn("dbo.Shops", "DeliveryCharge", c => c.Single(nullable: false));
            CreateIndex("dbo.Offers", "ShopId");
            AddForeignKey("dbo.Offers", "ShopId", "dbo.Shops", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Offers", "ShopId", "dbo.Shops");
            DropIndex("dbo.Offers", new[] { "ShopId" });
            DropColumn("dbo.Shops", "DeliveryCharge");
            DropColumn("dbo.Offers", "ShopId");
            DropTable("dbo.PromoOffers");
        }
    }
}
