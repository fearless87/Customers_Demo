using Customers_Demo_Service.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customers_Demo_Service.Model
{
    /// <summary>
    /// data model Customer
    /// </summary>
    public class Customer
    {
        public long CustomerID { get; set; }
        public decimal Score { get; set; } = 0;

        public decimal UpdateScore()
        {
            if (CustomerData.CustomerDatas.ContainsKey(this.CustomerID))
            {
                CustomerData.CustomerDatas[this.CustomerID] += this.Score;
            }
            else
            {
                CustomerData.CustomerDatas.TryAdd(this.CustomerID, this.Score);
            }

            return CustomerData.CustomerDatas[this.CustomerID];
        }
    }
}
