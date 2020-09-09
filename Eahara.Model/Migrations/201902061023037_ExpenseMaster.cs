namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExpenseMaster : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Expenses",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(maxLength: 120),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.CompanyExpenseDetails", "ExpenseId", c => c.Long(nullable: false));
            CreateIndex("dbo.CompanyExpenseDetails", "ExpenseId");
            AddForeignKey("dbo.CompanyExpenseDetails", "ExpenseId", "dbo.Expenses", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CompanyExpenseDetails", "ExpenseId", "dbo.Expenses");
            DropIndex("dbo.CompanyExpenseDetails", new[] { "ExpenseId" });
            DropColumn("dbo.CompanyExpenseDetails", "ExpenseId");
            DropTable("dbo.Expenses");
        }
    }
}
