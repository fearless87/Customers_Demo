using Customers_Demo_Service.Extension;
using Customers_Demo_Service.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customers_Demo_Service.Data
{
    internal static class CustomerData
    {
        public static ConcurrentDictionary<long, decimal> CustomerDatas { get; set; } = new ConcurrentDictionary<long, decimal>();
        public static ConcurrentQueue<Customer> CustomerQueue { get; set; } = new ConcurrentQueue<Customer>();

        [Obsolete]
        public static List<Leaderboard> Leaderboards { get; set; } = new List<Leaderboard>();

        /// <summary>
        /// 排序的Customer集合【SortedSet红黑树实现：Add方法是O（lg n）、Contains方法也是O（lg n）】
        /// </summary>
        public static SortedSet<Customer> SortedCustomers { get; set; } = new SortedSet<Customer>(new CompareCustomer());

        #region SortedCustomers基于指定Score划分为上下两部分（暂时中间值Score由人为指定）,part1为高分值、part2为低分值
        public static SortedSet<Customer> SortedCustomers_Part1 { get; set; } = new SortedSet<Customer>(new CompareCustomer());
        public static SortedSet<Customer> SortedCustomers_Part2 { get; set; } = new SortedSet<Customer>(new CompareCustomer());
        public static decimal MiddleScore = 400;
        #endregion


    }
}
