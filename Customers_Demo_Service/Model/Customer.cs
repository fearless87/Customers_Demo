using Customers_Demo_Service.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Customers_Demo_Service.Model
{
    /// <summary>
    /// data model Customer
    /// </summary>
    public class Customer : BaseModel
    {
        [JsonPropertyName("Customer ID")]
        public long CustomerID { get; set; }
        public decimal Score { get; set; } = 0;

        public sealed override T Update<T>()
        {
            if (CustomerData.CustomerDatas.ContainsKey(this.CustomerID))
            {
                CustomerData.CustomerDatas[this.CustomerID] += this.Score;
            }
            else
            {
                CustomerData.CustomerDatas.TryAdd(this.CustomerID, this.Score);
            }
            // enqueue
            CustomerData.CustomerQueue.Enqueue(this);

            return (T)Convert.ChangeType(CustomerData.CustomerDatas[this.CustomerID], typeof(T));
        }

        public override bool Equals(object? obj)
        {
            if (obj is not Customer customer) return false;
            return customer.CustomerID == CustomerID;
        }

        public override int GetHashCode()
        {
            return CustomerID.GetHashCode();
        }
    }
}
