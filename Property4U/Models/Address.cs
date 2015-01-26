using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Property4U.Models
{
    public class Address
    {
        public int ID { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Name cannot be longer than 50 characters.")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Number")]
        public int Number { get; set; }

        [Required]
        [Display(Name = "Floor")]
        public int Floor { get; set; }

        [Required]
        [Display(Name = "Area Name")]
        public string AreaName { get; set; }

        [Required]
        [StringLength(1, ErrorMessage = "Name cannot be longer than 1 character.")]
        [Display(Name = "Block")]
        public string Block { get; set; }

        [Required]
        [StringLength(180, ErrorMessage = "Street cannot be longer than 180 characters.")]
        [Display(Name = "Street Name")]
        public string Street { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "City cannot be longer than 100 characters.")]
        [Display(Name = "City")]
        public string City { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "State cannot be longer than 100 characters.")]
        [Display(Name = "State")]
        public string State { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Country cannot be longer than 100 characters.")]
        [Display(Name = "Country")]
        public string Country { get; set; }

        [Required]
        [StringLength(5, ErrorMessage = "Postal Code cannot be longer than 5 characters.")]
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }

        [Required]
        [StringLength(5, ErrorMessage = "Zip Code cannot be longer than 5 characters.")]
        [Display(Name = "Zip Code")]
        public string ZipCode { get; set; }

        [Required]
        public double Latitude { get; set; }

        [Required]
        public double Longitude { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Last Edit")]
        public DateTime? LastEdit { get; set; }

        public virtual ICollection<Property> Properties { get; set; }
    }
}