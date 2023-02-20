using Customers_Demo_Service.Service;
using Microsoft.AspNetCore.Mvc;
using Customers_Demo_Service.Model;

namespace Customers_Demo_Api.Controllers
{
    public class CustomerController : BaseController
    {
        private readonly ICustomerService _customerService;
        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [Route("/customer/{customerid:min(1)}/score/{score:range(-1000,1000)}")]
        [HttpPost]
        public async Task<decimal> UpdateScore(long customerid, decimal score)
        {
            return await _customerService.UpdateScore(new Customer { CustomerID = customerid, Score = score });
        }
    }
}
