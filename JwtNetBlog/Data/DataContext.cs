﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace JwtNetBlog.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .Property(u => u.Username)
                .HasMaxLength(50);

            modelBuilder.Entity<User>()
                .Property(u => u.FirstName)
                .HasMaxLength(50);

            modelBuilder.Entity<User>()
                .Property(u => u.LastName)
                .HasMaxLength(50);

            modelBuilder.Entity<User>()
                .Property(u => u.Email)
                .HasMaxLength(50);

        }
    }
}
