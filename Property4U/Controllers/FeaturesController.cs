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
using System.Threading.Tasks;
using System.IO;

namespace Property4U.Controllers
{
    [Authorize(Roles = "Admin, Agent")]
    public class FeaturesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Features
        public async Task<ActionResult> Index()
        {
            return View(await db.Features.ToListAsync());
        }

        // GET: Features/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Feature feature = await db.Features.FindAsync(id);
            if (feature == null)
            {
                return HttpNotFound();
            }
            return View(feature);
        }

        // GET: Features/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Features/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,Title,ImageIcon,ImageSize,Description,LastEdit")] Feature feature, HttpPostedFileBase fileIcon)
        {
            if (ModelState.IsValid)
            {
                if (fileIcon != null)
                {
                    string iconName = Path.GetFileName(fileIcon.FileName);
                    double iconSize = fileIcon.ContentLength;
                    string iconToPath = Path.Combine(Server.MapPath("~/Content/Uploads/Features"), iconName);
                    // fileIcon is uploaded
                    fileIcon.SaveAs(iconToPath);
                    feature.ImageIcon = iconToPath;
                    // New file size
                    feature.ImageSize = iconSize;
                }
                db.Features.Add(feature);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(feature);
        }

        // GET: Features/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Feature feature = await db.Features.FindAsync(id);
            if (feature == null)
            {
                return HttpNotFound();
            }
            ViewBag.LastEdit = DateTime.Now;
            return View(feature);
        }

        // POST: Features/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,Title,ImageIcon,ImageSize,Description,LastEdit")] Feature feature, HttpPostedFileBase fileIcon)
        {
            if (ModelState.IsValid)
            {
                if (fileIcon != null)
                {
                    string iconName = Path.GetFileName(fileIcon.FileName);
                    double iconSize = fileIcon.ContentLength;
                    string iconToPath = Path.Combine(Server.MapPath("~/Content/Uploads/Features"), iconName);
                    if (!System.IO.File.Exists(iconToPath))
                    {
                        // Delete previously uploaded file
                        System.IO.File.Delete(feature.ImageIcon);
                        // fileIcon is uploaded
                        fileIcon.SaveAs(iconToPath);
                        feature.ImageIcon = iconToPath;
                        // New file size
                        feature.ImageSize = iconSize;
                    }
                }
                db.Entry(feature).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.LastEdit = DateTime.Now;
            return View(feature);
        }

        // GET: Features/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Feature feature = await db.Features.FindAsync(id);
            if (feature == null)
            {
                return HttpNotFound();
            }
            return View(feature);
        }

        // POST: Features/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Feature feature = await db.Features.FindAsync(id);
            db.Features.Remove(feature);
            // Check for associated image file if exists then delete it
            if (System.IO.File.Exists(feature.ImageIcon))
            {
                System.IO.File.Delete(feature.ImageIcon);
            }
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
