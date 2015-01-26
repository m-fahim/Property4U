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
    public class RepliesController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private string strCurrentUserId;

        /// <summary>
        /// Authorize Roles - Admin, Agent, Member
        /// </summary>

        [Authorize(Roles = "Developer, Admin, Agent, Member")]
        // GET: api/GetReviewReplies
        public IEnumerable<Reply> GetReviewReplies(int RID)
        {
            return db.Replies.Where(re => re.ReviewID == RID).ToList();
        }

        /// <summary>
        /// Authorize Roles - Agent
        /// </summary>

        [Authorize(Roles = "Developer, Agent")]
        // GET: api/Replies
        public IEnumerable<Reply> GetReplies()
        {
            strCurrentUserId = User.Identity.GetUserId();
            return db.Replies.Where(re => re.AgentID == strCurrentUserId).ToList();
        }

        /// <summary>
        /// Authorize Roles - Agent
        /// </summary>

        [Authorize(Roles = "Developer, Agent")]
        // GET: api/Replies/5
        [ResponseType(typeof(Reply))]
        public async Task<IHttpActionResult> GetReply(int id)
        {
            Reply reply = await db.Replies.FindAsync(id);
            if (reply == null)
            {
                return NotFound();
            }

            return Ok(reply);
        }

        /// <summary>
        /// Authorize Roles - Agent
        /// </summary>

        [Authorize(Roles = "Developer, Agent")]
        // PUT: api/Replies/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutReply(int id, Reply reply)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != reply.ID)
            {
                return BadRequest();
            }

            db.Entry(reply).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReplyExists(id))
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
        // POST: api/Replies
        [ResponseType(typeof(Reply))]
        public async Task<IHttpActionResult> PostReply(Reply reply)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Replies.Add(reply);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = reply.ID }, reply);
        }

        /// <summary>
        /// Authorize Roles - Agent
        /// </summary>

        [Authorize(Roles = "Developer, Agent")]
        // DELETE: api/Replies/5
        [ResponseType(typeof(Reply))]
        public async Task<IHttpActionResult> DeleteReply(int id)
        {
            Reply reply = await db.Replies.FindAsync(id);
            if (reply == null)
            {
                return NotFound();
            }

            db.Replies.Remove(reply);
            await db.SaveChangesAsync();

            return Ok(reply);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ReplyExists(int id)
        {
            return db.Replies.Count(e => e.ID == id) > 0;
        }
    }
}