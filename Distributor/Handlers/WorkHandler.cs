using System;
using System.Threading.Tasks;

namespace Distributor
{
    public class WorkHandler
    {
        public async Task Run(string ip, int port, int jobID, int pixels, double x = -.70, double y = 0, double size = 2.5)
        {
            var distributorConnection = new DistributorConnection(ip, port);
            var workGenerator = new WorkGenerator(distributorConnection, jobID, pixels);
            await workGenerator.GenerateWork(1);

            while (true)
            {
                try
                {
                    Console.Write("X: ");
                    int xPos = int.Parse(Console.ReadLine());
                    Console.Write("Y: ");
                    int yPos = int.Parse(Console.ReadLine());

                    double minX = x - size / 2, minY = y - size / 2;
                    x = minX + (double)xPos / (double)pixels * size;
                    y = minY + (double)yPos / (double)pixels * size;
                    size = size * 0.5;

                    await workGenerator.GenerateWork(1, x, y, size);
                }
                catch (FormatException)
                {
                    Console.WriteLine("Wrong format...");
                }
            }
        }
    }
}
