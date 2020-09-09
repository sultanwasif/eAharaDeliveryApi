namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OfferIsPer : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Items", "OfferPrice", c => c.Single(nullable: false));
            AddColumn("dbo.Offers", "IsPercentage", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Offers", "IsPercentage");
            DropColumn("dbo.Items", "OfferPrice");
        }
    }
}
