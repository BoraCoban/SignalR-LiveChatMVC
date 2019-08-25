using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LiveChatMVC.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string Name{ get; set; }

        //Relations
        [JsonIgnore]
        public List<Message> Messages { get; set; }
        [JsonIgnore]
        public List<UserChat> UserChats { get; set; }
    }
}