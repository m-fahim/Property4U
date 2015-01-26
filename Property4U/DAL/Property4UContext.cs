using System;
using Property4U.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Property4U.DAL
{
    public class Property4UContext : DbContext, IDisposable
    {

        public Property4UContext() : base("Property4UContextS")
        {
        }

        //public DbSet<Ad> Ads { get; set; }
        //public DbSet<Address> Addresses { get; set; }
        //public DbSet<Admin> Admins { get; set; }
        //public DbSet<Agent> Agents { get; set; }
        //public DbSet<Bid> Bids { get; set; }
        //public DbSet<Bidding> Biddings { get; set; }
        //public DbSet<Feature> Features { get; set; }
        //public DbSet<Feedback> Feedbacks { get; set; }
        //public DbSet<Member> Members { get; set; }
        //public DbSet<Order> Orders { get; set; }
        //public DbSet<Photo> Photos { get; set; }
        //public DbSet<Property> Properties { get; set; }
        //public DbSet<Renewal> Renewals { get; set; }
        //public DbSet<Reponse> Reponses { get; set; }
        //public DbSet<Request> Requests { get; set; }
        //public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }

    }
}