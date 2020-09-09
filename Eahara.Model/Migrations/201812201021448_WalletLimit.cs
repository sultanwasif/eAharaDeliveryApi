namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WalletLimit : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CompanyProfiles", "WalletLimit", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CompanyProfiles", "WalletLimit");
        }
    }
}
