namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CancelRemarks1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Bookings", "CancelRemarks", c => c.String(maxLength: 250));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Bookings", "CancelRemarks");
        }
    }
}
