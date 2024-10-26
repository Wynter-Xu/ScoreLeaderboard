using ScoreLeaderboard.Models;

namespace ScoreLeaderboard.Services
{
    public class LeaderboardService : ILeaderboardService
    {
        private readonly SkipListRank skipListRank;

        public LeaderboardService()
        {
            skipListRank = new SkipListRank(8);
        }

        public List<CustomerResponse> GetCustomerNeighborhood(ulong customerId, int high, int low) => skipListRank.GetCustomerNeighborhood(customerId, high, low);



        public List<CustomerResponse> GetCustomersByRank(int start, int end) => skipListRank.GetCustomersByRank(start, end);


        public decimal UpdateScore(ulong customerId, decimal scoreChange) => skipListRank.UpdateScore(customerId, scoreChange);

    }
}