using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using System;
using SixLabors.ImageSharp.PixelFormats;
using Broker;
using ResultStore;
using System.Collections.Generic;

namespace Worker
{
    public class MandelbrotHandler
    {
        private BrokerConnection _brokerConnection;
        private ResultStoreConnection _resultStoreConnection;
        private Mandelbrot _mandelbrot;
        private Stopwatch _sw;
        public async Task Run(string broker_ip, int broker_port, string saver_ip, int saver_port, double x = -.70, double y = 0, double size = 2.5)
        {
            _brokerConnection = new BrokerConnection(broker_ip, broker_port);
            _resultStoreConnection = new ResultStoreConnection(saver_ip, saver_port);
            _mandelbrot = new Mandelbrot(_brokerConnection, _resultStoreConnection);

            _sw = Stopwatch.StartNew();
            await _mandelbrot.StartCalculation();
            _sw.Stop();
        }
    }
}
