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
    public class BiddingsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private string strCurrentUserId;

        //// GET: api/GetActiveBiddingsAG - ControlDeskActiveAG
        //public IEnumerable<ObjectCon> GetActiveBiddingsAG()
        //{

        //    strCurrentUserId = User.Identity.GetUserId();
        //    var biddingsA = db.Biddings.Where(r => r.Property.AgentID.Equals(strCurrentUserId) && r.BiddingStatus.ToString().Equals("Active")).ToList();
        //    int CountItemsA = biddingsA.ToList().Count;

        //    List<ObjectCon> biddingsL = new List<ObjectCon>();
        //    biddingsL.Add(new ObjectCon { ObjectValue = strCurrentUserId, ObjectValuesCount = CountItemsA });

        //    return biddingsL;
        //}

        /// <summary>
        /// Authorize Roles - Member
        /// </summary>

        [Authorize(Roles = "Developer, Member")]
        // GET: api/GetActiveBiddings ControlDeskActiveME
        public IEnumerable<ObjectCon> GetActiveBiddings()
        {
            var biddingsA = db.Biddings.Where(p => p.BiddingStatus.ToString().Equals("Active"));
            int CountItemsA = biddingsA.ToList().Count;

            List<ObjectCon> biddingsL = new List<ObjectCon>();
            biddingsL.Add(new ObjectCon { ObjectValuesCount = CountItemsA });

            return biddingsL;
        }

        /// <summary>
        /// Authorize Roles - Agent, Member
        /// </summary>

        [Authorize(Roles = "Developer, Agent, Member")]
        // GET: api/Biddings
        public IEnumerable<Bidding> GetBiddings()
        {
            strCurrentUserId = User.Identity.GetUserId();
            if(User.IsInRole("Agent"))
                return db.Biddings.Where(b => b.Property.AgentID == strCurrentUserId).ToList();
            else
                return db.Biddings.ToList();
        }

        /// <summary>
        /// Authorize Roles - Agent
        /// </summary>

        [Authorize(Roles = "Developer, Agent")]
        // GET: api/Biddings/5
        [ResponseType(typeof(Bidding))]
        public async Task<IHttpActionResult> GetBidding(int id)
        {
            Bidding bidding = await db.Biddings.FindAsync(id);
            if (bidding == null)
            {
                return NotFound();
            }

            return Ok(bidding);
        }

        /// <summary>
        /// Authorize Roles - Agent
        /// </summary>

        [Authorize(Roles = "Developer, Agent")]
        // PUT: api/Biddings/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutBidding(int id, Bidding bidding)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != bidding.ID)
            {
                return BadRequest();
            }

            db.Entry(bidding).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BiddingExists(id))
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
        // POST: api/Biddings
        [ResponseType(typeof(Bidding))]
        public async Task<IHttpActionResult> PostBidding(Bidding bidding)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Biddings.Add(bidding);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = bidding.ID }, bidding);
        }

        /// <summary>
        /// Authorize Roles - Agent
        /// </summary>

        [Authorize(Roles = "Developer, Agent")]
        // DELETE: api/Biddings/5
        [ResponseType(typeof(Bidding))]
        public async Task<IHttpActionResult> DeleteBidding(int id)
        {
            Bidding bidding = await db.Biddings.FindAsync(id);
            if (bidding == null)
            {
                return NotFound();
            }

            db.Biddings.Remove(bidding);
            await db.SaveChangesAsync();

            return Ok(bidding);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool BiddingExists(int id)
        {
            return db.Biddings.Count(e => e.ID == id) > 0;
        }
    }
}