using Movie_Ticket_Booking.Models;

namespace Movie_Ticket_Booking.ViewModels
{
    public class MovieVM
    {
        public IEnumerable<Category> Categories { get; set; }= new List<Category>();
        public IEnumerable<Cinema> Cinemas { get; set; } = new List<Cinema>();
        public IEnumerable<Actor> Actors { get; set; } = new List<Actor>();
        public Movie Movie { get; set; }= new Movie();


    }
}
