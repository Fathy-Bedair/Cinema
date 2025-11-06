using Movie_Ticket_Booking.DataAccess;
using Movie_Ticket_Booking.Models;
using Movie_Ticket_Booking.Repositories.IRepositories;

namespace Movie_Ticket_Booking.Repositories
{
    public class MovieSubImageRepository : Repository<MovieSubImage>, IMovieSubImageRepository
    {
        private ApplicationDbContext _context ;
        public MovieSubImageRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void RemoveRange(IEnumerable<MovieSubImage> movieSubImages)
        {
            _context.RemoveRange(movieSubImages);
        }
    }
}
