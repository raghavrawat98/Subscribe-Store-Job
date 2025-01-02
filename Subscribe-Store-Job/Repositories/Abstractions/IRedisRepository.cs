using NRedisStack.Search;

namespace Subscribe_Store_Job.Repositories.Abstractions
{
    public interface IRedisRepository
    {
        Task<bool> SetValueByKey(string key, object value); 
        Task<string> GetValueByIndexQuery(string indexName, Query query); 
        Task<string> GetValueByKey(string key); 
        Task<bool> DeleteKey(string key); 
    }
}
