using NRedisStack.Search;
using StackExchange.Redis;
using Subscribe_Store_Job.Models;
using Subscribe_Store_Job.Repositories.Abstractions;
using System.Text.Json;

namespace Subscribe_Store_Job.Repositories.Implementations
{
    public class MatchRepository : IMatchRepository
    {
        private IRedisRepository _redisRepo;
        private const string MatchIndex = "MatchIndex";

        public MatchRepository(
            IRedisRepository redisRepo
            )
        {
            _redisRepo = redisRepo;
        }
        public async Task<bool> AddMatches(List<Match> matches)
        {
            foreach (Match match in matches) 
            {
                string matchKey = $"MatchPk:{match.MatchPk}";
                return await _redisRepo.SetValueByKey(matchKey, match);
            }
            return true;
        }

        public async Task<bool> DeleteMatches(List<Match> matches)
        {
            foreach (Match match in matches)
            {
                string matchKey = $"MatchPk:{match.MatchPk}";
                await _redisRepo.DeleteKey(matchKey);
            }
            return true;
        }

        public async Task<List<Match>> SearchMacthes(long orderReleasePk)
        {
            Query query = new Query($"@OrderReleasePk:[{orderReleasePk} {orderReleasePk}]");
            string result = await _redisRepo.GetValueByIndexQuery(MatchIndex, query);
            return JsonSerializer.Deserialize<List<Match>>(result);
        }
    }
}
