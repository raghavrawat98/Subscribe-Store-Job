using Subscribe_Store_Job.Models;

namespace Subscribe_Store_Job.Repositories.Abstractions
{
    public interface IMatchRepository
    {
        Task<bool> AddMatches(List<Match> matches);
        Task<bool> DeleteMatches(List<Match> matches);
        Task<List<Match>> SearchMacthes(long orderReleasePk);
    }
}
