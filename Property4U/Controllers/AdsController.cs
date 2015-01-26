using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using IdentitySample.Models;
using Property4U.Models;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using System.IO;

namespace Property4U.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private string strCurrentUserId;
        private int orderStatus;

        // GET: Ads
        public async Task<ActionResult> Index()
        {
            var ads = db.Ads.Include(a => a.Admin).Include(a => a.Order);
            return View(await ads.ToListAsync());
        }

        // GET: Ads/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ad ad = await db.Ads.FindAsync(id);
            if (ad == null)
            {
                return HttpNotFound();
            }
            return View(ad);
        }

        // GET: Ads/Create
        public async Task<ActionResult> Create()
        {
            strCurrentUserId = User.Identity.GetUserId();
            var ownerAdmin = await db.Users.Where(d => d.Id == strCurrentUserId).ToListAsync();
            //Debug.Write(strCurrentUserId);

            SelectList slAdmin = new SelectList(ownerAdmin, "Id", "Id", strCurrentUserId);
            ViewBag.AdminIDList = slAdmin;
            ViewBag.OrderIDList = new SelectList(db.Orders, "ID", "ID");
            ViewBag.PostedOn = DateTime.Now.ToString("yyyy-MM-dd");
            return View();
        }

        // POST: Ads/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,AdminID,OrderID,Title,WebsiteURL,Path,ImageSize,AdDuration,AdStatus,Remedies,PostedOn,LastEdit")] Ad ad, HttpPostedFileBase adFile, string oldAdPath)
        {
            if (adFile == null)
            {
                ModelState.AddModelError(string.Empty, "Ad image file must be chosen.");
            }
            
            if (ModelState.IsValid)
            {
                if (adFile != null)
                {
                    string adImageNameWithEx = Path.GetFileName(adFile.FileName).Replace(" ", "-");
                    double adImageSize = adFile.ContentLength;
                    string configAdToPath = Path.Combine(Server.MapPath("~/Content/Uploads/Ads"), adImageNameWithEx);

                    if (!System.IO.File.Exists(configAdToPath))
                    {
                        //string adImageNameWithoutEx = Path.GetFileNameWithoutExtension(adFile.FileName).Replace(" ", "-");
                        //string adImageEx = Path.GetExtension(adFile.FileName);
                        //string adCustomFileName = adImageNameWithoutEx+adImageEx;
                        //configAdToPath = Path.Combine(Server.MapPath("~/Content/Uploads/Ads"), adCustomFileName);
                        // New file size
                        ad.ImageSize = adImageSize;

                        // New file whos name conflict with existing uploaded Image name - should given custom name
                        adFile.SaveAs(configAdToPath);
                        ad.Path = adImageNameWithEx;
                    }
                    
                }

                // Update Order db Status accordingly
                if (ad.AdStatus.ToString().Equals("Active"))
                {
                    orderStatus = 2;
                }
                else if (ad.AdStatus.ToString().Equals("Inactive"))
                {
                    orderStatus = 3;
                }
                else
                {
                    orderStatus = 4;
                }
                db.Database.ExecuteSqlCommand("UPDATE [dbo].[Order] SET OrderStatus = {0}, Remedies = {1} WHERE ID = {2}", orderStatus, ad.Remedies, ad.OrderID);
                db.Ads.Add(ad);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            strCurrentUserId = User.Identity.GetUserId();
            var ownerAdmin = await db.Users.Where(d => d.Id == strCurrentUserId).ToListAsync();
            ViewBag.AdminIDList = new SelectList(ownerAdmin, "Id", "Id", ad.AdminID);

            ViewBag.OrderIDList = new SelectList(db.Orders, "ID", "ID", ad.OrderID);
            ViewBag.PostedOn = DateTime.Now.ToString("yyyy-MM-dd");
            return View(ad);
        }

        // GET: Ads/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ad ad = await db.Ads.FindAsync(id);
            if (ad == null)
            {
                return HttpNotFound();
            }

            strCurrentUserId = User.Identity.GetUserId();
            var ownerAdmin = await db.Users.Where(d => d.Id == strCurrentUserId).ToListAsync();
            ViewBag.AdminIDList = new SelectList(ownerAdmin, "Id", "Id", strCurrentUserId);

            ViewBag.OrderIDList = new SelectList(db.Orders, "ID", "ID", ad.OrderID);
            ViewBag.LastEdit = DateTime.Now;
            return View(ad);
        }

        // POST: Ads/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,AdminID,OrderID,Title,WebsiteURL,Path,ImageSize,AdDuration,AdStatus,Remedies,PostedOn,LastEdit")] Ad ad, HttpPostedFileBase adFile, string oldAdPath)
        {
            if (ModelState.IsValid)
            {
                if (adFile != null)
                {
                    string adImageName = Path.GetFileName(adFile.FileName).Replace(" ", "-");
                    string configAdToPath = Path.Combine(Server.MapPath("~/Content/Uploads/Ads"), adImageName);

                    if (!System.IO.File.Exists(configAdToPath))
                    {
                        //string adImageNameWithoutEx = Path.GetFileNameWithoutExtension(adFile.FileName).Replace(" ", "-");
                        //string adImageEx = Path.GetExtension(adFile.FileName);
                        //string adCustomFileName = adImageNameWithoutEx + adImageEx;

                        // Delete previously uploaded file
                        System.IO.File.Delete(Path.Combine(Server.MapPath("~/Content/Uploads/Ads"), oldAdPath));
                        // New file size
                        ad.ImageSize = adFile.ContentLength;
                        // New file is uploaded
                        adFile.SaveAs(configAdToPath);
                        ad.Path = adImageName;
                    }
                    else
                    {
                        ad.Path = adImageName;
                    }
                }
                // Update Order db Status accordingly
                if (ad.AdStatus.ToString().Equals("Active"))
                {
                    orderStatus = 2;
                }
                else if (ad.AdStatus.ToString().Equals("Inactive"))
                {
                    orderStatus = 3;
                }
                else
                {
                    orderStatus = 4;
                }
                db.Database.ExecuteSqlCommand("UPDATE [dbo].[Order] SET OrderStatus = {0}, Remedies = {1} WHERE ID = {2}", orderStatus, ad.Remedies, ad.OrderID);

                db.Entry(ad).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            strCurrentUserId = User.Identity.GetUserId();
            var ownerAdmin = await db.Users.Where(d => d.Id == strCurrentUserId).ToListAsync();
            ViewBag.AdminIDList = new SelectList(ownerAdmin, "Id", "Id", ad.AdminID);

            ViewBag.OrderIDList = new SelectList(db.Orders, "ID", "ID", ad.OrderID);
            ViewBag.LastEdit = DateTime.Now;
            return View(ad);
        }

        // GET: Ads/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ad ad = await db.Ads.FindAsync(id);
            if (ad == null)
            {
                return HttpNotFound();
            }
            return View(ad);
        }

        // POST: Ads/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Ad ad = await db.Ads.FindAsync(id);

            string configAdToPath = Path.Combine(Server.MapPath("~/Content/Uploads/Ads"), ad.Path);
            if (System.IO.File.Exists(configAdToPath))
            {
                // Delete uploaded ad image file
                System.IO.File.Delete(configAdToPath);
            }

            db.Ads.Remove(ad);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
