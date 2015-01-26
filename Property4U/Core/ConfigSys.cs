using IdentitySample.Models;
using Property4U.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Property4U.Core
{
    public static class ConfigSys
    {
        static ApplicationDbContext dbo = new ApplicationDbContext();

        private static bool check = false;


        //public /*async*/ static /*Task<*/Configuration/*>*/ GetSysInfo()
        //{
        //    return /*await*/ dbo.Configurations.Find/*Async*/(1);
        //}

        // read-write variable
        public static Configuration Settings
        {
            get
            {
                if (check == false)
                {
                    check = true;
                    HttpContext.Current.Application["Settings"] = dbo.Configurations.Find(1);
                }
                return HttpContext.Current.Application["Settings"] as Configuration;
            }
            set
            {
                check = true;
                HttpContext.Current.Application["Settings"] = value;
            }
        }

        //ConfigSys()
        //{
        //    HttpContext.Current.Application["Settings"] = dbo.Configurations.Find(1);
        //}

    }
}