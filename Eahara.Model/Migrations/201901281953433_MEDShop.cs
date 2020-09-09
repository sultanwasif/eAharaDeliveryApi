namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MEDShop : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MEDShops",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(maxLength: 100),
                        TagLine = c.String(maxLength: 150),
                        MobileNo = c.String(maxLength: 150),
                        MobileNo2 = c.String(maxLength: 150),
                        MobileNo3 = c.String(maxLength: 150),
                        Address = c.String(maxLength: 250),
                        Description = c.String(maxLength: 400),
                        Image = c.String(maxLength: 200),
                        CommissionPercentage = c.Single(nullable: false),
                        Lat = c.String(maxLength: 150),
                        Lng = c.String(maxLength: 150),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.MEDItems", "MEDShopId", c => c.Long());
            CreateIndex("dbo.MEDItems", "MEDShopId");
            AddForeignKey("dbo.MEDItems", "MEDShopId", "dbo.MEDShops", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MEDItems", "MEDShopId", "dbo.MEDShops");
            DropIndex("dbo.MEDItems", new[] { "MEDShopId" });
            DropColumn("dbo.MEDItems", "MEDShopId");
            DropTable("dbo.MEDShops");
        }
    }
}
