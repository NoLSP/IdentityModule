using IdentityModule.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityModule.Database
{
    public class IdentityDataContext : IdentityDbContext<User, Role, long>
    {
        public IdentityDataContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IdentityUserRole<long>>().HasKey(u => new { u.UserId, u.RoleId });
            modelBuilder.Entity<IdentityUserLogin<long>>().HasKey(u => new { u.LoginProvider, u.ProviderKey });
            modelBuilder.Entity<IdentityUserToken<long>>().HasKey(u => new { u.UserId, u.LoginProvider, u.Name });

            modelBuilder.Entity<User>()
                .HasMany(e => e.Roles)
                .WithMany(e => e.Users)
                .UsingEntity<IdentityUserRole<long>>(
                    pt => pt
                    .HasOne<Role>()
                    .WithMany(e => e.UserRoles),
                    pt => pt
                    .HasOne<User>()
                    .WithMany(e => e.UserRoles))
                .ToTable("UserRoles");
        }

        public new DbSet<User> Users { get; set; }
        public new DbSet<Role> Roles { get; set; }
    }
}
