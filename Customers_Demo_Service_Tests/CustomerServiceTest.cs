using Customers_Demo_Service.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Customers_Demo_Service.Model;
using System.Diagnostics;
using static System.Formats.Asn1.AsnWriter;
using Xunit.Abstractions;
using Microsoft.VisualStudio.TestPlatform.Utilities;

namespace Customers_Demo_Service_Tests
{
    public class CustomerServiceTest
    {
        private readonly ICustomerService _customerServiceObsolete;
        private readonly ICustomerService _customerServiceList;
        private readonly ICustomerService _customerService;
        private readonly ICustomerService _customerServicePart;
        protected readonly ITestOutputHelper _output;

        private int _customerCount;

        public CustomerServiceTest(ITestOutputHelper output)
        {
            _customerServiceObsolete = new CustomerServiceObsolete();
            _customerServiceList = new CustomerServiceList();
            _customerService = new CustomerService();
            _customerServicePart = new CustomerServicePart();
            _output = output;

            _customerCount = 20000;
        }

        [Fact]
        public async void Can_UpsertScoreAsync()
        {
            _customerService.ClearData();

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            long customerId = 3333;
            decimal totalScore = 0;
            for (var i = 1; i < 10000; i++)
            {
                var score = await _customerService.UpsertScoreAsync(new Customer { CustomerID = customerId, Score = i });
                totalScore = totalScore + i;
                Assert.Equal(totalScore, score);
            }

            stopwatch.Stop();
            Console.WriteLine(stopwatch.ElapsedMilliseconds);

        }

