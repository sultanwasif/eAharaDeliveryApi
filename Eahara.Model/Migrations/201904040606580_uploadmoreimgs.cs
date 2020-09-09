namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class uploadmoreimgs : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MEDUploads", "Path2", c => c.String());
            AddColumn("dbo.MEDUploads", "Path3", c => c.String());
            AddColumn("dbo.MEDUploads", "Path4", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.MEDUploads", "Path4");
            DropColumn("dbo.MEDUploads", "Path3");
            DropColumn("dbo.MEDUploads", "Path2");
        }
    }
}
