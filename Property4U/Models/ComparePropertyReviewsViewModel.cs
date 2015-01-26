using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Property4U.Models
{
    public class ComparePropertyReviewsViewModel
    {
        public Property Property { get; set; }
        public Photo Photo { get; set; }
        public string SubType { get; set; }
        public int PropertyRating { get; set; }
        public int ReviewsCount { get; set; }
    }
}