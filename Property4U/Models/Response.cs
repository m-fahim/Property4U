using IdentitySample.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Property4U.Models
{
    public class Response
    {
        public int ID { get; set; }

        [Required]
        public int RequestID { get; set; }

        [Display(Name = "AgentID")]
        public string AgentID { get; set; }

        [Required]
        [StringLength(90, ErrorMessage = "Reponse Title cannot be longer than 90 characters.")]
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
        [Display(Name = "Response On")]
        public DateTime ResponseOn { get; set; }

        [Required]
        [Display(Name = "Response Status")]
        public ResponseStatus? ResponseStatus { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Last Edit")]
        public DateTime? LastEdit { get; set; }

        [ForeignKey("AgentID")]
        public virtual ApplicationUser Agent { get; set; }
        public virtual Request Request { get; set; }
    }

    public enum ResponseStatus
    {
        Agree,
        Disagree
    }
}