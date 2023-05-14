using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ResultStore
{
    public class ResultStoreConnection : IResultStoreConnection
    {
        private TcpClient client;
        private NetworkStream stream;
        private StreamWriter _streamWriter;
        private StreamReader _streamReader;
        private SemaphoreSlim _syncObj = new SemaphoreSlim(1);
        
        public ResultStoreConnection(string ip, int port)
        {
            client = new TcpClient(ip, port);
            stream = client.GetStream();
            _streamWriter = new StreamWriter(stream);
            _streamReader = new StreamReader(stream);
        }

        public async Task PublishRow(IDictionary<string, string> values)
        {
            await _syncObj.WaitAsync();
            var data = string.Join("|", values.Select(kvp => $"{kvp.Key}={kvp.Value}"));
            await _streamWriter.WriteLineAsync(data);
            await _streamWriter.FlushAsync();
            _syncObj.Release();
        }
    }
}