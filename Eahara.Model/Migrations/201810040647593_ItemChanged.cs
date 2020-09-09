namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemChanged : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Items", "Price", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Items", "Price", c => c.String(maxLength: 200));
        }
    }
}
