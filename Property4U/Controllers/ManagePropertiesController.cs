using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using IdentitySample.Models;
using Property4U.Models;
using Microsoft.AspNet.Identity;
using PagedList;
using System.Threading.Tasks;

namespace Property4U.Controllers
{
    public class ManagePropertiesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private string strCurrentUserId;

        [Authorize(Roles = "Admin, Agent, Member")]
        // GET: Properties
        public ActionResult Index(string sortOrder, string currentFilter, string searchProperty, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            //ViewBag.TypeSortParm = String.IsNullOrEmpty(sortOrder) ? "type_desc" : "";
            ViewBag.ForSortParm = String.IsNullOrEmpty(sortOrder) ? "for_desc" : "";
            ViewBag.BiddingSortParm = String.IsNullOrEmpty(sortOrder) ? "Bidding" : "";
            ViewBag.AvailabilitySortParm = String.IsNullOrEmpty(sortOrder) ? "Availability_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

            if (searchProperty != null)
            {
                page = 1;
            }
            else
            {
                searchProperty = currentFilter;
            }

            ViewBag.CurrentFilter = searchProperty;

            // Filter out Properties not belong to Agent and for Member all
            IQueryable<Property> properties = null;
            if (Request.IsAuthenticated && User.IsInRole("Agent"))
            {
                strCurrentUserId = User.Identity.GetUserId();
                properties = db.Properties.Include(p => p.Address).Include(p => p.OfType).Include(p => p.Agent).Where(p => p.AgentID.Equals(strCurrentUserId));
            }
            else if (Request.IsAuthenticated && User.IsInRole("Member"))
            {
                properties = db.Properties.Include(p => p.Address).Include(p => p.OfType).Include(p => p.Agent).Where(p => p.Availability.ToString().Equals("Yes"));
            }
            else {
                properties = db.Properties.Include(p => p.Address).Include(p => p.OfType).Include(p => p.Agent);
            }

            if (!String.IsNullOrEmpty(searchProperty))
            {
                properties = properties.Where(p => p.Title.ToUpper().Contains(searchProperty.ToUpper())
                                       || p.Title.ToUpper().Contains(searchProperty.ToUpper()));
            }

            switch (sortOrder)
            {
                //case "type_desc":
                //    properties = properties.OrderByDescending(p => p.OfType);
                //    break;
                case "for_desc":
                    properties = properties.OrderByDescending(p => p.For);
                    break;
                case "Bidding":
                    properties = properties.OrderBy(p => p.AllowBidding);
                    break;
                case "Availability_desc":
                    properties = properties.OrderBy(p => p.Availability);
                    break;
                case "Date":
                    properties = properties.OrderBy(p => p.PublishOn);
                    break;
                case "date_desc":
                    properties = properties.OrderByDescending(p => p.PublishOn);
                    break;
                default:
                    properties = properties.OrderBy(p => p.ID);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(properties.ToPagedList(pageNumber, pageSize));
            //return View(properties.ToListAsync());
        }

        [Authorize(Roles = "Admin, Agent, Member")]
        // GET: Properties/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Property property = await db.Properties.FindAsync(id);
            if (property == null)
            {
                return HttpNotFound();
            }
            // Increment value on each Property Details view page visit
            property.Views = (property.Views.ToString().Equals("")) ? 1 : property.Views+1;
            await db.SaveChangesAsync();
            return View(property);
        }

        [Authorize(Roles = "Agent")]
        // GET: Properties/Create
        public async Task<ActionResult> Create()
        {
            ViewBag.AddressIDList = new SelectList(db.Addresses, "ID", "ID");
            ViewBag.OfTypeIDList = new SelectList(db.OfTypes, "ID", "Title");
            ViewBag.OfSubTypeList = new SelectList(db.OfSubTypes, "ID", "Title");

            strCurrentUserId = User.Identity.GetUserId();
            var ownerAgent = await db.Users.Where(d => d.Id == strCurrentUserId).ToListAsync();

            SelectList slAgent = new SelectList(ownerAgent, "Id", "Id", strCurrentUserId);
            ViewBag.AgentIDList = slAgent;

            ViewBag.SelectedFeatures = new MultiSelectList(db.Features, "ID", "Title");
            ViewBag.PublishOn = DateTime.Now.ToString("yyyy-MM-dd");
            return View();
        }

