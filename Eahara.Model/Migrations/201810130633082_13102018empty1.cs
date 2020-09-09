namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _13102018empty1 : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.BokkingDetails", newName: "BookingDetails");
            AddColumn("dbo.BookingDetails", "Quantity", c => c.Int(nullable: false));
            DropColumn("dbo.BookingDetails", "Quentity");
        }
        
        public override void Down()
        {
            AddColumn("dbo.BookingDetails", "Quentity", c => c.Int(nullable: false));
            DropColumn("dbo.BookingDetails", "Quantity");
            RenameTable(name: "dbo.BookingDetails", newName: "BokkingDetails");
        }
    }
}
