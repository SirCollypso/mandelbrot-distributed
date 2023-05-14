using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Distributor
{
    public class WorkGenerator
    {
        private int _jobID;
        private int _pixels;
        private IDistributorConnection _connection;

        public WorkGenerator(IDistributorConnection connection, int jobID, int pixels)
        {
            _connection = connection;
            _jobID = jobID;
            _pixels = pixels;
        }
        
        public async Task GenerateWork(int chunkSize, double x = -.70, double y = 0, double size = 2.5)
        {
            var values = new Dictionary<string, string>();
            values.Add("jobID", _jobID.ToString());
            values.Add("x", x.ToString());
            values.Add("y", y.ToString());
            values.Add("size", size.ToString());
            values.Add("pixels", _pixels.ToString());
            values.Add("from", "");
            values.Add("upperBound", "");

            int numOfChunks = _pixels / chunkSize;
            for (int chunk = 0; chunk < numOfChunks; chunk++)
            {
                var upperChunkBound = chunk == numOfChunks - 1 ? _pixels : (chunk + 1) * chunkSize;
                values["from"] = (chunk * chunkSize).ToString();
                values["upperBound"] = upperChunkBound.ToString();
                await _connection.Publish(values);
            }
        }
    }
}