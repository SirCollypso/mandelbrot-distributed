using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Broker
{
    class ConnectionHandler
    {
        private IPAddress _address;
        private TcpListener _server;

        public ConnectionHandler(string ip, int port)
        {
            _address = IPAddress.Parse(ip);
            _server = new TcpListener(_address, port);
            _server.Start();
        }
        
        public async Task<Stream> ReceiveConnection()
        {
            var client = await _server.AcceptTcpClientAsync();
            return client.GetStream();
        }
    }
}