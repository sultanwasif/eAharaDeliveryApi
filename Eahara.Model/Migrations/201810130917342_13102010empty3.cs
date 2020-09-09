namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _13102010empty3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Bookings", "MobileNo", c => c.String(maxLength: 150));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Bookings", "MobileNo");
        }
    }
}
