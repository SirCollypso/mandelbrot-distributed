using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Distributor
{
    public class DistributorConnection : IDistributorConnection
    {

        private TcpClient client;
        private NetworkStream stream;
        private StreamWriter _streamWriter;
        private StreamReader _streamReader;
        private SemaphoreSlim _syncObj = new SemaphoreSlim(1);

        public DistributorConnection(string ip, int port)
        {
            client = new TcpClient(ip, port);
            stream = client.GetStream();
            _streamWriter = new StreamWriter(stream);
            _streamReader = new StreamReader(stream);
        }

        public async Task Publish(IDictionary<string, string> values)
        {
            await _syncObj.WaitAsync();
            await _streamWriter.WriteLineAsync("Publish");
            var data = string.Join("|", values.Select(kvp => $"{kvp.Key}={kvp.Value}"));
            await _streamWriter.WriteLineAsync(data);
            await _streamWriter.FlushAsync();
            _syncObj.Release();
        }
        
        public async Task<IDictionary<string, string>> Receive()
        {
            await _syncObj.WaitAsync();
            await _streamWriter.WriteLineAsync("Receive");
            await _streamWriter.FlushAsync();
            string data = await _streamReader.ReadLineAsync();
            _syncObj.Release();
            var substrings = data.Split('|');
            var values = new Dictionary<string, string>();
            foreach(string s in substrings)
            {
                var value = false;
                var k = "";
                var v = "";

                foreach(char c in s)
                {
                    if(c == '=')
                    {
                        value = true;
                    }
                    else if(value)
                    {
                        v += c;
                    }
                    else
                    {
                        k += c;
                    }
                }

                values.Add(k, v);
            }
            return values;
        }
    }
}