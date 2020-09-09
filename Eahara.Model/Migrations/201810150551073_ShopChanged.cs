namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ShopChanged : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Shops", "DeliveryTime", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Shops", "DeliveryTime");
        }
    }
}
