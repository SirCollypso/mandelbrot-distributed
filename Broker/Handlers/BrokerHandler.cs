using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Broker
{
    class BrokerHandler
    {
        private ConnectionHandler _server;
        private List<DataHandler> _dataHandlers;
        private ConcurrentQueue<(string jobID, string x, string y, string size, string pixels, string from, string upperBound)> _queue;

        public BrokerHandler()
        {
            _queue = new ConcurrentQueue<(string jobID, string x, string y, string size, string pixels, string from, string upperBound)>();
            _dataHandlers = new List<DataHandler>();
        }
        
        public async Task Run(string ip, int port)
        {
            _server = new ConnectionHandler(ip, port);

            while (true)
            {
                var stream = await _server.ReceiveConnection();
                var handler = new DataHandler(stream, _queue);
                _dataHandlers.Add(handler);
                handler.Run();
            }
        }
    }
}