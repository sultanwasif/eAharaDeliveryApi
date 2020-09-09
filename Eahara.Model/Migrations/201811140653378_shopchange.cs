namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class shopchange : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Shops", "StartTime", c => c.Int(nullable: false));
            AddColumn("dbo.Shops", "EndTime", c => c.Int(nullable: false));
            AddColumn("dbo.Shops", "Lat", c => c.String(maxLength: 150));
            AddColumn("dbo.Shops", "Lng", c => c.String(maxLength: 150));
            AddColumn("dbo.Shops", "DeliveryRange", c => c.Single(nullable: false));
            AlterColumn("dbo.Shops", "AverageCost", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Shops", "AverageCost", c => c.String(maxLength: 100));
            DropColumn("dbo.Shops", "DeliveryRange");
            DropColumn("dbo.Shops", "Lng");
            DropColumn("dbo.Shops", "Lat");
            DropColumn("dbo.Shops", "EndTime");
            DropColumn("dbo.Shops", "StartTime");
        }
    }
}
