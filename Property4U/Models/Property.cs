using IdentitySample.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Property4U.Models
{
    public class Property
    { 
        public int ID { get; set; }

        [Required]
        [Display(Name = "AgentID")]
        public string AgentID { get; set; }

        [Required]
        public int AddressID { get; set; }

        [Required]
        public int OfTypeID { get; set; }

        [Display(Name = "SubType")]
        public int OfSubType { get; set; }

        [Required]
        [StringLength(120, ErrorMessage = "Photo Title cannot be longer than 120 characters.")]
        public string Title { get; set; }

        [Required]
        [StringLength(120, ErrorMessage = "Seller Name cannot be longer than 120 characters.")]
        public string Seller { get; set; }

        //[Required]
        //[Display(Name = "Type")]
        //public OfType? OfType { get; set; }

        [Required]
        [StringLength(120, ErrorMessage = "Locality cannot be longer than 120 characters.")]
        public string Locality { get; set; }

        [Display(Name = "Covered Area")]
        public double? CoveredAreaMeasurement { get; set; }
        [Display(Name = "Covered Area Units")]
        public AreaUnits? CoveredAreaUnits { get; set; }

        public Condition? Condition { get; set; }
        public Furnished? Furnished { get; set; }
        public int? Stories { get; set; }

        [Display(Name = "Floor No")]
        public int? FloorNo { get; set; }

        public Flooring? Flooring { get; set; }

        public int? Baths { get; set; }

        public int? Kitchens { get; set; }

        [Display(Name = "Drawing Rooms")]
        public int? DrawingRooms { get; set; }

        [Display(Name = "Dining Rooms")]
        public int? DiningRooms { get; set; }

        [Display(Name = "Living Rooms")]
        public int? LivingRooms { get; set; }

        [Display(Name = "Number Of Rooms")]
        public int? NumberOfRooms { get; set; }

        [Display(Name = "Store Rooms")]
        public int? StoreRooms { get; set; }

        [Display(Name = "Servant Quarters")]
        public int? ServantQuarters { get; set; }

        public Lawn? Lawn { get; set; }

        [Display(Name = "Car Spaces")]
        public int? CarSpaces { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Build (Year)")]
        public DateTime? Build { get; set; }

        [Required]
        [Display(Name = "Area Measurement")]
        public double AreaMeasurement { get; set; }

        [Required]
        [Display(Name = "Area Units")]
        public AreaUnits AreaUnits { get; set; }

        [Required]
        [Display(Name = "Length")]
        public double AreaDiLength { get; set; }

        [Required]
        [Display(Name = "Width")]
        public double AreaDiWidth { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Display(Name = "Price")]
        public int Price { get; set; }

        [Required]
        [Display(Name = "For")]
        public For? For { get; set; }

        [Required]
        [Display(Name = "Allow Bidding")]
        public AllowBidding? AllowBidding { get; set; }

        [Required]
        [Display(Name = "Availability")]
        public Availability? Availability { get; set; }

        public int? Views { get; set; }

        public Status? Status { get; set; }
        public int? Discount { get; set; }

        public Featured? Featured { get; set; }

        public int? Flags { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Publish On")]
        public DateTime PublishOn { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Last Edit")]
        public DateTime? LastEdit { get; set; }

        
        [ForeignKey("AgentID")]
        public virtual ApplicationUser Agent { get; set; }
        public virtual ICollection<Request> Requests { get; set; }
        public virtual ICollection<Photo> Photos { get; set; }
        public virtual ICollection<Renewal> Renewals { get; set; }
        public virtual Address Address { get; set; }

        // Ignore because of Many to Many relation cause exception for Json (Properties/Features) requests.
        [JsonIgnore] 
        [IgnoreDataMember] 
        public virtual ICollection<Feature> Features { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
        public virtual ICollection<Bidding> Biddings { get; set; }
        public virtual OfType OfType { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }

    }

    //public enum OfType
    //{
    //    Developing,
    //    Apartment,
    //    House,
    //    Bunglows,
    //    Land,
    //    Commercial,
    //    Condominium,
    //    [Display(Name = "Office Space")]
    //    Office_Space
    //}

    public enum Condition
    {
        New,
        Repaired,
        [Display(Name = "Well Maintained")]
        WellMaintained
    }

    public enum Furnished
    {
        Yes,
        No
    }

    public enum Flooring
    {
        [Display(Name = "Ceramic or Porcelin Tile")]
        Ceramic_or_Porcelin_Tile,
        [Display(Name = "Flooting Wood Title")]
        Flooting_Wood_Title,
        [Display(Name = "Carpet")]
        Carpet,
        [Display(Name = "Hardwood")]
        Hardwood,
        [Display(Name = "Engineered Wood")]
        Engineered_Wood,
        [Display(Name = "Bamboo")]
        Bamboo,
        [Display(Name = "Cork")]
        Cork,
        [Display(Name = "Stone")]
        Stone,
        [Display(Name = "Vinyl")]
        Vinyl
    }

    public enum Lawn
    {
        Yes,
        No
    }

    public enum AreaUnits
    {
        [Display(Name = "ft²")]
        ft2,
        [Display(Name = "m²")]
        m2,
        Acre,
        Marla,
        Kanal
    }

    public enum For
    {
        Sale,
        Rent
    }

    public enum AllowBidding
    {
        Allowed,
        Disabled
    }

    public enum Availability
    {
        Yes,
        No
    }

    public enum Status
    {
        [Display(Name = "Sold")]
        Sold,
        [Display(Name = "Hot")]
        Hot,
        [Display(Name = "Reduced")]
        Reduced,
        [Display(Name = "Under Offer")]
        Under_Offer,
        [Display(Name = "In Discount")]
        In_Discount,
        [Display(Name = "Repossession")]
        Repossession,
        [Display(Name = "Foreclosure")]
        Foreclosure,
        [Display(Name = "No Chain")]
        No_Chain,
        [Display(Name = "Vacant")]
        Vacant,
        [Display(Name = "Free Hold")]
        Free_Hold,
        [Display(Name = "Lease")]
        Lease,
        [Display(Name = "New")]
        New
    }

    public enum Featured
    {
        Yes,
        No
    }
    
}


//public Furnished? Furnished { get; set; }
//public enum Furnished
//    {
//        Yes,
//        No
//    }

//[Display(Name = "Covered Area")]
//public double CoveredAreaMeasurement { get; set; }
//[Display(Name = "Covered Area Units")]
//public AreaUnits CoveredAreaUnits { get; set; }
//[Display(Name = "Floor No")]
//public int? FloorNo { get; set; }

//public Flooring? Flooring { get; set; }
//public enum Flooring
//    {
//        [Display(Name = "Ceramic or Porcelin Tile")]
//        UCeramic_or_Porcelin_Tile,
//        [Display(Name = "Flooting Wood Title")]
//        Flooting_Wood_Title,
//        [Display(Name = "Carpet")]
//        Carpet,
//        [Display(Name = "Hardwood")]
//        Hardwood,
//        [Display(Name = "Engineered Wood")]
//        Engineered_Wood,
//        [Display(Name = "Bamboo")]
//        Bamboo,
//        [Display(Name = "Cork")]
//        Cork,
//        [Display(Name = "Stone")]
//        Stone,
//        [Display(Name = "Vinyl")]
//        Vinyl
//    }
//public int? Flags { get; set; }