﻿using Customers_Demo_Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customers_Demo_Service.Service
{
    public interface ICustomerService
    {
        Task<Decimal> UpdateScore(Customer customer);

        Task AddLeaderboards();

        Task<List<Leaderboard>> GetLeaderboardsByRank(int start, int end);
        Task<List<Leaderboard>> GetLeaderboardsByCustomerId(long customerid, int high, int low);
    }
}
