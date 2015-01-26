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

namespace Property4U.Controllers
{
    public class RequestsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private string strCurrentUserId;

        [Authorize(Roles = "Agent, Member")]
        // GET: Requests
        public async Task<ActionResult> Index()
        {
            // Filter out Requests for Agent by Members and Member showed only his requests
            IQueryable<Request> requests = null;
            if (Request.IsAuthenticated && User.IsInRole("Agent"))
            {
                strCurrentUserId = User.Identity.GetUserId();
                requests = db.Requests.Include(r => r.Member).Include(r => r.Property).Where(a => a.Property.AgentID.Equals(strCurrentUserId));
            }
            else
            {
                strCurrentUserId = User.Identity.GetUserId();
                requests = db.Requests.Include(r => r.Member).Include(r => r.Property).Where(m => m.MemberID.Equals(strCurrentUserId));
            }
            return View( await requests.ToListAsync());
        }

        [Authorize(Roles = "Agent, Member")]
        // GET: Requests/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Request request = await db.Requests.FindAsync(id);
            if (request == null)
            {
                return HttpNotFound();
            }
            return View(request);
        }


        [Authorize(Roles = "Member")]
        // GET: Requests/Create
        public async Task<ActionResult> Create(int? BID)
        {
            strCurrentUserId = User.Identity.GetUserId();
            var ownerMember = await db.Users.Where(d => d.Id == strCurrentUserId).ToListAsync();
            ViewBag.MemberIDList = new SelectList(ownerMember, "Id", "ID");
            ViewBag.PropertyIDList = new SelectList(await db.Properties.Where(p => p.Availability.ToString().Equals("Yes")).ToListAsync(), "ID", "ID");
            ViewBag.RequestOn = DateTime.Now.ToString("yyyy-MM-dd");
            ViewBag.RequestStatus = "Pending";
            return View();
        }

        [Authorize(Roles = "Member")]
        // POST: Requests/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,MemberID,PropertyID,Title,Description,VisitingDate,VisitingTime,RequestOn,RequestStatus,LastEdit")] Request request)
        {
            if (ModelState.IsValid)
            {
                db.Requests.Add(request);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            strCurrentUserId = User.Identity.GetUserId();
            var ownerMember = await db.Users.Where(d => d.Id == strCurrentUserId).ToListAsync();
            ViewBag.MemberIDList = new SelectList(ownerMember, "Id", "ID", request.MemberID);
            ViewBag.PropertyIDList = new SelectList(await db.Properties.Where(p => p.Availability.ToString().Equals("Yes")).ToListAsync(), "ID", "ID", request.PropertyID);
            ViewBag.RequestOn = DateTime.Now.ToString("yyyy-MM-dd");
            ViewBag.RequestStatus = "Pending";
            return View(request);
        }

        [Authorize(Roles = "Member")]
        // GET: Requests/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Request request = await db.Requests.FindAsync(id);
            if (request == null)
            {
                return HttpNotFound();
            }

            strCurrentUserId = User.Identity.GetUserId();
            var ownerMember = await db.Users.Where(d => d.Id == strCurrentUserId).ToListAsync();
            ViewBag.MemberIDList = new SelectList(ownerMember, "Id", "ID", request.MemberID);
            ViewBag.PropertyIDList = new SelectList(await db.Properties.Where(p => p.Availability.ToString().Equals("Yes")).ToListAsync(), "ID", "ID", request.PropertyID);
            ViewBag.RequestStatus = request.RequestStatus;
            ViewBag.LastEdit = DateTime.Now;
            return View(request);
        }

        [Authorize(Roles = "Member")]
        // POST: Requests/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,MemberID,PropertyID,Title,Description,VisitingDate,VisitingTime,RequestOn,RequestStatus,LastEdit")] Request request)
        {
            if (ModelState.IsValid)
            {
                db.Entry(request).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            strCurrentUserId = User.Identity.GetUserId();
            var ownerMember = await db.Users.Where(d => d.Id == strCurrentUserId).ToListAsync();
            ViewBag.MemberIDList = new SelectList(ownerMember, "Id", "ID", request.MemberID);
            ViewBag.PropertyIDList = new SelectList(await db.Properties.Where(p => p.Availability.ToString().Equals("Yes")).ToListAsync(), "ID", "ID", request.PropertyID);
            ViewBag.RequestStatus = request.RequestStatus;
            ViewBag.LastEdit = DateTime.Now;
            return View(request);
        }

        [Authorize(Roles = "Member")]
        // GET: Requests/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Request request = await db.Requests.FindAsync(id);
            if (request == null)
            {
                return HttpNotFound();
            }
            return View(request);
        }

        [Authorize(Roles = "Member")]
        // POST: Requests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Request request = await db.Requests.FindAsync(id);
            db.Requests.Remove(request);
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
