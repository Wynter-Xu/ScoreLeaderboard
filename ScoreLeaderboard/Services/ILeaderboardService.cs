using ScoreLeaderboard.Models;

namespace ScoreLeaderboard.Services
{
    public interface ILeaderboardService
    {
        List<CustomerResponse> GetCustomerNeighborhood(ulong customerId, int high, int low);
        List<CustomerResponse> GetCustomersByRank(int start, int end);
        decimal UpdateScore(ulong customerId, decimal scoreChange);
    }
}