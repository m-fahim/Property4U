using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Property4U.Models
{
    public class ControlDeskActiveAG
    {
        // 5 Popular Properties
        public List<Property> Properties { get; set; }
        public string PropertiesYearlyProfit { get; set; }
        // Active Biddings Count - AgentID
        public int Biddings { get; set; }
        // Pending Orders Count- AgentID
        public int PendingOrders { get; set; }
        public int RejectedOrders { get; set; }
        // Active Ads Count- AgentID
        public int ActiveAds { get; set; }
        // Agree Responses - Meetings - AgentID
        public int ResponsesAgree { get; set; }
        // Uploaded Photos, Orders Data Size
        public double AgentUploadedDataSize { get; set; }
   }
}