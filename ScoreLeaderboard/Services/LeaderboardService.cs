using ScoreLeaderboard.Models;
using System.Collections.Concurrent;
using System.Collections.Immutable;

namespace ScoreLeaderboard.Services
{
    public class LeaderboardService : ILeaderboardService
    {
        private readonly ConcurrentDictionary<long, Customer> _customers = new();
        private ImmutableSortedSet<Customer> _rankedList = [];

        public decimal UpdateScore(long customerId, decimal scoreChange)
        {
            var customer = _customers.GetOrAdd(customerId, new Customer { CustomerId = customerId, Score = 0 });
            _rankedList = _rankedList.Remove(customer);
            customer.Score += scoreChange;
            if (customer.Score > 0)
            {
                //UpdateRankingList
                _rankedList = _rankedList.Add(customer);
            }

            return customer.Score;
        }

        public List<CustomerResponse> GetCustomersByRank(int start, int end)
        {
            var res = new List<CustomerResponse>();
            var count = _customers.Count;
            if (count < start) return res;
            for (int i = start; i <= int.Min(end, count-1); i++)
            {
                var customer = _rankedList[i - 1];
                res.Add(new CustomerResponse(customer.CustomerId, customer.Score, i));
            }
            return res;
            //return _rankedList.Skip(start - 1).Take(end - start + 1).ToList().Select();
        }



        public List<CustomerResponse> GetCustomerNeighborhood(long customerId, int high, int low)
        {
            var res = new List<CustomerResponse>();
            var customer = _customers.GetValueOrDefault(customerId);
            if (customer == null) return res;

            var rankCur = _rankedList.IndexOf(customer) + 1;
            if (rankCur < 0) return res;
            int start = int.Max(1, rankCur - low);
            int end = rankCur + high;
            return GetCustomersByRank(start, end);
        }

    }
}