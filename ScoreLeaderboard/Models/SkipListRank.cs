using System.Collections.Concurrent;

namespace ScoreLeaderboard.Models
{
    internal struct Customer : IComparable<Customer>
    {
        public ulong Id;
        public decimal Score;

        public Customer(ulong id, decimal score)
        {
            Id = id;
            Score = score;
        }

        public int CompareTo(Customer other)
        {
            // 先依据积分降序排序，然后依据 Id 升序排序  
            int scoreComparison = other.Score.CompareTo(this.Score);
            return scoreComparison != 0 ? scoreComparison : this.Id.CompareTo(other.Id);
        }
    }

    internal class SkipListNode
    {
        public Customer Customer;
        public SkipListNode[] Forward;

        public SkipListNode(Customer customer, int level)
        {
            Customer = customer;
            Forward = new SkipListNode[level];
        }
    }

    internal class SkipList
    {
        private readonly Random _random = new Random();
        public SkipListNode _header;  // 设置为 public  
        public int _currentLevel;      // 设置为 public  
        private readonly int _maxLevel;

        public SkipList(int maxLevel)
        {
            _maxLevel = maxLevel;
            _header = new SkipListNode(new Customer(0, 0), maxLevel);
            _currentLevel = 0;
        }

        public void Insert(Customer customer)
        {
            if (customer.Score <= 0) return;
            SkipListNode[] update = new SkipListNode[_maxLevel];
            SkipListNode node = _header;

            for (int i = _currentLevel - 1; i >= 0; i--)
            {
                while (node.Forward[i] != null && node.Forward[i].Customer.CompareTo(customer) < 0)
                {
                    node = node.Forward[i];
                }
                update[i] = node;
            }

            node = node.Forward[0];

            if (node == null || node.Customer.CompareTo(customer) != 0)
            {
                int newLevel = RandomLevel();
                if (newLevel > _currentLevel)
                {
                    for (int i = _currentLevel; i < newLevel; i++)
                    {
                        update[i] = _header;
                    }
                    _currentLevel = newLevel;
                }

                SkipListNode newNode = new SkipListNode(customer, newLevel);
                for (int i = 0; i < newLevel; i++)
                {
                    newNode.Forward[i] = update[i].Forward[i];
                    update[i].Forward[i] = newNode;
                }
            }
        }

        public void Delete(Customer customer)
        {
            SkipListNode[] update = new SkipListNode[_maxLevel];
            SkipListNode node = _header;

            for (int i = _currentLevel - 1; i >= 0; i--)
            {
                while (node.Forward[i] != null && node.Forward[i].Customer.CompareTo(customer) < 0)
                {
                    node = node.Forward[i];
                }
                update[i] = node;
            }

            node = node.Forward[0];
            if (node != null && node.Customer.CompareTo(customer) == 0)
            {
                for (int i = 0; i < _currentLevel; i++)
                {
                    if (update[i].Forward[i] != node) break;
                    update[i].Forward[i] = node.Forward[i];
                }

                while (_currentLevel > 0 && _header.Forward[_currentLevel - 1] == null)
                {
                    _currentLevel--;
                }

            }
        }

        private int RandomLevel()
        {
            int level = 1;
            while (_random.NextDouble() < 0.5 && level < _maxLevel)
            {
                level++;
            }
            return level;
        }
    }

    public record CustomerResponse(ulong CustomerId, decimal Score, int Rank);    

    public class SkipListRank
    {
        private readonly Dictionary<ulong, decimal> _customers;
        private readonly Dictionary<ulong, int> _ranks;
        private readonly SkipList _skipList;

        public SkipListRank(int maxLevel)
        {
            _customers = new();
            _ranks = new();
            _skipList = new SkipList(maxLevel);
        }

        public decimal UpdateScore(ulong customerId, decimal scoreChange)
        {
            _ranks.Clear();
            if (_customers.TryGetValue(customerId, out decimal score))
            {
                // 更新客户积分  
                decimal newScore = score + scoreChange;
                var customer = new Customer(customerId, newScore);
                // 在跳表中删除现有节点，然后重新插入新的分数  
                _skipList.Delete(customer);
                _skipList.Insert(customer);
                return newScore;
            }
            else
            {
                // 若客户不存在，则添加新客户  
                var customer = new Customer(customerId, scoreChange);
                _customers[customerId] = scoreChange;
                _skipList.Insert(customer);
                return customer.Score;
            }

        }

        public List<CustomerResponse> GetCustomersByRank(int start, int end)
        {
            List<CustomerResponse> result = new List<CustomerResponse>(end - start + 1);

            SkipListNode node = _skipList._header.Forward[0];
            int rank = 1;

            while (node != null && rank <= end)
            {
                _ranks[node.Customer.Id]=rank;
                if (rank >= start)
                {
                    result.Add(new CustomerResponse(node.Customer.Id, node.Customer.Score, rank));
                }
                node = node.Forward[0];
                rank++;
            }

            return result;
        }

        public List<CustomerResponse> GetCustomerNeighborhood(ulong customerId, int high = 0, int low = 0)
        {
            List<CustomerResponse> result = new List<CustomerResponse>(1 + low + high);
            if (_ranks.TryGetValue(customerId, out int rank))
            {
                result = GetCustomersByRank(rank - low, rank + high);
            }
            else
            {
                if (_customers.TryGetValue(customerId, out decimal score))
                {
                    Dictionary<int, ulong> rankIds = new(1 + low + high);

                    SkipListNode? node = _skipList._header.Forward[0];
                    rank = 1;
                    var customer = new Customer(customerId, score);
                    // 寻找当前客户的排名  
                    while (node != null)
                    {
                        _ranks[node.Customer.Id] = rank;
                        rankIds[rank] = node.Customer.Id;
                        if (node.Customer.CompareTo(customer) < 0)
                        {
                            rank++;
                        }
                        else if (node.Customer.CompareTo(customer) == 0 && node.Customer.Id == customerId)
                        {
                            break;
                        }
                        node = node.Forward[0];
                    }

                    // 找到高排名邻居  
                    for (int i = rank - high; i <= rank; i++)
                    {
                        var id = rankIds[i];
                        result.Add(new CustomerResponse(id, _customers[id], i));
                    }

                    // 找到低排名邻居  
                    for (int i = 0; i < low; i++)
                    {
                        node = node?.Forward[0];
                        if (node != null)
                        {
                            rank++;
                            _ranks[node.Customer.Id] = rank;
                            result.Add(new CustomerResponse(node.Customer.Id, node.Customer.Score, rank));
                        }
                    }
                }
            }
            return result;
        }
    }
}
