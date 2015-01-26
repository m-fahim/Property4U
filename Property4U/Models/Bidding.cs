using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Property4U.Models
{
    public class Bidding
    {
        public int ID { get; set; }

        [Required]
        public int PropertyID { get; set; }

        [Required]
        [StringLength(90, ErrorMessage = "Bidding Title cannot be longer than 90 characters.")]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Display(Name = "Minimum")]
        public double MinExp { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Display(Name = "Maximum")]
        public double MaxExp { get; set; }

        [Display(Name = "Winning Bid")]
        public int? WinningBid { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Posted On")]
        public DateTime PostedOn { get; set; }

        [Required]
        [Display(Name = "Bidding Status")]
        public BiddingStatus? BiddingStatus { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Last Edit")]
        public DateTime? LastEdit { get; set; }

        public virtual Property Property { get; set; }
        public virtual ICollection<Bid> Bids { get; set; }
    }

    public enum BiddingStatus
    {
        UpComing,
        Active,
        Closed,
        Blocked
    }
}