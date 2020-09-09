namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TraceNoCreated : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TraceNoes",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Type = c.String(maxLength: 60),
                        Prefix = c.String(maxLength: 100),
                        Number = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TraceNoes");
        }
    }
}
