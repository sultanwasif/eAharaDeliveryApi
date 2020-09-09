namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CommissionPercentage : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Shops", "CommissionPercentage", c => c.Single(nullable: false));
            AddColumn("dbo.Items", "CommissionPercentage", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Items", "CommissionPercentage");
            DropColumn("dbo.Shops", "CommissionPercentage");
        }
    }
}
