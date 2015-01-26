using IdentitySample.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Property4U.Models
{
    public class Reply
    {
        public int ID { get; set; }

        [Required]
        [Display(Name = "AgentID")]
        public string AgentID { get; set; }

        [Required]
        public int ReviewID { get; set; }

        [Required]
        [StringLength(250)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Reply On")]
        public DateTime ReplyOn { get; set; }

        [ForeignKey("AgentID")]
        public virtual ApplicationUser Agent { get; set; }
        public virtual Review Review { get; set; }
    }
}