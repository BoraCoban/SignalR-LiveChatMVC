using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using LiveChatMVC.Models;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json.Linq;
using LiveChatMVC.DummyApi;
using Newtonsoft.Json;
using User = LiveChatMVC.Models.User;
using System.Diagnostics;
using System.Collections.Concurrent;
using System.Collections;
using System.Media;
using static System.Net.Mime.MediaTypeNames;
using System.Data.SqlClient;
using System.Data;
using System.Data.Entity;

namespace LiveChatMVC.signalr.hubs
{
    // PS: When client call function from hub, 
    public class ChatHub : Hub
    {

        private static ConcurrentDictionary<string, Users> SignalRUsers = new ConcurrentDictionary<string, Users>(); // key = connectionId, value = Users object
        private static List<Users> AllConnectedUsers = new List<Users>();
        private string jsonStringUser; // Json string from api contains user information.
        private JObject jsonUser;   // Json object for user informations.
        static readonly Gldnhrn api = new Gldnhrn(); // Dummy api looks like goldenhorn api
        static readonly ChatContext db = new ChatContext(); // Database

        // PS: You have to override onConnect function normally but its example

        public void Connect(string senderToken)
        {
            // Fetching jsonstring contains user information from api via token
            jsonStringUser = api.requestUser(senderToken);

            // Is user exist or not 
            if (jsonStringUser == null)
                //TODO: Return error message to client or console and log it.
                return;

            // Convert JSON string to object
            jsonUser = JObject.Parse(jsonStringUser);
            
            // Initialiazing variables
            int userId = int.Parse(jsonUser["userId"].ToString());
            string userName = jsonUser["userName"].ToString();
            string userRole = jsonUser["role"].ToString();
            string chatId = api.ticketId() + " ticket4";
            


            //GLDNHRNSERVICE.DataClient c = new GLDNHRNSERVICE.DataClient();
            //c.GetUserInfo(senderToken);

            // We cant store object in hub because when client call function from hub, server create hub instance after that
            // server remove hub object and variables in hub object.
            // So store it in CallerState if u want to access variables from outside the functions
            // PS: Can call it from both client and server side.
            Clients.CallerState.userName = userName;
            Clients.CallerState.userId = userId;



            // Search user on database.
            var users = db.Users.Where(i => i.Id == userId).ToList();

            // If user doesnt exist create new user and insert it to database.
            if (users.Count < 1)
            {
                insertNewUser(userId, userName);
            }


            // Search for chatroom on database
            // PS: chatId will come from GoldenHorn api so we use static chatId for this example like its come from goldenhorn.
            // If you use chatId as a signalr group name like us its gonna be easier easier.
            var chats = db.Chats.Where(i => i.Id == chatId).ToList();
            

            // If chat doesnt exist create new chat and add to database.
            if (chats.Count < 1)
            {
                insertNewChat(chatId);
            }

            // Insert chat and user relation to database.
            insertChatUserRelation(chatId, userId);

            //If user already connected but not in this chatroom.
            //PS: This expression not necessary now but when we want to add 2 or more chat on page this expression must be used.
            if (SignalRUsers.ContainsKey(Context.ConnectionId) && SignalRUsers.Values.Any(i => (i.GroupNames.Contains(chatId) && i.UserName == userName)))
            {
                //Clients.All.logging(SignalRUsers.Count());
                SignalRUsers[Context.ConnectionId].addGroupName(chatId);

                // In signalr, adding group to user is asyncron. So this function returns task and we have to assign this to variable
                // but we dont care about returned value. In c# 7.0 we can assing to _ means we dont care returned value.
                _ = joinGroup(chatId);
                Clients.Group(chatId).updateUserCount(SignalRUsers.Count());
                Clients.Group(chatId).updateUserCounter(SignalRUsers.Count());

            }

            // If user exist in chat but his/her connection id changed.(Like reload)
            else if (SignalRUsers.Values.Any(i => (i.GroupNames.Contains(chatId) && i.UserName == userName)))
            {
                // Update users connection id.
                SignalRUsers.TryRemove(SignalRUsers.FirstOrDefault(i => i.Value.UserName == userName).Key, out _);
                SignalRUsers.TryAdd(Context.ConnectionId, new Users { ConnectionId = Context.ConnectionId, UserName = userName, GroupNames = new List<string>() { chatId } });

                _ = joinGroup(chatId);
                Clients.Group(chatId).updateUserCount(SignalRUsers.Count());;

            }


            else
            {
                SignalRUsers.TryAdd(Context.ConnectionId, new Users { ConnectionId = Context.ConnectionId, UserName = userName, GroupNames = new List<string>() { chatId } });
                _ = joinGroup(chatId);
                Clients.Group(chatId).updateUserCount(SignalRUsers.Count());

                //Clients.All.logging("Ayni chate 2 kere giremezsin.");
            }

            

            // Fetch chat history from database.
            /*var pageNumber = 1;
            var messages1 = db.Messages.OrderByDescending(x => x.Id).Take(20).Skip(20 * pageNumber).ToList();*/

            var messagesAndUsers = (from u in db.Users
                                    join m in db.Messages on u.Id equals m.UserId
                                    orderby m.CreatedAt descending
                                    select new { u, m })
                                   .Where(i => i.m.ChatId == chatId && i.m.UserId == i.u.Id);

            


            //select top 20 * from Messages order by Id desc

            // If chat has not any messages, return.
            if (messagesAndUsers.ToList().Count < 1)
                return;

            // Send message histroy as an array to client.
            Clients.Caller.printMessageHistory(messagesAndUsers,userId);
            Clients.AllExcept(chatId).updateUser(SignalRUsers.Values.Where(user => user.GroupNames.Contains(chatId)));

        }
    

