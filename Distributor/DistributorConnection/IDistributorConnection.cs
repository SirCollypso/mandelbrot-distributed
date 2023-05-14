using System.Collections.Generic;
using System.Threading.Tasks;

namespace Distributor
{
    public interface IDistributorConnection

    {
        Task Publish(IDictionary<string, string> values);
        
        Task<IDictionary<string, string>> Receive();
    }
}
