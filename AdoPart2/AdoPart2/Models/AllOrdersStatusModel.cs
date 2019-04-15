using AdoPart2.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdoPart2.Models
{
    public class AllOrdersStatusModel : AllOrdersDetailsModel
    {
        private StatusOrder statusOrder;

        public StatusOrder StatusOrder
        {
            private set
            {
                if (this.OrderDate == null)
                {
                    statusOrder = StatusOrder.New;
                }
                if (this.ShippedDate == null && this.OrderDate != null)
                {
                    statusOrder = StatusOrder.InProgress;
                }
                if (this.ShippedDate != null)
                {
                    statusOrder = StatusOrder.Delivered;
                }
            }
            get
            {
                return statusOrder;
            }
        }
    }
}
