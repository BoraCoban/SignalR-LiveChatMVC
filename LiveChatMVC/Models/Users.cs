using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LiveChatMVC.Models
{
    public class Users
    {
        public string ConnectionId { get; set; }

        public string UserName { get; set; }
        private List<string> groupNames;

        public List<string> GroupNames
        {
            set { groupNames = value; }
            get { return groupNames; }
        }

        public void addGroupName(string groupName)
        {
            groupNames.Add(groupName);
        }
    }
}