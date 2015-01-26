using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Property4U.Models
{
    public class ControlDeskActiveME
    {
        // Agents in System - Current Login Agents
        public int AvaliableAgents { get; set; }
        // 5 Popular Properties
        public List<Property> Properties { get; set; }
        public int AvaliablePropertiesCount { get; set; }
        // All Active Biddings Count
        public int ActiveBiddingsCount { get; set; }
        // Accepted Visiting Requests Count - Current Week Latest 5 Accepted Requests
        public int RequestsAccepted { get; set; }
        // Pending Bids Count - Recent 5 Wining Bids
        public int BidsProcess { get; set; }
        public int BidsWin { get; set; }
   }
}