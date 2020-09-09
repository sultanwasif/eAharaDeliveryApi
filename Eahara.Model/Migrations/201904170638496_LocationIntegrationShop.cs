namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LocationIntegrationShop : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Employees", "LocationId", c => c.Long());
            AddColumn("dbo.Shops", "LocationId", c => c.Long());
            CreateIndex("dbo.Employees", "LocationId");
            CreateIndex("dbo.Shops", "LocationId");
            AddForeignKey("dbo.Employees", "LocationId", "dbo.Locations", "Id");
            AddForeignKey("dbo.Shops", "LocationId", "dbo.Locations", "Id");
            DropColumn("dbo.Employees", "Location");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Employees", "Location", c => c.String(maxLength: 100));
            DropForeignKey("dbo.Shops", "LocationId", "dbo.Locations");
            DropForeignKey("dbo.Employees", "LocationId", "dbo.Locations");
            DropIndex("dbo.Shops", new[] { "LocationId" });
            DropIndex("dbo.Employees", new[] { "LocationId" });
            DropColumn("dbo.Shops", "LocationId");
            DropColumn("dbo.Employees", "LocationId");
        }
    }
}
