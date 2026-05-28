using Microsoft.EntityFrameworkCore;
using WMS.API.Models;
using WMS.Domain.Models; // This now correctly points to your new Domain layer

namespace WMS.API.Data
{
    public class WMSDbContext : DbContext
    {
        public WMSDbContext(DbContextOptions<WMSDbContext> options) : base(options) { }

        // Core Modules
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<UserLogin> UserLogins { get; set; }

        // New Modules for Dashboard & Analytics
        public DbSet<Project> Projects { get; set; }
        public DbSet<Leave> Leaves { get; set; }
        public DbSet<Announcement> Announcements { get; set; }

        public DbSet<ProjectAllocation> ProjectAllocations { get; set; }
        public DbSet<Client> Clients { get; set; }
      
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Existing unique constraints
            modelBuilder.Entity<Employee>().HasIndex(e => e.Email).IsUnique();
            modelBuilder.Entity<UserLogin>().HasIndex(u => u.Username).IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}