        private void insertNewUser(int userId, string userName)
        {
            var user = new User
            {
                Id = userId,
                Name = userName,
            };
            db.Users.Add(user);
            db.SaveChanges();

        }

        private void insertNewChat(string chatId)
        {
            var chat = new Chat
            {
                Id = chatId,
            };
            db.Chats.Add(chat);
            db.SaveChanges();
        }

        private void insertChatUserRelation(string chatId, int userId)
        {
            var userChat = new UserChat
            {
                ChatId = chatId,
                UserId = userId
            };
            db.UserChats.Add(userChat);
            db.SaveChanges();
        }

        //Add to group.
        private async Task joinGroup(string chatId)
        {
            await Groups.Add(Context.ConnectionId, chatId);

            // Update user count on group.
            await Clients.Group(chatId).updateUserCount(SignalRUsers.Values.Where(user => user.GroupNames.Contains(chatId)).Count());
            await Clients.Group(chatId).updateUser(SignalRUsers.Values.Where(user => user.GroupNames.Contains(chatId)));
        }

        // This function works when client wants to send message to group
        public void Send(string message)
        {
            // Initialiaze variables
            string userName = Clients.CallerState.userName;
            string chatId = api.ticketId() + " ticket4";


            // Update Message table on database.(Insert message to message table)
            var msg = new Message
            {
                MessageBody = message,
                ChatId = chatId,
                UserId = Convert.ToInt32(Clients.CallerState.userId),
                CreatedAt = DateTime.Now
            };

            db.Messages.Add(msg);
            db.SaveChanges();

            // Send message to group.
            Clients.OthersInGroup(chatId).addNewMessageToPage(userName, message, msg.CreatedAt);
    
            // Send message to caller client.
            Clients.Caller.addNewMessageToMe(userName, message, msg.CreatedAt);
        }


        // This function call after 6 second timeout. So when user reload the page it tooks 1 second and users client id was changed.
        // We have to handle this on connect function if we still have connection on list we have to re assign new Connection id and username
        // We dont need to remove client from group. Signalr makes it for us. And it's recommened to not remove client from group manually
        public override Task OnDisconnected(bool stopCalled)
        {
            string groupName = api.ticketId() + " ticket4";

            // If user reload the page and connect function works again, user must removed from list and users new connection id updated. So if user reload the page
            // and login in 6 seconds then connect function will remove expired user before the OnDisconnected function. We can't removed user again.
            // Check for user removed on OnDisconnect function.
            bool isRemoved = SignalRUsers.TryRemove(Context.ConnectionId, out Users disconnectedUser); // if isRemoved = false then it means user not disconnected its just reload page and tete his/her connection id.
            if (isRemoved)
            {

                Clients.OthersInGroup(groupName).userLeft(disconnectedUser.UserName);
                Clients.OthersInGroup(groupName).updateUserCount((SignalRUsers.Values.Where(i => i.GroupNames.Contains(groupName)).ToList().Count()));
                Clients.OthersInGroup(groupName).updateUser((SignalRUsers.Values.Where(i => i.GroupNames.Contains(groupName)).ToList()));
            }
            return base.OnDisconnected(stopCalled);
        }

        // When user reconnect add it to list. Signalr automaticly add user to group again so dont need add to user to group.

    }

}