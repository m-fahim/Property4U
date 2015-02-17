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
    public class ConfigurationsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        //// GET: api/Configurations
        //public IQueryable<Configuration> GetConfigurations()
        //{
        //    return db.Configurations;
        //}

        /// <summary>
        /// Authorize Roles - Admin, Agent, Member, Public
        /// </summary>

        //// GET: api/Configurations/5
        [ResponseType(typeof(Configuration))]
        public async Task<IHttpActionResult> GetConfiguration()
        {
            Configuration configuration = await db.Configurations.FindAsync(1);
            if (configuration == null)
            {
                return NotFound();
            }

            return Ok(configuration);
        }

        /// <summary>
        /// Authorize Roles - Admin
        /// </summary>

        [Authorize(Roles = "Developer, Admin")]
        // PUT: api/Configurations/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutConfiguration(int id, Configuration configuration)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != configuration.ID)
            {
                return BadRequest();
            }

            db.Entry(configuration).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ConfigurationExists(id))
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
        // POST: api/Configurations
        [ResponseType(typeof(Configuration))]
        public async Task<IHttpActionResult> PostConfiguration(Configuration configuration)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Configurations.Add(configuration);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = configuration.ID }, configuration);
        }

        //// DELETE: api/Configurations/5
        //[ResponseType(typeof(Configuration))]
        //public async Task<IHttpActionResult> DeleteConfiguration(int id)
        //{
        //    Configuration configuration = await db.Configurations.FindAsync(id);
        //    if (configuration == null)
        //    {
        //        return NotFound();
        //    }

        //    db.Configurations.Remove(configuration);
        //    await db.SaveChangesAsync();

        //    return Ok(configuration);
        //}

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}

        private bool ConfigurationExists(int id)
        {
            return db.Configurations.Count(e => e.ID == id) > 0;
        }
    }
}