        [Authorize(Roles = "Agent")]
        // POST: Properties/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,AgentID,AddressID,OfTypeID,OfSubType,Title,Seller,Locality,CoveredAreaMeasurement,CoveredAreaUnits,Condition,Furnished,Stories,FloorNo,Flooring,Baths,Kitchens,DrawingRooms,DiningRooms,LivingRooms,NumberOfRooms,StoreRooms,ServantQuarters,Lawn,CarSpaces,Build,AreaMeasurement,AreaUnits,AreaDiLength,AreaDiWidth,Price,For,AllowBidding,Availability,Views,Status,Discount,Featured,Flags,PublishOn,LastEdit")] Property property, int[] SelectedFeatures)
        {
            if (ModelState.IsValid)
            {
                db.Properties.Add(property);
                await db.SaveChangesAsync();

                if (SelectedFeatures != null)
                {
                    Property edit = await db.Properties.FindAsync(property.ID);
                    if (edit.Features == null) edit.Features = new List<Feature>();
                    foreach (var item in  edit.Features.ToList())
                    {
                        edit.Features.Remove(item);
                    }
                    foreach (var FeatureID in SelectedFeatures)
                    {
                        edit.Features.Add(await db.Features.FindAsync(FeatureID));
                    }
                    await db.SaveChangesAsync();
                }
                return RedirectToAction("Index");
            }

            ViewBag.AddressIDList = new SelectList(db.Addresses, "ID", "ID", property.AddressID);
            ViewBag.OfTypeIDList = new SelectList(db.OfTypes, "ID", "Title", property.OfTypeID);
            ViewBag.OfSubTypeList = new SelectList(db.OfSubTypes, "ID", "Title", property.OfSubType);

            strCurrentUserId = User.Identity.GetUserId();
            var ownerAgent = await db.Users.Where(d => d.Id == strCurrentUserId).ToListAsync();
            ViewBag.AgentIDList = new SelectList(ownerAgent, "ID", "ID", property.AgentID);

            ViewBag.SelectedFeatures = new MultiSelectList(db.Features, "ID", "Title");
            ViewBag.PublishOn = DateTime.Now.ToString("yyyy-MM-dd");
            return View(property);
        }

        [Authorize(Roles = "Agent")]
        // GET: Properties/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Property property = await db.Properties.FindAsync(id);
            if (property == null)
            {
                return HttpNotFound();
            }
            ViewBag.AddressIDList = new SelectList(db.Addresses, "ID", "ID", property.AddressID);
            ViewBag.OfTypeIDList = new SelectList(db.OfTypes, "ID", "Title", property.OfTypeID);
            //
            var subTypes = await db.OfSubTypes.Where(s => s.OfTypeID == property.OfTypeID).ToListAsync();
            ViewBag.OfSubType = property.OfSubType; 

            strCurrentUserId = User.Identity.GetUserId();
            var ownerAgent = await db.Users.Where(d => d.Id == strCurrentUserId).ToListAsync();
            ViewBag.AgentIDList = new SelectList(ownerAgent, "ID", "ID", property.AgentID);
            ViewBag.SelectedFeatures = new MultiSelectList(db.Features, "ID", "Title", property.Features.Select(f => f.ID).ToArray());
            ViewBag.Availability = property.Availability;
            ViewBag.LastEdit = DateTime.Now;

