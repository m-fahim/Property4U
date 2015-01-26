using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Property4U.Models
{
    public class Feature
    {
        public int ID { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Feature Title cannot be longer than 50 characters.")]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [DataType(DataType.ImageUrl)]
        [Display(Name = "Image Icon")]
        public string ImageIcon { get; set; }

        public double? ImageSize { get; set; }

        [Required]
        [StringLength(250)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Last Edit")]
        public DateTime? LastEdit { get; set; }

        // Ignore because of Many to Many relation cause exception for Json (Properties/Features) requests.
        [JsonIgnore]
        [IgnoreDataMember] 
        public virtual ICollection<Property> Properties { get; set; }
    }
}