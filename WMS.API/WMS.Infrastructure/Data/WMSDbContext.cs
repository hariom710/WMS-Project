using Microsoft.EntityFrameworkCore;
using WMS.Domain.Models;

namespace WMS.API.Data
{
    public class WMSDbContext : DbContext
    {
        public WMSDbContext(DbContextOptions<WMSDbContext> options) : base(options) { }

        
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<UserLogin> UserLogins { get; set; }

       
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

            // Employee relationships
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Department)
                .WithMany()
                .HasForeignKey(e => e.DepartmentId);

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Role)
                .WithMany()
                .HasForeignKey(e => e.RoleId);

            // Attendance relationships
            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Employee)
                .WithMany()
                .HasForeignKey(a => a.EmpId);

            // Leave relationships
            modelBuilder.Entity<Leave>()
                .HasOne(l => l.Employee)
                .WithMany()
                .HasForeignKey(l => l.EmpId);

            // Project relationships
            modelBuilder.Entity<Project>()
                .HasOne(p => p.Client)
                .WithMany()
                .HasForeignKey(p => p.ClientId);

            // ProjectAllocation relationships
            modelBuilder.Entity<ProjectAllocation>()
                .HasOne(pa => pa.Employee)
                .WithMany()
                .HasForeignKey(pa => pa.EmpId);

            modelBuilder.Entity<ProjectAllocation>()
                .HasOne(pa => pa.Project)
                .WithMany()
                .HasForeignKey(pa => pa.ProjectId);

            // Announcement relationships
            modelBuilder.Entity<Announcement>()
                .HasOne(a => a.CreatedByEmployee)
                .WithMany()
                .HasForeignKey(a => a.CreatedBy);

            base.OnModelCreating(modelBuilder);
        }
    }
}