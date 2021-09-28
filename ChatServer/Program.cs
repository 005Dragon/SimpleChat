using System;
using System.Net;

namespace ChatServer
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            var chatRoom = new ChatRoom("main chat");
            chatRoom.MessagePublished += ChatRoomOnMessagePublished;
            chatRoom.PublishSystemMessage($"Chat room \"{chatRoom.RoomName}\" created.");
            
            var distributor = new ChatUserDistributor(chatRoom);
            
            var chatUserAcceptor = new ChatUserAcceptor(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1234));
            chatUserAcceptor.Start();
            
            while (true)
            {
                ChatUser user = chatUserAcceptor.AcceptUser();
                user.StartSession();
                
                distributor.AddUser(user);
            }
        }

        private static void ChatRoomOnMessagePublished(object sender, MessageEventArgs eventArgs)
        {
            ChatMessage message = eventArgs.Message;
            
            Console.WriteLine($"{message.CreatedTime:t} {message.UserName}:{message.Text}");
        }
    }
}