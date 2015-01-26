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
    public class AdsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Authorize Roles - Admin
        /// </summary>

        [Authorize(Roles = "Developer, Admin")]
        // GET: api/GetActiveAds - ControlDeskActiveAD
        public IEnumerable<ObjectCon> GetActiveAds()
        {
            var adsA = db.Ads.Where(r => r.AdStatus.ToString().Equals("Active")).ToList();
            int CountItemsA = adsA.ToList().Count;

            List<ObjectCon> adsL = new List<ObjectCon>();
            adsL.Add(new ObjectCon { ObjectValuesCount = CountItemsA });

            return adsL;
        }

        // GET: api/Ads/GetActiveAdsWithDetailsWide
        public IEnumerable<Ad> GetActiveAdsWithDetailsWide()
        {
            return db.Ads.Where(ad => ad.AdStatus.ToString().Equals("Active") && (ad.Order.Size.ToString().Equals("FullBanner") || ad.Order.Size.ToString().Equals("SmallBanner"))).ToList();
        }

        // GET: api/Ads/GetActiveAdsWithDetailsSquare
        public IEnumerable<Ad> GetActiveAdsWithDetailsSquare()
        {
            return db.Ads.Where(ad => ad.AdStatus.ToString().Equals("Active") && (ad.Order.Size.ToString().Equals("Square") || ad.Order.Size.ToString().Equals("FatSkyscraper"))).ToList();
        }

        /// <summary>
        /// Authorize Roles - Admin
        /// </summary>

        [Authorize(Roles = "Developer, Admin")]
        // GET: api/Ads
        public IEnumerable<Ad> GetAds()
        {
            return db.Ads.ToList();
        }

        /// <summary>
        /// Authorize Roles - Admin
        /// </summary>

        [Authorize(Roles = "Developer, Admin")]
        // GET: api/Ads/5
        [ResponseType(typeof(Ad))]
        public async Task<IHttpActionResult> GetAd(int id)
        {
            Ad ad = await db.Ads.FindAsync(id);
            if (ad == null)
            {
                return NotFound();
            }

            return Ok(ad);
        }

        /// <summary>
        /// Authorize Roles - Admin
        /// </summary>

        [Authorize(Roles = "Developer, Admin")]
        // PUT: api/Ads/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutAd(int id, Ad ad)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != ad.ID)
            {
                return BadRequest();
            }

            db.Entry(ad).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdExists(id))
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
        // POST: api/Ads
        [ResponseType(typeof(Ad))]
        public async Task<IHttpActionResult> PostAd(Ad ad)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Ads.Add(ad);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = ad.ID }, ad);
        }

        /// <summary>
        /// Authorize Roles - Admin
        /// </summary>

        [Authorize(Roles = "Developer, Admin")]
        // DELETE: api/Ads/5
        [ResponseType(typeof(Ad))]
        public async Task<IHttpActionResult> DeleteAd(int id)
        {
            Ad ad = await db.Ads.FindAsync(id);
            if (ad == null)
            {
                return NotFound();
            }

            db.Ads.Remove(ad);
            await db.SaveChangesAsync();

            return Ok(ad);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AdExists(int id)
        {
            return db.Ads.Count(e => e.ID == id) > 0;
        }
    }
}