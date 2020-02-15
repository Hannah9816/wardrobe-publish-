using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WardRobe.Models;

namespace WardRobe.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<WardRobe.Models.Wardrobe> Wardrobe { get; set; }
        public DbSet<WardRobe.Models.Trip> Trip { get; set; }
        public DbSet<WardRobe.Models.MixnMatch> MixnMatch { get; set; }
        public DbSet<WardRobe.Models.Backpack> Backpack { get; set; }
        public DbSet<WardRobe.Models.Calendar> Calendar { get; set; }
    }
}
