namespace LiveChatMVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Chats",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        ChatName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Messages",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        MessageBody = c.String(),
                        CreatedAt = c.DateTime(),
                        UserId = c.Int(nullable: false),
                        ChatId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Chats", t => t.ChatId)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.ChatId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserChats",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        ChatId = c.String(maxLength: 128),
                        Message_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Chats", t => t.ChatId)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.Messages", t => t.Message_Id)
                .Index(t => t.UserId)
                .Index(t => t.ChatId)
                .Index(t => t.Message_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserChats", "Message_Id", "dbo.Messages");
            DropForeignKey("dbo.UserChats", "UserId", "dbo.Users");
            DropForeignKey("dbo.UserChats", "ChatId", "dbo.Chats");
            DropForeignKey("dbo.Messages", "UserId", "dbo.Users");
            DropForeignKey("dbo.Messages", "ChatId", "dbo.Chats");
            DropIndex("dbo.UserChats", new[] { "Message_Id" });
            DropIndex("dbo.UserChats", new[] { "ChatId" });
            DropIndex("dbo.UserChats", new[] { "UserId" });
            DropIndex("dbo.Messages", new[] { "ChatId" });
            DropIndex("dbo.Messages", new[] { "UserId" });
            DropTable("dbo.UserChats");
            DropTable("dbo.Users");
            DropTable("dbo.Messages");
            DropTable("dbo.Chats");
        }
    }
}
