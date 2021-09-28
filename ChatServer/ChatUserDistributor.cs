using System;
using System.Collections.Generic;

namespace ChatServer
{
    public class ChatUserDistributor
    {
        private readonly Dictionary<ChatMessageCommand, Action<ChatUser>> _userCommandActions;

        private readonly ChatRoom _chatRoom;

        private readonly List<ChatUser> _users;

        public ChatUserDistributor(ChatRoom chatRoom)
        {
            _chatRoom = chatRoom;
            _users = new List<ChatUser>();
            
            _userCommandActions = new Dictionary<ChatMessageCommand, Action<ChatUser>>
            {
                {ChatMessageCommand.Join, user => _chatRoom.AddUser(user)},
                {ChatMessageCommand.Exit, user => user.Disconnect()}
            };
        }

        public void AddUser(ChatUser user)
        {
            if (_users.Contains(user))
            {
                return;
            }
            
            user.MessageReceived += UserOnMessageReceived;
            user.Disconnected += UserOnDisconnected;
            _users.Add(user);
        }

        public void RemoveUser(ChatUser user)
        {
            if (!_users.Contains(user))
            {
                return;
            }
            
            user.MessageReceived -= UserOnMessageReceived;
            user.Disconnected -= UserOnDisconnected;
            _users.Remove(user);
        }

        private void UserOnMessageReceived(object sender, MessageEventArgs eventArgs)
        {
            if (_userCommandActions.ContainsKey(eventArgs.Message.Command))
            {
                var user = (ChatUser) sender;

                user.UserName = eventArgs.Message.UserName;
                
                _userCommandActions[eventArgs.Message.Command].Invoke(user);
            }
        }
        
        private void UserOnDisconnected(object sender, EventArgs eventArgs)
        {
            RemoveUser((ChatUser)sender);
        }
    }
}