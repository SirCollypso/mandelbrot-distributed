using System.Collections.Generic;
using System.Threading.Tasks;

namespace ResultStore
{
    public interface IResultStoreConnection
    {
        Task PublishRow(IDictionary<string, string> values);
    }
}