using IdentitySample.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace IdentitySample.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersAdminController : Controller
    {
        public UsersAdminController()
        {
        }

        public UsersAdminController(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
        }

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        private ApplicationRoleManager _roleManager;
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        //
        // GET: /Users/
        public async Task<ActionResult> Index()
        {
            return View(await UserManager.Users.ToListAsync());
        }

        //
        // GET: /Users/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);

            ViewBag.RoleNames = await UserManager.GetRolesAsync(user.Id);

            return View(user);
        }

        //
        // GET: /Users/Create
        public async Task<ActionResult> Create()
        {
            //Get the list of Roles
            ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");
            return View();
        }

        //
        // POST: /Users/Create
        [HttpPost]
        public async Task<ActionResult> Create(RegisterViewModel userViewModel, HttpPostedFileBase ImageFile, params string[] selectedRoles)
        {

            if (ModelState.IsValid)
            {
                if (ImageFile != null)
                {
                    string photoUserEx = Path.GetExtension(ImageFile.FileName);
                    //string photoUserCustomName = userViewModel.FirstName + "-" + DateTime.Today.ToString("MM-dd-yy") + "-" + System.Guid.NewGuid().ToString("N") + photoUserEx;
                    string photoUserCustomName = userViewModel.Email.Substring(0, userViewModel.Email.LastIndexOf('@')) + "-" + userViewModel.FirstName + "-" + userViewModel.JoinedDate.Value.ToString("MM-dd-yyyy") + photoUserEx;
                    string photoUserToPath = Path.Combine(Server.MapPath("~/Content/Uploads/Users"), photoUserCustomName);
                    // file is uploaded
                    ImageFile.SaveAs(photoUserToPath);
                    userViewModel.ProfileImage = photoUserCustomName;
                }

                var user = new ApplicationUser { 
                    UserName = userViewModel.Email, 
                    Email = userViewModel.Email,
                    /* Extending User Attributes for Property4U */
                    FirstName = userViewModel.FirstName,
                    LastName = userViewModel.LastName,
                    // Add the Address Info:
                    Address = userViewModel.Address,
                    City = userViewModel.City,
                    State = userViewModel.State,
                    PostalCode = userViewModel.PostalCode,
                    ProfileImage = userViewModel.ProfileImage,
                    JoinedDate = userViewModel.JoinedDate
                };

                /* Extending User Attributes for Property4U */
                user.FirstName = userViewModel.FirstName;
                user.LastName = userViewModel.LastName;
                // Add the Address Info:
                user.Address = userViewModel.Address;
                user.City = userViewModel.City;
                user.State = userViewModel.State;
                user.PostalCode = userViewModel.PostalCode;
                user.ProfileImage = userViewModel.ProfileImage;
                user.JoinedDate = userViewModel.JoinedDate;

                // Then create:
                var adminresult = await UserManager.CreateAsync(user, userViewModel.Password);

                //Add User to the selected Roles 
                if (adminresult.Succeeded)
                {
                    if (selectedRoles != null)
                    {
                        var result = await UserManager.AddToRolesAsync(user.Id, selectedRoles);
                        if (!result.Succeeded)
                        {
                            ModelState.AddModelError("", result.Errors.First());
                            ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");
                            return View();
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", adminresult.Errors.First());
                    ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
                    return View();

                }
                return RedirectToAction("Index");
            }
            ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
            return View();
        }

        //
        // GET: /Users/Edit/1
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            var userRoles = await UserManager.GetRolesAsync(user.Id);

            return View(new EditUserViewModel()
            {
                Id = user.Id,
                Email = user.Email,
                /* Extending User Attributes for Property4U */
                FirstName = user.FirstName,
                LastName = user.LastName,
                // Include the Addresss info:
                Address = user.Address,
                City = user.City,
                State = user.State,
                PostalCode = user.PostalCode,
                ProfileImage = user.ProfileImage,
                RolesList = RoleManager.Roles.ToList().Select(x => new SelectListItem()
                {
                    Selected = userRoles.Contains(x.Name),
                    Text = x.Name,
                    Value = x.Name
                })
            });
        }

        //
        // POST: /Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Email,Password,ConfirmPassword,FirstName,LastName,Id,Address,City,State,PostalCode,ProfileImage")] EditUserViewModel editUser, HttpPostedFileBase ImageFile, params string[] selectedRole)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(editUser.Id);
                if (user == null)
                {
                    return HttpNotFound();
                }
                user.UserName = editUser.Email;
                user.Email = editUser.Email;
                /* Extending User Attributes for Property4U */
                // Update Password after passing through HashPassword
                user.PasswordHash = UserManager.PasswordHasher.HashPassword(editUser.Password);
                user.FirstName = editUser.FirstName;
                user.LastName = editUser.LastName;
                user.Address = editUser.Address;
                user.City = editUser.City;
                user.State = editUser.State;
                user.PostalCode = editUser.PostalCode;

                if (ImageFile != null)
                {
                    string photoUserEx = Path.GetExtension(ImageFile.FileName);
                    string photoUserCustomName = user.Email.Substring(0, user.Email.LastIndexOf('@')) + "-" + user.FirstName + "-" + user.JoinedDate.Value.ToString("MM-dd-yyyy") + photoUserEx;
                    string photoUserToPath = Path.Combine(Server.MapPath("~/Content/Uploads/Users"), photoUserCustomName);

                    //if (!System.IO.File.Exists(photoUserToPath))
                    //{
                        // If no previously uploaded file
                        if (user.ProfileImage != null)
                            // Delete previously uploaded file
                            System.IO.File.Delete(Path.Combine(Server.MapPath("~/Content/Uploads/Users"), user.ProfileImage));

                        // new file is uploaded
                        ImageFile.SaveAs(photoUserToPath);
                        editUser.ProfileImage = photoUserCustomName;
                    //}
                }
                user.ProfileImage = editUser.ProfileImage;

                var userRoles = await UserManager.GetRolesAsync(user.Id);

                selectedRole = selectedRole ?? new string[] { };

                var result = await UserManager.AddToRolesAsync(user.Id, selectedRole.Except(userRoles).ToArray<string>());

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                result = await UserManager.RemoveFromRolesAsync(user.Id, userRoles.Except(selectedRole).ToArray<string>());

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "Something failed.");
            // Return all user data along with roles on Something failed using EditUserViewModel - P4U
            var userRolesS = await UserManager.GetRolesAsync(editUser.Id);
            return View(new EditUserViewModel()
            {
                Id = editUser.Id,
                Email = editUser.Email,
                /* Extending User Attributes for Property4U */
                FirstName = editUser.FirstName,
                LastName = editUser.LastName,
                // Include the Addresss info:
                Address = editUser.Address,
                City = editUser.City,
                State = editUser.State,
                PostalCode = editUser.PostalCode,
                ProfileImage = editUser.ProfileImage,
                RolesList = RoleManager.Roles.ToList().Select(x => new SelectListItem()
                {
                    Selected = userRolesS.Contains(x.Name),
                    Text = x.Name,
                    Value = x.Name
                })
            });
        }

        //
        // GET: /Users/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        //
        // POST: /Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var user = await UserManager.FindByIdAsync(id);
                if (user == null)
                {
                    return HttpNotFound();
                }
                var result = await UserManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }

                string photoUserToPath = Path.Combine(Server.MapPath("~/Content/Uploads/Users"), user.ProfileImage);
                // Delete uploaded profile image file
                if (System.IO.File.Exists(photoUserToPath))
                {
                    System.IO.File.Delete(photoUserToPath);
                }

                return RedirectToAction("Index");
            }
            return View();
        }
    }
}