namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MEDCreated : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MEDCategories",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(maxLength: 100),
                        Image = c.String(maxLength: 200),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MEDItems",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(maxLength: 100),
                        Tagline = c.String(maxLength: 200),
                        Price = c.Single(nullable: false),
                        MEDOfferId = c.Long(nullable: false),
                        MEDSubCategoryId = c.Long(nullable: false),
                        IsAvailable = c.Boolean(nullable: false),
                        OfferPrice = c.Single(nullable: false),
                        Image1 = c.String(maxLength: 250),
                        Image2 = c.String(maxLength: 250),
                        Image3 = c.String(maxLength: 250),
                        Image4 = c.String(maxLength: 250),
                        Description = c.String(maxLength: 1000),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MEDOffers", t => t.MEDOfferId, cascadeDelete: true)
                .ForeignKey("dbo.MEDSubCategories", t => t.MEDSubCategoryId, cascadeDelete: true)
                .Index(t => t.MEDOfferId)
                .Index(t => t.MEDSubCategoryId);
            
            CreateTable(
                "dbo.MEDOffers",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Percentage = c.Single(nullable: false),
                        IsPercentage = c.Boolean(nullable: false),
                        Image = c.String(maxLength: 250),
                        Title = c.String(maxLength: 200),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MEDSubCategories",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        MEDCategoryId = c.Long(nullable: false),
                        Name = c.String(maxLength: 100),
                        Image = c.String(maxLength: 200),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MEDCategories", t => t.MEDCategoryId, cascadeDelete: true)
                .Index(t => t.MEDCategoryId);
            
            CreateTable(
                "dbo.MEDStatus",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(maxLength: 100),
                        Description = c.String(maxLength: 250),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MEDItems", "MEDSubCategoryId", "dbo.MEDSubCategories");
            DropForeignKey("dbo.MEDSubCategories", "MEDCategoryId", "dbo.MEDCategories");
            DropForeignKey("dbo.MEDItems", "MEDOfferId", "dbo.MEDOffers");
            DropIndex("dbo.MEDSubCategories", new[] { "MEDCategoryId" });
            DropIndex("dbo.MEDItems", new[] { "MEDSubCategoryId" });
            DropIndex("dbo.MEDItems", new[] { "MEDOfferId" });
            DropTable("dbo.MEDStatus");
            DropTable("dbo.MEDSubCategories");
            DropTable("dbo.MEDOffers");
            DropTable("dbo.MEDItems");
            DropTable("dbo.MEDCategories");
        }
    }
}
