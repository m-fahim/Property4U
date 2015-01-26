using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Property4U.Core
{
    public class TaskInfo
    {

        //private static bool check = false;


        // read-write variable
        public static SysTask Info
        {
            get
            {
                //if (check == false)
                //{
                //    check = true;
                //    HttpContext.Current.Application["Info"] = dbo.Configurations.Find(1);
                //}
                return HttpContext.Current.Application["Info"] as SysTask;
            }
            set
            {
                //check = true;
                HttpContext.Current.Application["Info"] = value;
            }
        }
    }
}