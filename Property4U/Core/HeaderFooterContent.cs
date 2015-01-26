using IdentitySample.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Property4U.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Property4U.Core
{
    public static class HeaderFooterContent
    {
        static ApplicationDbContext dbo = new ApplicationDbContext();

        private static bool checkTopAgents = false, checkMenuPropertyTypes = false, checkMenuPropertyTypesWithCount = false;


        //public /*async*/ static /*Task<*/Configuration/*>*/ GetSysInfo()
        //{
        //    return /*await*/ dbo.Configurations.Find/*Async*/(1);
        //}

        // read-write variable
        public static IEnumerable<ApplicationUser> TopAgentsVM
        {
            get
            {
                if (checkTopAgents == false)
                {
                    checkTopAgents = true;

                    // Get RoleID from roleManager in order to find 4 agents base on JoinedDate (ASC)

                    var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
                    var agentUsersRoleID = roleManager.FindByName("Agent").Id;

                    var agentUsers = dbo.Users.Where(x => x.Roles.Select(y => y.RoleId).Contains(agentUsersRoleID)).OrderBy(u => u.JoinedDate).Take(4).ToList();

                    HttpContext.Current.Application["TopAgentsVM"] = agentUsers;
                }
                return HttpContext.Current.Application["TopAgentsVM"] as IEnumerable<ApplicationUser>;
            }
            set
            {
                checkTopAgents = true;
                HttpContext.Current.Application["TopAgentsVM"] = value;
            }
        }

        public static IEnumerable<OfType> MenuPropertyTypesVM
        {
            get
            {
                if (checkMenuPropertyTypes == false)
                {
                    checkMenuPropertyTypes = true;

                    // Get RoleID from roleManager in order to find 4 agents base on JoinedDate (ASC)

                    var ofTypes = dbo.OfTypes.OrderBy(t => t.ID).ToList();

                    HttpContext.Current.Application["MenuPropertyTypesVM"] = ofTypes;
                }
                return HttpContext.Current.Application["MenuPropertyTypesVM"] as IEnumerable<OfType>;
            }
            set
            {
                checkMenuPropertyTypes = true;
                HttpContext.Current.Application["MenuPropertyTypesVM"] = value;
            }
        }

        public static IEnumerable<HomeForViewModel> MenuPropertyTypesWithCountVM
        {
            get
            {
                if (checkMenuPropertyTypesWithCount == false)
                {
                    checkMenuPropertyTypesWithCount = true;

                    // Get RoleID from roleManager in order to find 4 agents base on JoinedDate (ASC)

                    var exploreTypesWithProperties = dbo.Properties.Where(p => p.Availability.ToString().Contains("Yes"))
                     .GroupBy(p => p.OfType.Title)
                     .Select(p => new HomeForViewModel
                     {
                         Title = p.Key.ToString(),
                         Count = p.Count()
                     }).ToList();

                    HttpContext.Current.Application["MenuPropertyTypesWithCountVM"] = exploreTypesWithProperties;
                }
                return HttpContext.Current.Application["MenuPropertyTypesWithCountVM"] as IEnumerable<HomeForViewModel>;
            }
            set
            {
                checkMenuPropertyTypesWithCount = true;
                HttpContext.Current.Application["MenuPropertyTypesWithCountVM"] = value;
            }
        }

    }
}