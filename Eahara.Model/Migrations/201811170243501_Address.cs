namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Address : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Addresses",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Title = c.String(maxLength: 100),
                        Location = c.String(maxLength: 150),
                        Description = c.String(maxLength: 400),
                        CustomerId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Customers", t => t.CustomerId, cascadeDelete: true)
                .Index(t => t.CustomerId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Addresses", "CustomerId", "dbo.Customers");
            DropIndex("dbo.Addresses", new[] { "CustomerId" });
            DropTable("dbo.Addresses");
        }
    }
}
