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
    public class BidsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private string strCurrentUserId;

        /// <summary>
        /// Authorize Roles - Member
        /// </summary>

        [Authorize(Roles = "Developer, Member")]
        // GET: api/GetWonBids
        public IEnumerable<Bid> GetWonBids()
        {
            strCurrentUserId = User.Identity.GetUserId();
            return db.Bids.Where(b => b.MemberID == strCurrentUserId && b.BidStatus.ToString().Equals("Winner")).ToList();
        }

        /// <summary>
        /// Authorize Roles - Agent, Member
        /// </summary>

        [Authorize(Roles = "Developer, Agent, Member")]
        // GET: api/Bids
        public IEnumerable<Bid> GetBids()
        {
            strCurrentUserId = User.Identity.GetUserId();
            if (User.IsInRole("Member"))
                return db.Bids.Where(b => b.MemberID == strCurrentUserId).ToList();
            else
                return db.Bids.Where(b => b.Bidding.Property.AgentID == strCurrentUserId).ToList();
        }

        /// <summary>
        /// Authorize Roles - Member
        /// </summary>

        [Authorize(Roles = "Developer, Member")]
        // GET: api/Bids/5
        [ResponseType(typeof(Bid))]
        public async Task<IHttpActionResult> GetBid(int id)
        {
            Bid bid = await db.Bids.FindAsync(id);
            if (bid == null)
            {
                return NotFound();
            }

            return Ok(bid);
        }

        /// <summary>
        /// Authorize Roles - Member
        /// </summary>

        [Authorize(Roles = "Developer, Member")]
        // PUT: api/Bids/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutBid(int id, Bid bid)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != bid.ID)
            {
                return BadRequest();
            }

            db.Entry(bid).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BidExists(id))
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
        /// Authorize Roles - Member
        /// </summary>

        [Authorize(Roles = "Developer, Member")]
        // POST: api/Bids
        [ResponseType(typeof(Bid))]
        public async Task<IHttpActionResult> PostBid(Bid bid)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Bids.Add(bid);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = bid.ID }, bid);
        }

        /// <summary>
        /// Authorize Roles - Member
        /// </summary>

        [Authorize(Roles = "Developer, Member")]
        // DELETE: api/Bids/5
        [ResponseType(typeof(Bid))]
        public async Task<IHttpActionResult> DeleteBid(int id)
        {
            Bid bid = await db.Bids.FindAsync(id);
            if (bid == null)
            {
                return NotFound();
            }

            db.Bids.Remove(bid);
            await db.SaveChangesAsync();

            return Ok(bid);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool BidExists(int id)
        {
            return db.Bids.Count(e => e.ID == id) > 0;
        }
    }
}