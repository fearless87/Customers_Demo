using Customers_Demo_Service.Service;
using Microsoft.AspNetCore.Mvc;
using Customers_Demo_Service.Model;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.ComponentModel.DataAnnotations;

namespace Customers_Demo_Api.Controllers
{
    public class CustomerController : BaseController
    {
        private readonly ICustomerService _customerService;
        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [Route("/customer/{customerid:min(1)}/score/{score:range(-1000,1000)}")]
        [HttpPost]
        public async Task<decimal> UpdateScore(long customerid, decimal score)
        {
            return await _customerService.UpdateScore(new Customer { CustomerID = customerid, Score = score });
        }

        [Route("/leaderboard")]
        [HttpGet]
        public async Task<List<Leaderboard>> GetLeaderboardsByRank([Required] int start, [Required] int end)
        {
            return await _customerService.GetLeaderboardsByRank(start, end);
        }

        [Route("/leaderboard/{customerid}")]
        [HttpGet]
        public async Task<List<Leaderboard>> GetLeaderboardsByRank(long customerid, [Required] int high = 0, [Required] int low = 0)
        {
            if (high < 0) { high = 0; }
            if (low < 0) { low = 0; }
            return await _customerService.GetLeaderboardsByCustomerId(customerid, high, low);
        }
    }
}
