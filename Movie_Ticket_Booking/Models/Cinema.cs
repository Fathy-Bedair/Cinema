namespace Movie_Ticket_Booking.Models
{
    public class Cinema
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Logo { get; set; }
        public string? Description { get; set; }
        // Relationships
        public List<Movie> Movies { get; set; } = new List<Movie>();
    }
}
