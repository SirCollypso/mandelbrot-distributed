using System;
using System.Threading.Tasks;

namespace Distributor
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string ip = "127.0.0.1";
            int port = 13000;
            int jobID = 1;
            int pixels = 500;

            if (args.Length > 3)
            {
                ip = args[0];
                port = int.Parse(args[1]);
                jobID = int.Parse(args[2]);
                pixels = int.Parse(args[3]);
            }
            else if (args.Length == 3)
            {
                ip = args[0];
                port = int.Parse(args[1]);
                jobID = int.Parse(args[2]);
            }
            else if (args.Length == 2)
            {
                ip = args[0];
                port = int.Parse(args[1]);
            }
            else if (args.Length == 1)
            {
                ip = args[0];
            }

            var workHandler = new WorkHandler();
            Console.WriteLine($"Connected to {ip}:{port}. Size: {pixels}");
            await workHandler.Run(ip, port, jobID, pixels);
        }
    }
}
