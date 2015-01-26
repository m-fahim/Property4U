using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Property4U.Models
{
    public class DetailsReviewRepliesViewModel
    {
        public Review Review { get; set; }
        public List<Reply> Replies { get; set; }
    }
}