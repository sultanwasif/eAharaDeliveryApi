namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ShopItemCreated : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Items",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(maxLength: 100),
                        TagLine = c.String(maxLength: 150),
                        Description = c.String(maxLength: 400),
                        Price = c.String(maxLength: 200),
                        Image = c.String(maxLength: 200),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Shops",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(maxLength: 100),
                        TagLine = c.String(maxLength: 150),
                        Address = c.String(maxLength: 250),
                        Description = c.String(maxLength: 400),
                        AverageCost = c.String(maxLength: 100),
                        OpeningHours = c.String(maxLength: 100),
                        Cuisines = c.String(maxLength: 200),
                        Image = c.String(maxLength: 200),
                        AverageRating = c.String(maxLength: 150),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Shops");
            DropTable("dbo.Items");
        }
    }
}
