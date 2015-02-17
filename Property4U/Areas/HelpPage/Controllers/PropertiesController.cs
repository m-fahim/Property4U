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
using System.Data.Entity.Spatial;

namespace Property4U.Areas.HelpPage.Controllers
{
    public class PropertiesController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private string strCurrentUserId;

        /// <summary>
        /// Authorize Roles - Admin, Agent, Member
        /// </summary>

        [Authorize(Roles = "Developer, Admin, Agent, Member")]
        // GET: api/Properties
        public IEnumerable<Property> GetProperties()
        {
            strCurrentUserId = User.Identity.GetUserId();
            if (User.IsInRole("Agent"))
                return db.Properties.Where(p => p.AgentID == strCurrentUserId).ToList();
            else
                return db.Properties.ToList();
        }

        /// <summary>
        /// Authorize Roles - Admin, Agent, Member
        /// </summary>

        [Authorize(Roles = "Developer, Admin, Agent, Member")]
        // GET: api/GetPopularProperties - ControlDeskActiveAD
        public IEnumerable<Property> GetPopularProperties()
        {
            return db.Properties.OrderByDescending(p => p.Views).Take(5).ToList();
        }

        /// <summary>
        /// Authorize Roles - Admin, Member
        /// </summary>

        [Authorize(Roles = "Developer, Admin, Member")]
        // GET: api/GetMonthlyPropertiesWorth - ControlDeskActiveAD-ME
        public IEnumerable<PeriodPropertiesWorth> GetMonthlyPropertiesWorth()
        {
            // Count Every Month Properties And There Worth
            return db.Database.SqlQuery<PeriodPropertiesWorth>("SELECT COUNT(*) AS [Properties], FORMAT(PublishOn,'yyyy-MM')+'-'+Cast(Day(EOMONTH(PublishOn)) as nvarchar) AS [Period], SUM(CAST(Price AS float)) AS Worth from Property Group By YEAR(PublishOn), MONTH(PublishOn) , FORMAT(PublishOn,'yyyy-MM')+'-'+Cast(Day(EOMONTH(PublishOn)) as nvarchar)").ToList<PeriodPropertiesWorth>();
        }

        /// <summary>
        /// Authorize Roles - Agent
        /// </summary>

        [Authorize(Roles = "Developer, Agent")]
        // GET: api/GetMonthlyPropertiesRequest - ControlDeskActiveAG
        public IEnumerable<PeriodPropertiesRequests> GetMonthlyPropertiesRequest()
        {
            List<PeriodPropertiesRequests> PeriodPropertiesRequestsL = new List<PeriodPropertiesRequests>();
            var PeriodPropertiesL = db.Database.SqlQuery<PeriodProperties>("SELECT COUNT(*) AS [Properties], FORMAT(PublishOn,'yyyy-MM')+'-'+Cast(Day(EOMONTH(PublishOn)) as nvarchar) AS [Period] from Property Group By YEAR(PublishOn), MONTH(PublishOn) , FORMAT(PublishOn,'yyyy-MM')+'-'+Cast(Day(EOMONTH(PublishOn)) as nvarchar)").ToList<PeriodProperties>();
            foreach (var periodProperty in PeriodPropertiesL)
            {
                var Requests = db.Database.SqlQuery<int>("SELECT COUNT(*) AS [Requests] from Request where FORMAT(RequestOn,'yyyy-MM') = FORMAT(Cast('" + periodProperty.Period + "' as datetime),'yyyy-MM') Group By YEAR(RequestOn), MONTH(RequestOn)").FirstOrDefault<int>();
                PeriodPropertiesRequestsL.Add(new PeriodPropertiesRequests { Period = periodProperty.Period, Properties = periodProperty.Properties, Requests = Requests });
            }
            return PeriodPropertiesRequestsL.ToList();
        }

        /// <summary>
        /// Authorize Roles - Admin, Agent, Member
        /// </summary>

        [Authorize(Roles = "Developer, Admin, Agent, Member")]
        // GET: api/GetPropertiesFor - ControlDeskActiveAD-AG-ME - Donut
        public IEnumerable<PropertiesFor> GetPropertiesFor()
        {
            List<PropertiesFor> propertiesForL = new List<PropertiesFor>();
            int forRent = db.Properties.Where(p => p.For.ToString().Equals("Rent")).Count();
            int forSale = db.Properties.Where(p => p.For.ToString().Equals("Sale")).Count();
            propertiesForL.Add(new PropertiesFor { label = "Rent", value = forRent });
            propertiesForL.Add(new PropertiesFor { label = "Sale", value = forSale });
            return propertiesForL.ToList();
        }

        /// <summary>
        /// Authorize Roles - Agent
        /// </summary>

        [Authorize(Roles = "Developer, Agent")]
        // GET: api/GetYearlyProfit - ControlDeskActiveAG
        public IEnumerable<ObjectCon> GetYearlyProfit()
        {
            var completeBiddings = db.Biddings.Where(b => b.WinningBid.ToString().Length != 0 && b.EndDate.Year.Equals(DateTime.Today.Year) && b.BiddingStatus.ToString().Equals("Closed"));
            var completeBiddingsL = completeBiddings.ToList();
            double totalProfitPerc = 0.00;

            foreach (var completeBiddingsItems in completeBiddingsL)
            {
                // Current month - Month(b.EndDate) = Month(GETDATE());
                var completeBiddingsBid = db.Bids.Find(completeBiddingsItems.WinningBid);

                // Profit per property sale through Bidding
                totalProfitPerc = totalProfitPerc + (((completeBiddingsBid.Price / completeBiddingsItems.Property.Price) - 1) * 100);
            }

            if (completeBiddingsL != null && completeBiddingsL.Count != 0)
            {
                totalProfitPerc = (totalProfitPerc / completeBiddingsL.Count);
            }

            List<ObjectCon> biddingsL = new List<ObjectCon>();
            biddingsL.Add(new ObjectCon { ObjectValue = totalProfitPerc.ToString("#.##") });

            return biddingsL;
        }


