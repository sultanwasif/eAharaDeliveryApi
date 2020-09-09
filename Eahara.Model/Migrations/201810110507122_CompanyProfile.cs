namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CompanyProfile : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CompanyProfiles",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(maxLength: 60),
                        Address = c.String(maxLength: 500),
                        Email = c.String(maxLength: 60),
                        MobileNo = c.String(maxLength: 60),
                        TeleNo = c.String(maxLength: 100),
                        MobNo = c.String(maxLength: 100),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.CompanyProfiles");
        }
    }
}
