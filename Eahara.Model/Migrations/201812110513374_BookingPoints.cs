namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BookingPoints : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CompanyProfiles", "BookingPoints", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CompanyProfiles", "BookingPoints");
        }
    }
}
