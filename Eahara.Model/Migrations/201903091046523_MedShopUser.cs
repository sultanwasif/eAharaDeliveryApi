namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MedShopUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MEDOffers", "MEDShopId", c => c.Long());
            AddColumn("dbo.Users", "MEDShopId", c => c.Long());
            CreateIndex("dbo.MEDOffers", "MEDShopId");
            CreateIndex("dbo.Users", "MEDShopId");
            AddForeignKey("dbo.MEDOffers", "MEDShopId", "dbo.MEDShops", "Id");
            AddForeignKey("dbo.Users", "MEDShopId", "dbo.MEDShops", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Users", "MEDShopId", "dbo.MEDShops");
            DropForeignKey("dbo.MEDOffers", "MEDShopId", "dbo.MEDShops");
            DropIndex("dbo.Users", new[] { "MEDShopId" });
            DropIndex("dbo.MEDOffers", new[] { "MEDShopId" });
            DropColumn("dbo.Users", "MEDShopId");
            DropColumn("dbo.MEDOffers", "MEDShopId");
        }
    }
}
