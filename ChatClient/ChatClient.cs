using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using ChatServer;

namespace ChatClient
{
    public class ChatClient : IDisposable
    {
        public event EventHandler<MessageEventArgs> MessageReceived;

        public bool Connected => _tcpClient.Connected;
        
        private string UserName { get; set; }

        private readonly Dictionary<string, ChatMessageCommand> _userMessageCommandBindings =
            new Dictionary<string, ChatMessageCommand>
            {
                {"/join", ChatMessageCommand.Join},
                {"/exit", ChatMessageCommand.Exit}
            };

        private readonly BinaryFormatter _binaryFormatter;
        
        private TcpClient _tcpClient;
        private NetworkStream _networkStream;
        private Task _listeningTask;

        public ChatClient(string userName)
        {
            UserName = userName;

            _binaryFormatter = new BinaryFormatter();
        }

        public void Connect(string hostName, int port)
        {
            _tcpClient = new TcpClient(hostName, port);

            _networkStream = _tcpClient.GetStream();

            _listeningTask = Task.Factory.StartNew(StartListening);
            
            SendMessage(new ChatMessage(UserName, DateTime.Now, String.Empty, ChatMessageCommand.Join));
        }

        public void SendMessage(string text)
        {
            ChatMessage message;
            
            if (TryGetChatMessageCommand(text, out var command, out string textWithoutCommand))
            {
                message = new ChatMessage(UserName, DateTime.Now, textWithoutCommand, command);
            }
            else
            {
                message = new ChatMessage(UserName, DateTime.Now, text);
            }
            
            SendMessage(message);
        }

        public void Dispose()
        {
            _listeningTask?.Dispose();
            _tcpClient?.Dispose();
            _networkStream?.Dispose();
        }
        
        private void SendMessage(ChatMessage message)
        {
            _binaryFormatter.Serialize(_networkStream, message);
        }
        
        private void StartListening()
        {
            try
            {
                while (_tcpClient.Connected)
                {
                    var message = (ChatMessage) _binaryFormatter.Deserialize(_networkStream);

                    MessageReceived?.Invoke(this, new MessageEventArgs(message));
                }
            }
            catch (Exception)
            {
                _tcpClient.Close();
            }
        }

        private bool TryGetChatMessageCommand(
            string text, 
            out ChatMessageCommand command, 
            out string textWithoutCommand)
        {
            command = default;
            textWithoutCommand = text;
            
            if (string.IsNullOrEmpty(text))
            {
                return false;
            }

            if (text[0] == '/')
            {
                int endCommandIndex = text.IndexOf(' ');

                string commandText = text;
                
                if (endCommandIndex != -1)
                {
                    commandText = text.Substring(endCommandIndex + 1);
                }

                // Не самый оптимальный способ сравнения строк, использован для простоты восприятия.
                if (_userMessageCommandBindings.ContainsKey(commandText))
                {
                    command = _userMessageCommandBindings[commandText];
                    textWithoutCommand = text.Remove(0, commandText.Length);

                    return true;
                }
            }

            return false;
        }
    }
}