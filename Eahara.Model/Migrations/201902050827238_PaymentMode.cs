namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PaymentMode : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PaymentModes",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(maxLength: 120),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.CompanyExpenses", "PaymentModeId", c => c.Long(nullable: false));
            CreateIndex("dbo.CompanyExpenses", "PaymentModeId");
            AddForeignKey("dbo.CompanyExpenses", "PaymentModeId", "dbo.PaymentModes", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CompanyExpenses", "PaymentModeId", "dbo.PaymentModes");
            DropIndex("dbo.CompanyExpenses", new[] { "PaymentModeId" });
            DropColumn("dbo.CompanyExpenses", "PaymentModeId");
            DropTable("dbo.PaymentModes");
        }
    }
}
