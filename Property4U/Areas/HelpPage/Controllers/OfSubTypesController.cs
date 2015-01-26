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
    public class OfSubTypesController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Authorize Roles - Admin, Member, Agent
        /// </summary>

        [Authorize(Roles = "Developer, Admin, Member, Agent")]
        // GET: api/OfSubTypes/5 - Get all OfSubTypes of OfTypeID
        public IEnumerable<OfSubType> GetOfSubTypes(int id)
        {
            return db.OfSubTypes.Where(o => o.OfTypeID == id).ToList();
        }

        /// <summary>
        /// Authorize Roles - Admin, Member, Agent
        /// </summary>

        [Authorize(Roles = "Developer, Admin, Member, Agent")]
        // GET: api/OfSubTypes
        public IEnumerable<OfSubType> GetOfSubTypes()
        {
            return db.OfSubTypes.ToList();
        }

        /// <summary>
        /// Authorize Roles - Admin, Member, Agent
        /// </summary>

        [Authorize(Roles = "Developer, Admin, Member, Agent")]
        // GET: api/OfSubTypes/5
        [ResponseType(typeof(OfSubType))]
        public async Task<IHttpActionResult> GetOfSubType(int id)
        {
            OfSubType ofSubType = await db.OfSubTypes.FindAsync(id);
            if (ofSubType == null)
            {
                return NotFound();
            }

            return Ok(ofSubType);
        }

        /// <summary>
        /// Authorize Roles - Admin
        /// </summary>

        [Authorize(Roles = "Developer, Admin")]
        // PUT: api/OfSubTypes/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutOfSubType(int id, OfSubType ofSubType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != ofSubType.ID)
            {
                return BadRequest();
            }

            db.Entry(ofSubType).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OfSubTypeExists(id))
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
        // POST: api/OfSubTypes
        [ResponseType(typeof(OfSubType))]
        public async Task<IHttpActionResult> PostOfSubType(OfSubType ofSubType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.OfSubTypes.Add(ofSubType);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = ofSubType.ID }, ofSubType);
        }

        /// <summary>
        /// Authorize Roles - Admin
        /// </summary>

        [Authorize(Roles = "Developer, Admin")]
        // DELETE: api/OfSubTypes/5
        [ResponseType(typeof(OfSubType))]
        public async Task<IHttpActionResult> DeleteOfSubType(int id)
        {
            OfSubType ofSubType = await db.OfSubTypes.FindAsync(id);
            if (ofSubType == null)
            {
                return NotFound();
            }

            db.OfSubTypes.Remove(ofSubType);
            await db.SaveChangesAsync();

            return Ok(ofSubType);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool OfSubTypeExists(int id)
        {
            return db.OfSubTypes.Count(e => e.ID == id) > 0;
        }
    }
}