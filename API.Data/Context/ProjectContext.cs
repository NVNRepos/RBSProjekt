using API.Data.Entities;
using API.Data.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data.Context {
    public class ProjectContext: IdentityDbContext<User> {

        public ProjectContext( DbContextOptions<ProjectContext> options ) : base( options ) { }

        public DbSet<Employee> Employees { get; set; }

        public DbSet<Department> Departments { get; set; }

        public DbSet<Stamp> Stamps { get; set; }

        protected override void OnModelCreating( ModelBuilder builder ) {

            // Configure User
            builder.Entity<User>().HasOne( u => u.Employee )
                .WithOne( e => e.User ).HasForeignKey<Employee>( e => e.Id ).IsRequired( false );

            // Configure Employee
            var employee = builder.Entity<Employee>();
            employee.HasKey( e => e.Id );
            employee.HasOne( e => e.User ).WithOne( u => u.Employee )
                .HasForeignKey<User>( u => u.EmployeeId ).IsRequired( false );
            employee.HasOne( e => e.Department ).WithMany( e => e.Employees );
            employee.HasMany( e => e.Stamps ).WithOne( s => s.Employee );
            employee.Property( e => e.Name ).HasMaxLength( 50 ).IsRequired().IsUnicode();
            employee.Property( e => e.FirstName ).HasMaxLength( 50 ).IsUnicode();

            // Configure Department
            var department = builder.Entity<Department>();
            department.HasKey( d => d.Id );
            department.HasMany( d => d.Employees ).WithOne( e => e.Department );
            department.Property( d => d.Name ).HasMaxLength( 80 ).IsUnicode();

            // Configure Stamps
            var stamp = builder.Entity<Stamp>();
            stamp.HasKey( s => s.Id );
            stamp.HasOne( s => s.Employee ).WithMany( e => e.Stamps );
            stamp.Property( s => s.StampType ).HasConversion(
                s => (int) s,
                value => (Domain.StampType) value
                ).IsRequired();
            stamp.Property( s => s.StampType ).IsRequired();

            base.OnModelCreating( builder );
        }
    }
}
