using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace LiveChatMVC.Models
{
    public class ChatContext:DbContext
    {
        public ChatContext() : base("LiveChatDatabase")
        {
            Database.SetInitializer(new ChatInitializer());
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Chat> Chats{ get; set; }
        public DbSet<UserChat> UserChats { get; set; }

    }
}