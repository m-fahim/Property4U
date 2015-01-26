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
    public class Order
    {
        public int ID { get; set; }

        [Required]
        [Display(Name = "AgentID")]
        public string AgentID { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Order Title cannot be longer than 50 characters.")]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Required]
        [StringLength(250)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Size")]
        public Size? Size { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Please enter valid Number Of Ads")]
        [Display(Name = "Number Of Ads")]
        public int NumberOfAds { get; set; }

        [Required]
        [DataType(DataType.ImageUrl)]
        [Display(Name = "Images Zip")]
        public string ZipFilePath { get; set; }

        [Display(Name = "Zip Size")]
        public double? ZipFileSize { get; set; }

        [Required]
        [Display(Name = "Ads Duration")]
        public AdsDuration? AdsDuration { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date")]
        public DateTime Date { get; set; }

        [Required]
        [Display(Name = "Order Status")]
        public OrderStatus? OrderStatus { get; set; }

        [DataType(DataType.MultilineText)]
        public string Remedies { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Last Edit")]
        public DateTime? LastEdit { get; set; }

        [ForeignKey("AgentID")]
        public virtual ApplicationUser Agent { get; set; }
        public virtual ICollection<Ad> Ads { get; set; }
    }

    public enum Size
    {
        [Display(Name = "1140 x 146")]
        FullBanner,
        [Display(Name = "600 x 50")]
        SmallBanner,
        [Display(Name = "270 x 300")]
        Square,
        [Display(Name = "270 x 400")]
        FatSkyscraper
    }

    public enum AdsDuration
    {
        [Display(Name = "24 Hours")]
        Hours_24 = 1,
        [Display(Name = "2 Days")]
        Days_2 = 2,
        [Display(Name = "3 Days")]
        Days_3 = 3,
        Week = 7
    }

    public enum OrderStatus
    {
        Pending,
        Process,
        Approved,
        Rejected,
        Expired
    }
}