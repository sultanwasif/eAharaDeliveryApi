namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BookingCreated : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Bookings",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        IsActive = c.Boolean(nullable: false),
                        Description = c.String(maxLength: 400),
                        Time = c.String(maxLength: 50),
                        Date = c.DateTime(nullable: false),
                        Count = c.Int(nullable: false),
                        Place = c.String(maxLength: 100),
                        Address = c.String(maxLength: 150),
                        RefNo = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Bookings");
        }
    }
}
