namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MEDUploadChanged : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MEDUploads", "MEDBookingId", c => c.Long());
            CreateIndex("dbo.MEDUploads", "MEDBookingId");
            AddForeignKey("dbo.MEDUploads", "MEDBookingId", "dbo.MEDBookings", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MEDUploads", "MEDBookingId", "dbo.MEDBookings");
            DropIndex("dbo.MEDUploads", new[] { "MEDBookingId" });
            DropColumn("dbo.MEDUploads", "MEDBookingId");
        }
    }
}
