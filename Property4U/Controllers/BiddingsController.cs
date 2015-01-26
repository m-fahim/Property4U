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
using Property4U.ViewModels;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace Property4U.Controllers
{
    public class BiddingsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private string strCurrentUserId;

        [Authorize(Roles = "Agent")]
        // GET: Biddings
        public async Task<ActionResult> Index(int? id)
        {
            var viewModel = new BiddingIndexData();
            strCurrentUserId = User.Identity.GetUserId();
            var biddings = db.Biddings.Include(b => b.Property).Where(b => b.Property.AgentID.Equals(strCurrentUserId));
            viewModel.Biddings = await biddings.ToListAsync();

            if (id != null)
            {
                ViewBag.BiddingID = id.Value;
                viewModel.Bids = viewModel.Biddings.Where(
                    x => x.ID == id).Single().Bids;
            }

            return View(viewModel);

        }

        [Authorize(Roles = "Agent")]
        // GET: Biddings/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bidding bidding = await db.Biddings.FindAsync(id);
            if (bidding == null)
            {
                return HttpNotFound();
            }
            return View(bidding);
        }

        [Authorize(Roles = "Agent")]
        // GET: Biddings/Create
        public async Task<ActionResult> Create()
        {
            strCurrentUserId = User.Identity.GetUserId();
            ViewBag.PropertyIDList = new SelectList(await db.Properties.Where(p => p.AllowBidding.ToString().Equals("Allowed") && p.Availability.ToString().Equals("Yes") && p.AgentID.Equals(strCurrentUserId)).ToListAsync(), "ID", "ID");
            ViewBag.PostedOn = DateTime.Now.ToString("yyyy-MM-dd");
            ViewBag.BiddingStatus = "Active";
            return View();
        }

        [Authorize(Roles = "Agent")]
        // POST: Biddings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,PropertyID,Title,StartDate,EndDate,MinExp,MaxExp,WinningBid,PostedOn,BiddingStatus,LastEdit")] Bidding bidding)
        {
            if (ModelState.IsValid)
            {
                db.Biddings.Add(bidding);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            strCurrentUserId = User.Identity.GetUserId();
            ViewBag.PropertyIDList = new SelectList(await db.Properties.Where(p => p.AllowBidding.ToString().Equals("Allowed") && p.Availability.ToString().Equals("Yes") && p.AgentID.Equals(strCurrentUserId)).ToListAsync(), "ID", "ID", bidding.PropertyID);
            ViewBag.PostedOn = DateTime.Now.ToString("yyyy-MM-dd");
            ViewBag.BiddingStatus = "Active";
            return View(bidding);
        }

        [Authorize(Roles = "Agent")]
        // GET: Biddings/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bidding bidding = await db.Biddings.FindAsync(id);
            if (bidding == null)
            {
                return HttpNotFound();
            }

            strCurrentUserId = User.Identity.GetUserId();
            ViewBag.PropertyIDList = new SelectList(await db.Properties.Where(p => p.AllowBidding.ToString().Equals("Allowed") && p.Availability.ToString().Equals("Yes") && p.AgentID.Equals(strCurrentUserId)).ToListAsync(), "ID", "ID", bidding.PropertyID);
            ViewBag.BiddingStatus = bidding.BiddingStatus;
            ViewBag.LastEdit = DateTime.Now;
            return View(bidding);
        }

        [Authorize(Roles = "Agent")]
        // POST: Biddings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,PropertyID,Title,StartDate,EndDate,MinExp,MaxExp,WinningBid,PostedOn,BiddingStatus,LastEdit")] Bidding bidding)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bidding).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            strCurrentUserId = User.Identity.GetUserId();
            ViewBag.PropertyIDList = new SelectList(await db.Properties.Where(p => p.AllowBidding.ToString().Equals("Allowed") && p.Availability.ToString().Equals("Yes") && p.AgentID.Equals(strCurrentUserId)).ToListAsync(), "ID", "ID", bidding.PropertyID);
            ViewBag.BiddingStatus = bidding.BiddingStatus;
            ViewBag.LastEdit = DateTime.Now;
            return View(bidding);
        }

        [Authorize(Roles = "Agent")]
        // GET: Biddings/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bidding bidding = await db.Biddings.FindAsync(id);
            if (bidding == null)
            {
                return HttpNotFound();
            }
            return View(bidding);
        }

        [Authorize(Roles = "Agent")]
        // POST: Biddings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Bidding bidding = await db.Biddings.FindAsync(id);
            db.Biddings.Remove(bidding);
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

        [Authorize(Roles = "Member")]
        public ActionResult Inquiry(string sortOrder, string currentFilter, int? id, int? biddingID)
        {
            ViewBag.CurrentSort = sortOrder;
            //ViewBag.TypeSortParm = String.IsNullOrEmpty(sortOrder) ? "type_desc" : "";
            ViewBag.ForSortParm = String.IsNullOrEmpty(sortOrder) ? "for_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            ViewBag.CurrentFilter = currentFilter;

            var viewModel = new BiddingIndexData();
            viewModel.Properties = db.Properties
                .Include(p => p.Address)
                .Include(p => p.Agent)
                //.OrderBy(p => p.Title)
                .Where(p => p.AllowBidding.ToString().Equals("Allowed") && p.Availability.ToString().Equals("Yes"));

            if (id != null)
            {
                ViewBag.PropertyID = id.Value;
                viewModel.Biddings = viewModel.Properties.Where(
                    i => i.ID == id.Value).Single().Biddings;
            }

            if (biddingID != null)
            {
                ViewBag.BiddingID = biddingID.Value;
                viewModel.Bids = viewModel.Biddings.Where(
                    x => x.ID == biddingID).Single().Bids;
            }

            switch (sortOrder)
            {
                //case "type_desc":
                //    viewModel.Properties = viewModel.Properties.OrderByDescending(p => p.OfType);
                //    break;
                case "for_desc":
                    viewModel.Properties = viewModel.Properties.OrderByDescending(p => p.For);
                    break;
                case "Date":
                    viewModel.Properties = viewModel.Properties.OrderBy(p => p.PublishOn);
                    break;
                case "date_desc":
                    viewModel.Properties = viewModel.Properties.OrderByDescending(p => p.PublishOn);
                    break;
                default:
                    viewModel.Properties = viewModel.Properties.OrderBy(p => p.Title);
                    break;
            }

            return View(viewModel);
        }
    }
}
