using IdentitySample.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Property4U.Models
{
    public class Ad
    {
        public int ID { get; set; }

        [Required]
        [Display(Name = "AdminID")]
        public string AdminID { get; set; }

        [Required]
        public int OrderID { get; set; }

        [Required]
        [StringLength(90, ErrorMessage = "Ad Title cannot be longer than 90 characters.")]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Required]
        [DataType(DataType.Url)]
        [Display(Name = "Website (URL)")]
        public string WebsiteURL { get; set; }

        [Required]
        [DataType(DataType.ImageUrl)]
        [Display(Name = "Path")]
        public string Path { get; set; }

        public double? ImageSize { get; set; }

        [Required]
        [Display(Name = "Ad Duration")]
        public AdDuration? AdDuration { get; set; }

        [Required]
        [Display(Name = "Ad Status")]
        public AdStatus? AdStatus { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string Remedies { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Posted On")]
        public DateTime PostedOn { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Last Edit")]
        public DateTime? LastEdit { get; set; }

        [ForeignKey("AdminID")]
        public virtual ApplicationUser Admin { get; set; }
        public virtual Order Order { get; set; }

    }

    public enum AdDuration
    {
        [Display (Name = "24 Hours")]
        Hours_24 = 1,
        [Display(Name = "2 Days")]
        Days_2 = 2,
        [Display(Name = "3 Days")]
        Days_3 = 3,
        Week = 7
    }

    public enum AdStatus
    {
        Active,
        Inactive,
        Expired
    }
}