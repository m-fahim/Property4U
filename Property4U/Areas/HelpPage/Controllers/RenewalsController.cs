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

namespace Property4U.Areas.HelpPage.Controllers
{
    [Authorize(Roles = "Developer, Admin")]
    public class RenewalsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Authorize Roles - Admin
        /// </summary>

        // GET: api/GetPendingRenewals - ControlDeskActiveAD
        public IEnumerable<ObjectCon> GetPendingRenewals()
        {
            var renewals = db.Renewals.Where(r => r.Status.ToString().Equals("Inactive")).ToList();
            int CountItems = renewals.ToList().Count;
            double price = 0.00;
            foreach (var reItemsData in renewals)
            {
                price = price + reItemsData.Price;
            }

            List<ObjectCon> renewalsL = new List<ObjectCon>();
            renewalsL.Add(new ObjectCon { ObjectValue = price.ToString(), ObjectValuesCount = CountItems });

            return renewalsL;
        }

        /// <summary>
        /// Authorize Roles - Admin
        /// </summary>

        // GET: api/Renewals
        public IQueryable<Renewal> GetRenewals()
        {
            return db.Renewals;
        }

        /// <summary>
        /// Authorize Roles - Admin
        /// </summary>

        // GET: api/Renewals/5
        [ResponseType(typeof(Renewal))]
        public async Task<IHttpActionResult> GetRenewal(int id)
        {
            Renewal renewal = await db.Renewals.FindAsync(id);
            if (renewal == null)
            {
                return NotFound();
            }

            return Ok(renewal);
        }

        /// <summary>
        /// Authorize Roles - Admin
        /// </summary>

        // PUT: api/Renewals/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutRenewal(int id, Renewal renewal)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != renewal.ID)
            {
                return BadRequest();
            }

            db.Entry(renewal).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RenewalExists(id))
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
        /// Authorize Roles - Admin
        /// </summary>

        // POST: api/Renewals
        [ResponseType(typeof(Renewal))]
        public async Task<IHttpActionResult> PostRenewal(Renewal renewal)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Renewals.Add(renewal);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = renewal.ID }, renewal);
        }

        /// <summary>
        /// Authorize Roles - Admin
        /// </summary>

        // DELETE: api/Renewals/5
        [ResponseType(typeof(Renewal))]
        public async Task<IHttpActionResult> DeleteRenewal(int id)
        {
            Renewal renewal = await db.Renewals.FindAsync(id);
            if (renewal == null)
            {
                return NotFound();
            }

            db.Renewals.Remove(renewal);
            await db.SaveChangesAsync();

            return Ok(renewal);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool RenewalExists(int id)
        {
            return db.Renewals.Count(e => e.ID == id) > 0;
        }
    }
}