            return View(property);
        }

        [Authorize(Roles = "Agent")]
        // POST: Properties/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,AgentID,AddressID,OfTypeID,OfSubType,Title,Seller,Locality,CoveredAreaMeasurement,CoveredAreaUnits,Condition,Furnished,Stories,FloorNo,Flooring,Baths,Kitchens,DrawingRooms,DiningRooms,LivingRooms,NumberOfRooms,StoreRooms,ServantQuarters,Lawn,CarSpaces,Build,AreaMeasurement,AreaUnits,AreaDiLength,AreaDiWidth,Price,For,AllowBidding,Availability,Views,Status,Discount,Featured,Flags,PublishOn,LastEdit")] Property property, int[] SelectedFeatures)
        {
            if (ModelState.IsValid)
            {
                Property edit = await db.Properties.FindAsync(property.ID);

                if (edit.Features == null) 
                    edit.Features = new List<Feature>();
                foreach (var item in edit.Features.ToList())
                {
                    edit.Features.Remove(item);
                    await db.SaveChangesAsync();
                }
                if (SelectedFeatures != null)
                {
                    foreach (var FeatureID in SelectedFeatures)
                    {
                        edit.Features.Add(await db.Features.FindAsync(FeatureID));
                    }
                }

                edit.AgentID = property.AgentID;
                edit.AddressID = property.AddressID;
                edit.OfTypeID = property.OfTypeID;
                edit.OfSubType = property.OfSubType;
                edit.Title = property.Title;
                edit.Seller = property.Seller;
                //edit.OfType = property.OfType;
                edit.Locality = property.Locality;
                edit.CoveredAreaMeasurement = property.CoveredAreaMeasurement; 
                edit.CoveredAreaUnits = property.CoveredAreaUnits; 
                edit.Condition = property.Condition;
                edit.Furnished = property.Furnished;
                edit.Stories = property.Stories;
                edit.FloorNo = property.FloorNo;
                edit.Flooring = property.Flooring;
                edit.Baths = property.Baths;
                edit.Kitchens = property.Kitchens;
                edit.DrawingRooms = property.DrawingRooms;
                edit.DiningRooms = property.DiningRooms;
                edit.LivingRooms = property.LivingRooms;
                edit.NumberOfRooms = property.NumberOfRooms;
                edit.StoreRooms = property.StoreRooms;
                edit.ServantQuarters = property.ServantQuarters;
                edit.Lawn = property.Lawn;
                edit.CarSpaces = property.CarSpaces;
                edit.Build = property.Build;
                edit.AreaMeasurement = property.AreaMeasurement;
                edit.AreaUnits = property.AreaUnits;
                edit.AreaDiLength = property.AreaDiLength;
                edit.AreaDiWidth = property.AreaDiWidth;
                edit.Price = property.Price;
                edit.For = property.For;
                edit.AllowBidding = property.AllowBidding;
                edit.Availability = property.Availability;
                edit.Views = property.Views;
                edit.Status = property.Status;
                edit.Discount = property.Discount;
                edit.Featured = property.Featured;
                edit.Flags = property.Flags;
                edit.PublishOn = property.PublishOn;
                edit.LastEdit = property.LastEdit;

                db.Entry(edit).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.AddressIDList = new SelectList(db.Addresses, "ID", "ID", property.AddressID);
            ViewBag.OfTypeIDList = new SelectList(db.OfTypes, "ID", "Title", property.OfTypeID);
            ViewBag.OfSubTypeList = new SelectList(db.OfSubTypes, "ID", "Title", property.OfSubType);

            strCurrentUserId = User.Identity.GetUserId();
            var ownerAgent = await db.Users.Where(d => d.Id == strCurrentUserId).ToListAsync();
            ViewBag.AgentIDList = new SelectList(ownerAgent, "ID", "ID", property.AgentID);
            ViewBag.Availability = property.Availability;
            ViewBag.LastEdit = DateTime.Now;

            //if (SelectedFeatures != null)
            //{
            //    ViewBag.SelectedFeatures = new MultiSelectList(db.Features, "ID", "Title", property.Features.Select(f => f.ID).ToArray());
            //}
            //else {
                ViewBag.SelectedFeatures = new MultiSelectList(db.Features, "ID", "Title");
            //}
            return View(property);
        }

        [Authorize(Roles = "Agent")]
        // GET: Properties/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Property property = await db.Properties.FindAsync(id);
            if (property == null)
            {
                return HttpNotFound();
            }
            return View(property);
        }

        [Authorize(Roles = "Agent")]
        // POST: Properties/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Property property = await db.Properties.FindAsync(id);
            db.Properties.Remove(property);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
