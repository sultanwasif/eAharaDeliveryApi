namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changescreated12 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CompanyProfiles", "Points", c => c.Single(nullable: false));
            AddColumn("dbo.CompanyProfiles", "RegPoints", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CompanyProfiles", "RegPoints");
            DropColumn("dbo.CompanyProfiles", "Points");
        }
    }
}
