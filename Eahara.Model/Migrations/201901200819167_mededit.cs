namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mededit : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.MEDItems", "MEDOfferId", "dbo.MEDOffers");
            DropIndex("dbo.MEDItems", new[] { "MEDOfferId" });
            AddColumn("dbo.MEDItems", "Bookings", c => c.Single(nullable: false));
            AlterColumn("dbo.MEDItems", "MEDOfferId", c => c.Long());
            CreateIndex("dbo.MEDItems", "MEDOfferId");
            AddForeignKey("dbo.MEDItems", "MEDOfferId", "dbo.MEDOffers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MEDItems", "MEDOfferId", "dbo.MEDOffers");
            DropIndex("dbo.MEDItems", new[] { "MEDOfferId" });
            AlterColumn("dbo.MEDItems", "MEDOfferId", c => c.Long(nullable: false));
            DropColumn("dbo.MEDItems", "Bookings");
            CreateIndex("dbo.MEDItems", "MEDOfferId");
            AddForeignKey("dbo.MEDItems", "MEDOfferId", "dbo.MEDOffers", "Id", cascadeDelete: true);
        }
    }
}
