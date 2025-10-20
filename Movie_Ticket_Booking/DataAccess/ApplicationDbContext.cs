using Microsoft.EntityFrameworkCore;
using Movie_Ticket_Booking.Models;

namespace Movie_Ticket_Booking.DataAccess
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Cinema> Cinemas { get; set; } = null!;
        public DbSet<Movie> Movies { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Actor> Actors { get; set; } = null!;
        public DbSet<MovieSubImage> MovieSubImages { get; set; } = null!;
        public DbSet<MovieActor> MovieActors { get; set; } = null!;


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer("Server=FAT7Y;Database=MovieTicketBookingDB;Trusted_Connection=True;TrustServerCertificate=True;");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);
            //modelBuilder.Entity<Movie>()
            //.HasMany(m => m.Actors)
            //.WithMany(a => a.Movies)
            //.UsingEntity(j => j.ToTable("MovieActors"));
        }
    }
}
