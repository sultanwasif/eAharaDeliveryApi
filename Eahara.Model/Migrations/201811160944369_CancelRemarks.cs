namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CancelRemarks : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BookingDetails", "Remarks", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.BookingDetails", "Remarks");
        }
    }
}
