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
    public class PhotosController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private string strCurrentUserId;

        /// <summary>
        /// Authorize Roles - Admin, Agent, Member
        /// </summary>

        [Authorize(Roles = "Developer, Admin, Agent, Member")]
        // GET: api/GetPropertyPhotos
        public IEnumerable<Photo> GetPropertyPhotos(int PID)
        {
            return db.Photos.Where(ph => ph.PropertyID == PID).ToList();
        }

        /// <summary>
        /// Authorize Roles - Agent
        /// </summary>

        [Authorize(Roles = "Developer, Agent")]
        // GET: api/Photos
        public IEnumerable<Photo> GetPhotos()
        {
            strCurrentUserId = User.Identity.GetUserId();
            return db.Photos.Where(ph => ph.Property.AgentID == strCurrentUserId).ToList();
        }

        /// <summary>
        /// Authorize Roles - Agent
        /// </summary>

        [Authorize(Roles = "Developer, Agent")]
        // GET: api/Photos/5
        [ResponseType(typeof(Photo))]
        public async Task<IHttpActionResult> GetPhoto(int id)
        {
            Photo photo = await db.Photos.FindAsync(id);
            if (photo == null)
            {
                return NotFound();
            }

            return Ok(photo);
        }

        /// <summary>
        /// Authorize Roles - Agent
        /// </summary>

        [Authorize(Roles = "Developer, Agent")]
        // PUT: api/Photos/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutPhoto(int id, Photo photo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != photo.ID)
            {
                return BadRequest();
            }

            db.Entry(photo).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PhotoExists(id))
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

        [Authorize(Roles = "Developer, Agent")]
        /// <summary>
        /// Authorize Roles - Agent
        /// </summary>

        // POST: api/Photos
        [ResponseType(typeof(Photo))]
        public async Task<IHttpActionResult> PostPhoto(Photo photo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Photos.Add(photo);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = photo.ID }, photo);
        }

        /// <summary>
        /// Authorize Roles - Agent
        /// </summary>

        [Authorize(Roles = "Developer, Agent")]
        // DELETE: api/Photos/5
        [ResponseType(typeof(Photo))]
        public async Task<IHttpActionResult> DeletePhoto(int id)
        {
            Photo photo = await db.Photos.FindAsync(id);
            if (photo == null)
            {
                return NotFound();
            }

            db.Photos.Remove(photo);
            await db.SaveChangesAsync();

            return Ok(photo);
        }

        /// <summary>
        /// Authorize Roles - Agent
        /// </summary>

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PhotoExists(int id)
        {
            return db.Photos.Count(e => e.ID == id) > 0;
        }
    }
}