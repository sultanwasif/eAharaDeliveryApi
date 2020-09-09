namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LocationInSetting : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CompanyProfiles", "Location", c => c.String(maxLength: 500));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CompanyProfiles", "Location");
        }
    }
}
