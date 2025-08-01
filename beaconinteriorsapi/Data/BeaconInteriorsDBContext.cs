﻿using beaconinteriorsapi.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace beaconinteriorsapi.Data
{
    public class BeaconInteriorsDBContext:IdentityDbContext<User>
    {
        public BeaconInteriorsDBContext(DbContextOptions<BeaconInteriorsDBContext> options):base(options) 
        {
            
        }
        public DbSet<Product> Products { get; set; } 
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Tracking> Tracking { get; set; }
        public DbSet<SessionModel> Sessions { get; set; }

        //public DbSet<Address> Addresses { get; set; }       
        //public DbSet<OrderItems> OrderItems { get; set; }

    }
}
