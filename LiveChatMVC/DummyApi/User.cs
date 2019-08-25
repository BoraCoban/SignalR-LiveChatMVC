
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LiveChatMVC.DummyApi
{
    
    public class User
    {
        public string token { get; }
        public int userId { get; set; }
        public string userName { get; set; }
        public string role;

        public User(int _userId, string _userName, int _role)
        {
            token = "user" + _userId;
            userId = _userId;
            userName = _userName;
            switch (_role)
            {
                case 0:
                    role = "Admin";
                    break;
                case 1:
                    role = "Support";
                    break;
                case 2:
                    role = "Manager";
                    break;
                case 3:
                    role = "Customer";
                    break;
            }
        }
    }
}