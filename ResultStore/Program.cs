using System;
using System.Threading.Tasks;

namespace ResultStore
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string ip = "127.0.0.1";
            int port = 10000;
            
            if (args.Length == 2)
            {
                ip = args[0];
                port = int.Parse(args[1]);
            }
            else if (args.Length == 1)
            {
                ip = args[0];
            }

            var imageHandler = new ImageHandler();
            Console.WriteLine($"Listening on {ip}:{port}");
            await imageHandler.Run(ip, port);
        }
    }
}
