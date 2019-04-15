using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdoPart2.Models
{
    public class AllOrdersDetailsModel
    {
        public DateTime? OrderDate { get; private set; }
        public DateTime? RequiredDate { get; private set; }
        public DateTime? ShippedDate { get; private set; }
        public int ShipVia { get; set; }
        public string ShipName { get; set; }
        public string ShipAddress { get; set; }
        public string ShipCity { get; set; }
        public string ShipRegion { get; set; }
        public string ShipPostalCode { get; set; }
        public string ShipCountry { get; set; }
        public string ProductName { get; set; }

        public int OrderID { get; set; }
        public int ProductID { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public float Discount { get; set; }
    }
}
