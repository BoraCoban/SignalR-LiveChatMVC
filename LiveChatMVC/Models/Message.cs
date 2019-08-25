using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LiveChatMVC.Models
{
    public class Message
    {
        [Key]
        public long Id { get; set; }
        public string MessageBody { get; set; }
        public DateTime? CreatedAt { get; set; }

        //Relations
        public int UserId { get; set; }
        public string ChatId { get; set; }
        [JsonIgnore]
        public User User { get; set; }
        [JsonIgnore]
        public Chat Chat { get; set; }
        [JsonIgnore]
        public List<UserChat> UserChats { get; set; }
    }
}