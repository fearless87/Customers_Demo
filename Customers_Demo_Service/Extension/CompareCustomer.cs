using Customers_Demo_Service.Model;

namespace Customers_Demo_Service.Extension
{
    public class CompareCustomer : IComparer<Customer>
    {
        public int Compare(Customer? x, Customer? y)
        {
            return x.Score - y.Score > 0 ? -1 : (x.Score - y.Score == 0 ? (x.CustomerID - y.CustomerID > 0 ? 1 : (x.CustomerID - y.CustomerID == 0 ? 0 : -1)) : 1);
        }
    }
}
