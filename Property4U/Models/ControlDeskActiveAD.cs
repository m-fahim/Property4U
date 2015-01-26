using IdentitySample.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Property4U.Models
{
    public class ControlDeskActiveAD
    {
        // Register/Members Users Count
        public List<IdentityUserRole> Users { get; set; }
        // 5 Popular Properties
        public List<Property> Properties { get; set; }
        // Pending Renewals Count - Inactive Renewals Payment($) Dues
        public int RenewalsCount { get; set; }
        public string RenewalsCost { get; set; }
        // Pending Orders Count - Disable Orders Payment($) Dues - Uploaded Orders Zip Data Size
        public int OrdersCount { get; set; }
        //public string OrdersPaymentDue { get; set; }
        // Active Ads Count
        public int AdsCount { get; set; }
        // Uploaded Photos, Orders Data Size
        public double UploadedDataSize { get; set; }
        // Server Information
        public ServerInfoAD ServerInfo { get; set; }
   }
}