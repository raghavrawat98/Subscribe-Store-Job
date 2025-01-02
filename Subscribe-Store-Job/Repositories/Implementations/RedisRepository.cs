using NRedisStack;
using NRedisStack.RedisStackCommands;
using NRedisStack.Search;
using StackExchange.Redis;
using Subscribe_Store_Job.Repositories.Abstractions;

namespace Subscribe_Store_Job.Repositories.Implementations
{
    public class RedisRepository : IRedisRepository
    {
        private readonly IDatabase _redisDatabase;
        private readonly JsonCommands _jsonCommands;

        public RedisRepository(IConnectionMultiplexer connectionMultiplexer)
        {
            _redisDatabase = connectionMultiplexer.GetDatabase();
            _jsonCommands = _redisDatabase.JSON();
        }

        public async Task<string> GetValueByIndexQuery(string indexName, Query query)
        {
            SearchResult searchResult = await _redisDatabase.FT().SearchAsync(indexName,query);
            return $"[{string.Join(
                ", ",
                searchResult.Documents.Select(x => x["json"])
                )}]";
        }

        public async Task<string> GetValueByKey(string key)
        {
            RedisResult redisResult = await _jsonCommands.GetAsync(key);
            return redisResult.ToString();
        }

        public async Task<bool> SetValueByKey(string key, object value)
        {
            await _jsonCommands.SetAsync(key,"$",value);
            return true;
        }

        public async Task<bool> DeleteKey(string key)
        {
            await _redisDatabase.KeyDeleteAsync(key);
            return true;
        }

    }
}
