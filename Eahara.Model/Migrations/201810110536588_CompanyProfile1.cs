namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CompanyProfile1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CompanyProfiles", "WhatsappNo", c => c.String(maxLength: 100));
            DropColumn("dbo.CompanyProfiles", "MobNo");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CompanyProfiles", "MobNo", c => c.String(maxLength: 100));
            DropColumn("dbo.CompanyProfiles", "WhatsappNo");
        }
    }
}
