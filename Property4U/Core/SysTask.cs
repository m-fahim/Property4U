using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Property4U.Core
{
    public class SysTask
    {
        public string Name { get; set; }
        public DateTime NextExecutionTime { get; set; }
        public DateTime StartTime { get; set; }
    }
}