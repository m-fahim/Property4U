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
using System.IO;

namespace Property4U.Controllers
{
    public class OrdersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private string strCurrentUserId;

        [Authorize(Roles = "Agent, Admin")]
        // GET: Orders
        public async Task<ActionResult> Index()
        {
            IQueryable<Order> orders = null;
            if (Request.IsAuthenticated && User.IsInRole("Admin"))
            {
                orders = db.Orders.Include(o => o.Agent);
            }
            else 
            {
                strCurrentUserId = User.Identity.GetUserId();
                orders = db.Orders.Include(o => o.Agent).Where(o => o.AgentID.Equals(strCurrentUserId));
            }
            return View(await orders.ToListAsync());
        }

        [Authorize(Roles = "Agent, Admin")]
        // GET: Orders/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = await db.Orders.FindAsync(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        [Authorize(Roles = "Agent")]
        // GET: Orders/Create
        public async Task<ActionResult> Create()
        {
            strCurrentUserId = User.Identity.GetUserId();
            var ownerAgent = await db.Users.Where(d => d.Id == strCurrentUserId).ToListAsync();
            ViewBag.AgentIDList = new SelectList(ownerAgent, "Id", "Id");
            ViewBag.Date = DateTime.Now.ToString("yyyy-MM-dd");
            ViewBag.OrderStatus = "Pending";
            return View();
        }

        [Authorize(Roles = "Agent")]
        // POST: Orders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,AgentID,Title,Description,Size,NumberOfAds,ZipFilePath,ZipFileSize,AdsDuration,Date,OrderStatus,Remedies,LastEdit")] Order order, HttpPostedFileBase orderFile)
        {
            if (orderFile == null)
            {
                ModelState.AddModelError(string.Empty, "Order zip file must be chosen.");
            }

            if (ModelState.IsValid)
            {
                if (orderFile != null)
                {
                    string OrderFileEx = Path.GetExtension(orderFile.FileName);
                    double OrderFileSize = orderFile.ContentLength;
                    string OrderCustomName = order.Title + "-" + order.Date.Date.ToString("MM-dd-yy") + "-" + System.Guid.NewGuid().ToString("N") + OrderFileEx;
                    string configOrderToPath = Path.Combine(Server.MapPath("~/Content/Uploads/Orders"), OrderCustomName);

                    // New file is uploaded
                    orderFile.SaveAs(configOrderToPath);
                    order.ZipFilePath = OrderCustomName;
                    order.ZipFileSize = OrderFileSize;
                }
                db.Orders.Add(order);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            strCurrentUserId = User.Identity.GetUserId();
            var ownerAgent = await db.Users.Where(d => d.Id == strCurrentUserId).ToListAsync();
            ViewBag.AgentIDList = new SelectList(ownerAgent, "Id", "Id", order.AgentID);
            ViewBag.Date = DateTime.Now.ToString("yyyy-MM-dd");
            ViewBag.OrderStatus = "Pending";
            return View(order);
        }

        [Authorize(Roles = "Agent")]
        // GET: Orders/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = await db.Orders.FindAsync(id);
            if (order == null)
            {
                return HttpNotFound();
            }

            strCurrentUserId = User.Identity.GetUserId();
            var ownerAgent = await db.Users.Where(d => d.Id == strCurrentUserId).ToListAsync();
            ViewBag.AgentIDList = new SelectList(ownerAgent, "Id", "Id", order.AgentID);
            ViewBag.OrderStatus = order.OrderStatus;
            ViewBag.LastEdit = DateTime.Now;
            return View(order);
        }

        [Authorize(Roles = "Agent")]
        // POST: Orders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,AgentID,Title,Description,Size,NumberOfAds,ZipFilePath,ZipFileSize,AdsDuration,Date,OrderStatus,Remedies,LastEdit")] Order order, HttpPostedFileBase orderFile, string oldOrderPath)
        {
            if (ModelState.IsValid)
            {
                if (orderFile != null)
                {
                    string OrderFileEx = Path.GetExtension(orderFile.FileName);
                    string OrderCustomName = order.Title + "-" + order.Date.Date.ToString("MM-dd-yy") + "-" + System.Guid.NewGuid().ToString("N") + OrderFileEx;
                    string configOrderToPath = Path.Combine(Server.MapPath("~/Content/Uploads/Orders"), OrderCustomName);

                    if (!System.IO.File.Exists(configOrderToPath))
                    {
                        // Delete previously uploaded file
                        System.IO.File.Delete(Path.Combine(Server.MapPath("~/Content/Uploads/Orders"), oldOrderPath));
                        // New file is uploaded
                        orderFile.SaveAs(configOrderToPath);
                        order.ZipFilePath = OrderCustomName;
                        order.ZipFileSize = orderFile.ContentLength;
                    }
                }
                db.Entry(order).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            strCurrentUserId = User.Identity.GetUserId();
            var ownerAgent = await db.Users.Where(d => d.Id == strCurrentUserId).ToListAsync();
            ViewBag.AgentIDList = new SelectList(ownerAgent, "Id", "Id", order.AgentID);
            ViewBag.OrderStatus = order.OrderStatus;
            ViewBag.LastEdit = DateTime.Now;
            return View(order);
        }

        [Authorize(Roles = "Agent")]
        // GET: Orders/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = await db.Orders.FindAsync(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        [Authorize(Roles = "Agent")]
        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Order order = await db.Orders.FindAsync(id);

            string configOrderToPath = Path.Combine(Server.MapPath("~/Content/Uploads/Orders"), order.ZipFilePath);
            if (System.IO.File.Exists(configOrderToPath))
            {
                // Delete uploaded zip file
                System.IO.File.Delete(configOrderToPath);
            }

            db.Orders.Remove(order);
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
