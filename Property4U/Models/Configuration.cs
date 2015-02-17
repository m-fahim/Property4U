using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Property4U.Models
{
    public class Configuration
    {
        [JsonIgnore]
        public int ID { get; set; }

        [JsonIgnore]
        [Required]
        [Display(Name = "AdminID")]
        public string ConfigAdminID { get; set; }

        [Required]
        [StringLength(60, ErrorMessage = "Company Name cannot be longer than 60 characters.")]
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "Short Title Title cannot be longer than 20 characters.")]
        [Display(Name = "Short Title")]
        public string ShortTitle { get; set; }

        [Required]
        [StringLength(160, ErrorMessage = "Tagline cannot be longer than 160 characters.")]
        public string Tagline { get; set; }

        [Required]
        [DataType(DataType.Url)]
        [Display(Name = "Website (URL)")]
        public string WebsiteURL { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Public Phone.No")]
        public string PublicPhoneNo { get; set; }

        [Required]
        [Display(Name = "Office Address")]
        public string OfficeAddress { get; set; }

        [Required]
        [DataType(DataType.ImageUrl)]
        [Display(Name = "Logo Path")]
        public string LogoPath { get; set; }

        [Required]
        [DataType(DataType.ImageUrl)]
        [Display(Name = "Favicon")]
        public string Favicon { get; set; }

        [Required]
        [Display(Name = "Theme")]
        public ThemeColor? ThemeColor { get; set; }

        [Required]
        [Display(Name = "Property Renewal")]
        public PropertyRenewal? PropertyRenewal { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Display(Name = "Renewal Cost")]
        public double RenewalCost { get; set; }

        //[Required]
        //[Display(Name = "Scheduler Interval")]
        //public SchedulerInterval? SchedulerInterval { get; set; }

        [Required]
        [Display(Name = "TimeZone")]
        public string TimeZoneId { get; set; }

        [NotMapped]
        public TimeZoneInfo TimeZone
        {
            get { return TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId); }
            set { TimeZoneId = value.Id; }
        }

        [Required]
        [DataType(DataType.MultilineText)]
        [StringLength(160, MinimumLength= 150 , ErrorMessage = "Company Description should be (150 - 160) characters.")]
        [Display(Name = "Company Description")]
        public string CompanyDescription { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string Keywords { get; set; }

        [Display(Name = "Facebook AppId")]
        public string FacebookAppId { get; set; }

        [JsonIgnore]
        [Display(Name = "Facebook AppSecret")]
        public string FacebookAppSecret { get; set; }

        [Display(Name = "Google ClientId")]
        public string GoogleClientId { get; set; }

        [JsonIgnore]
        [Display(Name = "Google ClientSecret")]
        public string GoogleClientSecret { get; set; }

        [DataType(DataType.Url)]
        [Display(Name = "Facebook")]
        public string FacebookURL { get; set; }

        [DataType(DataType.Url)]
        [Display(Name = "Twitter")]
        public string TwitterURL { get; set; }

        [DataType(DataType.Url)]
        [Display(Name = "Google Plus")]
        public string GooglePlusURL { get; set; }

        [DataType(DataType.Url)]
        [Display(Name = "LinkedIn")]
        public string LinkedInURL { get; set; }

        [DataType(DataType.Url)]
        [Display(Name = "Dribbble")]
        public string DribbbleURL { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Last Edit")]
        public DateTime? LastEdit { get; set; }
    }

    public enum ThemeColor
    {
        Teal /*= "008299"*/,
        Blue /*= "2672EC"*/,
        Purple /*= " 8C0095"*/,
        Gray /*= " 8C0095"*/,
        /*DarkPurple = "5133AB",*/
        Red /*= "AC193D"*/,
        Orange /*= "D24726"*/,
        Green /*= "008A00"*/,
        [Display (Name = "Sky Blue")]
        SkyBlue /*= "094AB2"*/
    }

    public enum PropertyRenewal
    {
        Monthly = 1,
        Quaterly = 4,
        Yearly = 12
    }

    //public enum SchedulerInterval
    //{
    //    Daily = 24,
    //    Weekly = 168,
    //    Monthly = 730 /*730.484 approx*/
    //}
}