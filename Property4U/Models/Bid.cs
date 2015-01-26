using IdentitySample.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Property4U.Models
{
    public class Bid
    {
        public int ID { get; set; }

        [Required]
        public int BiddingID { get; set; }

        [Required]
        [Display(Name = "MemberID")]
        public string MemberID { get; set; }

        [Required]
        [StringLength(90, ErrorMessage = "Bid Title cannot be longer than 90 characters.")]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Required]
        [StringLength(250)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Display(Name = "Price")]
        public double Price { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss:}", ApplyFormatInEditMode = true)]
        [Display(Name = "Bid On")]
        public DateTime BidOn { get; set; }

        [Required]
        [Display(Name = "Bid Status")]
        public BidStatus? BidStatus { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Last Edit")]
        public DateTime? LastEdit { get; set; }

        public virtual Bidding Bidding { get; set; }

        [ForeignKey("MemberID")]
        public virtual ApplicationUser Member { get; set; }
    }

    public enum BidStatus
    {
        Winner,
        Process,
        Accepted,
        Rejected
    }
}