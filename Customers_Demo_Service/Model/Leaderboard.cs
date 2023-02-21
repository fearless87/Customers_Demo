using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Customers_Demo_Service.Model
{
    /// <summary>
    /// data model Leaderboard
    /// </summary>
    public class Leaderboard : Customer
    {
        [JsonPropertyOrder(100)]
        public int Rank { get; set; }
    }
}
