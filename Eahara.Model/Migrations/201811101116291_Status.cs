namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Status : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Status",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(maxLength: 100),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.BookingDetails", "StatusId", c => c.Long());
            CreateIndex("dbo.BookingDetails", "StatusId");
            AddForeignKey("dbo.BookingDetails", "StatusId", "dbo.Status", "Id");
            DropColumn("dbo.BookingDetails", "Status");
        }
        
        public override void Down()
        {
            AddColumn("dbo.BookingDetails", "Status", c => c.String(maxLength: 60));
            DropForeignKey("dbo.BookingDetails", "StatusId", "dbo.Status");
            DropIndex("dbo.BookingDetails", new[] { "StatusId" });
            DropColumn("dbo.BookingDetails", "StatusId");
            DropTable("dbo.Status");
        }
    }
}
