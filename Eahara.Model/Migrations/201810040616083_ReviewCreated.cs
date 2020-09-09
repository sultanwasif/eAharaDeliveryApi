namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReviewCreated : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Reviews",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Rating = c.String(maxLength: 200),
                        Description = c.String(maxLength: 400),
                        ShopId = c.Long(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Shops", t => t.ShopId, cascadeDelete: true)
                .Index(t => t.ShopId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Reviews", "ShopId", "dbo.Shops");
            DropIndex("dbo.Reviews", new[] { "ShopId" });
            DropTable("dbo.Reviews");
        }
    }
}
