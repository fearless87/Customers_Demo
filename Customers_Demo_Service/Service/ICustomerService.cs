using Customers_Demo_Service.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customers_Demo_Service.Service
{
    public interface ICustomerService
    {
        Task<Decimal> UpsertScoreAsync(Customer customer);

        void AddLeaderboards();

        Task<List<Leaderboard>> GetLeaderboardsByRankAsync(int start, int end);

        Task<List<Leaderboard>> GetLeaderboardsByCustomerIdAsync(long customerid, int high, int low);

        Task<ConcurrentDictionary<long, decimal>> AllCustomersAsync();

        void ClearData();
    }
}
