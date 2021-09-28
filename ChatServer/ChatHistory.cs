using System.Collections.Generic;

namespace ChatServer
{
    public class ChatHistory
    {
        public int Size { get; set; }
        
        private readonly Queue<ChatMessage> _bufferedMessages = new Queue<ChatMessage>();

        public ChatHistory(int size)
        {
            Size = size;
        }

        public void Add(ChatMessage message)
        {
            _bufferedMessages.Enqueue(message);

            if (_bufferedMessages.Count > Size)
            {
                _bufferedMessages.Dequeue();
            }
        }

        public IEnumerable<ChatMessage> GetMessages() => _bufferedMessages;
    }
}