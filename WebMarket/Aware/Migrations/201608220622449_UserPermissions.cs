namespace Aware.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserPermissions : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.User", "Permissions", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.User", "Permissions");
        }
    }
}
