using System;

namespace ChatServer
{
    [Serializable]
    public readonly struct ChatMessage
    {
        public readonly string UserName;
        public readonly DateTime CreatedTime;
        public readonly string Text;
        public readonly ChatMessageCommand Command;

        public ChatMessage(
            string userName, 
            DateTime createdTime, 
            string text,
            ChatMessageCommand command = ChatMessageCommand.None)
        {
            UserName = userName;
            CreatedTime = createdTime;
            Command = command;
            Text = text;
        }
    }
}