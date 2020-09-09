namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CustomerCreated : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Customers",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(maxLength: 60),
                        Email = c.String(maxLength: 60),
                        MobileNo = c.String(maxLength: 50),
                        TelephoneNo = c.String(maxLength: 50),
                        IsActive = c.Boolean(nullable: false),
                        Location = c.String(maxLength: 60),
                        Address = c.String(maxLength: 250),
                        Photo = c.String(maxLength: 250),
                        RefNo = c.String(maxLength: 100),
                        InstRefNo = c.String(maxLength: 100),
                        Points = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserOTPs",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        OTP = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        MobileNo = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Users", "CustomeId", c => c.Long());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "CustomeId");
            DropTable("dbo.UserOTPs");
            DropTable("dbo.Customers");
        }
    }
}
