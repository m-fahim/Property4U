using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using IdentitySample.Models;
using Property4U.Models;
using Microsoft.AspNet.Identity;
using System.IO;
using Property4U.Core;

namespace Property4U.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ConfigurationsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private string strCurrentUserId;

        //// GET: Configurations
        //public async Task<ActionResult> Index()
        //{
        //    return View(await db.Configurations.ToListAsync());
        //}

        //// GET: Configurations/Details/5
        //public async Task<ActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Configuration configuration = await db.Configurations.FindAsync(id);
        //    if (configuration == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(configuration);
        //}

        //// GET: Configurations/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        //// POST: Configurations/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Create([Bind(Include = "ID,ConfigAdminID,CompanyName,ShortTitle,Tagline,WebsiteURL,Email,LogoPath,ThemeColor,PropertyRenewal,SchedulerInterval,TimeZoneId,CompanyDescription")] Configuration configuration)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Configurations.Add(configuration);
        //        await db.SaveChangesAsync();
        //        return RedirectToAction("Index");
        //    }

        //    return View(configuration);
        //}

        // GET: Configurations/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Configuration configuration = await db.Configurations.FindAsync(id);
            if (configuration == null && id != 1)
            {
                return HttpNotFound();
            }
            else if (configuration == null && id == 1)
            {
                strCurrentUserId = User.Identity.GetUserId();
                Configuration con = new Configuration { ID = 1, ConfigAdminID = strCurrentUserId, CompanyName = "Property4U", ShortTitle = "P4U", Tagline = "Open Source Real Estate 2.0", WebsiteURL = "www.property4u.com.pk", Email = "contact@property4u.com.pk", OfficeAddress = "Minar-e-Pakistan, Circular Rd Lahore Punjab, Pakistan", LogoPath = "P4U_logo.png", Favicon = "P4U_favicon.png", ThemeColor = ThemeColor.Blue, PropertyRenewal = PropertyRenewal.Monthly, RenewalCost = 20.00, TimeZoneId = "Pakistan Standard Time", CompanyDescription = "Property4U is an Open Source Real Estate 2.0 solution completely equipped with Buying, Selling, Renting and Bidding features. It also provides Developer APIs.", Keywords = "p4u, realtors, selling house fast, quickly configurable, open source, real estate 2.0, real estate bidding 2.0, search real estate agent", LastEdit = DateTime.Now };
                db.Configurations.Add(con);
                await db.SaveChangesAsync();
                Property4U.Core.ConfigSys.Settings = con;

                return RedirectToAction("Edit", null, new { id = id });
            }
            ViewBag.ConfigAdminID = configuration.ConfigAdminID;
            SelectList timeZones = new SelectList(TimeZones.GetTimeZoneHelpers(), "Id", "DisplayName", configuration.TimeZone);
            ViewBag.TimeZoneId = timeZones;
            ViewBag.LastEdit = DateTime.Now;
            return View(configuration);
        }

        // POST: Configurations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,ConfigAdminID,CompanyName,ShortTitle,Tagline,WebsiteURL,Email,PublicPhoneNo,OfficeAddress,LogoPath,Favicon,ThemeColor,PropertyRenewal,RenewalCost,SchedulerInterval,TimeZoneId,CompanyDescription,Keywords,FacebookAppId,FacebookAppSecret,GoogleClientId,GoogleClientSecret,FacebookURL,TwitterURL,GooglePlusURL,LinkedInURL,DribbbleURL,LastEdit")] Configuration configuration, HttpPostedFileBase logoFile, string oldLogoPath, HttpPostedFileBase faviconFile, string oldfaviconPath)
        {

            if (ModelState.IsValid)
            {
                if (logoFile != null)
                {
                    string logoImageName = Path.GetFileName(logoFile.FileName);
                    string configlogoToPath = Path.Combine(Server.MapPath("~/Content/Uploads/Assets"), logoImageName);

                    if (!System.IO.File.Exists(configlogoToPath))
                    {
                        // Delete previously uploaded file
                        System.IO.File.Delete(Path.Combine(Server.MapPath("~/Content/Uploads/Assets"), oldLogoPath));
                        // New file is uploaded
                        logoFile.SaveAs(configlogoToPath);
                        configuration.LogoPath = logoImageName;
                    }
                }

                if (faviconFile != null)
                {
                    string favImageName = Path.GetFileName(faviconFile.FileName);
                    string configFavToPath = Path.Combine(Server.MapPath("~/Content/Uploads/Assets"), favImageName);

                    if (!System.IO.File.Exists(configFavToPath))
                    {
                        // Delete previously uploaded file
                        System.IO.File.Delete(Path.Combine(Server.MapPath("~/Content/Uploads/Assets"), oldfaviconPath));
                        // New file is uploaded
                        faviconFile.SaveAs(configFavToPath);
                        configuration.Favicon = favImageName;
                    }
                }

                // Remove PROTOCOL(http://) from URI
                System.Uri uri = new Uri(configuration.WebsiteURL);
                configuration.WebsiteURL = uri.Host + uri.PathAndQuery;

                db.Entry(configuration).State = EntityState.Modified;
                await db.SaveChangesAsync();

                Property4U.Core.ConfigSys.Settings = configuration;

                return RedirectToAction("Edit", null, new { id = configuration.ID });
            }
            ViewBag.ConfigAdminID = configuration.ConfigAdminID;
            SelectList timeZones = new SelectList(TimeZones.GetTimeZoneHelpers(), "Id", "DisplayName", configuration.TimeZone);
            ViewBag.TimeZoneId = timeZones;
            ViewBag.LastEdit = DateTime.Now;
            return View(configuration);
        }

        //// GET: Configurations/Delete/5
        //public async Task<ActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Configuration configuration = await db.Configurations.FindAsync(id);
        //    if (configuration == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(configuration);
        //}

        //// POST: Configurations/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> DeleteConfirmed(int id)
        //{
        //    Configuration configuration = await db.Configurations.FindAsync(id);
        //    db.Configurations.Remove(configuration);
        //    await db.SaveChangesAsync();
        //    return RedirectToAction("Index");
        //}

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
    }
}
