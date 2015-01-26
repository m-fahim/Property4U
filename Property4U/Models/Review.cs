using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Property4U.Models
{
    public class Review
    {
        public int ID { get; set; }

        [Required]
        public int PropertyID { get; set; }

        [Required]
        [StringLength(90, ErrorMessage = "Name cannot be longer than 90 characters.")]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Please provide valid Rating")]
        public int Rating { get; set; }

        [Required]
        [StringLength(250)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Review On")]
        public DateTime ReviewOn { get; set; }

        [Display(Name = "IP Address")]
        public string IPAddress { get; set; }

        public virtual Property Property { get; set; }
        public virtual ICollection<Reply> Replies { get; set; }
    }
}