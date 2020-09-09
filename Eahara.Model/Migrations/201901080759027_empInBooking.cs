namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class empInBooking : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Bookings", "EmployeeId", c => c.Long());
            CreateIndex("dbo.Bookings", "EmployeeId");
            AddForeignKey("dbo.Bookings", "EmployeeId", "dbo.Employees", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Bookings", "EmployeeId", "dbo.Employees");
            DropIndex("dbo.Bookings", new[] { "EmployeeId" });
            DropColumn("dbo.Bookings", "EmployeeId");
        }
    }
}
