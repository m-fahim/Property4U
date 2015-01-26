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

namespace Property4U.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RenewalsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Renewals
        public async Task<ActionResult> Index()
        {
            var renewals = db.Renewals.Include(r => r.Property);
            return View( await renewals.ToListAsync());
        }

        // GET: Renewals/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Renewal renewal = await db.Renewals.FindAsync(id);
            if (renewal == null)
            {
                return HttpNotFound();
            }
            return View(renewal);
        }

        // GET: Renewals/Create
        public ActionResult Create()
        {
            ViewBag.PropertyIDList = new SelectList(db.Properties, "ID", "ID");
            ViewBag.Dated = DateTime.Now.ToString("yyyy-MM-dd");
            return View();
        }

        // POST: Renewals/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,PropertyID,Description,Price,Status,Dated,LastEdit")] Renewal renewal)
        {
            if (ModelState.IsValid)
            {
                string propertyStatus = renewal.Status.ToString();
                // Update Property db Status accordingly
                switch (propertyStatus)
                {
                    case "Inactive":
                        db.Database.ExecuteSqlCommand("UPDATE [dbo].[Property] SET Avaliability = 0 WHERE ID = {0}", renewal.PropertyID);
                        break;
                    case "Active":
                        db.Database.ExecuteSqlCommand("UPDATE [dbo].[Property] SET Avaliability = 1 WHERE ID = {0}", renewal.PropertyID);
                        break;
                    case "Block":
                        db.Database.ExecuteSqlCommand("UPDATE [dbo].[Property] SET Avaliability = 1 AND AllowBidding = 2 WHERE ID = {0}", renewal.PropertyID);
                        break;
                    case "Expired":
                        db.Database.ExecuteSqlCommand("UPDATE [dbo].[Property] SET Avaliability = 1 WHERE ID = {0}", renewal.PropertyID);
                        break;
                    default:
                        break;
                }
                db.Renewals.Add(renewal);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.PropertyIDList = new SelectList(db.Properties, "ID", "ID", renewal.PropertyID);
            ViewBag.Dated = DateTime.Now.ToString("yyyy-MM-dd");
            return View(renewal);
        }

        // GET: Renewals/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Renewal renewal = await db.Renewals.FindAsync(id);
            if (renewal == null)
            {
                return HttpNotFound();
            }
            ViewBag.PropertyIDList = new SelectList(db.Properties, "ID", "ID", renewal.PropertyID);
            ViewBag.LastEdit = DateTime.Now;
            return View(renewal);
        }

        // POST: Renewals/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,PropertyID,Description,Price,Status,Dated,LastEdit")] Renewal renewal)
        {
            if (ModelState.IsValid)
            {
                string propertyStatus = renewal.Status.ToString();
                // Update Property db Status accordingly
                switch(propertyStatus){
                    case "Inactive":
                    db.Database.ExecuteSqlCommand("UPDATE [dbo].[Property] SET Avaliability = 0 WHERE ID = {0}", renewal.PropertyID);
                    break;
                    case  "Active":
                    db.Database.ExecuteSqlCommand("UPDATE [dbo].[Property] SET Avaliability = 1 WHERE ID = {0}", renewal.PropertyID);
                    break;
                    case "Block":
                    db.Database.ExecuteSqlCommand("UPDATE [dbo].[Property] SET Avaliability = 1 AND AllowBidding = 2 WHERE ID = {0}", renewal.PropertyID);
                    break;
                    case "Expired":
                    db.Database.ExecuteSqlCommand("UPDATE [dbo].[Property] SET Avaliability = 1 WHERE ID = {0}", renewal.PropertyID);
                    break;
                    default:
                    break;
                }

                db.Entry(renewal).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.PropertyIDList = new SelectList(db.Properties, "ID", "ID", renewal.PropertyID);
            ViewBag.LastEdit = DateTime.Now;
            return View(renewal);
        }

        // GET: Renewals/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Renewal renewal = await db.Renewals.FindAsync(id);
            if (renewal == null)
            {
                return HttpNotFound();
            }
            return View(renewal);
        }

        // POST: Renewals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Renewal renewal = await db.Renewals.FindAsync(id);
            db.Renewals.Remove(renewal);
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
