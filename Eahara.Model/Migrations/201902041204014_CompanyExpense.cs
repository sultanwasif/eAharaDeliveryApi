namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CompanyExpense : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CompanyExpenseDetails",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Description = c.String(maxLength: 600),
                        Quantity = c.Int(nullable: false),
                        Price = c.Single(nullable: false),
                        Total = c.Single(nullable: false),
                        VATPrice = c.Single(nullable: false),
                        SubTotal = c.Single(nullable: false),
                        GrandTotal = c.Single(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        CompanyExpenseId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CompanyExpenses", t => t.CompanyExpenseId, cascadeDelete: true)
                .Index(t => t.CompanyExpenseId);
            
            CreateTable(
                "dbo.CompanyExpenses",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Description = c.String(maxLength: 600),
                        Date = c.DateTime(nullable: false),
                        Total = c.Single(nullable: false),
                        TotalVAT = c.Single(nullable: false),
                        GrandTotal = c.Single(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CompanyExpenseDetails", "CompanyExpenseId", "dbo.CompanyExpenses");
            DropIndex("dbo.CompanyExpenseDetails", new[] { "CompanyExpenseId" });
            DropTable("dbo.CompanyExpenses");
            DropTable("dbo.CompanyExpenseDetails");
        }
    }
}
