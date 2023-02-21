using Customers_Demo_Service.Data;
using Customers_Demo_Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customers_Demo_Service.Service
{
    public class CustomerService : BaseService, ICustomerService
    {
        public async Task<decimal> UpdateScore(Customer customer)
        {
            var updatedScore = customer.Update<decimal>();
            return await Task.FromResult(updatedScore);
        }

        public async Task AddLeaderboards()
        {
            Customer? curCustomer;
            while (CustomerData.CustomerQueue.TryDequeue(out curCustomer))
            {
                DoAddLeaderboard(curCustomer);
            }

            await Task.CompletedTask;
        }
        private void DoAddLeaderboard(Customer customer)
        {
            var curItem = CustomerData.Leaderboards.FirstOrDefault(predicate => predicate.CustomerID == customer.CustomerID);
            if (curItem != default)
            {
                curItem.Score += customer.Score;
            }
            else
            {
                CustomerData.Leaderboards.Add(new Leaderboard
                {
                    CustomerID = customer.CustomerID,
                    Score = customer.Score,
                    Rank = 0
                });
            }
            CustomerData.Leaderboards.Sort((x, y) => { return x.Score - y.Score > 0 ? -1 : (x.Score - y.Score == 0 ? (x.CustomerID - y.CustomerID > 0 ? 1 : (x.CustomerID - y.CustomerID == 0 ? 0 : -1)) : 1); });
            for (var i = 0; i < CustomerData.Leaderboards.Count; i++)
            {
                CustomerData.Leaderboards[i].Rank = i + 1;
            }
        }

        public async Task<List<Leaderboard>> GetLeaderboardsByRank(int start, int end)
        {
            List<Leaderboard> result = new List<Leaderboard>();
            var leaderboardArr = CustomerData.Leaderboards.ToArray();
            while (start <= end && start <= leaderboardArr.Length)
            {
                result.Add(leaderboardArr[start - 1]);
                start++;
            }
            return result;
        }

        public async Task<List<Leaderboard>> GetLeaderboardsByCustomerId(long customerid, int high, int low)
        {
            var leaderboardArr = CustomerData.Leaderboards.ToArray();
            int customerIndex = BinarySearch(leaderboardArr, customerid);
            if (customerIndex >= 0)
            {
                var curRand = leaderboardArr[customerIndex].Rank;
                return await GetLeaderboardsByRank(curRand - high, curRand + low);
            }
            else
            {
                return new List<Leaderboard>();
            }
        }
        private int BinarySearch(Leaderboard[] leaderboardArr, long customerid)
        {
            int low = 0, high = leaderboardArr.Length - 1;
            while (low <= high)
            {
                int mid = (low + high) / 2;
                if (customerid == leaderboardArr[mid].CustomerID)
                {
                    return mid;
                }
                else if (customerid > leaderboardArr[mid].CustomerID)
                {
                    low = mid + 1;
                }
                else
                {
                    high = mid - 1;
                }
            }
            return -1;
        }

    }
}
