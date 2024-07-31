
using GRHs.Entities;
using Microsoft.EntityFrameworkCore;

namespace GRHs.Data
{
    public class EmployeeManagementDbContext : DbContext
    {
        public DbSet<Users> Users { get; set; }
        public DbSet<Admins> Admins { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Attestation> Attestations { get; set; }
        public DbSet<Holiday> Holidays { get; set; }
        public DbSet<UserSessions> UserSessions { get; set; }
        public DbSet<Leave> Leaves { get; set; }
        public DbSet<Departement> Departements { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=DESKTOP-ASRVTQG;Database=EmployeeManagementDB2;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserSessions>()
                .HasOne(us => us.User)
                .WithMany(u => u.UserSessions)
                .HasForeignKey(us => us.UserId);

            modelBuilder.Entity<Admins>()
                .HasKey(a => a.AdminID);

            modelBuilder.Entity<Users>()
                .HasKey(u => u.UserID);
        }
    }
}
