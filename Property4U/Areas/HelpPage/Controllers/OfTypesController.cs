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
    public class OfTypesController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Authorize Roles - Admin, Member, Agent
        /// </summary>

        [Authorize(Roles = "Developer, Admin, Member, Agent")]
        // GET: api/OfTypes
        public IEnumerable<OfType> GetOfTypes()
        {
            return db.OfTypes.ToList();
        }

        /// <summary>
        /// Authorize Roles - Admin, Member, Agent
        /// </summary>

        [Authorize(Roles = "Developer, Admin, Member, Agent")]
        // GET: api/OfTypes/5
        [ResponseType(typeof(OfType))]
        public async Task<IHttpActionResult> GetOfType(int id)
        {
            OfType ofType = await db.OfTypes.FindAsync(id);
            if (ofType == null)
            {
                return NotFound();
            }

            return Ok(ofType);
        }

        /// <summary>
        /// Authorize Roles - Admin
        /// </summary>

        [Authorize(Roles = "Developer, Admin")]
        // PUT: api/OfTypes/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutOfType(int id, OfType ofType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != ofType.ID)
            {
                return BadRequest();
            }

            db.Entry(ofType).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OfTypeExists(id))
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

        [Authorize(Roles = "Developer, Admin")]
        // POST: api/OfTypes
        [ResponseType(typeof(OfType))]
        public async Task<IHttpActionResult> PostOfType(OfType ofType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.OfTypes.Add(ofType);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = ofType.ID }, ofType);
        }

        /// <summary>
        /// Authorize Roles - Admin
        /// </summary>

        [Authorize(Roles = "Developer, Admin")]
        // DELETE: api/OfTypes/5
        [ResponseType(typeof(OfType))]
        public async Task<IHttpActionResult> DeleteOfType(int id)
        {
            OfType ofType = await db.OfTypes.FindAsync(id);
            if (ofType == null)
            {
                return NotFound();
            }

            db.OfTypes.Remove(ofType);
            await db.SaveChangesAsync();

            return Ok(ofType);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool OfTypeExists(int id)
        {
            return db.OfTypes.Count(e => e.ID == id) > 0;
        }
    }
}