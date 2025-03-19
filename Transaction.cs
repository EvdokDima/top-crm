using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM
{
    public class Transaction
    {
        public int TranzactionId { get; set; }
        public int ClientId { get; set; }
        public DateTime TranzactionTime { get; set; }
        public decimal Cost { get; set; }
        public string ClientFullName { get; set; }
    }
}
