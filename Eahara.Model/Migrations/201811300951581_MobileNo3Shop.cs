namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MobileNo3Shop : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Shops", "MobileNo2", c => c.String(maxLength: 150));
            AddColumn("dbo.Shops", "MobileNo3", c => c.String(maxLength: 150));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Shops", "MobileNo3");
            DropColumn("dbo.Shops", "MobileNo2");
        }
    }
}
