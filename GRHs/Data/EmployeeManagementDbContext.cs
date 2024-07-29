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
        public DbSet<Calendar> Calendars { get; set; }
        public DbSet<CalendarHoliday> CalendarHolidays { get; set; }
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

            modelBuilder.Entity<CalendarHoliday>()
                .HasKey(ch => new { ch.CalendarID, ch.HolidayID });

            modelBuilder.Entity<CalendarHoliday>()
                .HasOne(ch => ch.Calendar)
                .WithMany(c => c.CalendarHolidays)
                .HasForeignKey(ch => ch.CalendarID);

            modelBuilder.Entity<CalendarHoliday>()
                .HasOne(ch => ch.Holiday)
                .WithMany(h => h.CalendarHolidays)
                .HasForeignKey(ch => ch.HolidayID);

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
