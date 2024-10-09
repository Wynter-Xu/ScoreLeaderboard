namespace ScoreLeaderboard.Models
{
    public class Customer : IComparable<Customer>
    {
        public long CustomerId { get; set; }
        public decimal Score { get; set; }

        public int CompareTo(Customer? other)
        {
            if (other == null) return 1;
            // Sort by score descending, then by CustomerId ascending  
            int scoreComparison = other.Score.CompareTo(this.Score);
            return scoreComparison != 0 ? scoreComparison : this.CustomerId.CompareTo(other.CustomerId);
            //int scoreComparison = this.Score.CompareTo(other.Score);
            //return scoreComparison != 0 ? scoreComparison : other.CustomerId.CompareTo(this.CustomerId);
        }

    }

    public record CustomerResponse(long CustomerId, decimal Score, int Rank);
}
