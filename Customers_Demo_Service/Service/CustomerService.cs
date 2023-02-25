using Customers_Demo_Service.Data;
using Customers_Demo_Service.Extension;
using Customers_Demo_Service.Model;
using System.Collections.Concurrent;

namespace Customers_Demo_Service.Service
{
    public class CustomerService : BaseService, ICustomerService
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

            var curCustomerItem = CustomerData.SortedCustomers.SingleOrDefault(predicate => predicate.CustomerID == customer.CustomerID);
            if (curCustomerItem != null && CustomerData.SortedCustomers.Contains(curCustomerItem))
            {
                if (curItemScore < 1)
                {
                    CustomerData.SortedCustomers.Remove(curCustomerItem);
                }
                else
                {
                    curCustomerItem.Score = curItemScore;
                }
            }
            else
            {
                CustomerData.SortedCustomers.Add(new Customer { CustomerID = customer.CustomerID, Score = curItemScore });
            }
        }

        public async Task<List<Leaderboard>> GetLeaderboardsByRankAsync(int start, int end)
        {
            var customers = CustomerData.SortedCustomers.Skip(start - 1).Take(end);
            List<Leaderboard> result = new List<Leaderboard>();
            for (var i = 0; i < customers.Count(); i++)
            {
                var item = customers.ElementAt(i);
                result.Add(new Leaderboard
                {
                    CustomerID = item.CustomerID,
                    Score = item.Score,
                    Rank = i + 1
                });
            }

            return result;
        }

        public async Task<List<Leaderboard>> GetLeaderboardsByCustomerIdAsync(long customerid, int high, int low)
        {
            var curIndex = CustomerData.SortedCustomers.ToList().FindIndex(predicate => predicate.CustomerID == customerid);
            if (curIndex == -1)
            {
                return new List<Leaderboard>();
            }
            return await GetLeaderboardsByRankAsync(curIndex + 1 - high, curIndex + 1 + low);
        }

        public async Task<ConcurrentDictionary<long, decimal>> AllCustomersAsync()
        {
            return CustomerData.CustomerDatas;
        }

        public void ClearData()
        {
            CustomerData.CustomerDatas = new ConcurrentDictionary<long, decimal>();
            CustomerData.SortedCustomers = new SortedSet<Customer>(new CompareCustomer());
            CustomerData.CustomerQueue.Clear();
        }

    }
}
