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
    public class OfSubTypesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: OfSubTypes
        public async Task<ActionResult> Index()
        {
            var ofSubTypes = db.OfSubTypes.Include(o => o.OfType);
            return View(await ofSubTypes.ToListAsync());
        }

        // GET: OfSubTypes/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OfSubType ofSubType = await db.OfSubTypes.FindAsync(id);
            if (ofSubType == null)
            {
                return HttpNotFound();
            }
            return View(ofSubType);
        }

        // GET: OfSubTypes/Create
        public ActionResult Create()
        {
            ViewBag.OfTypeIDList = new SelectList(db.OfTypes, "ID", "Title");
            return View();
        }

        // POST: OfSubTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,OfTypeID,Title,ImageFile,ImageSize,Description,LastEdit")] OfSubType ofSubType, HttpPostedFileBase imgFile)
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
                    ofSubType.ImageFile = imgToPath;
                    // New file size
                    ofSubType.ImageSize = imgSize;
                }
                db.OfSubTypes.Add(ofSubType);
                await db.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            ViewBag.OfTypeIDList = new SelectList(db.OfTypes, "ID", "Title", ofSubType.OfTypeID);
            return View(ofSubType);
        }

        // GET: OfSubTypes/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OfSubType ofSubType = await db.OfSubTypes.FindAsync(id);
            if (ofSubType == null)
            {
                return HttpNotFound();
            }
            ViewBag.LastEdit = DateTime.Now;
            ViewBag.OfTypeIDList = new SelectList(db.OfTypes, "ID", "Title", ofSubType.OfTypeID);
            return View(ofSubType);
        }

        // POST: OfSubTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,OfTypeID,Title,ImageFile,ImageSize,Description,LastEdit")] OfSubType ofSubType, HttpPostedFileBase imgFile)
        {
            if (ModelState.IsValid)
            {
                if (imgFile != null)
                {
                    string imgName = Path.GetFileName(imgFile.FileName);
                    double imgSize = imgFile.ContentLength;
                    string imgToPath = Path.Combine(Server.MapPath("~/Content/Uploads/SubTypes"), imgName);
                    if (!System.IO.File.Exists(imgToPath))
                    {
                        // Delete previously uploaded file
                        System.IO.File.Delete(ofSubType.ImageFile);
                        // Image file is uploaded
                        imgFile.SaveAs(imgToPath);
                        ofSubType.ImageFile = imgToPath;
                        // New file size
                        ofSubType.ImageSize = imgSize;
                    }
                }
                db.Entry(ofSubType).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.LastEdit = DateTime.Now;
            ViewBag.OfTypeIDList = new SelectList(db.OfTypes, "ID", "Title", ofSubType.OfTypeID);
            return View(ofSubType);
        }

        // GET: OfSubTypes/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OfSubType ofSubType = await db.OfSubTypes.FindAsync(id);
            if (ofSubType == null)
            {
                return HttpNotFound();
            }
            return View(ofSubType);
        }

        // POST: OfSubTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            OfSubType ofSubType = await db.OfSubTypes.FindAsync(id);
            db.OfSubTypes.Remove(ofSubType);
            await db.SaveChangesAsync();

            // Check for associated image file if exists then delete it
            if (System.IO.File.Exists(ofSubType.ImageFile))
            {
                System.IO.File.Delete(ofSubType.ImageFile);
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
