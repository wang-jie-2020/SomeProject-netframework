namespace SomeProject.Core.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeleteIsDeleteColumn : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.LoginLog", "IsDeleted");
            DropColumn("dbo.SysUser", "IsDeleted");
            DropColumn("dbo.SysRole", "IsDeleted");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SysRole", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.SysUser", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.LoginLog", "IsDeleted", c => c.Boolean(nullable: false));
        }
    }
}
