using Property4U.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Property4U.ViewModels
{
    public class BiddingIndexData
    {
        public IEnumerable<Property> Properties { get; set; }
        public IEnumerable<Bidding> Biddings { get; set; }
        public IEnumerable<Bid> Bids { get; set; }

    }
}