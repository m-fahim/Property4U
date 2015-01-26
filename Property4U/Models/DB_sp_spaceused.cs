using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Property4U.Models
{
    public class DB_sp_spaceused
    {
        public string database_name { get; set; }
        public string database_size { get; set; }
        public string unallocated_space { get; set; }
    }
}