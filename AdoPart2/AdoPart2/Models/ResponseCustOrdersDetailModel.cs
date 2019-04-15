using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdoPart2.Models
{
    public class ResponseCustOrdersDetailModel
    {
        public string ProductName { get; set; }
        public string Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public int ExtendedPrice { get; set; }
        public int Discount { get; set; }
    }
}
