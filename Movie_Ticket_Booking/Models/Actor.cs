namespace Movie_Ticket_Booking.Models
{
    public class Actor
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public string? ProfilePictureURL { get; set; }
        // Relationships
        public List<Movie> Movies { get; set; } = new List<Movie>();

    }
}
