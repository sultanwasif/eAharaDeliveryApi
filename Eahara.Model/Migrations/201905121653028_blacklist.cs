namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class blacklist : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Customers", "BLCount", c => c.Int(nullable: false));
            AddColumn("dbo.Bookings", "IsBlackList", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Bookings", "IsBlackList");
            DropColumn("dbo.Customers", "BLCount");
        }
    }
}
