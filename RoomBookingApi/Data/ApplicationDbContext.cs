using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RoomBookingApi.Models;

namespace RoomBookingApi.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Room> Rooms { get; set; }
        public DbSet<Reservation> Reservations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // 🔹 Important pour ASP.NET Identity

            // Données initiales pour les salles
            modelBuilder.Entity<Room>().HasData(
                new Room { Id = 1, Name = "Salle A", Capacity = 10, Equipment = "Projecteur, Tableau blanc" },
                new Room { Id = 2, Name = "Salle B", Capacity = 20, Equipment = "Écran TV, Système de visioconférence" },
                new Room { Id = 3, Name = "Salle C", Capacity = 30, Equipment = "Projecteur, Système audio" },
                new Room { Id = 4, Name = "Salle D", Capacity = 15, Equipment = "Tableau blanc, Wifi haute vitesse" }
            );
        }
    }
}
