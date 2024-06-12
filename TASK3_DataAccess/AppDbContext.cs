using System;
using Azure;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TASK3_Core.Entities;

namespace TASK3_DataAccess {
  public class AppDbContext : IdentityDbContext<AppUser> {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Group> Groups { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<AppUser> AppUsers { get; set; }

    protected override void OnModelCreating(ModelBuilder builder) {
      base.OnModelCreating(builder);
      builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
  }
}

