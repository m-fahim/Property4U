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
    [Authorize(Roles = "Developer, Admin, Agent")]
    public class FeaturesController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Authorize Roles - Admin, Agent
        /// </summary>

        // GET: api/Features
        public IEnumerable<Feature> GetFeatures()
        {
            return db.Features.ToList();
        }

        /// <summary>
        /// Authorize Roles - Admin, Agent
        /// </summary>

        // GET: api/Features/5
        [ResponseType(typeof(Feature))]
        public async Task<IHttpActionResult> GetFeature(int id)
        {
            Feature feature = await db.Features.FindAsync(id);
            if (feature == null)
            {
                return NotFound();
            }

            return Ok(feature);
        }

        /// <summary>
        /// Authorize Roles - Admin, Agent
        /// </summary>

        // PUT: api/Features/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutFeature(int id, Feature feature)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != feature.ID)
            {
                return BadRequest();
            }

            db.Entry(feature).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FeatureExists(id))
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
        /// Authorize Roles - Admin, Agent
        /// </summary>

        // POST: api/Features
        [ResponseType(typeof(Feature))]
        public async Task<IHttpActionResult> PostFeature(Feature feature)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Features.Add(feature);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = feature.ID }, feature);
        }

        /// <summary>
        /// Authorize Roles - Admin, Agent
        /// </summary>

        // DELETE: api/Features/5
        [ResponseType(typeof(Feature))]
        public async Task<IHttpActionResult> DeleteFeature(int id)
        {
            Feature feature = await db.Features.FindAsync(id);
            if (feature == null)
            {
                return NotFound();
            }

            db.Features.Remove(feature);
            await db.SaveChangesAsync();

            return Ok(feature);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool FeatureExists(int id)
        {
            return db.Features.Count(e => e.ID == id) > 0;
        }
    }
}