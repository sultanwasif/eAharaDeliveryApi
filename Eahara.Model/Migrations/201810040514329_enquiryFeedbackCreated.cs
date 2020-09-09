namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class enquiryFeedbackCreated : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Enquiries",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(maxLength: 60),
                        MobileNo = c.String(maxLength: 60),
                        Email = c.String(maxLength: 60),
                        Subject = c.String(maxLength: 100),
                        Remarks = c.String(maxLength: 500),
                        IsActive = c.Boolean(nullable: false),
                        IsClosed = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Feedbacks",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(maxLength: 100),
                        Designation = c.String(maxLength: 100),
                        PhoneNo = c.String(maxLength: 100),
                        Description = c.String(maxLength: 500),
                        Satisfaction = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsAccepted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Feedbacks");
            DropTable("dbo.Enquiries");
        }
    }
}
