using Customers_Demo_Service.Data;
using Customers_Demo_Service.Model;
using System;
using System.Collections.Concurrent;

namespace Customers_Demo_Service.Service
{
    /// <summary>
    /// 废弃的（初始低性能版本）
    /// </summary>
    public class CustomerServiceObsolete : BaseService, ICustomerService
    {
        public async ValueTask<decimal> UpsertScoreAsync(Customer customer)
        {
            var updatedScore = customer.Update<decimal>();
            return await Task.FromResult(updatedScore);
        }

        public void AddLeaderboards()
        {
            Customer? curCustomer;
            while (CustomerData.CustomerQueue.TryDequeue(out curCustomer))
            {
                DoAddLeaderboard(curCustomer);
            }
        }
        private void DoAddLeaderboard(Customer customer)
        {
            decimal curItemScore;
            CustomerData.CustomerDatas.TryGetValue(customer.CustomerID, out curItemScore);
            var curItem = CustomerData.Leaderboards.FirstOrDefault(predicate => predicate.CustomerID == customer.CustomerID);
            if (curItem != default)
            {
                if (curItemScore < 1)
                {
                    CustomerData.Leaderboards.Remove(curItem);
                }
                else
                {
                    curItem.Score = curItemScore;
                }
            }
            else
            {
                if (curItemScore > 0)
                {
                    CustomerData.Leaderboards.Add(new Leaderboard
                    {
                        CustomerID = customer.CustomerID,
                        Score = curItemScore,
                        Rank = 0
                    });
                }
            }
            CustomerData.Leaderboards.Sort((x, y) => { return x.Score - y.Score > 0 ? -1 : (x.Score - y.Score == 0 ? (x.CustomerID - y.CustomerID > 0 ? 1 : (x.CustomerID - y.CustomerID == 0 ? 0 : -1)) : 1); });
            for (var i = 0; i < CustomerData.Leaderboards.Count; i++)
            {
                CustomerData.Leaderboards[i].Rank = i + 1;
            }
        }

        public async Task<List<Leaderboard>> GetLeaderboardsByRankAsync(int start, int end)
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

        public async Task<List<Leaderboard>> GetLeaderboardsByCustomerIdAsync(long customerid, int high, int low)
        {
            var leaderboardArr = CustomerData.Leaderboards.ToArray();
            int customerIndex = -1;
            for (var i = 0; i < leaderboardArr.Length; i++)
            {
                if (leaderboardArr[i].CustomerID == customerid)
                {
                    customerIndex = i;
                }
            }
            if (customerIndex >= 0)
            {
                var curRand = leaderboardArr[customerIndex].Rank;
                return await GetLeaderboardsByRankAsync(curRand - high, curRand + low);
            }
            else
            {
                return new List<Leaderboard>();
            }
        }

        public async Task<ConcurrentDictionary<long, decimal>> AllCustomersAsync()
        {
            return CustomerData.CustomerDatas;
        }

        public void ClearData()
        {
            CustomerData.CustomerDatas = new ConcurrentDictionary<long, decimal>();
            CustomerData.Leaderboards = new List<Leaderboard>();
            CustomerData.CustomerQueue.Clear();
        }

    }
}