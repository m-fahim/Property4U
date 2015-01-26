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
    public class FeedbacksController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private string strCurrentUserId;

        /// <summary>
        /// Authorize Roles - Admin
        /// </summary>

        [Authorize(Roles = "Developer, Admin")]
        // GET: api/GetRecentFeedbacks
        public IEnumerable<Feedback> GetRecentFeedbacks()
        {
            return db.Feedbacks.Where(o => o.For.ToString().Equals("Report_Abuse")).ToList();
        }

        /// <summary>
        /// Authorize Roles - Member, Admin
        /// </summary>

        [Authorize(Roles = "Developer, Member, Admin")]
        // GET: api/Feedbacks
        public IEnumerable<Feedback> GetFeedbacks()
        {
            strCurrentUserId = User.Identity.GetUserId();
            if (User.IsInRole("Member"))
                return db.Feedbacks.Where(b => b.MemberID == strCurrentUserId).ToList();
            else
                return db.Feedbacks.ToList();
        }

        /// <summary>
        /// Authorize Roles - Member, Admin
        /// </summary>

        [Authorize(Roles = "Developer, Member, Admin")]
        // GET: api/Feedbacks/5
        [ResponseType(typeof(Feedback))]
        public async Task<IHttpActionResult> GetFeedback(int id)
        {
            Feedback feedback = await db.Feedbacks.FindAsync(id);
            if (feedback == null)
            {
                return NotFound();
            }

            return Ok(feedback);
        }

        /// <summary>
        /// Authorize Roles - Member
        /// </summary>

        [Authorize(Roles = "Developer, Member")]
        // PUT: api/Feedbacks/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutFeedback(int id, Feedback feedback)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != feedback.ID)
            {
                return BadRequest();
            }

            db.Entry(feedback).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FeedbackExists(id))
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
        // POST: api/Feedbacks
        [ResponseType(typeof(Feedback))]
        public async Task<IHttpActionResult> PostFeedback(Feedback feedback)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Feedbacks.Add(feedback);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = feedback.ID }, feedback);
        }

        /// <summary>
        /// Authorize Roles - Member
        /// </summary>

        [Authorize(Roles = "Developer, Member")]
        // DELETE: api/Feedbacks/5
        [ResponseType(typeof(Feedback))]
        public async Task<IHttpActionResult> DeleteFeedback(int id)
        {
            Feedback feedback = await db.Feedbacks.FindAsync(id);
            if (feedback == null)
            {
                return NotFound();
            }

            db.Feedbacks.Remove(feedback);
            await db.SaveChangesAsync();

            return Ok(feedback);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool FeedbackExists(int id)
        {
            return db.Feedbacks.Count(e => e.ID == id) > 0;
        }
    }
}