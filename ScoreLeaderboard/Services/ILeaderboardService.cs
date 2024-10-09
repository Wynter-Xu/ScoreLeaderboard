using ScoreLeaderboard.Models;

namespace ScoreLeaderboard.Services
{
    public interface ILeaderboardService
    {
        List<CustomerResponse> GetCustomerNeighborhood(long customerId, int high, int low);
        List<CustomerResponse> GetCustomersByRank(int start, int end);
        decimal UpdateScore(long customerId, decimal scoreChange);
    }
}