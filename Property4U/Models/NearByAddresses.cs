using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Property4U.Models
{
    public class NearByAddresses
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }
        public int Floor { get; set; }
        public string AreaName { get; set; }
        public string Block { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public string ZipCode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double? Distance { get; set; }
    }
}