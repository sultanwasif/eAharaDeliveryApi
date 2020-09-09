namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CustomerInBookind : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Bookings", "CustomerId", c => c.Long());
            CreateIndex("dbo.Bookings", "CustomerId");
            AddForeignKey("dbo.Bookings", "CustomerId", "dbo.Customers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Bookings", "CustomerId", "dbo.Customers");
            DropIndex("dbo.Bookings", new[] { "CustomerId" });
            DropColumn("dbo.Bookings", "CustomerId");
        }
    }
}
