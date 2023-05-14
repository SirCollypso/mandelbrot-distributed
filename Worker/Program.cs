using System;
using System.Threading.Tasks;

namespace Worker
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string broker_ip = "127.0.0.1";
            int broker_port = 13000;
            string saver_ip = "127.0.0.1";
            int saver_port = 10000;

            if (args.Length > 3)
            {
                broker_ip = args[0];
                broker_port = int.Parse(args[1]);
                saver_ip = args[2];
                saver_port = int.Parse(args[3]);
            }
            else if (args.Length == 3)
            {
                broker_ip = args[0];
                broker_port = int.Parse(args[1]);
                saver_ip = args[2];
            }
            else if (args.Length == 2)
            {
                broker_ip = args[0];
                broker_port = int.Parse(args[1]);
            }
            else if (args.Length == 1)
            {
                broker_ip = args[0];
            }

            var mandelbrotHandler = new MandelbrotHandler();
            Console.WriteLine($"Connected to Broker {broker_ip}:{broker_port} and ResultSaver {saver_ip}:{saver_port}");
            await mandelbrotHandler.Run(broker_ip, broker_port, saver_ip, saver_port); 
        }
    }
}
