namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _13102010empty2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Bookings", "LocationId", c => c.Long(nullable: false));
            CreateIndex("dbo.Bookings", "LocationId");
            AddForeignKey("dbo.Bookings", "LocationId", "dbo.Locations", "Id", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Bookings", "LocationId", "dbo.Locations");
            DropIndex("dbo.Bookings", new[] { "LocationId" });
            DropColumn("dbo.Bookings", "LocationId");
        }
    }
}
