namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemPreference : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Items", "Preference", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Items", "Preference");
        }
    }
}
