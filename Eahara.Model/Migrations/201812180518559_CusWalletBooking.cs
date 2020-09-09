namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CusWalletBooking : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Bookings", "WalletCash", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Bookings", "WalletCash");
        }
    }
}
