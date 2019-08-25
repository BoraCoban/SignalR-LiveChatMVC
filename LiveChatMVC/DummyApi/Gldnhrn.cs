using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LiveChatMVC.DummyApi
{
    public class Gldnhrn
    {
        private List<User> userList = new List<User>();
        public Gldnhrn()
        {
            for (int i = 0; i < 50; i++)
            {
                userList.Add(new User(i, "User" + i, i % 4));
            }
        }

        public string requestUser(string validationToken)
        {
            foreach (User user in userList)
            {
                if (user.token == validationToken)
                    return JsonConvert.SerializeObject(user);
            }
            return null;
        }

        public string ticketId()
        {
            return "123a";
        }

        public string ticketId2()
        {
            return "1234a";
        }

    }
}
