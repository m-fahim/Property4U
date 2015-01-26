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
    public class ResponsesController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private string strCurrentUserId;

        /// <summary>
        /// Authorize Roles - Member
        /// </summary>

        [Authorize(Roles = "Developer, Member")]
        // GET: api/GetRecentAgreeResponses
        public IEnumerable<Response> GetRecentAgreeResponses()
        {
            //HttpContext.Current.User.Identity.GetUserId();
            //RequestContext.Principal.Identity.GetUserId();
            strCurrentUserId = User.Identity.GetUserId();
            return db.Responses.Where(r => r.Request.MemberID == strCurrentUserId && r.ResponseStatus.ToString().Equals("Agree")).ToList();
        }

        /// <summary>
        /// Authorize Roles - Agent, Member
        /// </summary>

        [Authorize(Roles = "Developer, Agent, Member")]
        // GET: api/Responses
        public IEnumerable<Response> GetResponses()
        {
            strCurrentUserId = User.Identity.GetUserId();
            if (User.IsInRole("Agent"))
                return db.Responses.Where(res => res.AgentID == strCurrentUserId).ToList();
            else if(User.IsInRole("Member"))
                return db.Responses.Where(req => req.Request.MemberID == strCurrentUserId).ToList();
            else
                return db.Responses.ToList();
        }

        /// <summary>
        /// Authorize Roles - Agent, Member
        /// </summary>

        [Authorize(Roles = "Developer, Agent, Member")]
        // GET: api/Responses/5
        [ResponseType(typeof(Response))]
        public async Task<IHttpActionResult> GetResponse(int id)
        {
            Response response = await db.Responses.FindAsync(id);
            if (response == null)
            {
                return NotFound();
            }

            return Ok(response);
        }

        /// <summary>
        /// Authorize Roles - Agent
        /// </summary>

        [Authorize(Roles = "Developer, Agent")]
        // PUT: api/Responses/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutResponse(int id, Response response)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != response.ID)
            {
                return BadRequest();
            }

            db.Entry(response).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ResponseExists(id))
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
        // POST: api/Responses
        [ResponseType(typeof(Response))]
        public async Task<IHttpActionResult> PostResponse(Response response)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Responses.Add(response);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = response.ID }, response);
        }

        /// <summary>
        /// Authorize Roles - Agent
        /// </summary>

        [Authorize(Roles = "Developer, Agent")]
        // DELETE: api/Responses/5
        [ResponseType(typeof(Response))]
        public async Task<IHttpActionResult> DeleteResponse(int id)
        {
            Response response = await db.Responses.FindAsync(id);
            if (response == null)
            {
                return NotFound();
            }

            db.Responses.Remove(response);
            await db.SaveChangesAsync();

            return Ok(response);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ResponseExists(int id)
        {
            return db.Responses.Count(e => e.ID == id) > 0;
        }
    }
}