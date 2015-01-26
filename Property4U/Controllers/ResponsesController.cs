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
using System.Diagnostics;
using System.Threading.Tasks;

namespace Property4U.Controllers
{
    public class ResponsesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private string strCurrentUserId;
        private int reqStatus;

        [Authorize(Roles = "Agent, Member")]
        // GET: responses
        public async Task<ActionResult> Index()
        {
            // Filter out Requests for Agent by Members and Member showed only his requests
            IQueryable<Response> responses = null;
            if (Request.IsAuthenticated && User.IsInRole("Member"))
            {
                strCurrentUserId = User.Identity.GetUserId();
                responses = db.Responses.Include(r => r.Agent).Include(r => r.Request).Where(r => r.Request.MemberID.Equals(strCurrentUserId));
            }
            else
            {
                strCurrentUserId = User.Identity.GetUserId();
                responses = db.Responses.Include(r => r.Agent).Include(r => r.Request).Where(r => r.AgentID.Equals(strCurrentUserId));
            }
            return View( await responses.ToListAsync());
        }

        [Authorize(Roles = "Agent, Member")]
        // GET: Responses/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Response response = await db.Responses.FindAsync(id);
            if (response == null)
            {
                return HttpNotFound();
            }
            return View(response);
        }

        [Authorize(Roles = "Agent")]
        // GET: Responses/Create
        public async Task<ActionResult> Create()
        {
            strCurrentUserId = User.Identity.GetUserId();
            var ownerAgent = await db.Users.Where(d => d.Id == strCurrentUserId).ToListAsync();
            ViewBag.AgentIDList = new SelectList(ownerAgent, "Id", "Id");
            // Get Requests belong to current Agent's Property
            ViewBag.RequestIDList = new SelectList(await db.Requests.Where(r => r.Property.AgentID.Equals(strCurrentUserId)).ToListAsync(), "ID", "ID");
            ViewBag.ResponseOn = DateTime.Now.ToString("yyyy-MM-dd");

            return View();
        }

        [Authorize(Roles = "Agent")]
        // POST: Responses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,RequestID,AgentID,Title,Description,ResponseOn,ResponseStatus,LastEdit")] Response response)
        {
            if (ModelState.IsValid)
            {
                // Update Request db Status accordingly
                reqStatus = (response.ResponseStatus.ToString().Equals("Agree")) ? 2 : 3;
                db.Database.ExecuteSqlCommand("UPDATE Request SET RequestStatus = {0} WHERE ID = {1}", reqStatus, response.RequestID);
                db.Responses.Add(response);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            strCurrentUserId = User.Identity.GetUserId();
            var ownerAgent = await db.Users.Where(d => d.Id == strCurrentUserId).ToListAsync();
            ViewBag.AgentIDList = new SelectList(ownerAgent, "Id", "Id", response.AgentID);
            // Get Requests belong to current Agent's Property
            ViewBag.RequestIDList = new SelectList(await db.Requests.Where(r => r.Property.AgentID.Equals(strCurrentUserId)).ToListAsync(), "ID", "ID", response.RequestID);
            ViewBag.ResponseOn = DateTime.Now.ToString("yyyy-MM-dd");
            return View(response);
        }

        [Authorize(Roles = "Agent")]
        // GET: Responses/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Response response = await db.Responses.FindAsync(id);
            if (response == null)
            {
                return HttpNotFound();
            }
            strCurrentUserId = User.Identity.GetUserId();
            var ownerAgent = await db.Users.Where(d => d.Id == strCurrentUserId).ToListAsync();
            ViewBag.AgentIDList = new SelectList(ownerAgent, "Id", "Id", response.AgentID);
            // Get Requests belong to current Agent's Property
            ViewBag.RequestIDList = new SelectList(await db.Requests.Where(r => r.Property.AgentID.Equals(strCurrentUserId)).ToListAsync(), "ID", "ID", response.RequestID);
            ViewBag.LastEdit = DateTime.Now;
            return View(response);
        }

        [Authorize(Roles = "Agent")]
        // POST: Responses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,RequestID,AgentID,Title,Description,ResponseOn,ResponseStatus,LastEdit")] Response response)
        {
            if (ModelState.IsValid)
            {
                Debug.WriteLine(response.ResponseStatus.ToString());
                // Update Request db Status accordingly
                reqStatus = (response.ResponseStatus.ToString().Equals("Agree")) ? 2 : 3;
                db.Database.ExecuteSqlCommand("UPDATE Request SET RequestStatus = {0} Where ID = {1}", reqStatus, response.RequestID);
                db.Entry(response).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            strCurrentUserId = User.Identity.GetUserId();
            var ownerAgent = await db.Users.Where(d => d.Id == strCurrentUserId).ToListAsync();
            ViewBag.AgentIDList = new SelectList(ownerAgent, "Id", "Id", response.AgentID);
            // Get Requests belong to current Agent's Property
            ViewBag.RequestIDList = new SelectList(await db.Requests.Where(r => r.Property.AgentID.Equals(strCurrentUserId)).ToListAsync(), "ID", "ID", response.RequestID);
            ViewBag.LastEdit = DateTime.Now;
            return View(response);
        }

        [Authorize(Roles = "Agent")]
        // GET: Responses/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Response response = await db.Responses.FindAsync(id);
            if (response == null)
            {
                return HttpNotFound();
            }
            return View(response);
        }

        [Authorize(Roles = "Agent")]
        // POST: Responses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Response response = await db.Responses.FindAsync(id);
            db.Responses.Remove(response);
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
