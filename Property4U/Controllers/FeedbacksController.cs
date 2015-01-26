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
    public class FeedbacksController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private string strCurrentUserId;

        [Authorize(Roles = "Admin, Member")]
        // GET: Feedbacks
        public async Task<ActionResult> Index(bool? reportAbuse)
        {
            strCurrentUserId = User.Identity.GetUserId();
            IQueryable<Feedback> feedbacks = null;
            if (User.IsInRole("Admin"))
            {
                if (reportAbuse == null)
                {
                    feedbacks = db.Feedbacks.Include(f => f.Member).Include(f => f.Properties).Where(m => m.For.ToString().Equals("Process_Feedback"));
                }
                else if (reportAbuse == true)
                {
                    feedbacks = db.Feedbacks.Include(f => f.Member).Include(f => f.Properties).Where(m => m.For.ToString().Equals("Report_Abuse"));
                }
            }
            else {
                if (reportAbuse == null)
                {
                    feedbacks = db.Feedbacks.Include(f => f.Member).Include(f => f.Properties).Where(m => m.MemberID.Equals(strCurrentUserId) && m.For.ToString().Equals("Process_Feedback"));
                }
                else if (reportAbuse == true)
                {
                    feedbacks = db.Feedbacks.Include(f => f.Member).Include(f => f.Properties).Where(m => m.MemberID.Equals(strCurrentUserId) && m.For.ToString().Equals("Report_Abuse"));
                }
            }
            return View(await feedbacks.ToListAsync());
        }

        [Authorize(Roles = "Admin, Member")]
        // GET: Feedbacks/Details/5
        public async Task<ActionResult> Details(int? id, bool? reportAbuse)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Feedback feedback = await db.Feedbacks.FindAsync(id);
            if (feedback == null)
            {
                return HttpNotFound();
            }
            return View(feedback);
        }

        [Authorize(Roles = "Member")]
        // GET: Feedbacks/Create
        public async Task<ActionResult> Create(int? PID, bool? reportAbuse)
        {
            strCurrentUserId = User.Identity.GetUserId();
            var ownerMember = await db.Users.Where(d => d.Id == strCurrentUserId).ToListAsync();
            ViewBag.MemberIDList = new SelectList(ownerMember, "Id", "Id");

            // Allow Feeback to those Properties for which the Member Requested for visted with RequestStatus "Accepted" 
            var allowedFeebackProperties = await db.Properties.SqlQuery("SELECT p.* FROM Property p INNER JOIN Request rq ON p.ID = rq.PropertyID WHERE MemberID = {0} AND RequestStatus = 2", strCurrentUserId).ToListAsync();

            ViewBag.PropertyIDList = new SelectList(allowedFeebackProperties, "ID", "ID");
            ViewBag.FeedbackOn = DateTime.Now.ToString("yyyy-MM-dd");
            return View();
        }

        [Authorize(Roles = "Member")]
        // POST: Feedbacks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,PropertyID,MemberID,For,Title,Description,AgentRating,AgentReview,OverallExperience,FeedbackOn,LastEdit")] Feedback feedback, int? PID, bool? reportAbuse)
        {
            if (ModelState.IsValid)
            {
                if (feedback.For.ToString().Equals("Report_Abuse"))
                {
                    var property = await db.Properties.FindAsync(feedback.PropertyID);
                    var flagCount = (property.Flags == null) ? 1 : (property.Flags + 1);
                    // Report Abuse > 5 - Availability Disabled
                    var propertyState = (flagCount < 5) ? "0" : "1";
                    db.Database.ExecuteSqlCommand("UPDATE [dbo].[Property] SET Flags = {0}, Availability = {1} WHERE ID = {2}", flagCount, propertyState, feedback.PropertyID);
                }
                db.Feedbacks.Add(feedback);
                await db.SaveChangesAsync();

                // Redirect According to type For
                if (feedback.For.ToString().Equals("Report_Abuse") || reportAbuse != null)
                {
                    return RedirectToAction("Index", new { reportAbuse = true });
                }else{
                    return RedirectToAction("Index");
                }
            }

            strCurrentUserId = User.Identity.GetUserId();
            var ownerMember = await db.Users.Where(d => d.Id == strCurrentUserId).ToListAsync();
            ViewBag.MemberIDList = new SelectList(ownerMember, "Id", "Id", feedback.MemberID);
            
            // Allow Feeback to those Properties for which the Member Requested for visted with RequestStatus "Accepted" 
            var allowedFeebackProperties = await db.Properties.SqlQuery("SELECT p.* FROM Property p INNER JOIN Request rq ON p.ID = rq.PropertyID WHERE MemberID = {0} AND RequestStatus = 2", strCurrentUserId).ToListAsync();

            ViewBag.PropertyIDList = new SelectList(allowedFeebackProperties, "ID", "ID", feedback.PropertyID);
            ViewBag.FeedbackOn = DateTime.Now.ToString("yyyy-MM-dd");
            return View(feedback);
        }

        // GET: Feedbacks/Edit/5
        //public async Task<ActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Feedback feedback = await db.Feedbacks.FindAsync(id);
        //    if (feedback == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    strCurrentUserId = User.Identity.GetUserId();
        //    var ownerMember = await db.Users.Where(d => d.Id == strCurrentUserId).ToListAsync();
        //    ViewBag.MemberIDList = new SelectList(ownerMember, "Id", "Id", feedback.MemberID);
        //    ViewBag.PropertyIDList = new SelectList(db.Properties, "ID", "ID", feedback.PropertyID);
        //    ViewBag.LastEdit = DateTime.Now;
        //    return View(feedback);
        //}

        //// POST: Feedbacks/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Edit([Bind(Include = "ID,PropertyID,MemberID,For,Title,Description,AgentRating,AgentReview,OverallExperience,FeedbackOn,LastEdit")] Feedback feedback)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(feedback).State = EntityState.Modified;
        //        await db.SaveChangesAsync();
        //        return RedirectToAction("Index");
        //    }
        //    strCurrentUserId = User.Identity.GetUserId();
        //    var ownerMember = await db.Users.Where(d => d.Id == strCurrentUserId).ToListAsync();
        //    ViewBag.MemberIDList = new SelectList(ownerMember, "Id", "Id", feedback.MemberID);
        //    ViewBag.PropertyIDList = new SelectList(db.Properties, "ID", "ID", feedback.PropertyID);
        //    ViewBag.LastEdit = DateTime.Now;
        //    return View(feedback);
        //}

        [Authorize(Roles = "Member")]
        // GET: Feedbacks/Delete/5
        public async Task<ActionResult> Delete(int? id, bool? reportAbuse)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Feedback feedback = await db.Feedbacks.FindAsync(id);
            if (feedback == null)
            {
                return HttpNotFound();
            }
            return View(feedback);
        }

        [Authorize(Roles = "Member")]
        // POST: Feedbacks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Feedback feedback = await db.Feedbacks.FindAsync(id);

            if (feedback.For.ToString().Equals("Report_Abuse"))
            {
                int? flagCount = 0;
                if(feedback.Properties.Flags != null){
                    flagCount = (feedback.Properties.Flags != 0)? (feedback.Properties.Flags- 1) : 0;
                }
                // Report Abuse > 5 - Availability Disabled
                var propertyState = (flagCount <= 5) ? 0 : 1;
                db.Database.ExecuteSqlCommand("UPDATE [dbo].[Property] SET Flags = {0}, Availability = {1} WHERE ID = {2}", flagCount, propertyState, feedback.PropertyID);
            }

            db.Feedbacks.Remove(feedback);
            await db.SaveChangesAsync();

            // Redirect According to type For
            if (feedback.For.ToString().Equals("Report_Abuse"))
            {
                return RedirectToAction("Index", new { reportAbuse = true });
            }
            else
            {
                return RedirectToAction("Index");
            }
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
