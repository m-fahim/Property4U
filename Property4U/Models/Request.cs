using IdentitySample.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Property4U.Models
{
    public class Request
    {
        public int ID { get; set; }

        [Required]
        [Display(Name = "MemberID")]
        public string MemberID { get; set; }

        [Required]
        public int PropertyID { get; set; }

        [Required]
        [StringLength(90, ErrorMessage = "Request Title cannot be longer than 90 characters.")]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Required]
        [StringLength(250)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Visiting Date")]
        public DateTime VisitingDate { get; set; }

        [Required]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh:mm tt}", ApplyFormatInEditMode = true)]
        //[RegularExpression(@"^(0[1-9]|1[0-2]):[0-5][0-9] (am|pm|AM|PM)$", ErrorMessage = "Invalid Visiting Time.")]
        [Display(Name = "Visiting Time")]
        public DateTime VisitingTime { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Request On")]
        public DateTime RequestOn { get; set; }

        [Required]
        [Display(Name = "Request Status")]
        public RequestStatus? RequestStatus { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Last Edit")]
        public DateTime? LastEdit { get; set; }

        [ForeignKey("MemberID")]
        public virtual ApplicationUser Member { get; set; }
        public virtual Property Property { get; set; }
        public virtual ICollection<Response> Responses { get; set; }

    }

    public enum RequestStatus
    {
        Pending,
        Process,
        Accepted,
        Rejected
    }
}