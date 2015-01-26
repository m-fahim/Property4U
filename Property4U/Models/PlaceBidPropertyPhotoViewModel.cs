using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Property4U.Models
{
    public class PlaceBidPropertyPhotoViewModel
    {
        public DetailsAgentProfile AgentProperties { get; set; }
        public DetailsAgentProfile AgentBiddings { get; set; }
        public Bidding Bidding { get; set; }
        public string SubType { get; set; }
        public Bid WinningBid { get; set; }
        public Photo Photo { get; set; }
        public IEnumerable<BiddingPhotoViewModel> RelatedBiddings { get; set; }
        public PagedList.IPagedList<Bid> Bids { get; set; }
    }
}