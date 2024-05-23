using HRMS.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HRMS.API.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<Grade> Grades { get; set; }
        public virtual DbSet<Branch> Branches { get; set; }
        public virtual DbSet<Position> Positions { get; set; }
        public virtual DbSet<LeaveType> LeaveTypes { get; set; }
        public virtual DbSet<Leave> Leaves { get; set; }
        public virtual DbSet<LeaveBalance> LeaveBalances { get; set; }
        public virtual DbSet<Degree> Degrees { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<Experience> Experiences { get; set; }
        public virtual DbSet<ContactPerson> ContactPersons { get; set; }
        public virtual DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public virtual DbSet<Education> Educations { get; set; }
        public virtual DbSet<Promotion> Promotions { get; set; }
        public virtual DbSet<InternalJob> InternalJobs { get; set; }
        public virtual DbSet<EmployeeJobApplication> EmployeeJobApplications { get; set; }
        public virtual DbSet<ChildInformation> ChildInformations { get; set; }
        public virtual DbSet<Resignation> Resignations { get; set; }
        public virtual DbSet<Compliant> Compliants { get; set; }
        public virtual DbSet<EmployeePhoto> EmployeePhotos { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Grade)
                .WithMany(g => g.Employees)
                .HasForeignKey(e => e.GradeId);

            modelBuilder.Entity<Promotion>()
               .HasOne(p => p.PreviousPosition)
               .WithMany()
               .HasForeignKey(p => p.PreviousPositionId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Position)
                .WithMany(p => p.Employees)
                .HasForeignKey(e => e.PositionId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Compliant>()
            .HasOne(c => c.Employee)
            .WithMany()
            .HasForeignKey(c => c.EmployeeId)
            .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Compliant>()
                .HasOne(c => c.Position)
                .WithMany()
                .HasForeignKey(c => c.PositionId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Compliant>()
                .HasOne(c => c.Branch)
                .WithMany()
                .HasForeignKey(c => c.BranchId)
                .OnDelete(DeleteBehavior.NoAction);

            //navigation property for both resignation and department 
            modelBuilder.Entity<Resignation>()
                .HasOne(r => r.Position)
                .WithMany(p => p.Resignations)
                .HasForeignKey(r => r.PositionId)
                .OnDelete(DeleteBehavior.Restrict);

            //navigation property for both resignation and department 
            modelBuilder.Entity<Resignation>()
                .HasOne(r => r.Department)
                .WithMany(p => p.Resignations)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);


        }
    }
}
