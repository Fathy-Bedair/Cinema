using Microsoft.EntityFrameworkCore;

namespace Movie_Ticket_Booking.DataAccess
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Models.Cinema> Cinemas { get; set; } = null!;
        public DbSet<Models.Movie> Movies { get; set; } = null!;
        public DbSet<Models.Category> Categories { get; set; } = null!;
        public DbSet<Models.Actor> Actors { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer("Server=FAT7Y;Database=MovieTicketBookingDB;Trusted_Connection=True;TrustServerCertificate=True;");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configure many-to-many relationship between Movies and Actors
            modelBuilder.Entity<Models.Movie>()
                .HasMany(m => m.Actors)
                .WithMany(a => a.Movies)
                .UsingEntity<Dictionary<string, object>>(
                    "MovieActor",
                    j => j.HasOne<Models.Actor>().WithMany().HasForeignKey("ActorId"),
                    j => j.HasOne<Models.Movie>().WithMany().HasForeignKey("MovieId"),
                    j =>
                    {
                        j.HasKey("MovieId", "ActorId");
                        j.ToTable("MovieActors");
                    });
        }
    }
}
