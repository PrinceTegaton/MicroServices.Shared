using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroServices.Shared.DataAccess
{
    public interface IDatabaseQueryAccessor
    {
        string ConnectionString { get; set; }

        Task<Dictionary<string, object>> GetSingleQueryToDictionaryAsync(SqlQueryType type, string query, IDictionary<string, object> parameters = null);
        Task<Q> GetSingleQueryAsync<Q>(SqlQueryType type, string query, IDictionary<string, object> parameters = null);
        Task<List<Q>> GetListFromQueryAsync<Q>(SqlQueryType type, string query, IDictionary<string, object> parameters = null);
    }
}