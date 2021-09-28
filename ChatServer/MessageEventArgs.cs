using System;

namespace ChatServer
{
    public class MessageEventArgs : EventArgs
    {
        public ChatMessage Message { get; }

        public MessageEventArgs(ChatMessage message)
        {
            Message = message;
        }
    }
}