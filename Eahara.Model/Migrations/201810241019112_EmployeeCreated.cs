namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EmployeeCreated : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Employees",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(maxLength: 60),
                        Designation = c.String(maxLength: 60),
                        Email = c.String(maxLength: 60),
                        MobileNo = c.String(maxLength: 50),
                        TelephoneNo = c.String(maxLength: 50),
                        Address = c.String(maxLength: 200),
                        Location = c.String(maxLength: 100),
                        BankName = c.String(maxLength: 100),
                        BankAccount = c.String(maxLength: 100),
                        IsActive = c.Boolean(nullable: false),
                        JoiningDate = c.DateTime(nullable: false),
                        IsTemp = c.Boolean(nullable: false),
                        IsInActive = c.Boolean(nullable: false),
                        IsOwnEmployee = c.Boolean(nullable: false),
                        NormalWorkingHours = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Employees");
        }
    }
}
