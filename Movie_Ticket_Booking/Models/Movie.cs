using System.ComponentModel.DataAnnotations;

namespace Movie_Ticket_Booking.Models
{
    public class Movie
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        [MinLength(3)]
        public string Title { get; set; } = string.Empty;
        [MaxLength(1000)]
        [MinLength(10)]
        public string? Description { get; set; }
        public DateTime ReleaseDate { get; set; }
        [Required]
        public string PosterURL { get; set; } = string.Empty;
        [Required]
        public double Price { get; set; }
        [Required]
        public bool InCinema { get; set; }
        // Foreign Keys
        [Required]
        public int CinemaId { get; set; }
        [Required]
        public int CategoryId { get; set; }
        // Navigation Properties
        public Cinema Cinema { get; set; } = null!;
        public Category Category { get; set; } = null!;
        public List<Actor> Actors { get; set; } = new List<Actor>();
        public List<MovieSubImage> MovieSubImages { get; set; } = new List<MovieSubImage>();


    }
}
