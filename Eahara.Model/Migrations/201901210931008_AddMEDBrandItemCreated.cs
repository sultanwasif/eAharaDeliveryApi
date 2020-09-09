namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMEDBrandItemCreated : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MEDItems", "MEDBrandId", c => c.Long());
            CreateIndex("dbo.MEDItems", "MEDBrandId");
            AddForeignKey("dbo.MEDItems", "MEDBrandId", "dbo.MEDBrands", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MEDItems", "MEDBrandId", "dbo.MEDBrands");
            DropIndex("dbo.MEDItems", new[] { "MEDBrandId" });
            DropColumn("dbo.MEDItems", "MEDBrandId");
        }
    }
}
