using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SWZSR.Models;

namespace SWZSR.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public ApplicationDbContext() : base() { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>(b => { b.HasMany(e => e.Orders).WithOne(e => e.User).HasForeignKey(e => e.UserId).IsRequired(); });

            builder.Entity<ApplicationUser>(b => { b.ToTable("Users"); });
            builder.Entity<IdentityUserRole<string>>(b => { b.ToTable("UserRoles"); });
            builder.Entity<IdentityUserLogin<string>>(b => { b.ToTable("UserLogins"); });
            builder.Entity<IdentityUserClaim<string>>(b => { b.ToTable("UserClaims"); });
            builder.Entity<IdentityUserToken<string>>(b => { b.ToTable("UserTokens"); });
            builder.Entity<IdentityRoleClaim<string>>(b => { b.ToTable("RoleClaims"); });
            builder.Entity<IdentityRole>(b => { b.ToTable("Roles"); });
        }

        public DbSet<Service> Services { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderItemService> OrderItemServices { get; set; }
        public DbSet<Setting> Settings { get; set; }
    }
}
