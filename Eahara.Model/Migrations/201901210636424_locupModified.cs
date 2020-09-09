namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class locupModified : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Locations", "Lat", c => c.String(maxLength: 150));
            AddColumn("dbo.Locations", "Lng", c => c.String(maxLength: 150));
            AddColumn("dbo.Locations", "DeliveryRange", c => c.Single(nullable: false));
            AddColumn("dbo.Locations", "DeliveryCharge", c => c.Single(nullable: false));
            AddColumn("dbo.MEDUploads", "Name", c => c.String(maxLength: 150));
            AddColumn("dbo.MEDUploads", "MobileNo", c => c.String(maxLength: 150));
            AddColumn("dbo.MEDUploads", "EmailId", c => c.String(maxLength: 150));
            AddColumn("dbo.MEDUploads", "OrderDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.MEDUploads", "OrderDate");
            DropColumn("dbo.MEDUploads", "EmailId");
            DropColumn("dbo.MEDUploads", "MobileNo");
            DropColumn("dbo.MEDUploads", "Name");
            DropColumn("dbo.Locations", "DeliveryCharge");
            DropColumn("dbo.Locations", "DeliveryRange");
            DropColumn("dbo.Locations", "Lng");
            DropColumn("dbo.Locations", "Lat");
        }
    }
}
