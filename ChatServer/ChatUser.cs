using System;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace ChatServer
{
    public class ChatUser : IDisposable
    {
        public event EventHandler<MessageEventArgs> MessageReceived;
        public event EventHandler Disconnected;
        
        public bool Connected => _tcpClient.Connected;
        
        public string UserName { get; set; }
        
        private NetworkStream _networkStream;
        private Task _listenerTask;

        private readonly TcpClient _tcpClient;
        private readonly BinaryFormatter _binaryFormatter;

        public ChatUser(TcpClient tcpClient)
        {
            _tcpClient = tcpClient;
            UserName = "User" + DateTime.Now.Ticks;
            
            _binaryFormatter = new BinaryFormatter();
        }

        public void StartSession()
        {
            _networkStream = _tcpClient.GetStream();
            _listenerTask = Task.Factory.StartNew(StartListening);
        }

        public void SendMessage(ChatMessage message)
        {
            _binaryFormatter.Serialize(_networkStream, message);
        }

        public void Disconnect()
        {
            if (_tcpClient.Connected)
            {
                _tcpClient.Close();
            }
            
            Disconnected?.Invoke(this, EventArgs.Empty);
        }

        public void Dispose()
        {
            _listenerTask?.Dispose();
            _tcpClient?.Dispose();
            _networkStream?.Dispose();
        }

        private void StartListening()
        {
            try
            {
                while (_tcpClient.Connected)
                {
                    ChatMessage message = (ChatMessage) _binaryFormatter.Deserialize(_networkStream);

                    MessageReceived?.Invoke(this, new MessageEventArgs(message));
                }
            }
            catch (Exception)
            {
                Disconnected?.Invoke(this, EventArgs.Empty);
                throw;
            }
        }
    }
}