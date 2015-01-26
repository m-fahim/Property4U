using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Property4U.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentitySample.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }

        /* Extending User Attributes for Property4U */
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        // Use a sensible display name for views:
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }

        // Concatenate the address info for display in tables and such:
        [Display(Name = "Address")]
        public string DisplayAddress
        {
            get
            {
                string dspAddress =
                    string.IsNullOrWhiteSpace(this.Address) ? "" : this.Address;
                string dspCity =
                    string.IsNullOrWhiteSpace(this.City) ? "" : this.City;
                string dspState =
                    string.IsNullOrWhiteSpace(this.State) ? "" : this.State;
                string dspPostalCode =
                    string.IsNullOrWhiteSpace(this.PostalCode) ? "" : this.PostalCode;

                return string
                    .Format("{0} {1} {2} {3}", dspAddress, dspCity, dspState, dspPostalCode);
            }
        }

        // Concatenate the User Full Name Info
        [Display(Name = "Full Name")]
        public string FullName
        {
            get
            {
                string firstName =
                    string.IsNullOrWhiteSpace(this.FirstName) ? "" : this.FirstName;
                string lastName =
                    string.IsNullOrWhiteSpace(this.LastName) ? "" : this.LastName;

                return string
                    .Format("{0} {1}", firstName, lastName);
            }
        }

        // Add birth date and home town for Google Auth - P4U
        [Display(Name = "Home Town")]
        public string HomeTown { get; set; }

        [Display(Name = "Birth Date")]
        public System.DateTime? BirthDate { get; set; }

        [DataType(DataType.ImageUrl)]
        [Display(Name = "Profile Image")]
        public string ProfileImage { get; set; }

        [Display(Name = "Joined Date")]
        public System.DateTime? JoinedDate { get; set; }

    }

    // Custom Implementation Derived from the Default IdentityRole Class - P4U
    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole() : base() { }
        public ApplicationRole(string name) : base(name) { }
        // Custom Attribute - P4U
        public string Responsibilities { get; set; }
    }


    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("Property4UContextS", throwIfV1Schema: false)
        {
        }

        static ApplicationDbContext()
        {
            // Set the database intializer which is run once during application start
            // This seeds the database with admin user credentials and admin role
            Database.SetInitializer<ApplicationDbContext>(new ApplicationDbInitializer());
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<Ad> Ads { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Bid> Bids { get; set; }
        public DbSet<Bidding> Biddings { get; set; }
        public DbSet<Feature> Features { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<Renewal> Renewals { get; set; }
        public DbSet<Response> Responses { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<Configuration> Configurations { get; set; }
        public DbSet<OfType> OfTypes { get; set; }
        public DbSet<OfSubType> OfSubTypes { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Reply> Replies { get; set; }
        // Initialize to Make RoleManager work with "ApplicationRole" instead of defualt "IdentityRole" in "AccountController API"
        public DbSet<ApplicationRole> Roles { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }

    }
}