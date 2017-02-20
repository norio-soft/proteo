using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orchestrator.WebUI.Job
{
    public class MergableRunOrder
    {
        public int OrderID { get; set; }
        public string Customer { get; set; }
        public string BusinessType { get; set; }
        public string CustomerOrderNumber { get; set; }
        public string DeliveryOrderNumber { get; set; }
        public string DeliveringResource { get; set; }
        public bool CanAddThisOrder { get; set; }
        public string Status { get; set; }
    }
}