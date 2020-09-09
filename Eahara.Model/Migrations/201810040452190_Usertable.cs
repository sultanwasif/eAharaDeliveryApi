namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Usertable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserName = c.String(maxLength: 100),
                        PasswordSalt = c.String(maxLength: 256),
                        Password = c.String(maxLength: 256),
                        IsActive = c.Boolean(nullable: false),
                        Role = c.String(maxLength: 50),
                        IsBlocked = c.Boolean(nullable: false),
                        Type = c.String(maxLength: 60),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Users");
        }
    }
}
