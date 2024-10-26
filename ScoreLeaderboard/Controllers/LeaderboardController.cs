using Microsoft.AspNetCore.Mvc;
using ScoreLeaderboard.Models;
using ScoreLeaderboard.Services;

namespace ScoreLeaderboard.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LeaderboardController : ControllerBase
    {
        private readonly ILeaderboardService _leaderboardService;

        public LeaderboardController(ILeaderboardService leaderboardService)
        {
            _leaderboardService = leaderboardService;
        }

        [HttpPost("customer/{customerId}/score/{score}")]
        public ActionResult<decimal> UpdateScore(ulong customerId, decimal score)
        {
            var updatedScore = _leaderboardService.UpdateScore(customerId, score);
            return Ok(updatedScore);
        }

        [HttpGet("leaderboard")]
        public ActionResult<List<CustomerResponse>> GetCustomersByRank([FromQuery] int start, [FromQuery] int end)
        {
            var customers = _leaderboardService.GetCustomersByRank(start, end);
            return Ok(customers);
        }

        [HttpGet("leaderboard/{customerId}")]
        public ActionResult<List<CustomerResponse>> GetCustomerNeighborhood(ulong customerId, [FromQuery] int high = 0, [FromQuery] int low = 0)
        {
            var neighbors = _leaderboardService.GetCustomerNeighborhood(customerId, high, low);
            return Ok(neighbors);
        }
    }
}