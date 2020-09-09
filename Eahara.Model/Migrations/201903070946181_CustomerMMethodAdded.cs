namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CustomerMMethodAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CustomerMMethods",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        CustomerId = c.Long(nullable: false),
                        MMethodId = c.Long(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Customers", t => t.CustomerId, cascadeDelete: false)
                .ForeignKey("dbo.MMethods", t => t.MMethodId, cascadeDelete: false)
                .Index(t => t.CustomerId)
                .Index(t => t.MMethodId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CustomerMMethods", "MMethodId", "dbo.MMethods");
            DropForeignKey("dbo.CustomerMMethods", "CustomerId", "dbo.Customers");
            DropIndex("dbo.CustomerMMethods", new[] { "MMethodId" });
            DropIndex("dbo.CustomerMMethods", new[] { "CustomerId" });
            DropTable("dbo.CustomerMMethods");
        }
    }
}