        [Fact]
        public async void Can_GetLeaderboardsByRankAsync()
        {
            _customerService.ClearData();

            List<long> cunstomers = new List<long> { 111, 222, 333, 444, 555, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            List<int> loopNums = new List<int> { 1000, 3000, 2000, 10000, 500, 100, 200, 300, 400, 500, 600, 700, 800, 900, 1000 };
            List<int> loopScores = new List<int> { 1, 1, 1, 1, 1, 1, -1, -3, 1, 0, 3, -2, 1, 2, -10 };

            for (var i = 0; i < cunstomers.Count; i++)
            {
                for (var j = 0; j < loopNums[i]; j++)
                {
                    _customerService.UpsertScoreAsync(new Customer { CustomerID = cunstomers[i], Score = loopScores[i] });
                }
                _customerService.AddLeaderboards();
            }

            var leaderboards = await _customerService.GetLeaderboardsByRankAsync(1, 2);
            Assert.Equal(2, leaderboards.Count);
            Assert.Equal(444, leaderboards[0].CustomerID);
            Assert.Equal(222, leaderboards[1].CustomerID);
            Assert.Equal(10000, leaderboards[0].Score);
            Assert.Equal(3000, leaderboards[1].Score);

            var allLeaderboards = await _customerService.GetLeaderboardsByRankAsync(1, cunstomers.Count);
            Assert.Equal(loopScores.Where(predicate => predicate > 0).Count(), allLeaderboards.Count);
        }

        [Fact]
        public async void Can_GetLeaderboardsByRank_ContainDel()
        {
            _customerService.ClearData();

            List<long> cunstomers = new List<long> { 111, 222, 333, 444, 555, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            List<int> loopNums = new List<int> { 1000, 3000, 2000, 10000, 500, 100, 200, 300, 400, 500, 600, 700, 800, 900, 1000 };
            List<int> loopScores1 = new List<int> { 1, 1, 1, 1, 1, 1, -1, -3, 1, 0, 3, -2, 1, 2, -10 };
            List<int> loopScores2 = new List<int> { 1, 1, 1, -2, 1, 1, -1, 3, 1, 0, 3, 2, 1, 2, -10 };

            for (var i = 0; i < cunstomers.Count; i++)
            {
                for (var j = 0; j < loopNums[i]; j++)
                {
                    _customerService.UpsertScoreAsync(new Customer { CustomerID = cunstomers[i], Score = loopScores1[i] });
                    _customerService.UpsertScoreAsync(new Customer { CustomerID = cunstomers[i], Score = loopScores2[i] });
                }
                _customerService.AddLeaderboards();
            }

            var leaderboards = await _customerService.GetLeaderboardsByRankAsync(1, 2);
            Assert.Equal(2, leaderboards.Count);
            Assert.Equal(222, leaderboards[0].CustomerID);
            Assert.Equal(333, leaderboards[1].CustomerID);
            Assert.Equal(6000, leaderboards[0].Score);
            Assert.Equal(4000, leaderboards[1].Score);


            var allLeaderboards = await _customerService.GetLeaderboardsByRankAsync(1, cunstomers.Count);
            int leaderboardsNum = 0;
            for (var i = 0; i < loopScores1.Count; i++)
            {
                if (loopScores1[i] + loopScores2[i] > 0)
                {
                    leaderboardsNum++;
                }
            }

            var allCustomers = await _customerService.AllCustomersAsync();
            Assert.Equal(leaderboardsNum, allLeaderboards.Count);
        }

        [Fact]
        public async void Valid_RetrivedTime_Obsolete()
        {
            _customerServiceObsolete.ClearData();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            #region 构建Customer
            int[] scoreArr = new int[100];
            for (var i = 0; i < 100; i++)
            {
                scoreArr[i] = new Random().Next(-2000, 2000);
            }
            // 构建1万个Customer
            for (var i = 0; i < _customerCount; i++)
            {
                // 每个Customer随机Update 10次积分
                for (var j = 0; j < 10; j++)
                {
                    await _customerServiceObsolete.UpsertScoreAsync(new Customer { CustomerID = i + 1, Score = scoreArr[new Random().Next(0, 99)] });
                }
            }
            _customerServiceObsolete.AddLeaderboards();
            #endregion
            #region 随机获取100个Customer，且up、down均等于10
            for (var i = 0; i < 100; i++)
            {
                await _customerServiceObsolete.GetLeaderboardsByCustomerIdAsync(new Random().Next(1, _customerCount), 10, 10);
            }
            #endregion

            stopwatch.Stop();
            _output.WriteLine((stopwatch.ElapsedMilliseconds / 1000).ToString());
            Assert.True(true);
        }

        [Fact]
        public async void Valid_RetrivedTime_List()
        {
            _customerServiceList.ClearData();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            #region 构建Customer
            int[] scoreArr = new int[100];
            for (var i = 0; i < 100; i++)
            {
                scoreArr[i] = new Random().Next(-2000, 2000);
            }
            // 构建1万个Customer
            for (var i = 0; i < _customerCount; i++)
            {
                // 每个Customer随机Update 10次积分
                for (var j = 0; j < 10; j++)
                {
                    await _customerServiceList.UpsertScoreAsync(new Customer { CustomerID = i + 1, Score = scoreArr[new Random().Next(0, 99)] });
                }
            }
            _customerServiceList.AddLeaderboards();
            #endregion
            #region 随机获取100个Customer，且up、down均等于10
            for (var i = 0; i < 100; i++)
            {
                await _customerServiceList.GetLeaderboardsByCustomerIdAsync(new Random().Next(1, _customerCount), 10, 10);
            }
            #endregion

            stopwatch.Stop();
            _output.WriteLine((stopwatch.ElapsedMilliseconds / 1000).ToString());
            Assert.True(true);
        }

        [Fact]
        public async void Valid_RetrivedTime()
        {
            _customerService.ClearData();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            #region 构建Customer
            int[] scoreArr = new int[100];
            for (var i = 0; i < 100; i++)
            {
                scoreArr[i] = new Random().Next(-2000, 2000);
            }
            // 构建1万个Customer
            for (var i = 0; i < _customerCount; i++)
            {
                // 每个Customer随机Update 10次积分
                for (var j = 0; j < 10; j++)
                {
                    await _customerService.UpsertScoreAsync(new Customer { CustomerID = i + 1, Score = scoreArr[new Random().Next(0, 99)] });
                }
            }
            _customerService.AddLeaderboards();
            #endregion
            #region 随机获取100个Customer，且up、down均等于10
            for (var i = 0; i < 100; i++)
            {
                await _customerService.GetLeaderboardsByCustomerIdAsync(new Random().Next(1, _customerCount), 10, 10);
            }
            #endregion

            stopwatch.Stop();
            _output.WriteLine((stopwatch.ElapsedMilliseconds / 1000).ToString());
            Assert.True(true);
        }

        [Fact]
        public async void Valid_RetrivedTime_Part()
        {
            _customerServicePart.ClearData();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            #region 构建Customer
            int[] scoreArr = new int[100];
            for (var i = 0; i < 100; i++)
            {
                scoreArr[i] = new Random().Next(-2000, 2000);
            }
            // 构建1万个Customer
            for (var i = 0; i < _customerCount; i++)
            {
                // 每个Customer随机Update 10次积分
                for (var j = 0; j < 10; j++)
                {
                    await _customerServicePart.UpsertScoreAsync(new Customer { CustomerID = i + 1, Score = scoreArr[new Random().Next(0, 99)] });
                }
            }
            _customerServicePart.AddLeaderboards();
            #endregion
            #region 随机获取100个Customer，且up、down均等于10
            for (var i = 0; i < 100; i++)
            {
                await _customerServicePart.GetLeaderboardsByCustomerIdAsync(new Random().Next(1, _customerCount), 10, 10);
            }
            #endregion

            stopwatch.Stop();
            _output.WriteLine((stopwatch.ElapsedMilliseconds / 1000).ToString());
            Assert.True(true);
        }

    }
}
