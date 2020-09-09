namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserSessionCreated : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserSessions",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Token = c.String(maxLength: 256),
                        SessionTimeStamp = c.DateTime(nullable: false),
                        ExpiresInMinutes = c.Long(nullable: false),
                        UserId = c.Long(nullable: false),
                        UserSessionStatus = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserSessions", "UserId", "dbo.Users");
            DropIndex("dbo.UserSessions", new[] { "UserId" });
            DropTable("dbo.UserSessions");
        }
    }
}
