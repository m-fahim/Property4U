using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Property4U.Models
{
    public class BiddingsPropertyPhotoViewModel
    {
        public PagedList.IPagedList<BiddingsPhotoViewModel> BiddingsPhotoPaged { get; set; }
    }
}