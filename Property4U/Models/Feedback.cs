using IdentitySample.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Property4U.Models
{
    public class Feedback
    {
        public int ID { get; set; }

        [Required]
        public int PropertyID { get; set; }

        [Required]
        [Display(Name = "MemberID")]
        public string MemberID { get; set; }

        [Required]
        public For_Report? For { get; set; }

        [StringLength(90, ErrorMessage = "Feedback Title cannot be longer than 90 characters.")]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Required]
        [StringLength(250)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Agent Rating")]
        public AgentRating? AgentRating { get; set; }

        [StringLength(250)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Agent Review")]
        public string AgentReview { get; set; }

        [Display(Name = "Overall Experience")]
        public OverallExperience? OverallExperience { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Feedback On")]
        public DateTime FeedbackOn { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Last Edit")]
        public DateTime? LastEdit { get; set; }

        public virtual Property Properties { get; set; }

        [ForeignKey("MemberID")]
        public virtual ApplicationUser Member { get; set; }
    }

    public enum For_Report
    {
        [Display(Name = "Report Abuse")]
        Report_Abuse = 1,
        [Display(Name = "Process Feedback")]
        Process_Feedback = 2
    }

    public enum AgentRating {
        [Display(Name = "Very Unhappy")]
        Very_Unhappy = 1,
        [Display(Name = "Unhappy")]
        Unhappy = 2,
        Neutral = 3,
        Happy = 4,
        [Display(Name = "Very Happy")]
        Very_Happy = 5
    }

    public enum OverallExperience
    {
        Bad = 1,
        Fine = 2,
        Satisfactory = 3,
        Great = 4,
        Memorable = 5
    }
}