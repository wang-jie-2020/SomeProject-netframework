namespace SomeProject.Core.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class Create : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LoginLog",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    IpAddress = c.String(nullable: false, maxLength: 15),
                    IsDeleted = c.Boolean(nullable: false),
                    AddDate = c.DateTime(nullable: false),
                    Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    User_Id = c.Guid(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SysUser", t => t.User_Id)
                .Index(t => t.User_Id);

            CreateTable(
                "dbo.SysUser",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    UserName = c.String(nullable: false, maxLength: 20),
                    Password = c.String(nullable: false, maxLength: 32),
                    NickName = c.String(nullable: false, maxLength: 20),
                    Email = c.String(nullable: false, maxLength: 50),
                    IsDeleted = c.Boolean(nullable: false),
                    AddDate = c.DateTime(nullable: false),
                    Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.SysRole",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    Name = c.String(nullable: false, maxLength: 20),
                    Description = c.String(maxLength: 100),
                    RoleType = c.Int(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    AddDate = c.DateTime(nullable: false),
                    Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.SysRoleSysUser",
                c => new
                {
                    SysRole_Id = c.Guid(nullable: false),
                    SysUser_Id = c.Guid(nullable: false),
                })
                .PrimaryKey(t => new { t.SysRole_Id, t.SysUser_Id })
                .ForeignKey("dbo.SysRole", t => t.SysRole_Id)
                .ForeignKey("dbo.SysUser", t => t.SysUser_Id)
                .Index(t => t.SysRole_Id)
                .Index(t => t.SysUser_Id);

        }

        public override void Down()
        {
            DropForeignKey("dbo.SysRoleSysUser", "SysUser_Id", "dbo.SysUser");
            DropForeignKey("dbo.SysRoleSysUser", "SysRole_Id", "dbo.SysRole");
            DropForeignKey("dbo.LoginLog", "User_Id", "dbo.SysUser");
            DropIndex("dbo.SysRoleSysUser", new[] { "SysUser_Id" });
            DropIndex("dbo.SysRoleSysUser", new[] { "SysRole_Id" });
            DropIndex("dbo.LoginLog", new[] { "User_Id" });
            DropTable("dbo.SysRoleSysUser");
            DropTable("dbo.SysRole");
            DropTable("dbo.SysUser");
            DropTable("dbo.LoginLog");
        }
    }
}
