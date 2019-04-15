using AdoPart2.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdoPart2
{
    class Program
    {
        static void Main(string[] args)
        {
            var dbMannager = new DBManager("DBCS");
            //var result = dbMannager.GetOrderDetails(10248);
            var resultAllOrders = dbMannager.GetOrders();
            var t = dbMannager.GetCustOrdersDetail(10248);

        }
    }
}
