using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Property4U.Models
{
    public class SearchBiddingsPropertyPhViewModel
    {
        public PagedList.IPagedList<BiddingsPhotoViewModel> BiddingsPhotoPaged { get; set; }
        public PagedList.IPagedList<HomePropertyPhotoViewModel> PropertyPhotoPaged { get; set; }
    }
}