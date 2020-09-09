namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Address2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Addresses", "Lat", c => c.String(maxLength: 150));
            AddColumn("dbo.Addresses", "Lng", c => c.String(maxLength: 150));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Addresses", "Lng");
            DropColumn("dbo.Addresses", "Lat");
        }
    }
}