        /// <summary>
        /// Authorize Roles - Member
        /// </summary>

        [Authorize(Roles = "Developer, Member")]
        // GET: api/GetAvaliableProperties ControlDeskActiveME
        public IEnumerable<ObjectCon> GetAvaliableProperties()
        {
            var popertiesA = db.Properties.Where(p => p.Availability.ToString().Equals("Yes"));
            int CountItemsA = popertiesA.ToList().Count;

            List<ObjectCon> popertiesL = new List<ObjectCon>();
            popertiesL.Add(new ObjectCon { ObjectValuesCount = CountItemsA });

            return popertiesL;
        }


        /// <summary>
        /// Authorize Roles - Admin
        /// </summary> 

        [Authorize(Roles = "Developer, Admin")]
        // POST: api/Properties/SetFeatured
        public async Task<IHttpActionResult> SetFeatured(int pid, string featured)
        {
            Property property = await db.Properties.FindAsync(pid);
            if (property == null)
                return NotFound();

            if (property.Availability.ToString().Equals("No"))
                return NotFound();

            //if(property.Featured != null){
            if (!property.Featured.ToString().Equals(featured))
            {
                Property featuredState;
                if (property.Featured.ToString().Equals("No"))
                    featuredState = new Property { Featured = Featured.Yes };
                else
                    featuredState = new Property { Featured = Featured.No };

                db.Database.ExecuteSqlCommand("UPDATE [dbo].[Property] SET Featured = {0} WHERE ID = {1}", featuredState.Featured.Value, property.ID);
                await db.SaveChangesAsync();
            }
            // }

            return Ok(property);
        }

        /// <summary>
        /// Authorize Roles - Member
        /// </summary>

        [Authorize(Roles = "Developer, Member")]
        // GET: api/GetPropertiesInRange - iOS APP P4U
        public IEnumerable<Property> GetPropertiesInRange(int min, int max)
        {
            return db.Properties.Where(p => p.Price <= max || p.Price >= min).ToList();
        }

        /// <summary>
        /// Authorize Roles - Member
        /// </summary>

        [Authorize(Roles = "Developer, Member")]
        // GET: api/FindNearByProperties - iOS APP P4U
        public IEnumerable<NearByAddresses> GetFindNearByProperties(double longitude, double latitude)
        {
            DbGeography searchLocation = DbGeography.FromText(String.Format("POINT({0} {1})", longitude, latitude));

            return
                (from location in db.Addresses
                 where longitude != null && latitude != null
                 select new NearByAddresses
                 {
                     ID = location.ID,
                     Name = location.Name,
                     Number = location.Number,
                     Floor = location.Floor,
                     AreaName = location.AreaName,
                     Block = location.Block,
                     Street = location.Street,
                     City = location.City,
                     State = location.State,
                     Country = location.Country,
                     PostalCode = location.PostalCode,
                     ZipCode = location.ZipCode,
                     Latitude = location.Latitude,
                     Longitude = location.Longitude,
                     Distance = searchLocation.Distance(
                          DbGeography.FromText("POINT(" + location.Longitude + " " + location.Latitude + ")"))
                 })
                .OrderBy(location => location.Distance).ToList();
        }

        /// <summary>
        /// Authorize Roles - Admin, Agent, Member
        /// </summary>

        [Authorize(Roles = "Developer, Admin, Agent, Member")]
        // GET: api/Properties/5
        [ResponseType(typeof(Property))]
        public async Task<IHttpActionResult> GetProperty(int id)
        {
            Property property = await db.Properties.FindAsync(id);
            if (property == null)
            {
                return NotFound();
            }

            return Ok(property);
        }

        /// <summary>
        /// Authorize Roles - Agent
        /// </summary>

        [Authorize(Roles = "Developer, Agent")]
        // PUT: api/Properties/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutProperty(int id, Property property)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != property.ID)
            {
                return BadRequest();
            }

            db.Entry(property).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PropertyExists(id))
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
        // POST: api/Properties
        [ResponseType(typeof(Property))]
        public async Task<IHttpActionResult> PostProperty(Property property)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Properties.Add(property);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = property.ID }, property);
        }

        /// <summary>
        /// Authorize Roles - Agent
        /// </summary>

        [Authorize(Roles = "Developer, Agent")]
        // DELETE: api/Properties/5
        [ResponseType(typeof(Property))]
        public async Task<IHttpActionResult> DeleteProperty(int id)
        {
            Property property = await db.Properties.FindAsync(id);
            if (property == null)
            {
                return NotFound();
            }

            db.Properties.Remove(property);
            await db.SaveChangesAsync();

            return Ok(property);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PropertyExists(int id)
        {
            return db.Properties.Count(e => e.ID == id) > 0;
        }
    }
}