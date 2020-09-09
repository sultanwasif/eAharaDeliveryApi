namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changescreated : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CompanyProfiles", "SMSID", c => c.String(maxLength: 50));
            AddColumn("dbo.CompanyProfiles", "SMSpassword", c => c.String(maxLength: 50));
            AddColumn("dbo.CompanyProfiles", "SMSusername", c => c.String(maxLength: 50));
            AddColumn("dbo.Users", "CustomerId", c => c.Long());
            DropColumn("dbo.Users", "CustomeId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "CustomeId", c => c.Long());
            DropColumn("dbo.Users", "CustomerId");
            DropColumn("dbo.CompanyProfiles", "SMSusername");
            DropColumn("dbo.CompanyProfiles", "SMSpassword");
            DropColumn("dbo.CompanyProfiles", "SMSID");
        }
    }
}
