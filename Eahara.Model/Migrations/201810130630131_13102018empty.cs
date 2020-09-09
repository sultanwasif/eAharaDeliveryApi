namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _13102018empty : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Bookings", "ItemId", "dbo.Items");
            DropIndex("dbo.Bookings", new[] { "ItemId" });
            AddColumn("dbo.Bookings", "Total", c => c.Single(nullable: false));
            DropColumn("dbo.Bookings", "ItemId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Bookings", "ItemId", c => c.Long());
            DropColumn("dbo.Bookings", "Total");
            CreateIndex("dbo.Bookings", "ItemId");
            AddForeignKey("dbo.Bookings", "ItemId", "dbo.Items", "Id");
        }
    }
}
