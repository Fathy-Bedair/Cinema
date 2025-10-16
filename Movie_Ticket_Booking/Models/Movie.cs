namespace Movie_Ticket_Booking.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string? PosterURL { get; set; }
        public string? SubImages { get; set; }
        public double Price { get; set; }
        public bool InCinema { get; set; }
        // Foreign Keys
        public int CinemaId { get; set; }
        public int CategoryId { get; set; }
        // Navigation Properties
        public Cinema Cinema { get; set; } = null!;
        public Category Category { get; set; } = null!;
        public List<Actor> Actors { get; set; } = new List<Actor>();
    }
}
