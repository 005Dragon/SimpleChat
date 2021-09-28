using System;

namespace ChatServer
{
    [Serializable]
    public enum ChatMessageCommand
    {
        None,
        Join,
        Exit
    }
}