namespace MyApp.data
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialMigrations : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "LastName", c => c.String());
            AddColumn("dbo.AspNetUsers", "AvatarId", c => c.String());
            DropColumn("dbo.AspNetUsers", "SecondName");
            DropColumn("dbo.AspNetUsers", "Patronymic");
            DropColumn("dbo.AspNetUsers", "Tin");
            DropColumn("dbo.AspNetUsers", "Address");
            DropColumn("dbo.AspNetUsers", "ExternalId");
            DropColumn("dbo.AspNetUsers", "ImageId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "ImageId", c => c.String());
            AddColumn("dbo.AspNetUsers", "ExternalId", c => c.String());
            AddColumn("dbo.AspNetUsers", "Address", c => c.String());
            AddColumn("dbo.AspNetUsers", "Tin", c => c.String());
            AddColumn("dbo.AspNetUsers", "Patronymic", c => c.String());
            AddColumn("dbo.AspNetUsers", "SecondName", c => c.String());
            DropColumn("dbo.AspNetUsers", "AvatarId");
            DropColumn("dbo.AspNetUsers", "LastName");
        }
    }
}
