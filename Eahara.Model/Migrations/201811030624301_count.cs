namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class count : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PromoOffers", "Count", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PromoOffers", "Count");
        }
    }
}
