using Customers_Demo_Service.Data;
using Customers_Demo_Service.Extension;
using Customers_Demo_Service.Model;
using System.Collections.Concurrent;

namespace Customers_Demo_Service.Service
{
    public class CustomerServicePart : BaseService, ICustomerService
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

            // part1
            if (curItemScore > CustomerData.MiddleScore)
            {
                DoAddLeaderboardPart(CustomerData.SortedCustomers_Part1, CustomerData.SortedCustomers_Part2, customer, curItemScore);
            }
            // part2
            else
            {
                DoAddLeaderboardPart(CustomerData.SortedCustomers_Part2, CustomerData.SortedCustomers_Part1, customer, curItemScore);
            }

        }
        private void DoAddLeaderboardPart(SortedSet<Customer> master, SortedSet<Customer> slave, Customer customer, decimal curItemScore)
        {
            var curCustomerItem = master.SingleOrDefault(predicate => predicate.CustomerID == customer.CustomerID);
            if (curCustomerItem != null)
            {
                if (curItemScore < 1)
                {
                    master.Remove(curCustomerItem);
                }
                else
                {
                    curCustomerItem.Score = curItemScore;
                }
            }
            else
            {
                master.Add(new Customer { CustomerID = customer.CustomerID, Score = curItemScore });
            }

            var curCustomerItemOther = slave.SingleOrDefault(predicate => predicate.CustomerID == customer.CustomerID);
            if (curCustomerItemOther != null)
            {
                slave.Remove(curCustomerItemOther);
            }
        }

        public async Task<List<Leaderboard>> GetLeaderboardsByRankAsync(int start, int end)
        {
            var part1Count = CustomerData.SortedCustomers_Part1.Count;
            IEnumerable<Customer>? customers = null;
            if (end <= part1Count)
            {
                customers = CustomerData.SortedCustomers_Part1.Skip(start - 1).Take(end - start + 1);
            }
            else if (end > part1Count && start <= part1Count)
            {
                customers = CustomerData.SortedCustomers_Part1.Skip(start - 1).Take(end - start + 1).Concat(CustomerData.SortedCustomers_Part2.Skip(0).Take(end - part1Count));
            }
            else
            {
                customers = CustomerData.SortedCustomers_Part2.Skip(start - part1Count).Take(end - start + 1);
            }
            List<Leaderboard> result = new List<Leaderboard>();
            for (var i = 0; i < customers.Count(); i++)
            {
                var item = customers.ElementAt(i);
                result.Add(new Leaderboard
                {
                    CustomerID = item.CustomerID,
                    Score = item.Score,
                    Rank = start + i
                });
            }

            return result;
        }

        public async Task<List<Leaderboard>> GetLeaderboardsByCustomerIdAsync(long customerid, int high, int low)
        {
            decimal curItemScore;
            CustomerData.CustomerDatas.TryGetValue(customerid, out curItemScore);

            int curIndex = -1;
            if (curItemScore > CustomerData.MiddleScore)
            {
                curIndex = CustomerData.SortedCustomers_Part1.ToList().FindIndex(predicate => predicate.CustomerID == customerid);
            }
            else
            {
                curIndex = CustomerData.SortedCustomers_Part2.ToList().FindIndex(predicate => predicate.CustomerID == customerid) + CustomerData.SortedCustomers_Part1.Count;
            }
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
            CustomerData.SortedCustomers_Part1 = new SortedSet<Customer>(new CompareCustomer());
            CustomerData.SortedCustomers_Part2 = new SortedSet<Customer>(new CompareCustomer());
            CustomerData.CustomerQueue.Clear();
        }

    }
}
