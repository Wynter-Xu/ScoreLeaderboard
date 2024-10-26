using ScoreLeaderboard.Models;

namespace ScoreLeaderboard.Test
{
    public class SkipListRankTests
    {
        private SkipListRank _skipListRank;

        [SetUp]
        public void SetUp()
        {
            // ÿ������ǰ��ʼ��  
            _skipListRank = new SkipListRank(8);
        }

        [Test]
        public void TestUpdateScore()
        {
            var newScore1 = _skipListRank.UpdateScore(1, 100);
            var newScore2 = _skipListRank.UpdateScore(2, 150);
            var newScore3 = _skipListRank.UpdateScore(3, 120);
            var newScore4 = _skipListRank.UpdateScore(2, 30); // �ٴθ���  

            Assert.AreEqual(100, newScore1);
            Assert.AreEqual(150, newScore2);
            Assert.AreEqual(120, newScore3);
            Assert.AreEqual(180, newScore4); // IDΪ2�Ŀͻ����º��»���  
        }

        [Test]
        public void TestGetCustomersByRank()
        {
            _skipListRank.UpdateScore(1, 100);
            _skipListRank.UpdateScore(2, 150);
            _skipListRank.UpdateScore(3, 120);
            _skipListRank.UpdateScore(4, 190);

            var customers = _skipListRank.GetCustomersByRank(1, 3);

            Assert.AreEqual(3, customers.Count);
            Assert.AreEqual(1, customers[0].Rank);
            Assert.AreEqual(2, customers[1].Rank);
            Assert.AreEqual(3, customers[2].Rank);
        }

        [Test]
        public void TestGetCustomerNeighborhood()
        {
            _skipListRank.UpdateScore(1, 100);
            _skipListRank.UpdateScore(2, 150);
            _skipListRank.UpdateScore(4, 190);
            _skipListRank.UpdateScore(3, 120);
            _skipListRank.UpdateScore(5, 20);
            _skipListRank.UpdateScore(6, 90);
            _skipListRank.UpdateScore(1, 110);  
            _skipListRank.UpdateScore(7, 110);
            _skipListRank.UpdateScore(8, 110);
            _skipListRank.UpdateScore(9, 10);
            _skipListRank.UpdateScore(10, 100);
            var allRanks = _skipListRank.GetCustomersByRank(1, 10);
            int high = 2, low = 3;
            var neighborhood = _skipListRank.GetCustomerNeighborhood(3, high, low);

            var newScore = _skipListRank.UpdateScore(3, -15);
            Assert.AreEqual(newScore, 120 - 15);

            allRanks = _skipListRank.GetCustomersByRank(1, 10);
            neighborhood = _skipListRank.GetCustomerNeighborhood(3, high, low);

            Assert.AreEqual(high + low + 1, neighborhood.Count);
            //Assert.IsTrue(neighborhood.Exists(c => c.CustomerId == 3 && c.Rank == 3));
            //Assert.IsTrue(neighborhood.Exists(c => c.CustomerId == 2 && c.Rank==2)); // �������ھ�  
            //Assert.IsTrue(neighborhood.Exists(c => c.CustomerId == 1 && c.Rank==4)); // �������ھ�  
        }
    }
}