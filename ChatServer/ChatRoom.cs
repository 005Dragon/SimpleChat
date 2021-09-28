using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace ChatServer
{
    public class ChatRoom
    {
        public event EventHandler<MessageEventArgs> MessagePublished; 
        
        public string RoomName { get; }
        public string ChatSystemUserName { get; set; } = "system";

        private readonly List<ChatUser> _users;
        private readonly ChatHistory _history;
        private readonly int _sizeHistoryMessages = 10;

        public ChatRoom(string roomName)
        {
            RoomName = roomName;

            _history = new ChatHistory(_sizeHistoryMessages);
            _users = new List<ChatUser>();
        }

        public void AddUser(ChatUser user)
        {
            if (_users.Contains(user))
            {
                return;
            }
            
            PublishSystemMessage($"user \"{user.UserName}\" joined");
            
            user.MessageReceived += UserOnMessageReceived;
            user.Disconnected += UserOnDisconnected;
            _users.Add(user);

            foreach (ChatMessage message in _history.GetMessages())
            {
                user.SendMessage(message);
            }
        }

        public void RemoveUser(ChatUser user)
        {
            if (!_users.Contains(user))
            {
                return;
            }

            user.Disconnected -= UserOnDisconnected;
            user.MessageReceived -= UserOnMessageReceived;
            _users.Remove(user);
            
            PublishSystemMessage($"user \"{user.UserName}\" disconnected");
        }
        
        public void PublishSystemMessage(string text)
        {
            PublishMessage(null, new ChatMessage(ChatSystemUserName, DateTime.Now, text));
        }
        
        private void PublishMessage(ChatUser sender, ChatMessage message)
        {
            for (int i = 0; i < _users.Count; i++)
            {
                // Не отправляем сообщение обратно отправителю.
                if (_users[i] == sender)
                {
                    continue;
                }
                
                try
                {
                    _users[i].SendMessage(message);
                }
                catch (Exception exception)
                {
                    _users[i--].Disconnect();
                    
                    Console.WriteLine(exception);
                }
            }
            
            MessagePublished?.Invoke(this, new MessageEventArgs(message));
            _history.Add(message);
        }

        private void UserOnMessageReceived(object sender, MessageEventArgs eventArgs)
        {
            var user = (ChatUser) sender;

            bool nameChanged = 
                string.Compare(user.UserName, eventArgs.Message.UserName, StringComparison.Ordinal) != 0;

            if (nameChanged)
            {
                PublishSystemMessage($"User \"{user.UserName}\" change username on \"{eventArgs.Message.UserName}\"");

                user.UserName = eventArgs.Message.UserName;
            }

            // Не публикуем сообщения с командами серверу.
            if (eventArgs.Message.Command == ChatMessageCommand.None)
            {
                PublishMessage(user, eventArgs.Message);
            }
        }

        private void UserOnDisconnected(object sender, EventArgs eventArgs)
        {
            RemoveUser((ChatUser) sender);
        }
    }
}