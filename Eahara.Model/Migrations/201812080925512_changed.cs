namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changed : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Customers", "CreatedDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Customers", "CreatedDate");
        }
    }
}
