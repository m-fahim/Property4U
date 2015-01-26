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
using System.IO;

namespace Property4U.Controllers
{
    [Authorize(Roles = "Admin")]
    public class OfTypesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: OfTypes
        public async Task<ActionResult> Index()
        {
            return View(await db.OfTypes.ToListAsync());
        }

        // GET: OfTypes/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OfType ofType = await db.OfTypes.FindAsync(id);
            if (ofType == null)
            {
                return HttpNotFound();
            }
            return View(ofType);
        }

        // GET: OfTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: OfTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,Title,ImageFile,ImageSize,Description,LastEdit")] OfType ofType, HttpPostedFileBase imgFile)
        {
            if (ModelState.IsValid)
            {
                if (imgFile != null)
                {
                    string imgName = Path.GetFileName(imgFile.FileName);
                    double imgSize = imgFile.ContentLength;
                    string imgToPath = Path.Combine(Server.MapPath("~/Content/Uploads/SubTypes"), imgName);
                    // Image file is uploaded
                    imgFile.SaveAs(imgToPath);
                    ofType.ImageFile = imgToPath;
                    // New file size
                    ofType.ImageSize = imgSize;
                }
                db.OfTypes.Add(ofType);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(ofType);
        }

        // GET: OfTypes/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OfType ofType = await db.OfTypes.FindAsync(id);
            if (ofType == null)
            {
                return HttpNotFound();
            }
            ViewBag.LastEdit = DateTime.Now;
            return View(ofType);
        }

        // POST: OfTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,Title,ImageFile,ImageSize,Description,LastEdit")] OfType ofType, HttpPostedFileBase imgFile)
        {
            if (ModelState.IsValid)
            {
                if (imgFile != null)
                {
                    string imgName = Path.GetFileName(imgFile.FileName);
                    double imgSize = imgFile.ContentLength;
                    string imgToPath = Path.Combine(Server.MapPath("~/Content/Uploads/Types"), imgName);
                    if (!System.IO.File.Exists(imgToPath))
                    {
                        // Delete previously uploaded file
                        System.IO.File.Delete(ofType.ImageFile);
                        // Image file is uploaded
                        imgFile.SaveAs(imgToPath);
                        ofType.ImageFile = imgToPath;
                        // New file size
                        ofType.ImageSize = imgSize;
                    }
                }
                db.Entry(ofType).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.LastEdit = DateTime.Now;
            return View(ofType);
        }

        // GET: OfTypes/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OfType ofType = await db.OfTypes.FindAsync(id);
            if (ofType == null)
            {
                return HttpNotFound();
            }
            return View(ofType);
        }

        // POST: OfTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            OfType ofType = await db.OfTypes.FindAsync(id);
            db.OfTypes.Remove(ofType);
            await db.SaveChangesAsync();

            // Check for associated image file if exists then delete it
            if (System.IO.File.Exists(ofType.ImageFile))
            {
                System.IO.File.Delete(ofType.ImageFile);
            }
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
