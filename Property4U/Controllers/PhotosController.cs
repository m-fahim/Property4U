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
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using System.IO;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Microsoft.AspNet.Identity;

namespace Property4U.Controllers
{
    [Authorize(Roles = "Agent")]
    public class PhotosController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private string strCurrentUserId;

        // GET: Photos
        public async Task<ActionResult> Index()
        {
            strCurrentUserId = User.Identity.GetUserId();
            var photos = db.Photos.Include(p => p.Property).Where(p => p.Property.AgentID == strCurrentUserId);
            return View( await photos.ToListAsync());
        }

        // GET: Photos/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Photo photo = await db.Photos.FindAsync(id);
            if (photo == null)
            {
                return HttpNotFound();
            }
            return View(photo);
        }

        // GET: Photos/Create
        public async Task<ActionResult> Create()
        {
            strCurrentUserId = User.Identity.GetUserId();
            ViewBag.PropertyIDList = new SelectList(await db.Properties.Where(p => p.Availability.ToString().Equals("Yes") && p.AgentID.Equals(strCurrentUserId)).ToListAsync(), "ID", "ID");
            ViewBag.UploadedOn = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            return View();
        }

        // POST: Photos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,PropertyID,PhotoTitle,AltText,Caption,UploadedFrom,UploadedTo,Size,Extension,UploadedOn,LastEdit")] Photo photo, HttpPostedFileBase file)
        {
            if (file == null)
            {
                ModelState.AddModelError(string.Empty, "A photo file must be chosen.");
            }

            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    string photoPropertyName = Path.GetFileName(file.FileName);
                    double photoSize = file.ContentLength;
                    string photoPropertyEx = Path.GetExtension(file.FileName);
                    string photoPropertyToPath = Path.Combine(Server.MapPath("~/Content/Uploads/Properties"), photoPropertyName);
                    // file is uploaded
                    file.SaveAs(photoPropertyToPath);

                    // Create Small thumbnail from image using FixedSize
                    string photoSThumbPropertyToPath = Path.Combine(Server.MapPath("~/Content/Uploads/Properties/Small"), photoPropertyName);
                    Image photoThumb = Image.FromStream(file.InputStream, true, true);
                    FixedSize(photoSThumbPropertyToPath, photoThumb, 256, 236, true);

                    // Create Medium thumbnail from image using FixedSize
                    string photoMThumbPropertyToPath = Path.Combine(Server.MapPath("~/Content/Uploads/Properties/Medium"), photoPropertyName);
                    FixedSize(photoMThumbPropertyToPath, photoThumb, 328, 378, true);

                    photo.PhotoTitle = photoPropertyName;
                    photo.UploadedTo = photoPropertyToPath;
                    // New file size
                    photo.Size = photoSize;
                    photo.Extension = photoPropertyEx;
                }

                db.Photos.Add(photo);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            strCurrentUserId = User.Identity.GetUserId();
            ViewBag.PropertyIDList = new SelectList(await db.Properties.Where(p => p.Availability.ToString().Equals("Yes") && p.AgentID.Equals(strCurrentUserId)).ToListAsync(), "ID", "ID", photo.PropertyID);
            ViewBag.UploadedOn = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            return View(photo);
        }

        // GET: Photos/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Photo photo = await db.Photos.FindAsync(id);
            if (photo == null)
            {
                return HttpNotFound();
            }
            strCurrentUserId = User.Identity.GetUserId();
            ViewBag.PropertyIDList = new SelectList(await db.Properties.Where(p => p.Availability.ToString().Equals("Yes") && p.AgentID.Equals(strCurrentUserId)).ToListAsync(), "ID", "ID", photo.PropertyID);
            ViewBag.LastEdit = DateTime.Now;
            return View(photo);
        }

        // POST: Photos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,PropertyID,PhotoTitle,AltText,Caption,UploadedFrom,UploadedTo,Size,Extension,UploadedOn,LastEdit")] Photo photo, HttpPostedFileBase file, string oldPhotoTitle)
        {

            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    string photoPropertyName = Path.GetFileName(file.FileName);
                    string photoPropertyEx = Path.GetExtension(file.FileName);
                    string photoPropertyToPath = Path.Combine(Server.MapPath("~/Content/Uploads/Properties"), photoPropertyName);

                    if (!System.IO.File.Exists(photoPropertyToPath))
                    {
                        // Delete previously uploaded file
                        System.IO.File.Delete(photo.UploadedTo);

                        // New file is uploaded
                        file.SaveAs(photoPropertyToPath);
                        photo.PhotoTitle = photoPropertyName;
                        photo.UploadedTo = photoPropertyToPath;
                        // New file size
                        photo.Size = file.ContentLength;
                        photo.Extension = photoPropertyEx;

                        // Delete previously uploaded thumbnail - Small, Medium
                        string photoSThumbPropertyToPath = Path.Combine(Server.MapPath("~/Content/Uploads/Properties/Small"), photoPropertyName);
                        System.IO.File.Delete(photoSThumbPropertyToPath);
                        string photoMThumbPropertyToPath = Path.Combine(Server.MapPath("~/Content/Uploads/Properties/Medium"), photoPropertyName);
                        System.IO.File.Delete(photoMThumbPropertyToPath);

                        // Create updated thumbnail from image using FixedSize - Small, Medium
                        Image photoThumb = Image.FromStream(file.InputStream, true, true);
                        FixedSize(photoSThumbPropertyToPath, photoThumb, 256, 236, true);
                        FixedSize(photoMThumbPropertyToPath, photoThumb, 328, 378, true);

                        //ImageResizer imageResizer;
                        //using (var binaryReader = new BinaryReader(file.InputStream))
                        //{
                        //    imageResizer = new ImageResizer(binaryReader.ReadBytes(file.ContentLength));
                        //}
                        //imageResizer.Resize(256, 236, ImageEncoding.Jpg100);
                        //imageResizer.SaveToFile(Path.Combine(Server.MapPath("~/Content/Uploads/Properties/Thumbs"), photoPropertyName));
                    }
                }
                db.Entry(photo).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            strCurrentUserId = User.Identity.GetUserId();
            ViewBag.PropertyIDList = new SelectList(await db.Properties.Where(p => p.Availability.ToString().Equals("Yes") && p.AgentID.Equals(strCurrentUserId)).ToListAsync(), "ID", "ID", photo.PropertyID);
            ViewBag.LastEdit = DateTime.Now;
            return View(photo);
        }

        // GET: Photos/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Photo photo = await db.Photos.FindAsync(id);
            if (photo == null)
            {
                return HttpNotFound();
            }
            return View(photo);
        }

        // POST: Photos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Photo photo = await db.Photos.FindAsync(id);
            db.Photos.Remove(photo);
            await db.SaveChangesAsync();

            // Check for associated image file if exists then delete it
            if (System.IO.File.Exists(photo.UploadedTo))
            {
                System.IO.File.Delete(photo.UploadedTo);
                // Delete photoThumb - Small, Medium
                System.IO.File.Delete(Path.Combine(Server.MapPath("~/Content/Uploads/Properties/Small"), photo.PhotoTitle));
                System.IO.File.Delete(Path.Combine(Server.MapPath("~/Content/Uploads/Properties/Medium"), photo.PhotoTitle));
            }
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

        // Generate thumbnail from Image
        public static void FixedSize(string filePathName, Image image, int Width, int Height, bool needToFill)
        {

            Bitmap bmpThumb = null;
            try
            {
                int sourceWidth = image.Width;
                int sourceHeight = image.Height;
                int sourceX = 0;
                int sourceY = 0;
                double destX = 0;
                double destY = 0;

                double nScale = 0;
                double nScaleW = 0;
                double nScaleH = 0;

                nScaleW = ((double)Width / (double)sourceWidth);
                nScaleH = ((double)Height / (double)sourceHeight);
                if (!needToFill)
                {
                    nScale = Math.Min(nScaleH, nScaleW);
                }
                else
                {
                    nScale = Math.Max(nScaleH, nScaleW);
                    destY = (Height - sourceHeight * nScale) / 2;
                    destX = (Width - sourceWidth * nScale) / 2;
                }

                if (nScale > 1)
                    nScale = 1;

                int destWidth = (int)Math.Round(sourceWidth * nScale);
                int destHeight = (int)Math.Round(sourceHeight * nScale);

                bmpThumb = new Bitmap(destWidth + (int)Math.Round(2 * destX), destHeight + (int)Math.Round(2 * destY));

                using (Graphics graphic = Graphics.FromImage(bmpThumb))
                {
                    graphic.Clear(Color.White);
                    graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphic.SmoothingMode = SmoothingMode.HighQuality;
                    graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    graphic.CompositingQuality = CompositingQuality.HighQuality;

                    Rectangle to = new System.Drawing.Rectangle((int)Math.Round(destX), (int)Math.Round(destY), destWidth, destHeight);
                    Rectangle from = new System.Drawing.Rectangle(sourceX, sourceY, sourceWidth, sourceHeight);

                    graphic.DrawImage(image, to, from, System.Drawing.GraphicsUnit.Pixel);
                    image = bmpThumb;
                    // created thumbnail is uploaded
                    image.Save(filePathName, ImageFormat.Jpeg);

                }//done with drawing on "graphic"
            }
            catch
            { //error before IDisposable ownership transfer
                if (bmpThumb != null) bmpThumb.Dispose();
                throw;
            }
        }

        //public string ConvertBytes(int IBytes)
        //{
        //    string sSize = string.Empty;

        //    if (IBytes >= 1073741824)
        //        sSize = String.Format("{0:##.##}", IBytes / 1073741824) + " GB";
        //    else if (IBytes >= 1048576)
        //        sSize = String.Format("{0:D}", IBytes / 1048576) + " MB";
        //    else if (IBytes >= 1024)
        //        sSize = String.Format("{0:##.##}", IBytes / 1024) + " KB";
        //    else if (IBytes > 0 && IBytes < 1024)
        //        sSize = IBytes.ToString() + " bytes";

        //    return sSize;
        //}
    }
}
