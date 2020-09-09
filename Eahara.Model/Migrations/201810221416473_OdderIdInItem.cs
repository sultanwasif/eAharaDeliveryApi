namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OdderIdInItem : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Items", "OfferId", c => c.Long());
            CreateIndex("dbo.Items", "OfferId");
            AddForeignKey("dbo.Items", "OfferId", "dbo.Offers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Items", "OfferId", "dbo.Offers");
            DropIndex("dbo.Items", new[] { "OfferId" });
            DropColumn("dbo.Items", "OfferId");
        }
    }
}
