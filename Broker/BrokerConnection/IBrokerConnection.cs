using System.Collections.Generic;
using System.Threading.Tasks;

namespace Broker
{
    public interface IBrokerConnection
    {
        Task Publish(IDictionary<string, string> values);
        
        Task<IDictionary<string, string>> Receive();
    }
}
