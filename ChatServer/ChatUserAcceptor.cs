using System.Net;
using System.Net.Sockets;

namespace ChatServer
{
    public class ChatUserAcceptor
    {
        public IPEndPoint EndPoint { get; }

        private readonly TcpListener _listener;

        public ChatUserAcceptor(IPEndPoint endPoint)
        {
            EndPoint = endPoint;
            _listener = new TcpListener(endPoint);
        }

        public void Start()
        {
            _listener.Start();
        }

        public ChatUser AcceptUser()
        {
            TcpClient tcpClient = _listener.AcceptTcpClient();

            return new ChatUser(tcpClient);
        }
    }
}