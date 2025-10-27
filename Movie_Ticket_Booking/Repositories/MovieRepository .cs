using Movie_Ticket_Booking.DataAccess;
using Movie_Ticket_Booking.Models;
using Movie_Ticket_Booking.Repositories.IRepositories;

namespace Movie_Ticket_Booking.Repositories
{
    public class MovieRepository : Repository<Movie> , IMovieRepository
    {
        private ApplicationDbContext _context = new();

        public async Task AddRangeAsync(IEnumerable<Movie> movies, CancellationToken cancellationToken = default)
        {
            await _context.AddRangeAsync(movies, cancellationToken);
        }
    }
}
