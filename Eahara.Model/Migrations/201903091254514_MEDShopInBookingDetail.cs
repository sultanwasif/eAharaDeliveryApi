namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MEDShopInBookingDetail : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MEDBookingDetails", "MEDShopId", c => c.Long());
            CreateIndex("dbo.MEDBookingDetails", "MEDShopId");
            AddForeignKey("dbo.MEDBookingDetails", "MEDShopId", "dbo.MEDShops", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MEDBookingDetails", "MEDShopId", "dbo.MEDShops");
            DropIndex("dbo.MEDBookingDetails", new[] { "MEDShopId" });
            DropColumn("dbo.MEDBookingDetails", "MEDShopId");
        }
    }
}
