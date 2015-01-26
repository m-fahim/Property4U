using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Property4U.Models
{
    public class OfType
    {
        public int ID { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Type Title cannot be longer than 50 characters.")]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [DataType(DataType.ImageUrl)]
        [Display(Name = "Image File")]
        public string ImageFile { get; set; }

        public double? ImageSize { get; set; }

        [StringLength(250)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Last Edit")]
        public DateTime? LastEdit { get; set; }
    }
}