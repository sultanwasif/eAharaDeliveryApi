namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Offer : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Offers", "Percentage", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Offers", "Percentage");
        }
    }
}
