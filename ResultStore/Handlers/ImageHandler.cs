using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ResultStore
{
    class ImageHandler
    {
        private List<Stream> _streamList;
        private List<StreamReader> _streamReaderList;
        private Dictionary<string, Image<Rgba32>> _imageList;
        private ConcurrentDictionary<string, int> _listOfCompletedRows;
        private System.Drawing.Color[] _mapping;
        private object lockObject = new object();
        public ImageHandler()
        {
            _mapping = new System.Drawing.Color[16];
            _mapping[0] = System.Drawing.Color.FromArgb(66, 30, 15);
            _mapping[1] = System.Drawing.Color.FromArgb(25, 7, 26);
            _mapping[2] = System.Drawing.Color.FromArgb(9, 1, 47);
            _mapping[3] = System.Drawing.Color.FromArgb(4, 4, 73);
            _mapping[4] = System.Drawing.Color.FromArgb(0, 7, 100);
            _mapping[5] = System.Drawing.Color.FromArgb(12, 44, 138);
            _mapping[6] = System.Drawing.Color.FromArgb(24, 82, 177);
            _mapping[7] = System.Drawing.Color.FromArgb(57, 125, 209);
            _mapping[8] = System.Drawing.Color.FromArgb(134, 181, 229);
            _mapping[9] = System.Drawing.Color.FromArgb(211, 236, 248);
            _mapping[10] = System.Drawing.Color.FromArgb(241, 233, 191);
            _mapping[11] = System.Drawing.Color.FromArgb(248, 201, 95);
            _mapping[12] = System.Drawing.Color.FromArgb(255, 170, 0);
            _mapping[13] = System.Drawing.Color.FromArgb(204, 128, 0);
            _mapping[14] = System.Drawing.Color.FromArgb(153, 87, 0);
            _mapping[15] = System.Drawing.Color.FromArgb(106, 52, 3);

            _streamList = new List<Stream>();
            _streamReaderList = new List<StreamReader>();
            _imageList = new Dictionary<string, Image<Rgba32>>();
            _listOfCompletedRows = new ConcurrentDictionary<string, int>();
        }
        public async Task Run(string ip, int port)
        {
            int index = 0;
            var server = new ConnectionHandler(ip, port);


            while (true)
            {
                var stream = await server.ReceiveConnection();
                _streamList.Add(stream);
                _streamReaderList.Add(new StreamReader(_streamList[index]));
                HandleImage(index);
                index++;
            }
        }
        private async Task HandleImage(int index)
        {
            string data;

            while ((data = await _streamReaderList[index].ReadLineAsync()) != "FIN" && !string.IsNullOrEmpty(data))
            {
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

                int row = int.Parse(values["row"]);
                int[] valuesOfRow = new int[int.Parse(values["pixels"])];

                int arrayLen = int.Parse(values["pixels"]);
                int j = 0;
                for (int i = 0; i < arrayLen; i++)
                {
                    valuesOfRow[i] = int.Parse(values["valuesOfRow"].AsSpan(j, 10));
                    j += 10;
                }

                if (!_imageList.ContainsKey(values["jobID"]))
                {
                    int pixels = int.Parse(values["pixels"]);
                    _imageList.Add(values["jobID"], new Image<Rgba32>(pixels, pixels));
                    _listOfCompletedRows.TryAdd(values["jobID"], 0);
                }

                _imageList.TryGetValue(values["jobID"], out Image<Rgba32> image);

                for (int x = 0; x < valuesOfRow.Length; x++)
                {
                    int v = valuesOfRow[x];
                    System.Drawing.Color c = GetColor(v, "Wikipedia");
                    image[x, row] = new Rgba32(c.R, c.G, c.B, c.A);
                }

                lock (lockObject)
                {
                    _listOfCompletedRows[values["jobID"]]++;
                }

                Console.WriteLine(_listOfCompletedRows[values["jobID"]]);

                if (_listOfCompletedRows[values["jobID"]] == image.Width)
                {
                    lock (lockObject)
                    {
                        _listOfCompletedRows[values["jobID"]] = 0;
                    }
                    await SaveImage(_imageList[values["jobID"]]);
                    int pixels = int.Parse(values["pixels"]);
                    _imageList[values["jobID"]] = new Image<Rgba32>(pixels, pixels);
                    Console.WriteLine("Image Saved!");
                }
            }
        }
        private System.Drawing.Color GetColor(int v, string style)
        {
            if (v == 69887)
            {
                return System.Drawing.Color.FromArgb(0, 0, 0);
            }

            switch (style)
            {
                case "Razer":
                    double quotient = (double)v / 69887 * 1000;
                    if (quotient > 0.5)
                    {
                        return System.Drawing.Color.FromArgb((int)(quotient * 255 % 255), 255, (int)(quotient * 255 % 255));
                    }
                    else
                    {
                        return System.Drawing.Color.FromArgb(0, (int)(quotient * 255 % 255), 0);
                    }
                case "Wikipedia":
                    int i = v % 16;
                    return _mapping[i];
                default:
                    return System.Drawing.Color.FromArgb(255 - (v * 3 % 256), 255 - (v * 7 % 256), 255 - (v * 13 % 256));
            }
        }

        private async Task SaveImage(Image<Rgba32> image)
        {
            using (image)
            {
                image.Save($"{image.Width}.bmp");
            }
        }
    }
}