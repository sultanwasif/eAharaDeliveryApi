namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class maxvaluepromo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PromoOffers", "MaxValue", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PromoOffers", "MaxValue");
        }
    }
}
