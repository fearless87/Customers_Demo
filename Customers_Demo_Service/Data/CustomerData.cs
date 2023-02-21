﻿using Customers_Demo_Service.Model;
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
        public static ConcurrentQueue<Customer> CustomerQueue = new ConcurrentQueue<Customer>();
        public static List<Leaderboard> Leaderboards { get; set; } = new List<Leaderboard>();
    }
}
