using Customers_Demo_Service.Data;
using Customers_Demo_Service.Model;
using System;
using System.Collections.Concurrent;

namespace Customers_Demo_Service.Service
{
    /// <summary>
    /// 采用List，写法干净
    /// </summary>
    public class CustomerServiceList : BaseService, ICustomerService
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
                        Score = curItemScore
                    });
                }
            }
            CustomerData.Leaderboards.Sort((x, y) => { return x.Score - y.Score > 0 ? -1 : (x.Score - y.Score == 0 ? (x.CustomerID - y.CustomerID > 0 ? 1 : (x.CustomerID - y.CustomerID == 0 ? 0 : -1)) : 1); });
        }

        public async Task<List<Leaderboard>> GetLeaderboardsByRankAsync(int start, int end)
        {
            List<Leaderboard> result = new List<Leaderboard>();
            var customers = CustomerData.Leaderboards.Skip(start - 1).Take(end - start + 1);
            for (var i = 0; i < customers.Count(); i++)
            {
                customers.ElementAt(i).Rank = start + i;
            }

            return result;
        }

        public async Task<List<Leaderboard>> GetLeaderboardsByCustomerIdAsync(long customerid, int high, int low)
        {
            int curIndex = BinarySearch(customerid);
            if (curIndex == -1)
            {
                return new List<Leaderboard>();
            }
            return await GetLeaderboardsByRankAsync(curIndex + 1 - high, curIndex + 1 + low);
        }
        private int BinarySearch(long customerid)
        {
            int low = 0, high = CustomerData.Leaderboards.Count - 1;
            // 长度为2时特殊处理
            if (high == 2)
            {
                return CustomerData.Leaderboards.FindIndex(match => match.CustomerID == customerid);
            }

            while (low <= high)
            {
                int mid = (low + high) / 2;
                long? curCustomerId = CustomerData.Leaderboards.GetRange(mid, 1).FirstOrDefault()?.CustomerID;
                if (customerid == curCustomerId)
                {
                    return mid;
                }
                else if (customerid > curCustomerId)
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