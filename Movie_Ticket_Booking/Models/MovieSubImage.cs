using Microsoft.EntityFrameworkCore;
using Movie_Ticket_Booking.ViewModels;

namespace Movie_Ticket_Booking.Models
{
    [PrimaryKey(nameof(MovieId), nameof(Img))]
    public class MovieSubImage
    {
        public int MovieId { get; set; }
        public Movie Movie { get; set; } = null!;
        public string Img { get; set; } = string.Empty;      
    }

}

