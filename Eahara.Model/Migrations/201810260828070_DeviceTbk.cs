namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeviceTbk : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Devices",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Platform = c.String(maxLength: 250),
                        UUId = c.String(maxLength: 250),
                        version = c.String(maxLength: 150),
                        Language = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Devices");
        }
    }
}
