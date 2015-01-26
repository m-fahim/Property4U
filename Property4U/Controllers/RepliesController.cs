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
    [Authorize(Roles = "Agent")]
    public class RepliesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private string strCurrentUserId;

        // GET: Replies
        public async Task<ActionResult> Index()
        {
            strCurrentUserId = User.Identity.GetUserId();
            var replies = db.Replies.Include(r => r.Agent).Include(r => r.Review).Where(r => r.AgentID.Equals(strCurrentUserId));
            return View(await replies.ToListAsync());
        }

        // GET: Replies/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reply reply = await db.Replies.FindAsync(id);
            if (reply == null)
            {
                return HttpNotFound();
            }
            return View(reply);
        }

        // GET: Replies/Create
        public async Task<ActionResult> Create()
        {
            strCurrentUserId = User.Identity.GetUserId(); 
            var ownerAgent = await db.Users.Where(d => d.Id == strCurrentUserId).ToListAsync();
            ViewBag.AgentIDList = new SelectList(ownerAgent, "Id", "ID");
            ViewBag.ReviewIDList = new SelectList(await db.Reviews.Where(p => p.Property.AgentID.ToString().Equals(strCurrentUserId)).ToListAsync(), "ID", "ID");
            ViewBag.ReplyOn = DateTime.Now;
            return View();
        }

        // POST: Replies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,AgentID,ReviewID,Description,ReplyOn")] Reply reply)
        {
            if (ModelState.IsValid)
            {
                db.Replies.Add(reply);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            strCurrentUserId = User.Identity.GetUserId();
            var ownerAgent = await db.Users.Where(d => d.Id == strCurrentUserId).ToListAsync();
            ViewBag.AgentIDList = new SelectList(ownerAgent, "Id", "ID");
            ViewBag.ReviewIDList = new SelectList(await db.Reviews.Where(p => p.Property.AgentID.ToString().Equals(strCurrentUserId)).ToListAsync(), "ID", "ID");
            ViewBag.ReplyOn = DateTime.Now;
            return View(reply);
        }

        // GET: Replies/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reply reply = await db.Replies.FindAsync(id);
            if (reply == null)
            {
                return HttpNotFound();
            }

            strCurrentUserId = User.Identity.GetUserId();
            var ownerAgent = await db.Users.Where(d => d.Id == strCurrentUserId).ToListAsync();
            ViewBag.AgentIDList = new SelectList(ownerAgent, "Id", "ID");
            ViewBag.ReviewIDList = new SelectList(await db.Reviews.Where(p => p.Property.AgentID.ToString().Equals(strCurrentUserId)).ToListAsync(), "ID", "ID");
            ViewBag.ReplyOn = reply.ReplyOn;
            return View(reply);
        }

        // POST: Replies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,AgentID,ReviewID,Description,ReplyOn")] Reply reply)
        {
            if (ModelState.IsValid)
            {
                db.Entry(reply).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            strCurrentUserId = User.Identity.GetUserId();
            var ownerAgent = await db.Users.Where(d => d.Id == strCurrentUserId).ToListAsync();
            ViewBag.AgentIDList = new SelectList(ownerAgent, "Id", "ID");
            ViewBag.ReviewIDList = new SelectList(await db.Reviews.Where(p => p.Property.AgentID.ToString().Equals(strCurrentUserId)).ToListAsync(), "ID", "ID");
            ViewBag.ReplyOn = reply.ReplyOn;
            return View(reply);
        }

        // GET: Replies/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reply reply = await db.Replies.FindAsync(id);
            if (reply == null)
            {
                return HttpNotFound();
            }
            return View(reply);
        }

        // POST: Replies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Reply reply = await db.Replies.FindAsync(id);
            db.Replies.Remove(reply);
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
