using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customers_Demo_Service.Model
{
    public class BaseModel
    {
        public virtual T Update<T>()
        {
            return default;
        }
    }
}
