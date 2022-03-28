using System;
using System.Threading.Tasks;

namespace Broker
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string ip = "127.0.0.1";
            int port = 13000;

            if (args.Length > 1)
            {
                ip = args[0];
                port = int.Parse(args[1]);
            }
            else if (args.Length == 1)
            {
                ip = args[0];
            }

            var brokerHandler = new BrokerHandler();
            Console.WriteLine($"Listening on {ip}:{port}");
            await brokerHandler.Run(ip, port);
        }
    }
}
