using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Broker
{
    class DataHandler
    {
        private StreamReader _streamReader;
        private StreamWriter _streamWriter;
        private ConcurrentQueue<(string jobID, string x, string y, string size, string pixels, string from, string upperBound)> _queue;
       
        public DataHandler(Stream stream, ConcurrentQueue<(string jobID, string x, string y, string size, string pixels, string from, string upperBound)> queue)
        {
            _streamReader = new StreamReader(stream);
            _streamWriter = new StreamWriter(stream);
            _queue = queue;
        }

        public async Task Run()
        {
            string cmd;

            while ((cmd = await _streamReader.ReadLineAsync()).ToUpper() != "FIN" && !string.IsNullOrEmpty(cmd))
            {
                if (cmd == "Publish")
                {
                    var values = await GetPublishedValue();
                    _queue.Enqueue((values["jobID"], values["x"], values["y"], values["size"], values["pixels"], values["from"], values["upperBound"]));
                    Console.WriteLine("JobID: {0}, X: {1}, Y: {2}, Size: {3}, Pixels: {4}, From {5}, UpperBound: {6}", values["jobID"], values["x"], values["y"], values["size"], values["pixels"], values["from"], values["upperBound"]);
                    Console.WriteLine("Publish");

                }
                else if (cmd == "Receive")
                {
                    if (_queue.TryDequeue(out var workItem))
                    {
                        var values = new Dictionary<string, string>();
                        values.Add("jobID", workItem.jobID);
                        values.Add("x", workItem.x);
                        values.Add("y", workItem.y);
                        values.Add("size", workItem.size);
                        values.Add("pixels", workItem.pixels);
                        values.Add("from", workItem.from);
                        values.Add("upperBound", workItem.upperBound);
                        await SendValue(values);
                        Console.WriteLine("JobID: {0}, X: {1}, Y: {2}, Size: {3}, Pixels: {4}, From {5}, UpperBound: {6}", values["jobID"], values["x"], values["y"], values["size"], values["pixels"], values["from"], values["upperBound"]);
                        Console.WriteLine("Receive");
                    }
                    else
                    {
                        while (true)
                        {
                            if (_queue.TryDequeue(out var workItem2))
                            {
                                var values = new Dictionary<string, string>();
                                values.Add("jobID", workItem2.jobID);
                                values.Add("x", workItem2.x);
                                values.Add("y", workItem2.y);
                                values.Add("size", workItem2.size);
                                values.Add("pixels", workItem2.pixels);
                                values.Add("from", workItem2.from);
                                values.Add("upperBound", workItem2.upperBound);
                                await SendValue(values);
                                Console.WriteLine("JobID: {0}, X: {1}, Y: {2}, Size: {3}, Pixels: {4}, From {5}, UpperBound: {6}", values["jobID"], values["x"], values["y"], values["size"], values["pixels"], values["from"], values["upperBound"]);
                                Console.WriteLine("Receive");
                                break;
                            }
                        }
                    }
                }
            }
        }

        private async Task<IDictionary<string, string>> GetPublishedValue()
        {
            string data = await _streamReader.ReadLineAsync();
            var substrings = data.Split('|');
            var values = new Dictionary<string, string>();
            foreach (string s in substrings)
            {
                var value = false;
                var k = "";
                var v = "";

                foreach (char c in s)
                {
                    if (c == '=')
                    {
                        value = true;
                    }
                    else if (value)
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
        
        private async Task SendValue(IDictionary<string, string> values)
        {
            var data = string.Join("|", values.Select(kvp => $"{kvp.Key}={kvp.Value}"));
            await _streamWriter.WriteLineAsync(data);
            await _streamWriter.FlushAsync();
        }
    }
}