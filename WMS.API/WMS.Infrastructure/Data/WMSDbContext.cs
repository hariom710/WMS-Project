using Microsoft.EntityFrameworkCore;
using WMS.Domain.Models;

namespace WMS.Infrastructure.Data
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
            // ── Global Query Filters (Soft Delete) ──
            modelBuilder.Entity<Employee>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Department>().HasQueryFilter(d => !d.IsDeleted);
            modelBuilder.Entity<Project>().HasQueryFilter(p => !p.IsDeleted);
            modelBuilder.Entity<Client>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<Leave>().HasQueryFilter(l => !l.IsDeleted);
            modelBuilder.Entity<Announcement>().HasQueryFilter(a => !a.IsDeleted);
            modelBuilder.Entity<ProjectAllocation>().HasQueryFilter(pa => !pa.IsDeleted);

            // ── Employee ──
            modelBuilder.Entity<Employee>().HasIndex(e => e.Email).IsUnique();
            modelBuilder.Entity<Employee>().HasIndex(e => e.DepartmentId);
            modelBuilder.Entity<Employee>().HasIndex(e => e.RoleId);
            modelBuilder.Entity<Employee>().HasIndex(e => e.Status);
            modelBuilder.Entity<Employee>().HasIndex(e => e.LastName);
            modelBuilder.Entity<Employee>().HasIndex(e => e.PhoneNumber);
            modelBuilder.Entity<Employee>().HasIndex(e => e.IsDeleted);

            // ── Department ──
            modelBuilder.Entity<Department>().HasIndex(d => d.IsDeleted);

            // ── Project ──
            modelBuilder.Entity<Project>().HasIndex(p => p.ClientId);
            modelBuilder.Entity<Project>().HasIndex(p => p.Status);
            modelBuilder.Entity<Project>().HasIndex(p => p.IsDeleted);
            modelBuilder.Entity<Project>().HasIndex(p => new { p.Status, p.ClientId });

            // ── Client ──
            modelBuilder.Entity<Client>().HasIndex(c => c.ClientName);
            modelBuilder.Entity<Client>().HasIndex(c => c.IsDeleted);

            // ── Leave ──
            modelBuilder.Entity<Leave>().HasIndex(l => l.EmpId);
            modelBuilder.Entity<Leave>().HasIndex(l => l.Status);
            modelBuilder.Entity<Leave>().HasIndex(l => l.FromDate);
            modelBuilder.Entity<Leave>().HasIndex(l => l.IsDeleted);
            modelBuilder.Entity<Leave>().HasIndex(l => new { l.EmpId, l.Status });

            // ── Announcement ──
            modelBuilder.Entity<Announcement>().HasIndex(a => a.CreatedByEmployeeId);
            modelBuilder.Entity<Announcement>().HasIndex(a => a.IsActive);
            modelBuilder.Entity<Announcement>().HasIndex(a => a.IsDeleted);

            // ── ProjectAllocation ──
            modelBuilder.Entity<ProjectAllocation>().HasIndex(pa => pa.EmpId);
            modelBuilder.Entity<ProjectAllocation>().HasIndex(pa => pa.ProjectId);
            modelBuilder.Entity<ProjectAllocation>().HasIndex(pa => pa.IsDeleted);
            modelBuilder.Entity<ProjectAllocation>().HasIndex(pa => new { pa.EmpId, pa.ProjectId });
            modelBuilder.Entity<ProjectAllocation>().HasIndex(pa => new { pa.ProjectId, pa.Status });

            // ── UserLogin ──
            modelBuilder.Entity<UserLogin>().HasIndex(u => u.Username).IsUnique();
            modelBuilder.Entity<UserLogin>().HasIndex(u => u.RoleId);

            // ── Attendance ──
            modelBuilder.Entity<Attendance>().HasIndex(a => a.EmpId);
            modelBuilder.Entity<Attendance>().HasIndex(a => a.AttendanceDate);
            modelBuilder.Entity<Attendance>().HasIndex(a => new { a.EmpId, a.AttendanceDate });

            // ── AuditLog ──
            modelBuilder.Entity<AuditLog>().HasIndex(a => a.EntityName);
            modelBuilder.Entity<AuditLog>().HasIndex(a => a.RecordId);
            modelBuilder.Entity<AuditLog>().HasIndex(a => a.Action);
            modelBuilder.Entity<AuditLog>().HasIndex(a => a.Username);
            modelBuilder.Entity<AuditLog>().HasIndex(a => a.Timestamp);
            modelBuilder.Entity<AuditLog>().HasIndex(a => new { a.EntityName, a.Action });
            modelBuilder.Entity<AuditLog>().HasIndex(a => new { a.Username, a.Timestamp });

            // ── Relationships ──
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Department).WithMany().HasForeignKey(e => e.DepartmentId);
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Role).WithMany().HasForeignKey(e => e.RoleId);
            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Employee).WithMany().HasForeignKey(a => a.EmpId);
            modelBuilder.Entity<Leave>()
                .HasOne(l => l.Employee).WithMany().HasForeignKey(l => l.EmpId);
            modelBuilder.Entity<Project>()
                .HasOne(p => p.Client).WithMany().HasForeignKey(p => p.ClientId);
            modelBuilder.Entity<ProjectAllocation>()
                .HasOne(pa => pa.Employee).WithMany().HasForeignKey(pa => pa.EmpId);
            modelBuilder.Entity<ProjectAllocation>()
                .HasOne(pa => pa.Project).WithMany().HasForeignKey(pa => pa.ProjectId);
            modelBuilder.Entity<Announcement>()
                .HasOne(a => a.CreatedByEmployee).WithMany().HasForeignKey(a => a.CreatedByEmployeeId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
