namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class shopno : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Shops", "MobileNo", c => c.String(maxLength: 150));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Shops", "MobileNo");
        }
    }
}
