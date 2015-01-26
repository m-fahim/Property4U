using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using IdentitySample.Models;
using Property4U.Models;
using Microsoft.AspNet.Identity;

namespace Property4U.Areas.HelpPage.Controllers
{
    public class OrdersController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private string strCurrentUserId;

        /// <summary>
        /// Authorize Roles - Agent
        /// </summary>

        [Authorize(Roles = "Developer, Agent")]
        // GET: api/GetAllAlerts
        public List<HomeForViewModel> GetAllAlerts()
        {
            strCurrentUserId = User.Identity.GetUserId();
            List<HomeForViewModel> alertsL = new List<HomeForViewModel>();
            var Orders = db.Orders.Where(o => o.AgentID == strCurrentUserId && o.OrderStatus.ToString().Equals("Approved"))
                     .GroupBy(o => o.AgentID)
                     .Select(o => new HomeForViewModel
                     {
                         Title = "Orders (Approved)",
                         Count = o.Count()
                     }).FirstOrDefault();
            if (Orders != null)
                alertsL.Add(Orders);

            var Reviews = db.Reviews.Where(rev => rev.Property.AgentID == strCurrentUserId)
                     .GroupBy(rev => rev.Property.AgentID)
                     .Select(rev => new HomeForViewModel
                     {
                         Title = "Reviews",
                         Count = rev.Count()
                     }).FirstOrDefault();
            if (Reviews != null)
                alertsL.Add(Reviews);

            var Renewals = db.Renewals.Where(r => r.Property.AgentID == strCurrentUserId && r.Status.ToString().Equals("Active"))
                     .GroupBy(r => r.Property.AgentID)
                     .Select(r => new HomeForViewModel
                     {
                         Title = "Renewals (Active)",
                         Count = r.Count()
                     }).FirstOrDefault();
            if (Renewals != null)
                alertsL.Add(Renewals);

            return alertsL.ToList();
        }

        /// <summary>
        /// Authorize Roles - Admin
        /// </summary>

        [Authorize(Roles = "Developer, Admin")]
        // GET: api/GetRecentPendingOrders
        public IEnumerable<Order> GetRecentPendingOrders()
        {
            return db.Orders.Where(o => o.OrderStatus.ToString().Equals("Pending")).ToList();
        }

        /// <summary>
        /// Authorize Roles - Admin
        /// </summary>

        [Authorize(Roles = "Developer, Admin")]
        // GET: api/GetPendingOrders - ControlDeskActiveAD
        public IEnumerable<ObjectCon> GetPendingOrders()
        {
            var ordersP = db.Orders.Where(r => r.OrderStatus.ToString().Equals("Pending")).ToList();
            int CountItemsP = ordersP.ToList().Count;

            List<ObjectCon> ordersL = new List<ObjectCon>();
            ordersL.Add(new ObjectCon { ObjectValuesCount = CountItemsP });

            return ordersL;
        }

        //// GET: api/GetPendingOrdersAG - ControlDeskActiveAG
        //public IEnumerable<ObjectCon> GetPendingOrdersAG()
        //{
        //    strCurrentUserId = User.Identity.GetUserId();
        //    var ordersP = db.Orders.Where(r => r.AgentID.Equals(strCurrentUserId) && r.OrderStatus.ToString().Equals("Pending")).ToList();
        //    int CountItemsP = ordersP.ToList().Count;

        //    List<ObjectCon> ordersL = new List<ObjectCon>();
        //    ordersL.Add(new ObjectCon { ObjectValuesCount = CountItemsP });

        //    return ordersL;
        //}

        //// GET: api/GetRejectedOrdersAG - ControlDeskActiveAG
        //public IEnumerable<ObjectCon> GetRejectedOrdersAG()
        //{
        //    var ordersR = db.Orders.Where(r => r.AgentID.Equals(strCurrentUserId) && r.OrderStatus.ToString().Equals("Rejected")).ToList();
        //    int CountItemsE = ordersR.ToList().Count;

        //    List<ObjectCon> ordersL = new List<ObjectCon>();
        //    ordersL.Add(new ObjectCon { ObjectValuesCount = CountItemsE });

        //    return ordersL;
        //}

        /// <summary>
        /// Authorize Roles - Admin, Agent
        /// </summary>

        [Authorize(Roles = "Developer, Agent, Admin")]
        // GET: api/Orders
        public IEnumerable<Order> GetOrders()
        {
            strCurrentUserId = User.Identity.GetUserId();
            if (User.IsInRole("Agent"))
                return db.Orders.Where(o => o.AgentID == strCurrentUserId).ToList();
            else
                return db.Orders.ToList();
        }

        /// <summary>
        /// Authorize Roles - Admin, Agent
        /// </summary>

        [Authorize(Roles = "Developer, Agent, Admin")]
        // GET: api/Orders/5
        [ResponseType(typeof(Order))]
        public async Task<IHttpActionResult> GetOrder(int id)
        {
            Order order = await db.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        /// <summary>
        /// Authorize Roles - Agent
        /// </summary>

        [Authorize(Roles = "Developer, Agent")]
        // PUT: api/Orders/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutOrder(int id, Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != order.ID)
            {
                return BadRequest();
            }

            db.Entry(order).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Authorize Roles - Agent
        /// </summary>

        [Authorize(Roles = "Developer, Agent")]
        // POST: api/Orders
        [ResponseType(typeof(Order))]
        public async Task<IHttpActionResult> PostOrder(Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Orders.Add(order);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = order.ID }, order);
        }

        /// <summary>
        /// Authorize Roles - Agent
        /// </summary>

        [Authorize(Roles = "Developer, Agent")]
        // DELETE: api/Orders/5
        [ResponseType(typeof(Order))]
        public async Task<IHttpActionResult> DeleteOrder(int id)
        {
            Order order = await db.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            db.Orders.Remove(order);
            await db.SaveChangesAsync();

            return Ok(order);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool OrderExists(int id)
        {
            return db.Orders.Count(e => e.ID == id) > 0;
        }
    }
}