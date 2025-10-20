using Microsoft.EntityFrameworkCore;

namespace Movie_Ticket_Booking.Models
{
    [PrimaryKey(nameof(MovieId), nameof(ActorId))]
    public class MovieActor
    {
        public int MovieId { get; set; }
        public Movie Movie { get; set; } = null!;
        public int ActorId { get; set; }
        public Actor Actor { get; set; } = null!;
    }
}