namespace Eahara.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MEDBrandCreated : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MEDBrands",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(maxLength: 100),
                        Image = c.String(maxLength: 200),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
           
        }
        
        public override void Down()
        {
            
            DropTable("dbo.MEDBrands");
        }
    }
}
