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
    [Authorize(Roles = "Member")]
    public class BidsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private string strCurrentUserId;

        // GET: Bids
        public async Task<ActionResult> Index()
        {
            strCurrentUserId = User.Identity.GetUserId();
            var bids = db.Bids.Include(b => b.Bidding).Include(b => b.Member).Where(m => m.MemberID.Equals(strCurrentUserId));
            return View( await bids.ToListAsync());
        }

        // GET: Bids/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bid bid = await db.Bids.FindAsync(id);
            if (bid == null)
            {
                return HttpNotFound();
            }
            return View(bid);
        }

        // GET: Bids/Create
        public async Task<ActionResult> Create(int? PID)
        {
            // Only Biddings with Status Active
            ViewBag.BiddingIDList = new SelectList(await db.Biddings.Where(b => b.BiddingStatus.ToString().Equals("Active")).ToListAsync(), "ID", "ID");
            strCurrentUserId = User.Identity.GetUserId();
            var ownerMember = await db.Users.Where(d => d.Id == strCurrentUserId).ToListAsync();
            ViewBag.MemberIDList = new SelectList(ownerMember, "Id", "Id");
            ViewBag.BidOn = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff tt");
            ViewBag.BidStatus = "Process";
            return View();
        }

        // POST: Bids/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,BiddingID,MemberID,Title,Description,Price,BidOn,BidStatus,LastEdit")] Bid bidO)
        {
            if (ModelState.IsValid)
            {
                db.Bids.Add(bidO);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.BiddingIDList = new SelectList(await db.Biddings.Where(b => b.BiddingStatus.ToString().Equals("Active")).ToListAsync(), "ID", "ID", bidO.BiddingID);
            strCurrentUserId = User.Identity.GetUserId();
            var ownerMember = await db.Users.Where(d => d.Id == strCurrentUserId).ToListAsync();
            ViewBag.MemberIDList = new SelectList(ownerMember, "Id", "Id", bidO.MemberID);
            ViewBag.BidOn = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff tt");
            ViewBag.BidStatus = "Process";
            return View(bidO);
        }

        //// GET: Bids/Edit/5
        //public async Task<ActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Bid bid = await db.Bids.FindAsync(id);
        //    if (bid == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.BiddingIDList = new SelectList(await db.Biddings.Where(b => b.BiddingStatus.ToString().Equals("Active")).ToListAsync(), "ID", "Title", bid.BiddingID);
        //    strCurrentUserId = User.Identity.GetUserId();
        //    var ownerMember = await db.Users.Where(d => d.Id == strCurrentUserId).ToListAsync();
        //    ViewBag.MemberIDList = new SelectList(ownerMember, "Id", "Id", bid.MemberID);
        //    ViewBag.BidStatus = bid.BidStatus;
        //    return View(bid);
        //}

        //// POST: Bids/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Edit([Bind(Include = "ID,BiddingID,MemberID,Title,Description,Price,BidOn,BidStatus")] Bid bid)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(bid).State = EntityState.Modified;
        //        await db.SaveChangesAsync();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.BiddingIDList = new SelectList(await db.Biddings.Where(b => b.BiddingStatus.ToString().Equals("Active")).ToListAsync(), "ID", "Title", bid.BiddingID);
        //    strCurrentUserId = User.Identity.GetUserId();
        //    var ownerMember = await db.Users.Where(d => d.Id == strCurrentUserId).ToListAsync();
        //    ViewBag.MemberIDList = new SelectList(ownerMember, "Id", "Id", bid.MemberID);
        //    ViewBag.BidStatus = bid.BidStatus;
        //    return View(bid);
        //}

        // GET: Bids/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bid bid = await db.Bids.FindAsync(id);
            if (bid == null)
            {
                return HttpNotFound();
            }
            return View(bid);
        }

        // POST: Bids/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Bid bid = await db.Bids.FindAsync(id);
            db.Bids.Remove(bid);
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
