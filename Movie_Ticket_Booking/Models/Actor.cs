using System.ComponentModel.DataAnnotations;

namespace Movie_Ticket_Booking.Models
{
    public class Actor
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(length: 30)]
        [MinLength(3)]
        public string Name { get; set; } = string.Empty;
        [MaxLength(1000)]
        [MinLength(10)]
        public string? Bio { get; set; }
        public string? ProfilePictureURL { get; set; }
        // Relationships
        public List<Movie> Movies { get; set; } = new List<Movie>();


    }
}
