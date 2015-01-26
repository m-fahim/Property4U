using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Property4U.Models
{
    public class DetailsPropertyPhotoViewModel
    {
        public DetailsAgentProfile AgentProperties { get; set; }
        public DetailsAgentProfile AgentBiddings { get; set; }
        public Property Property { get; set; }
        public string SubType { get; set; }
        public IEnumerable<Photo> Photos { get; set; }
        public IEnumerable<HomePropertyPhotoViewModel> RelatedProperties { get; set; }
        public PagedList.IPagedList<DetailsReviewRepliesViewModel> Reviews { get; set; }
        public int PropertyRating { get; set; }
    }
}