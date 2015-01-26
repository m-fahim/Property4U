using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Property4U.Models
{
    public class ServerInfoAD
    {
        public string ProductVersion { get; set; }
        public string PatchLevel { get; set; }
        public string ProductEdition { get; set; }
        public string CLRVersion { get; set; }
        public string DefaultCollation { get; set; }
        public string Instance { get; set; }
        public int LCID { get; set; }
        public string ServerName { get; set; }
    }
}