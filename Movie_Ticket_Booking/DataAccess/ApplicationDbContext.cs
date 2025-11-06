using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Movie_Ticket_Booking.Models;

namespace Movie_Ticket_Booking.DataAccess
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }
        public DbSet<Cinema> Cinemas { get; set; } = null!;
        public DbSet<Movie> Movies { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Actor> Actors { get; set; } = null!;
        public DbSet<MovieSubImage> MovieSubImages { get; set; } = null!;
        public DbSet<MovieActor> MovieActors { get; set; } = null!;
        public DbSet<ApplicationUserOTP> ApplicationUserOTPs { get; set; } = null!;


        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    base.OnConfiguring(optionsBuilder);
        //    optionsBuilder.UseSqlServer("Server=FAT7Y;Database=MovieTicketBookingDB;Trusted_Connection=True;TrustServerCertificate=True;");
        //}
        
    }
}
