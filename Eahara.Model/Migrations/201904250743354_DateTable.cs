namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DateTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DateReports",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        From = c.DateTime(nullable: false),
                        To = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.DateReports");
        }
    }
}
