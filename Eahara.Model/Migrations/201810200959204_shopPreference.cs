namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class shopPreference : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Shops", "Preference", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Shops", "Preference");
        }
    }
}
