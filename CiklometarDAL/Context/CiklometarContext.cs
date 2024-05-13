using CiklometarDAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CiklometarDAL
{
   public class CiklometarContext : DbContext 
    {
        public CiklometarContext(DbContextOptions<CiklometarContext> options)
       : base(options)
        {
        }
       

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>()
                .HasKey(bc => new { bc.UserId, bc.OrganizationId,bc.UserType});
            modelBuilder.Entity<Role>()
                .HasOne(bc => bc.User)
                .WithMany(b => b.Roles)
                .HasForeignKey(bc => bc.UserId);
            modelBuilder.Entity<Role>()
                .HasOne(bc => bc.Organization)
                .WithMany(b =>b.Roles)
                .HasForeignKey(bc => bc.OrganizationId);

            modelBuilder.Entity<UserBan>()
                .HasOne(bc => bc.User)
                .WithMany(b => b.Bans)
                .HasForeignKey(bc => bc.UserId);
            modelBuilder.Entity<UserBan>()
                .HasOne(bc => bc.Organization)
                .WithMany(b => b.Bans)
                .HasForeignKey(bc => bc.OrganizationId);

            modelBuilder.Entity<Activity>(entity => {
                entity.HasIndex(e => e.ActivityId).IsUnique();
            });
        }

       public DbSet<User> Users { get; set; }
       public DbSet<Organization> Organizations { get; set; }       
       public DbSet<Role> Roles { get; set; }
       public DbSet<Requests> Requests { get; set; }
       public DbSet<Location> Locations { get; set; }
       public DbSet<CiklometarStatistics> Statistics { get; set; }
       public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
       public DbSet<Activity> Activities { get; set; }
       public DbSet<StravaTokens> StravaTokens { get; set; }
       public DbSet<UserBan> Bans { get; set; }
    }
}
