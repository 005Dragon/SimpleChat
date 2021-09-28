using System;
using ChatServer;

namespace ChatClient
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            Console.Write("Enter username: ");
            string userName = Console.ReadLine();
            
            var client = new ChatClient(userName);
            client.MessageReceived += ClientOnMessageReceived;

            try
            {
                client.Connect("127.0.0.1", 1234);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }

            while (true)
            {
                var messageText = Console.ReadLine();

                if (!client.Connected)
                {
                    break;
                }

                try
                {
                    client.SendMessage(messageText);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }
            
            Console.WriteLine("Session finished.");
        }

        private static void ClientOnMessageReceived(object sender, MessageEventArgs eventArgs)
        {
            ChatMessage message = eventArgs.Message;
            
            Console.WriteLine($"{message.CreatedTime:t} {message.UserName}:{message.Text}");
        }
    }
}