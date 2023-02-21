using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customers_Demo_Service.Model
{
    /// <summary>
    /// data model Leaderboard
    /// </summary>
    public class Leaderboard : Customer
    {
        public int Rank { get; set; }
    }
}
