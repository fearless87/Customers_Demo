using Customers_Demo_Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customers_Demo_Service.Service
{
    public class CustomerService : ICustomerService
    {
        public Task<decimal> UpdateScore(Customer customer)
        {
            var updatedScore = customer.UpdateScore();
            return Task.FromResult(updatedScore);
        }

    }
}
