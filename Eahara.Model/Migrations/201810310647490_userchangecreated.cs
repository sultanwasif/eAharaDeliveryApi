namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class userchangecreated : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "EmployeeId", c => c.Long());
            AddColumn("dbo.Users", "ShopId", c => c.Long());
            CreateIndex("dbo.Users", "EmployeeId");
            CreateIndex("dbo.Users", "ShopId");
            AddForeignKey("dbo.Users", "EmployeeId", "dbo.Employees", "Id");
            AddForeignKey("dbo.Users", "ShopId", "dbo.Shops", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Users", "ShopId", "dbo.Shops");
            DropForeignKey("dbo.Users", "EmployeeId", "dbo.Employees");
            DropIndex("dbo.Users", new[] { "ShopId" });
            DropIndex("dbo.Users", new[] { "EmployeeId" });
            DropColumn("dbo.Users", "ShopId");
            DropColumn("dbo.Users", "EmployeeId");
        }
    }
}
