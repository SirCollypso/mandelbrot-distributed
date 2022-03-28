using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Broker;
using ResultStore;

namespace Worker
{
    public class Mandelbrot
    {
        private IBrokerConnection _brokerConnection;
        private IResultStoreConnection _resultStoreConnection;
        public const int MAXVALUE = 69887;

        public Mandelbrot(IBrokerConnection brokerConnection, IResultStoreConnection resultStoreConnection)
        {
            _brokerConnection = brokerConnection;
            _resultStoreConnection = resultStoreConnection;
        }
        public async Task StartCalculation(double x = -.70, double y = 0, double size = 2.5)
        {
            var procs = Environment.ProcessorCount;
            var tasks = new List<Task>();

            while (tasks.Count < procs)
            {
                var task = Task.Run(async () =>
                {
                    while (true)
                    {
                        var values = await _brokerConnection.Receive();
                        Console.WriteLine("JobID: {0}, X: {1}, Y: {2}, Size: {3}, Pixels: {4}, From {5}, UpperBound: {6}", values["jobID"], values["x"], values["y"], values["size"], values["pixels"], values["from"], values["upperBound"]);
                        if (!string.IsNullOrEmpty(values["from"]) && !string.IsNullOrEmpty(values["upperBound"]))
                        {
                            await CalculateChunk(double.Parse(values["x"]), double.Parse(values["y"]), double.Parse(values["size"]), int.Parse(values["from"]), int.Parse(values["upperBound"]), int.Parse(values["jobID"]), int.Parse(values["pixels"]));
                        }      
                    }
                });

                tasks.Add(task);
            }
            Console.WriteLine(tasks.Count);
            await Task.WhenAll(tasks);
        }
        private double _posConst;
        private double _halfSize;
        private int GetMandelbrotValue(double x, double y, double yp, double xp)
        {
            double ypos = y + _posConst * yp - _halfSize; //_y + _size * (yp - Pixels / 2) / ((double)Pixels);
            double xpos = x + _posConst * xp - _halfSize; //_x + _size * (xp - Pixels / 2) / ((double)Pixels);

            double y_ = ypos;
            double x_ = xpos;

            double y2 = y_ * y_;
            double x2 = x_ * x_;

            int value = 1;

            while ((y2 + x2) <= 4 && value < MAXVALUE)
            {
                y_ = 2 * x_ * y_ + ypos;
                x_ = x2 - y2 + xpos;

                y2 = y_ * y_;
                x2 = x_ * x_;

                value++;
            }

            return value;
        }
        private async Task CalculateChunk(double x, double y, double size, int from, int upperBound, int jobID, int pixels)
        {
            for (int row = from; row < upperBound; row++)
            {
                int[] values = new int[pixels];

                for (int column = 0; column < pixels; column++)
                {
                    _halfSize = size / 2;
                    _posConst = size / ((double)pixels);
                    values[column] = GetMandelbrotValue(x, y, row, column);
                }

                await OnRowCreated(row, values, jobID, pixels);
            }
        }

        private async Task OnRowCreated(int row, int[] valuesOfRow, int jobID, int pixels)
        {
            var values = new Dictionary<string, string>();

            if (!(row == 0 && valuesOfRow == null && jobID == 0 && pixels == 0))
            {
                values.Add("row", row.ToString());
                string valuesOfRow_ = "";
                foreach (int i in valuesOfRow)
                {
                    valuesOfRow_ += i.ToString("D10");
                }
                values.Add("valuesOfRow", valuesOfRow_);
                values.Add("jobID", jobID.ToString());
                values.Add("pixels", pixels.ToString());
                await _resultStoreConnection.PublishRow(values);
            }
        }
    }
}