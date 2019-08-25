using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LiveChatMVC.Models
{
    public class UserChat
    {
        public int Id { get; set; }

        //Relations
        public int UserId { get; set; }
        public string ChatId { get; set; }
        public User User { get; set; }
        public Chat Chat { get; set; }
    }
}